namespace MiniZinc.Net.Tests

open System.Runtime.CompilerServices
open FluentAssertions
open MiniZinc

[<AutoOpen>]
module Prelude =
            
    // Test parsing the string, no sanitizing occurs    
    let test_parse_raw parser input =
        match Parse.string parser input with
        | Result.Ok ok ->
            ok
        | Result.Error err ->
            let trace = err.Trace
            let msg = err.Message
            failwith msg
            
    // Test parsing the string, it is sanitized first            
    let test_parse parser input =
        let clean = Parse.sanitize input
        test_parse_raw parser clean


    [<Extension>]
    type Extensions() =
        
        [<Extension>]
        static member ShouldEqual(a: obj, b: string) =
            (string a).Should().Be(b, "")