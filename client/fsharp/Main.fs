module Main

open Elmish
open Fable.Core
open Fable.Core.JsInterop
open Fable.React
open Fetch
open Nojaf.CapitalGuardian.Shared
open System
open Thoth.Json

module Projections =
    let isInMonth month year (a: DateTime) = a.Year = year && a.Month = month

    let isNotCancelledEventChecker events =
        let cancelled =
            List.choose (function
                | Event.CancelTransaction id -> Some id
                | _ -> None) events

        fun id -> not (List.contains id cancelled)

    let calculateBalance month year events =
        let filter = isInMonth month year
        let isNotCancelled = isNotCancelledEventChecker events

        events
        |> List.fold (fun acc ev ->
            match ev with
            | AddExpense ({ Id = id; Amount = amount; Created = created }) when (filter created && isNotCancelled id) ->
                acc - amount
            | AddIncome ({ Id = id; Amount = amount; Created = created }) when (filter created && isNotCancelled id) ->
                acc + amount
            | _ -> acc) 0.

let private f g = System.Func<_, _>(g)

type Toast =
    { Icon: string
      Title: string
      Body: string }

type SpreadDetails =
    { Start: DateTime
      Name: string
      Amount: float
      Pieces: int }

type Msg =
    | AddIncome of Transaction
    | AddExpense of Transaction
    | LoadEvents
    | EventsLoaded of Event list
    | NetworkError of exn
    | ShowToast of Toast
    | ClearToast of int
    | SpreadOver of SpreadDetails
    | CancelTransaction of Id
    | CloneTransaction of Id

type Model =
    { Events: Event list
      IsLoading: bool
      Toasts: Map<int, Toast>
      Token: string }

[<Emit("process.env.REACT_APP_BACKEND")>]
let private baseUrl = jsNative

let private decodeEvent = Decode.Auto.generateDecoder<Event> ()
let private encodeEvent = Encode.Auto.generateEncoder<Event> ()

let private authorizationHeader token =
    requestHeaders [ HttpRequestHeaders.Authorization(sprintf "bearer %s" token) ]

let private fetchEvents token =
#if DEBUG
    printfn "token: %s" token
#endif
    let url = sprintf "%s/api/GetEvents" baseUrl

    fetch url [ authorizationHeader token ]
    |> Promise.bind (fun res -> res.text ())
    |> Promise.map (fun json ->
        Decode.fromString (Decode.list decodeEvent) json
        |> function
        | Ok events -> events
        | Error e -> failwithf "%s" e)

let private postEvents (token, events) =
    let url = sprintf "%s/api/AddEvents" baseUrl

    let json =
        events
        |> List.map encodeEvent
        |> Encode.list
        |> Encode.toString 2

    fetch
        url
        [ RequestProperties.Body(!^json)
          RequestProperties.Method HttpMethod.POST
          authorizationHeader token ]
    |> Promise.map (fun _ ->
        { Icon = "success"
          Title = "Saved"
          Body = "☁ persisted events to the cloud." })

let internal initialState =
    { IsLoading = true
      Events = []
      Toasts = Map.empty
      Token = "" }

let internal init token =
    { initialState with Token = token }, Cmd.OfPromise.either fetchEvents token EventsLoaded NetworkError

let private nextKey map =
    if Map.isEmpty map then
        0
    else
        Map.toArray map
        |> Array.map fst
        |> Array.max
        |> (+) 1

let private hideToastIn toastId miliSecondes dispatch =
    JS.setTimeout (fun () -> dispatch (Msg.ClearToast toastId)) miliSecondes
    |> ignore

let private postEventsCommand token events =
    Cmd.OfPromise.either postEvents (token, events) ShowToast NetworkError

let private postEventCommand token event = postEventsCommand token [ event ]

let internal update (msg: Msg) (model: Model) =
#if DEBUG
    JS.console.log msg
#endif
    match msg with
    | EventsLoaded events ->
        { model with
              Events = events
              IsLoading = false },
        Cmd.none
    | AddIncome transaction ->
        let event = Event.AddIncome transaction

        { model with
              Events = event :: model.Events },
        postEventCommand model.Token event

    | AddExpense transaction ->
        let event = Event.AddExpense transaction

        { model with
              Events = event :: model.Events },
        postEventCommand model.Token event

    | ShowToast (toast) ->
        let toastId = nextKey model.Toasts
        let toasts = Map.add toastId toast model.Toasts
        { model with Toasts = toasts }, Cmd.ofSub (hideToastIn toastId 2500)

    | ClearToast toastId ->
        let toasts = Map.remove toastId model.Toasts
        { model with Toasts = toasts }, Cmd.none

    | NetworkError ne ->
        model,
        Cmd.ofMsg
            (ShowToast
                ({ Title = "Network Error"
                   Icon = "danger"
                   Body = ne.Message }))

    | SpreadOver details ->
        let expensePerPiece =
            Math.Round((details.Amount / (float) details.Pieces), 2)

        let events =
            [ 1 .. details.Pieces ]
            |> List.map (fun i ->
                let name =
                    sprintf "%s (%i/%i)" details.Name i details.Pieces

                let date = details.Start.AddMonths(i - 1)

                Event.AddExpense
                    ({ Name = name
                       Amount = expensePerPiece
                       Id = newId ()
                       Created = date }))

        { model with
              Events = events @ model.Events },
        postEventsCommand model.Token events

    | CancelTransaction id ->
        let event = Event.CancelTransaction(id)
        let events = event :: model.Events
        { model with Events = events }, postEventsCommand model.Token [ event ]

    | CloneTransaction id ->
        let updateTransaction t =
            { t with
                  Created = DateTime.Now
                  Id = newId () }

        let event =
            model.Events
            |> List.choose (fun e ->
                match e with
                | Event.AddExpense te when (te.Id = id) -> updateTransaction te |> Event.AddExpense |> Some
                | Event.AddIncome ti when (ti.Id = id) -> updateTransaction ti |> Event.AddIncome |> Some
                | _ -> None)
            |> List.tryHead

        let events =
            event
            |> function
            | Some e -> e :: model.Events
            | None -> model.Events

        { model with Events = events }, postEventsCommand model.Token (Option.toList event)

    | _ -> failwithf "Msg %A not implemented" msg

