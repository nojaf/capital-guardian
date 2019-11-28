#load "../../.paket/load/netstandard2.0/client/client.group.fsx"
#load "../../shared/Shared.fs"

open Fable.Core
open Fable.React
open Elmish
open Nojaf.CapitalGuardian.Shared

module Projections =
    let calculateBalance events (month:Month) (year: Year) =
        // TODO: filter on month & year
        events
        |> List.fold (fun acc ev ->
            match ev with
            | AddExpense(_,amount,_) -> acc - amount
            | AddIncoming(_,amount,_) -> acc + amount
            | _ -> acc) 0.

let f g =
    System.Func<_,_>(g)

let f2 g =
    System.Func<_,_,_>(g)

type Msg =
    | AddIncome of Income
    | AddExpense of Expense
    | LoadEvents
    | EventsLoaded of Event list
    | NetworkError of exn

type Model = Event list // TODO should be the Event type from CosmoStore.

let private fetchEvents () =
    promise {
        return sampleEvents
    }

let private init _ = [], Cmd.OfPromise.either fetchEvents () EventsLoaded NetworkError
let private update (msg: Msg) (model: Model) =
    JS.console.log msg
    match msg with
    | EventsLoaded events -> events, Cmd.none
    | _ ->
        failwithf "Msg %A not implemented" msg

[<NoComparison>]
type AppContext =
    { Model: Model
      Dispatch : Dispatch<Msg> }

let private defaultContextValue : AppContext = Fable.Core.JS.undefined
let appContext = ReactBindings.React.createContext(defaultContextValue)

let ElmishCapture =
    FunctionComponent.Of (
        fun (props: {|children:ReactElement; loading: ReactElement|}) ->

            let initialModel = init () |> fst
            let state : IStateHook<AppContext> = Hooks.useState({ Model = initialModel; Dispatch = ignore})
            let isMounted = Hooks.useState(false)
            let view model dispatch =
                state.update({ Model = model; Dispatch = dispatch})

            Hooks.useEffect(
                (fun () ->
                    isMounted.update(true)
                    Program.mkProgram init update view
                    |> Program.run
                ), Array.empty)

            if isMounted.current then
                contextProvider appContext state.current [props.children]
            else
                props.loading

        , "ElmishCapture", memoEqualsButFunctions)

let useBalance() =
    let { Model = model } = Hooks.useContext(appContext)
    let today = System.DateTime.Now
    Projections.calculateBalance model today.Month today.Year

