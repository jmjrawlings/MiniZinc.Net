open System
open System.Diagnostics
open Fake.Core
open Fake.DotNet
open Fake.IO
open Fake.IO.FileSystemOperators
open Fake.IO.Globbing.Operators
open Fake.Core.TargetOperators
open Fake.Api
open Fake.BuildServer
open Fake.Tools
open MiniZinc
open System.Text

let obj = cwd <//> "obj"

let createClientIntegrationTests() =
    let suites = parseTestSuites()
    ClientTests.create suites

let createParserIntegrationTests() =
    let suites = parseTestSuites()        
    ParserTests.create suites
        
let downloadLibMiniZincTestSuite() =    
    LibMiniZinc.downloadTestSuite()
    
 
let init() =

    Target.create "DownloadTestSuite" <| fun _ ->
        LibMiniZinc.downloadTestSuite()
        
    Target.create "CreateClientIntegrationTests" <| fun _ ->
        createClientIntegrationTests()
        
    Target.create "CreateParserIntegrationTests" <| fun _ ->
        createParserIntegrationTests()
        
    Target.create "All" <| fun _ ->
        ()

    "DownloadTestSuite"
        ==> "CreateIntegrationTests"
        
    "DownloadTestSuite"
        ==> "CreateParserIntegrationTests"
        
    "RunTests"
        ==> "All"
        
    
[<EntryPoint>]
let main args =
    args
    |> Array.toList
    |> Context.FakeExecutionContext.Create false "build.fsx"
    |> Context.RuntimeContext.Fake
    |> Context.setExecutionContext
    
    init ()
    Target.runOrDefaultWithArguments "All"
    0