[<NoComparison>]
type AppContext =
    { Model: Model
      Dispatch: Dispatch<Msg> }

let private defaultContextValue: AppContext = Fable.Core.JS.undefined

let appContext =
    ReactBindings.React.createContext (defaultContextValue)

let ElmishCapture =
    FunctionComponent.Of
        ((fun (props: {| children: ReactElement
                         token: string |}) ->

            let state: IStateHook<AppContext> =
                Hooks.useState
                    ({ Model = initialState
                       Dispatch = ignore })

            let view model dispatch =
                state.update ({ Model = model; Dispatch = dispatch })

            Hooks.useEffect
                ((fun () ->
                    Program.mkProgram init update view
                    |> Program.runWith props.token),
                 Array.empty)

            contextProvider appContext state.current [ props.children ]),
         "ElmishCapture",
         memoEqualsButFunctions)

let private useModel () =
    let { Model = model } = Hooks.useContext (appContext)
    model

let private useDispatch () =
    let { Dispatch = dispatch } = Hooks.useContext (appContext)
    dispatch

let useBalance month year =
    let { Events = events } = useModel ()
    Projections.calculateBalance month year events

/// Returns a  list of income and expense of the current month
let useEntries month year =
    let { Events = events } = useModel ()

    let isNotCancelled =
        Projections.isNotCancelledEventChecker events

    let filter = Projections.isInMonth month year

    let sortMapAndToArray (input: Transaction seq) =
        input
        |> Seq.sortBy (fun ai -> ai.Created)
        |> Seq.map (fun ai ->
            {| id = ai.Id
               date = ai.Created.ToString("dd/MM")
               name = ai.Name
               amount = ai.Amount |})
        |> Seq.toArray

    let income =
        events
        |> Seq.choose (function
            | Event.AddIncome (ai) when (filter ai.Created && isNotCancelled ai.Id) -> Some ai
            | _ -> None)
        |> sortMapAndToArray

    let expenses =
        events
        |> Seq.choose (function
            | Event.AddExpense (ae) when (filter ae.Created && isNotCancelled ae.Id) -> Some ae
            | _ -> None)
        |> sortMapAndToArray

    (income, expenses)

let useIsLoading () =
    let { IsLoading = isLoading } = useModel ()
    isLoading

let private parseDate (value: string) =
    value.Split([| '-' |])
    |> Array.map (Int32.Parse)
    |> fun pieces ->
        match pieces with
        | [| year; month; day |] -> DateTime(year, month, day, 12, 0, 0)
        | _ -> DateTime.Now

let useAddEntry () =
    let dispatch = useDispatch ()

    fun (input: {| name: string
                   amount: string
                   isIncome: bool
                   created: string |}) ->
        let createdDate = parseDate input.created

        let entry =
            { Id = newId ()
              Name = input.name
              Amount = JS.parseFloat input.amount
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
    let { Events = events } = useModel ()

    let months =
        events
        |> List.choose (fun msg ->
            match msg with
            | Event.AddIncome ({ Created = created })
            | Event.AddExpense ({ Created = created }) -> Some(created.Month, created.Year)
            | _ -> None)
        |> List.distinct
        |> List.sort
        |> List.groupBy snd
        |> List.map (fun (year, months) ->
            let rows =
                months
                |> List.map (fun (m, y) ->
                    {| name = getMonthName m
                       month = m
                       balance = Projections.calculateBalance m y events |})
                |> List.toArray

            let balance =
                rows |> Array.sumBy (fun mth -> mth.balance)

            {| name = year
               months = rows
               balance = balance |})
        |> List.toArray

    months

let useDefaultCreateDate month year =
    let today = DateTime.Now

    if today.Month = month && today.Year = year then today.ToString("dd") else "01"
    |> sprintf "%02i-%02i-%s" year month

let useFirstOfCurrentMonthDate () =
    let today = DateTime.Now
    sprintf "%02i-%02i-01" today.Year today.Month

let useToasts () =
    let { Toasts = toasts } = useModel ()

    toasts
    |> Map.toArray
    |> Array.map (fun (id, t) ->
        {| id = id
           title = t.Title
           icon = t.Icon
           body = t.Body |})

let useSpreadOverMonths () =
    let dispatch = useDispatch ()

    fun (input: {| name: string
                   amount: Amount
                   start: string
                   pieces: int |}) ->
        Msg.SpreadOver
            ({ Name = input.name
               Amount = input.amount
               Start = parseDate input.start
               Pieces = input.pieces })
        |> dispatch
    |> f

let useCancelEvent () =
    let dispatch = useDispatch ()

    (fun (id: Id) -> Msg.CancelTransaction(id) |> dispatch)
    |> f

let useCloneEvent () =
    let dispatch = useDispatch ()

    (fun (id: Id) -> Msg.CloneTransaction(id) |> dispatch)
    |> f
