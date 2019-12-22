module Nojaf.CapitalGuardian.Function

open FSharp.Control.Tasks
open Microsoft.AspNetCore.Http
open Microsoft.Azure.WebJobs
open Microsoft.Azure.WebJobs.Extensions.Http
open Microsoft.Extensions.Logging
open Nojaf.CapitalGuardian
open Thoth.Json.Net
open System.Net
open System.Net.Http

let private sendJson json =
    new HttpResponseMessage(HttpStatusCode.OK, Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json"))

[<FunctionName("AddEvents")>]
let AddEvents([<HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)>] req: HttpRequest, log: ILogger) =
    log.LogInformation("F# HTTP trigger function processed a request...")

    // TODO: clean this up, this is a temp thing
    (EventStore.appendEvents Shared.sampleEvents).Start()


    new HttpResponseMessage(HttpStatusCode.OK,
                            Content = new StringContent("events saved", System.Text.Encoding.UTF8, "application/json"))

[<FunctionName("GetEvents")>]
let GetEvents([<HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)>] req: HttpRequest, log: ILogger) =
    log.LogInformation("F# HTTP trigger function processed a request.........")

    task {
        let! events = EventStore.getEvents ()
        let json =
            Encode.list events
            |> Encode.toString 4
        return sendJson json
    }


