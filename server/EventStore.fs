module Nojaf.CapitalGuardian.EventStore

open CosmoStore
open FSharp.Control.Tasks
open Microsoft.FSharp.Reflection
open Nojaf.CapitalGuardian.Shared
open System
open Thoth.Json.Net

let private storageAccountName = Environment.GetEnvironmentVariable("StorageAccountName")
let private storageAuthKey = Environment.GetEnvironmentVariable("StorageAccountKey")
let private config = TableStorage.Configuration.CreateDefault storageAccountName storageAuthKey

let private streamName = "CapitalGuardian"

let private eventStore = TableStorage.EventStore.getEventStore config

let private encodeEvent = Encode.Auto.generateEncoder<Event>()
let decodeEvent = Decode.Auto.generateDecoder<Event>()

let private getUnionCaseName (x: 'a) =
    match FSharpValue.GetUnionFields(x, typeof<'a>) with
    | case, _ -> case.Name

let private createEvent event =
    { Id = (Guid.NewGuid())
      CorrelationId = None
      CausationId = None
      Name = getUnionCaseName event
      Data = encodeEvent event
      Metadata = None }

let appendEvents (events: Event list) =
    let cosmoEvents = List.map createEvent events
    task {
        let! _ = eventStore.AppendEvents streamName Any cosmoEvents
        return () }


let getEvents() =
    task {
        let! cosmoEvents = eventStore.GetEvents streamName AllEvents
        let events = List.map (fun (ce: EventRead<JsonValue, _>) -> ce.Data) cosmoEvents
        return events
    }
