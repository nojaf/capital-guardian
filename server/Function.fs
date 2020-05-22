module Nojaf.CapitalGuardian.Function

open FSharp.Control.Tasks
open Microsoft.AspNetCore.Http
open Microsoft.Azure.WebJobs
open Microsoft.Azure.WebJobs.Extensions.Http
open Microsoft.Extensions.Logging
open Nojaf.CapitalGuardian
open Nojaf.CapitalGuardian.Authentication
open System.IO
open System.Net
open System.Net.Http
open Thoth.Json.Net

let private sendJson json =
    new HttpResponseMessage(HttpStatusCode.OK,
                            Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json"))

let private sendText text =
    new HttpResponseMessage(HttpStatusCode.OK,
                            Content = new StringContent(text, System.Text.Encoding.UTF8, "application/text"))

let private sendInternalError err =
    new HttpResponseMessage(HttpStatusCode.InternalServerError,
                            Content = new StringContent(err, System.Text.Encoding.UTF8, "application/text"))

let private unAuthorized () =
    new HttpResponseMessage(HttpStatusCode.Unauthorized)

[<FunctionName("AddEvents")>]
let AddEvents ([<HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)>] req: HttpRequest, log: ILogger) =
    log.LogInformation("F# HTTP trigger function processed a request...")
    task {
        match! req.Authenticate(log) with
        | Some userId ->
            let json =
                using (new StreamReader(req.Body)) (fun stream -> stream.ReadToEnd())

            let eventResult =
                Decode.fromString (Decode.list EventStore.decodeEvent) json

            match eventResult with
            | Ok events ->
                do! EventStore.appendEvents userId events
                return sendText "Events persisted"
            | Error err -> return sendInternalError err
        | None -> return unAuthorized ()
    }

[<FunctionName("GetEvents")>]
let GetEvents ([<HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)>] req: HttpRequest, log: ILogger) =
    log.LogInformation("F# HTTP trigger function processed a request.........")

    task {
        match! req.Authenticate(log) with
        | Some userId ->
            let! events = EventStore.getEvents userId
            let json = Encode.list events |> Encode.toString 4
            return sendJson json
        | None -> return unAuthorized ()
    }
