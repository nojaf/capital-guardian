#r "paket: groupref build //"
#load ".fake/build.fsx/intellisense.fsx"

open Fake.Core
open Fake.Core.TargetOperators
open Fake.DotNet
open Fake.IO
open Fake.IO.FileSystemOperators
open Fake.IO.Globbing.Operators
open Fake.JavaScript
open Fantomas.Extras.FakeHelpers
open System
open Thoth.Json.Net

let clientPath = Path.getFullName "./client"
let setYarnWorkingDirectory (args: Yarn.YarnParams) = { args with WorkingDirectory = clientPath }
let serverPath = Path.getFullName "./server"
let sharedPath = Path.getFullName "./shared"
let infrastructurePath = Path.getFullName "./infrastructure"

module Azure =
    let az parameters =
        let azPath = ProcessUtils.findPath [] "az"
        CreateProcess.fromRawCommand azPath parameters
        |> Proc.run
        |> ignore

    let func parameters =
        let funcPath = ProcessUtils.findPath [] "func"
        CreateProcess.fromRawCommand funcPath parameters
        |> CreateProcess.withWorkingDirectory serverPath
        |> Proc.run
        |> ignore

type AzureParameters =
    { ResourceGroup: string
      StorageAccount:string
      CdnEndpoint: string
      CdnProfile: string
      FunctionApp: string
      Auth0Domain: string
      Auth0Audience: string
      Auth0ClientId: string
      Auth0Scope: string
      ParameterFile: string }

    with static member Decoder parameterFile =
            Decode.object (fun get ->
                { ResourceGroup = get.Required.At ["parameters";"resourceGroupName";"value"] Decode.string
                  StorageAccount = get.Required.At ["parameters";"storageName";"value"] Decode.string
                  CdnEndpoint =  get.Required.At ["parameters";"endpointName";"value"] Decode.string
                  CdnProfile =  get.Required.At ["parameters";"cdnProfileName";"value"] Decode.string
                  FunctionApp = get.Required.At ["parameters";"functionappName";"value"] Decode.string
                  Auth0Domain = get.Required.At ["parameters"; "auth0Domain"; "value"] Decode.string
                  Auth0Audience = get.Required.At ["parameters"; "auth0Audience"; "value"] Decode.string
                  Auth0ClientId = get.Required.At ["parameters"; "auth0ClientId"; "value"] Decode.string
                  Auth0Scope = get.Required.At ["parameters"; "auth0Scope"; "value"] Decode.string
                  ParameterFile = parameterFile })

let private tryGetParameters p =
    let environment =
        p.Context.Arguments
        |> List.choose (fun a ->
            match a.Split([|'='|]) with
            | [|"env";env|] -> Some env
            | _ -> None)
        |> List.tryHead
        |> Option.defaultValue "dev"
    let parameterFile = Path.combine infrastructurePath (sprintf "%s.json" environment)
    if File.exists parameterFile then
        File.readAsString parameterFile
        |> Decode.fromString (AzureParameters.Decoder parameterFile)
        |> function | Ok p -> p | Result.Error err -> failwithf "%A" err
        |> Some
    else
        None

let private getParameters p =
    tryGetParameters p
    |> function | Some p -> p | None -> failwithf "Could not read parameters from %A" p

Target.create "Clean" (fun _ ->
    Shell.rm_rf (clientPath </> ".fable")
    Shell.rm_rf (clientPath </> "src" </> "bin")
    Shell.rm_rf (serverPath </> "bin")
    Shell.rm_rf (serverPath </> "obj"))

Target.create "Yarn" (fun _ -> Yarn.installPureLock setYarnWorkingDirectory)

Target.create "Paket" (fun _ -> Paket.restore (fun p -> { p with ToolType = ToolType.CreateLocalTool() }))

Target.create "BuildClient" (fun p ->
    tryGetParameters p
    |> Option.iter (fun parameters ->
        Environment.setEnvironVar "REACT_APP_BACKEND" (sprintf "https://%s.azurewebsites.net" parameters.FunctionApp)
        Environment.setEnvironVar "REACT_APP_AUTH0_DOMAIN" parameters.Auth0Domain
        Environment.setEnvironVar "REACT_APP_AUTH0_CIENT_ID" parameters.Auth0ClientId
        Environment.setEnvironVar "REACT_APP_AUTH0_AUDIENCE" parameters.Auth0Audience
        Environment.setEnvironVar "REACT_APP_AUTH0_SCOPE" parameters.Auth0Scope)

    Yarn.exec "build" setYarnWorkingDirectory)

