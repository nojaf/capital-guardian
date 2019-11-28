module Nojaf.CapitalGuardian.Shared

open System

type Amount = double
type Year = int
type Month = int
type Id = System.Guid
type Income= string * Amount * Id option
type Expense = string * Amount * Id option
type RecurringRule = Id * string * Amount
type ProcessedRecurringRule = Id * Year * Month

type Event =
    | AddExpense of Expense
    | AddIncoming of Income
    | AddRecurringIncomingRule of RecurringRule
    | AddRecurringExpenseRule of  RecurringRule
    | ProcessedRecurringRule of ProcessedRecurringRule
    | CancelRecurringRule of Id

let sampleEvents =
    let rentRuleId = Guid.Parse("92C3A64D-EE6E-4536-9A83-04682D21E3F2")
    let wageId = Guid.Parse("DD860AB1-5A07-49D7-831D-4723C1DB8284")

    [ AddRecurringExpenseRule(rentRuleId, "Rent", 800.)
      AddRecurringIncomingRule(wageId, "Wage", 3000.)
      AddExpense("Rent July", 800., Some rentRuleId)
      AddIncoming("Wage July", 3000., Some wageId)
      AddExpense("Movie night", 30., None)
      AddExpense("Some beers with the boyz", 50., None) ]