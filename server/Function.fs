module Nojaf.CapitalGuardian.Function

open Microsoft.AspNetCore.Http
open Microsoft.Azure.WebJobs
open Microsoft.Azure.WebJobs.Extensions.Http
open Microsoft.Extensions.Logging
open Newtonsoft.Json.Linq
open System.IO
open System.Net
open System.Net.Http

[<FunctionName("AddEvents")>]
let AddEvents([<HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = null)>] req: HttpRequest, log: ILogger) =
    log.LogInformation("F# HTTP trigger function processed a request...")
    let content = using (new StreamReader(req.Body)) (fun stream -> stream.ReadToEnd())
    new HttpResponseMessage(HttpStatusCode.OK, Content = new StringContent(content, System.Text.Encoding.UTF8, "application/json"))
//    let model = Decode.fromString GetTokensRequest.Decode content
//    match model with
//    | Ok model ->
//        let json =
//            TokenParser.tokenize model.Defines model.SourceCode
//            |> fst
//            |> toJson
//        new HttpResponseMessage(HttpStatusCode.OK, Content = new StringContent(json, System.Text.Encoding.UTF8, "application/json"))
//    | Error err ->
//        printfn "Failed to decode: %A" err
//        new HttpResponseMessage(HttpStatusCode.BadRequest, Content = new StringContent(err, System.Text.Encoding.UTF8, "text/plain"))

[<FunctionName("GetEvents")>]
let GetEvents([<HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = null)>] req: HttpRequest, log: ILogger) =
    log.LogInformation("F# HTTP trigger function processed a request.........")
    let content = (req.Query.Item "name") |> (string)
    new HttpResponseMessage(HttpStatusCode.OK, Content = new StringContent(content, System.Text.Encoding.UTF8, "application/json"))
