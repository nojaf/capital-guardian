module Nojaf.CapitalGuardian.Shared

open System

type Amount = double

type Year = int

type Month = int

type Id = System.Guid

type Transaction =
    { Name: string
      Amount: Amount
      Rule: Id option
      Created: DateTime }

type RecurringRule = Id * string * Amount

type ProcessedRecurringRule = Id * Year * Month

type Event =
    | AddExpense of Transaction
    | AddIncome of Transaction
    | AddRecurringIncomingRule of RecurringRule
    | AddRecurringExpenseRule of RecurringRule
    | CancelRecurringRule of Id * DateTime

let sampleEvents =
    let rentRuleId = Guid.Parse("92C3A64D-EE6E-4536-9A83-04682D21E3F2")
    let wageId = Guid.Parse("DD860AB1-5A07-49D7-831D-4723C1DB8284")
    let now = DateTime.Now

    [ AddRecurringExpenseRule(rentRuleId, "Rent", 800.)
      AddRecurringIncomingRule(wageId, "Wage", 3000.)
      AddExpense
          ({ Name = "Rent July"
             Amount = 1000.
             Rule = Some rentRuleId
             Created = now })
      AddIncome
          ({ Name = "Wage July"
             Amount = 4000.
             Rule = Some wageId
             Created = now })
      AddExpense
          ({ Name = "Movie night"
             Amount = 30.
             Rule = None
             Created = now })
      AddExpense
          ({ Name = "Some beers with the boyz"
             Amount = 50.
             Rule = None
             Created = now }) ]
