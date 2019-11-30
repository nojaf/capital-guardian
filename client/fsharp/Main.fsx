#load "../../.paket/load/netstandard2.0/client/client.group.fsx"
#load "../../shared/Shared.fs"

open Elmish
open Fable.Core
open Fable.React
open Nojaf.CapitalGuardian.Shared
open System

module Projections =
    let isInCurrentMonth (a: DateTime) = DateTime.Now.Year = a.Year && DateTime.Now.Month = a.Month

    let calculateBalanceForCurrentMonth events =
        events
        |> List.fold (fun acc ev ->
            match ev with
            | AddExpense({ Amount = amount; Created = created }) when (isInCurrentMonth created) -> acc - amount
            | AddIncome({ Amount = amount; Created = created }) when (isInCurrentMonth created) -> acc + amount
            | _ -> acc) 0.

let private f g = System.Func<_, _>(g)

type Msg =
    | AddIncome of Transaction
    | AddExpense of Transaction
    | LoadEvents
    | EventsLoaded of Event list
    | NetworkError of exn

type Model =
    { Events: Event list
      IsLoading: bool }

let private fetchEvents() =
    promise {
        do! Promise.sleep 500
        return sampleEvents
    }

let private postEvents events _ = printfn "Posting events %A to the backend" events

let private init _ =
    { IsLoading = true
      Events = [] }, Cmd.OfPromise.either fetchEvents () EventsLoaded NetworkError

let private update (msg: Msg) (model: Model) =
    JS.console.log msg
    match msg with
    | EventsLoaded events ->
        { model with
              Events = events
              IsLoading = false }, Cmd.none
    | AddIncome event
    | AddExpense event ->
        { model with Events = (Event.AddIncome event) :: model.Events }, Cmd.ofSub (postEvents [ event ])

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

let useBalance() =
    let { Events = events } = useModel()
    Projections.calculateBalanceForCurrentMonth events

/// Returns a  list of income and expense of the current month
let useEntries() =
    let { Events = events } = useModel()

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
            | Event.AddIncome(ai) when (Projections.isInCurrentMonth ai.Created) -> Some ai
            | _ -> None)
        |> sortMapAndToArray

    let expenses =
        events
        |> Seq.choose (function
            | Event.AddExpense(ae) when (Projections.isInCurrentMonth ae.Created) -> Some ae
            | _ -> None)
        |> sortMapAndToArray

    (income, expenses)

let useIsLoading() =
    let { IsLoading = isLoading } = useModel()
    isLoading

let useAddEntry() =
    let dispatch = useDispatch()
    fun (input: {| name: string; amount: Amount; isIncome: bool |}) ->
        let today = DateTime.Now

        let entry =
            { Name = input.name
              Amount = input.amount
              Rule = None
              Created = today }

        let msg =
            if input.isIncome then AddIncome entry else AddExpense entry

        dispatch msg
    |> f
