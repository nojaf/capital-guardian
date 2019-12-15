#r "paket: groupref build //"
#load ".fake/build.fsx/intellisense.fsx"

open Fake.Core
open Fake.Core.TargetOperators
open Fake.DotNet
open Fake.IO
open Fake.IO.FileSystemOperators
open Fake.IO.Globbing.Operators
open Fake.JavaScript
open Fantomas.FakeHelpers
open Fantomas.FormatConfig
open System
open Farmer
open Farmer.Resources

let clientPath = Path.getFullName "./client"
let setYarnWorkingDirectory (args: Yarn.YarnParams) = { args with WorkingDirectory = clientPath }
let serverPath = Path.getFullName "./server"
let sharedPath = Path.getFullName "./shared"

module Paket =
    let private runCmd cmd args =
        CreateProcess.fromRawCommand cmd args
        |> Proc.run
        |> ignore

    let private paket args = runCmd "dotnet" ("paket" :: args)

    let ``generate load script``() = paket [ "generate-load-scripts"; "-f"; "netstandard2.0"; "-t"; "fsx" ]

Target.create "Clean" (fun _ ->
    Shell.rm_rf (clientPath </> ".fable")
    Shell.rm_rf (clientPath </> "src" </> "bin")
    Shell.rm_rf (serverPath </> "bin")
    Shell.rm_rf (serverPath </> "obj"))

Target.create "Yarn" (fun _ -> Yarn.installPureLock setYarnWorkingDirectory)

Target.create "Paket" (fun _ ->
    Paket.restore (fun p -> { p with ToolType = ToolType.CreateLocalTool() })
    Shell.rm_rf (".paket" </> "load")
    Paket.``generate load script``())

Target.create "BuildClient" (fun _ -> Yarn.exec "build" setYarnWorkingDirectory)

Target.create "BuildServer" (fun _ ->
    DotNet.build (fun config -> { config with Configuration = DotNet.BuildConfiguration.Release })
        (serverPath </> "server.fsproj"))

Target.create "Build" ignore

Target.create "Watch" (fun _ ->
    let fableOutput output =
        Trace.tracefn "%s" output
        if output = "fable: Watching..." then Yarn.exec "start" setYarnWorkingDirectory

    let fableError output =
        Trace.traceErrorfn "\n%s\n" output

    let compileFable =
        CreateProcess.fromRawCommand Yarn.defaultYarnParams.YarnFilePath [ "fable"; "--watch" ]
        |> CreateProcess.withWorkingDirectory clientPath
        |> CreateProcess.redirectOutput
        |> CreateProcess.withOutputEventsNotNull fableOutput fableError
        |> Proc.startAndAwait
        |> Async.Ignore

    let stopFunc() = System.Diagnostics.Process.GetProcessesByName("func") |> Seq.iter (fun p -> p.Kill())

    let rec startFunc() =
        match ProcessUtils.tryFindPath [] "func" with
        | None -> failwith "func command was not found"
        | Some funcPath ->
            let dirtyWatcher: IDisposable ref = ref null

            let watcher =
                !!(serverPath </> "*.fs") ++ (serverPath </> "*.fsproj")
                |> ChangeWatcher.run (fun changes ->
                    printfn "FILE CHANGE %A" changes
                    if !dirtyWatcher <> null then
                        (!dirtyWatcher).Dispose()
                        stopFunc()
                        startFunc())

            dirtyWatcher := watcher

            CreateProcess.fromRawCommand funcPath [ "start" ]
            |> CreateProcess.withWorkingDirectory serverPath
            |> Proc.run
            |> ignore

    let runAzureFunction = async { startFunc() }

    Async.Parallel [ runAzureFunction; compileFable ]
    |> Async.Ignore
    |> Async.RunSynchronously)

Target.create "Format" (fun _ ->
    let fantomasConfig =
        { FormatConfig.Default with
              ReorderOpenDeclaration = true
              KeepNewlineAfter = true }

    let fsharpFiles = !!(serverPath </> "*.fs") ++ (clientPath </> "fsharp" </> "*.fsx") ++ (sharedPath </> "*.fs")

    fsharpFiles
    |> formatCode fantomasConfig
    |> Async.RunSynchronously
    |> printfn "Formatted F# files: %A"

    Yarn.exec "format" setYarnWorkingDirectory)

// dotnet fake run build.fsx -t AzureResources -- env=dev
Target.create "AzureResources" (fun p ->
    let environment =
        p.Context.Arguments
        |> List.choose (fun a ->
            match a.Split([|'='|]) with
            | [|"env";env|] -> Some env
            | _ -> None)
        |> List.tryHead
        |> Option.defaultValue "dev"

    let resourceGroup = sprintf "rg-capitalgardian-%s" environment

    let storageAccountConfig = storageAccount {
        name (sprintf "storcptlgrddata%s" environment)
        sku Sku.StandardLRS
    }

    let applicationInsights = appInsights  {
        name (sprintf "ai-capitalguardian-%s" environment)
    }

    let azureFunctions = functions {
        name (sprintf "azfun-capitalguardian-%s" environment)
        app_insights_manual applicationInsights.Name
        storage_account_link storageAccountConfig.Name.Value
    }

    let template = arm {
        location WestEurope
        add_resource storageAccountConfig
        add_resource applicationInsights
        add_resource azureFunctions
    }

    Writer.quickDeploy resourceGroup template
)


Target.create "Default" ignore

"BuildClient" ==> "Build"
"BuildServer" ==> "Build"

"Clean" ==> "Paket" ==> "Yarn" ==> "Build"

"Paket" ==> "Yarn" ==> "Watch"

Target.runOrDefaultWithArguments "Build"
