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
open MiniZinc.Tests
open System.Text

let obj = cwd <//> "obj"

let createClientIntegrationTests() =
    ClientTests.create ()

let createParserIntegrationTests() =      
    ParserTests.create ()
            
let downloadLibMiniZincTestSuite() =    
    LibMiniZinc.downloadTests()
 
let init() =

    Target.create "DownloadTestSuite" <| fun _ ->
        LibMiniZinc.downloadTests()
        
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