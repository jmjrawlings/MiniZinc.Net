namespace MiniZinc.Model.Tests

open System.Runtime.CompilerServices
open FluentAssertions
open MiniZinc

[<AutoOpen>]
module Prelude =
            
    // Test parsing the string, no sanitizing occurs    
    let test_parse_raw parser input =
        match Parse.string parser input with
        | Result.Ok ok  ->
            ok
        | Result.Error err ->
            let trace = err.Trace
            let msg = err.Message
            failwith msg
            
    // Test parsing the string, it is sanitized first            
    let test_parse parser input =
        let source, comments = Parse.sanitize input
        test_parse_raw parser source
        
    [<Extension>]
    type Extensions() =
        
        [<Extension>]
        static member AssertOk(result: Result<'ok, 'err>) =
            match result with
            | Ok value -> ()
            | Error err -> failwith $"Expected an ok value but got {err}"
            
        [<Extension>]
        static member AssertErr(result: Result<'ok, 'err>) =
            match result with
            | Ok value -> failwith $"Expected an error but got {value}"
            | Error err -> ()
                        