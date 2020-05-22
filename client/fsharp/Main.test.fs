module MainTests

open Jest
open Main
open Nojaf.CapitalGuardian.Shared
open System

describe "capital guardian tests" (fun () ->
    it "should spread over multiple events" (fun () ->
        let spreadMsg =
            { Start = DateTime(2019, 10, 1)
              Amount = 1000.
              Pieces = 3
              Name = "Split payment" }
            |> SpreadOver

        let { Events = events }: Model = update spreadMsg initialState |> fst
        expect(3).toBe(List.length events)

        let eventContent =
            match events with
            | AddExpense ({ Name = "Split payment (1/3)" }: Transaction) :: AddExpense ({ Name = "Split payment (2/3)" }: Transaction) :: AddExpense ({ Name = "Split payment (3/3)" }: Transaction) :: _ ->
                true
            | _ -> false

        expect(eventContent).toBeTruthy())

    it "should cancel previous transaction" (fun () ->
        let id = newId ()

        let initialEvents =
            [ AddExpense
                ({ Name = "Initial expense"
                   Amount = 500.
                   Created = DateTime(2019, 12, 1)
                   Id = id }) ]

        let model =
            { initialState with
                  Events = initialEvents }

        let { Events = events } =
            update (Msg.CancelTransaction(id)) model |> fst

        let balance =
            Projections.calculateBalance 12 2019 events

        expect(balance).toBe(0.))

    it "should create a new entry for the current month" (fun () ->
        let id = newId ()

        let initialEvents =
            [ AddExpense
                ({ Name = "Some expense"
                   Amount = 500.
                   Created = DateTime(2018, 12, 1)
                   Id = id }) ]

        let model =
            { initialState with
                  Events = initialEvents }

        let { Events = events } =
            update (Msg.CloneTransaction(id)) model |> fst

        expect(List.length events).toBe(2)

        let today = DateTime.Today

        let balance =
            Projections.calculateBalance today.Month today.Year events

        expect(balance).toBe(-500.0)))
