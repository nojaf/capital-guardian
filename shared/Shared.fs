module Nojaf.CapitalGuardian.Shared

open System

type Amount = double

type Year = int

type Month = int

type Id = System.Guid

type Transaction =
    { Id: Id
      Name: string
      Amount: Amount
      Created: DateTime }

type Event =
    | AddExpense of Transaction
    | AddIncome of Transaction
    | CancelTransaction of Id

let newId() = Guid.NewGuid()

let sampleEvents =
    let now = DateTime.Now

    [ AddExpense
          ({ Id = newId()
             Name = "Rent July"
             Amount = 1000.
             Created = now })
      AddIncome
          ({ Id = newId()
             Name = "Wage July"
             Amount = 4000.
             Created = now })
      AddExpense
          ({ Id = newId()
             Name = "Movie night"
             Amount = 30.
             Created = now })
      AddExpense
          ({ Id = newId()
             Name = "Some beers with the boyz"
             Amount = 50.
             Created = now }) ]
