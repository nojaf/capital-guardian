#load "../../.paket/load/netstandard2.0/client/client.group.fsx"
#load "../../shared/Shared.fs"

open Elmish
open Fable.Core
open Fable.React
open Nojaf.CapitalGuardian.Shared
open System
open Thoth.Json
open Fetch
open Fable.Core.JsInterop

module Projections =
    let isInMonth month year (a: DateTime) = a.Year = year && a.Month = month
    let calculateBalance month year events =
        let filter = isInMonth month year
        events
        |> List.fold (fun acc ev ->
            match ev with
            | AddExpense({ Amount = amount; Created = created }) when (filter created) -> acc - amount
            | AddIncome({ Amount = amount; Created = created }) when (filter created) -> acc + amount
            | _ -> acc) 0.

let private f g = System.Func<_, _>(g)

type Toast = { Icon:string; Title:string; Body:string }

type Msg =
    | AddIncome of Transaction
    | AddExpense of Transaction
    | LoadEvents
    | EventsLoaded of Event list
    | NetworkError of exn
    | ShowToast of Toast
    | ClearToast of int

type Model =
    { Events: Event list
      IsLoading: bool
      Toasts: Map<int,Toast> }

let private baseUrl =
    #if DEBUG
    "http://localhost:7071"
    #else
    "<tdb>"
    #endif

let private decodeEvent = Decode.Auto.generateDecoder<Event>()
let private encodeEvent = Encode.Auto.generateEncoder<Event>()

let private fetchEvents() =
    let url = sprintf "%s/api/GetEvents" baseUrl
    fetch url []
    |> Promise.bind (fun res -> res.text())
    |> Promise.map (fun json ->
        Decode.fromString (Decode.list decodeEvent) json
        |> function | Ok events -> events
                    | Error e -> failwithf "%s" e
    )

let private postEvents events =
    let url = sprintf "%s/api/AddEvents" baseUrl
    let json =
        events
        |> List.map encodeEvent
        |> Encode.list
        |> Encode.toString 2
    fetch url [RequestProperties.Body (!^ json); RequestProperties.Method HttpMethod.POST]
    |> Promise.map (fun _ ->
        { Icon = "success"
          Title = "Saved"
          Body = "☁ persisted events to the cloud." }
    )

let private init _ =
    { IsLoading = true
      Events = []
      Toasts = Map.empty }, Cmd.OfPromise.either fetchEvents () EventsLoaded NetworkError

let private nextKey map =
    if Map.isEmpty map then
        0
    else
        Map.toArray map
        |> Array.map fst
        |> Array.max
        |> (+) 1

let private hideToastIn toastId miliSecondes dispatch =
    JS.setTimeout(fun () -> dispatch (Msg.ClearToast toastId)) miliSecondes
    |> ignore

let private update (msg: Msg) (model: Model) =
    JS.console.log msg
    match msg with
    | EventsLoaded events ->
        { model with
              Events = events
              IsLoading = false }, Cmd.none
    | AddIncome transaction ->
        let event = Event.AddIncome transaction
        { model with Events = event :: model.Events },
        Cmd.OfPromise.either postEvents [event] ShowToast NetworkError

    | AddExpense transaction ->
        let event = Event.AddExpense transaction
        { model with Events = event :: model.Events },
        Cmd.OfPromise.either postEvents [event] ShowToast NetworkError

    | ShowToast(toast) ->
        let toastId = nextKey model.Toasts
        let toasts = Map.add toastId toast model.Toasts
        { model with Toasts = toasts }, Cmd.ofSub (hideToastIn toastId 2500)

    | ClearToast toastId ->
        let toasts = Map.remove toastId model.Toasts
        { model with Toasts = toasts }, Cmd.none

    | NetworkError ne ->
        model, Cmd.ofMsg (ShowToast({ Title = "Network Error"; Icon = "danger"; Body = ne.Message }))

    | _ -> failwithf "Msg %A not implemented" msg

[<NoComparison>]
type AppContext =
    { Model: Model
      Dispatch: Dispatch<Msg> }

let private defaultContextValue: AppContext = Fable.Core.JS.undefined
let appContext = ReactBindings.React.createContext (defaultContextValue)

