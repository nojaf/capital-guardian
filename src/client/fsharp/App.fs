module CapitalGuardian.Client.App

open Fable.Core.JsInterop
open Fable.React
open Fable.React.Props
open Feliz
open CapitalGuardian.Client.Hooks
open CapitalGuardian.Client.Components.Navigation
open CapitalGuardian.Client.Styles

[<ReactComponent()>]
let private App () =
    let small = useSm()

    div [] [
        if small then
            Navigation ()
        (*
                          {small && <Navigation />}
              <Container>
                {!small && <Header />}
                {!small && <Navigation />}
                {routeResult || <NotFoundPage />}
              </Container>
              <ToastContainer />
        *)
    ]

exportDefault App