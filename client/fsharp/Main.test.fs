module MainTests

open System
open Jest
open Main
open Nojaf.CapitalGuardian.Shared

describe "capital guardian tests" (fun () ->
    it "should spread over multiple events" (fun () ->
        let spreadMsg =
            { Start = DateTime(2019,10,1)
              Amount = 1000.
              Pieces = 3
              Name = "Split payment" }
            |> SpreadOver
        let initialState = init () |> fst
        let { Events = events } : Model = update spreadMsg initialState |> fst
        expect(3).toBe(List.length events)
        let eventContent =
            match events with
            | AddExpense({ Name = "Split payment (1/3)" }:Transaction)::
              AddExpense({ Name = "Split payment (2/3)" }:Transaction)::
              AddExpense({ Name = "Split payment (3/3)" }:Transaction)::_ ->
                true
            | _ -> false

        expect(eventContent).toBeTruthy()
    ))