let ElmishCapture =
    FunctionComponent.Of
        ((fun (props: {| children: ReactElement; loading: ReactElementType |}) ->

            let initialModel = init() |> fst

            let state: IStateHook<AppContext> =
                Hooks.useState
                    ({ Model = initialModel
                       Dispatch = ignore })

            let isMounted = Hooks.useState (false)

            let view model dispatch =
                state.update
                    ({ Model = model
                       Dispatch = dispatch })

            Hooks.useEffect
                ((fun () ->
                    isMounted.update (true)
                    Program.mkProgram init update view |> Program.run), Array.empty)

            if isMounted.current
            then contextProvider appContext state.current [ props.children ]
            else ReactBindings.React.createElement (props.loading, null, [])),

         "ElmishCapture", memoEqualsButFunctions)

let private useModel() =
    let { Model = model } = Hooks.useContext (appContext)
    model

let private useDispatch() =
    let { Dispatch = dispatch } = Hooks.useContext (appContext)
    dispatch

let useBalance month year =
    let { Events = events } = useModel()
    Projections.calculateBalance month year events

/// Returns a  list of income and expense of the current month
let useEntries month year =
    let { Events = events } = useModel()

    let filter = Projections.isInMonth month year
    let sortMapAndToArray (input: Transaction seq) =
        input
        |> Seq.sortBy (fun ai -> ai.Created)
        |> Seq.map (fun ai ->
            {| name = ai.Name
               amount = ai.Amount |})
        |> Seq.toArray

    let income =
        events
        |> Seq.choose (function
            | Event.AddIncome(ai) when (filter ai.Created) -> Some ai
            | _ -> None)
        |> sortMapAndToArray

    let expenses =
        events
        |> Seq.choose (function
            | Event.AddExpense(ae) when (filter ae.Created) -> Some ae
            | _ -> None)
        |> sortMapAndToArray

    (income, expenses)

let useIsLoading() =
    let { IsLoading = isLoading } = useModel()
    isLoading

let useAddEntry() =
    let dispatch = useDispatch()
    fun (input: {| name: string; amount: Amount; isIncome: bool; created: string |}) ->
        let createdDate =
            input.created.Split([|'-'|])
            |> Array.map (System.Int32.Parse)
            |> fun pieces ->
                match pieces with
                | [|year;month;day|] -> DateTime(year,month, day)
                | _ -> DateTime.Now

        let entry =
            { Name = input.name
              Amount = input.amount
              Rule = None
              Created = createdDate }

        let msg =
            if input.isIncome then AddIncome entry else AddExpense entry

        dispatch msg
    |> f

let private getMonthName n =
    match n with
    | 1 -> "January"
    | 2 -> "February"
    | 3 -> "March"
    | 4 -> "April"
    | 5 -> "May"
    | 6 -> "June"
    | 7 -> "July"
    | 8 -> "August"
    | 9 -> "September"
    | 10 -> "October"
    | 11 -> "November"
    | 12 -> "December"
    | _ -> "Unknown month"

let useOverviewPerMonth () =
    let { Events = events } = useModel()
    let months =
        events
        |> List.choose (fun msg ->
            match msg with
            | Event.AddIncome({Created = created})
            | Event.AddExpense({Created = created}) -> Some(created.Month, created.Year)
            | _ -> None)
        |> List.distinct
        |> List.sort
        |> List.groupBy snd
        |> List.map (fun (year, months) ->
            let rows =
                months
                |> List.map (fun (m,y) -> {| name = getMonthName m
                                             month = m
                                             balance = Projections.calculateBalance m y events |})
                |> List.toArray
            let balance = rows |> Array.sumBy (fun mth -> mth.balance)
            {| name = year; months = rows; balance = balance |}
            )
        |> List.toArray
    months

let useDefaultCreateDate month year =
    let today = DateTime.Now
    if today.Month = month && today.Year = year
    then today.ToString("dd")
    else "01"
    |> sprintf "%i-%i-%s" year month

let useToasts () =
    let { Toasts = toasts } = useModel()
    toasts
    |> Map.toArray
    |> Array.map (fun (id,t) -> {| id = id
                                   title = t.Title
                                   icon = t.Icon
                                   body = t.Body |})