Target.create "BuildServer" (fun _ ->
    DotNet.build (fun config -> { config with Configuration = DotNet.BuildConfiguration.Release })
        (serverPath </> "server.fsproj"))

// dotnet fake run build.fsx -t Build -- env=dev
Target.create "Build" ignore

Target.create "Watch" (fun _ ->
    let fableOutput output =
        Trace.tracefn "%s" output
        if output = "fable: Watching..." then Yarn.exec "start" setYarnWorkingDirectory

    let fableError output =
        Trace.traceErrorfn "\n%s\n" output

    let compileFable =
        CreateProcess.fromRawCommand Yarn.defaultYarnParams.YarnFilePath [ "fable"; "-d"; "--watch" ]
        |> CreateProcess.withWorkingDirectory clientPath
        |> CreateProcess.redirectOutput
        |> CreateProcess.withOutputEventsNotNull fableOutput fableError
        |> Proc.startAndAwait
        |> Async.Ignore

    let stopFunc() = System.Diagnostics.Process.GetProcessesByName("func") |> Seq.iter (fun p -> p.Kill())

    let rec startFunc() =
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

        Azure.func ["start"]

    let runAzureFunction = async { startFunc() }

    Async.Parallel [ runAzureFunction; compileFable ]
    |> Async.Ignore
    |> Async.RunSynchronously)

Target.create "Tests" (fun _ ->
    Environment.setEnvironVar "CI" "true"
    Yarn.exec "test" setYarnWorkingDirectory)

Target.create "Format" (fun _ ->
    let result = DotNet.exec id "fantomas" "src -r"
    if not result.OK then
        printfn "Errors while formatting all files: %A" result.Messages

    Yarn.exec "format" setYarnWorkingDirectory)

Target.create "AzureLogin" (fun _ -> Azure.az ["login"])

// dotnet fake run build.fsx -t AzureResources -- env=dev
Target.create "AzureResources" (fun p ->
    let parameters = getParameters p
    let armFile = Path.combine infrastructurePath "azuredeploy.json"

    // create resource group
    Azure.az ["group"; "create" ;"-l"; "westeurope"; "-n"; parameters.ResourceGroup]

    // populate resource group
    Azure.az ["group"; "deployment"; "validate"; "-g"; parameters.ResourceGroup; "--template-file"; armFile; "--parameters"; parameters.ParameterFile]
    Azure.az ["group"; "deployment"; "create"; "-g"; parameters.ResourceGroup; "--template-file"; armFile; "--parameters"; parameters.ParameterFile]

    // Mark storage container as static website
    Azure.az ["storage"; "blob"; "service-properties"; "update"; "--account-name"; parameters.StorageAccount; "--static-website";  "--index-document"; "index.html"]
)

Target.create "DeployClient" (fun p ->
    let parameters = getParameters p

    // Deploy static website
    let clientBuildPath = Path.combine clientPath "build"
    Azure.az ["storage";"blob";"sync";"-c";"$web";"--account-name";parameters.StorageAccount;"-s";clientBuildPath]

    // Purge CDN
    Azure.az ["cdn";"endpoint";"purge"
              "-g"; parameters.ResourceGroup
              "-n"; parameters.CdnEndpoint
              "--profile-name"; parameters.CdnProfile
              "--content-paths"; "/*"]
    )

Target.create "DeployServer" (fun p ->
    let parameters = getParameters p
    Azure.func ["azure";"functionapp";"publish"; parameters.FunctionApp; "--csharp"]
)

Target.create "Default" ignore

"Clean" ==> "Paket" ==> "Yarn" ==> "BuildClient" ==> "BuildServer" ==> "Build"

"Paket" ==> "Yarn" ==> "Watch"

"BuildClient" ==> "DeployClient"

"Paket" ==> "DeployServer"

Target.runOrDefaultWithArguments "Build"
