namespace MiniZinc.Tests

open System
open Microsoft.Extensions.Logging
open Serilog
open MiniZinc

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
        
               