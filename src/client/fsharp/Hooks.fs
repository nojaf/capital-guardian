module CapitalGuardian.Client.Hooks

open Fable.Core.JsInterop
open Fable.React
open Feliz
open System

type BreakPointConfig =
    { xs: int
      sm: int
      md: int
      lg: int
      xl: int }

type BreakPointResult = { breakpoint: string }

let private useBreakpoint (config: BreakPointConfig) (name: string) : BreakPointResult = importDefault "use-breakpoint"

let private config : BreakPointConfig =
    { xs = 0
      sm = 576
      md = 768
      lg = 992
      xl = 1200 }

let useXs () =
    let { breakpoint = breakpoint } = useBreakpoint config "xs"
    breakpoint = "xs"

let useSm () : bool =
    let { breakpoint = breakpoint } = useBreakpoint config "sm"
    breakpoint = "sm"

let useXsOrSm () : bool =
    let xs = useXs ()
    let sm = useSm ()
    xs || sm

let useNotLarge () : bool =
    let { breakpoint = breakpoint } = useBreakpoint config "md"
    let xsOrSm = useXsOrSm ()
    breakpoint = "md" || xsOrSm
