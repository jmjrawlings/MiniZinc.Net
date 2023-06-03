namespace MiniZinc.Tests

open Microsoft.Extensions.Logging
open Serilog

[<AutoOpen>]
module Logging =
    
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
