namespace MiniZinc.Tests

open System
open Microsoft.Extensions.Logging
open MiniZinc.Parser
open Serilog
open MiniZinc
open Xunit
open System.IO

module Fixture =
    
    let mutable logger: ILogger<MiniZincClient> = Unchecked.defaultof<ILogger<MiniZincClient>>
    
    let getLogger () =
        match logger with
        | null ->
            let serilogLogger =
                LoggerConfiguration()
                    .WriteTo.Console()
                    .CreateLogger()
                    
            Log.Logger <- serilogLogger
                                        
            let factory =
                LoggerFactory.Create(fun b -> ignore (b.AddSerilog serilogLogger))
                
            logger <-
                factory.CreateLogger<MiniZincClient>()
            logger
        | _ ->
            logger

type ClientFixture() =
    
    let logger =
        Fixture.getLogger()
            
    let client =
        MiniZincClient.Create(logger)

    member this.Client =
        client

   
[<AbstractClass>]
type IntegrationTestSuite(fixture: ClientFixture) =
        
    let client = fixture.Client
            
    let fail msg =
        Assert.Fail(msg)
        failwith ""

    let parseOptions =
        { ParseOptions.Default with Debug = true } 
            
    interface IClassFixture<ClientFixture>
    
    abstract Name: string
        
    member this.Suite =
        LibMiniZinc.testSpec[this.Name]
               
    member this.test (testCase: TestCase) (solver: string) =
        
        let options =
            SolveOptions.create solver
            
        let model =
            match parseModelFile parseOptions testCase.TestFile.FullName with
            | Result.Ok model ->
                model
            | Result.Error err ->
                fail err.Message
                
        let solution =
            client.SolveSync(model, options)
            
        ()