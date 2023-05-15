namespace MiniZinc.Net.Tests

open System.Runtime.CompilerServices
open FluentAssertions
open MiniZinc

[<AutoOpen>]
module Prelude =
    
    let test_parser parser input =
        let clean = Parse.clean input
        match Parse.string parser input with
        | Result.Ok ok ->
            ok
        | Result.Error err ->
            let trace = err.Trace
            let msg = err.Message
            failwith msg


    [<Extension>]
    type Extensions() =
        
        [<Extension>]
        static member ShouldEqual(a: obj, b: string) =
            (string a).Should().Be(b, "")