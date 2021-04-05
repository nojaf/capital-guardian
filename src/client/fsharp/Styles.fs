module CapitalGuardian.Client.Styles

open Zanaptak.TypedCssClasses

type Bootstrap =
    CssClasses<"./style.css", Naming.PascalCase>

let classNames names =
    String.concat " " names
    |> Fable.React.Props.ClassName