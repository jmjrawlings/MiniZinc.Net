namespace MiniZinc.Tests

open System
open Microsoft.Extensions.Logging
open Serilog
open MiniZinc
open Xunit
open System.IO

type ClientFixture() =
    
    let logger =
            
        let serilogLogger =
            LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger()
                
        let factory =
            new LoggerFactory()
        
        factory
            .AddSerilog(serilogLogger)
            .CreateLogger("MiniZinc")
            
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
        
    interface IClassFixture<ClientFixture>
    
    abstract Name: string
        
    member this.Suite =
        LibMiniZinc.testSpec[this.Name]
                
    member this.test (testCase: TestCase) (solver: string) =
        
        let options =
            SolveOptions.create solver
            
        let model =
            match parseModelFile testCase.TestFile.FullName with
            | Result.Ok model ->
                model
            | Result.Error err ->
                fail err.Message
                
        let solution =
            client.SolveSync(model, options)
            
        ()