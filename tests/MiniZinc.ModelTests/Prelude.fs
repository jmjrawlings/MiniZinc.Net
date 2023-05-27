namespace MiniZinc.Model.Tests

open System.Runtime.CompilerServices
open MiniZinc

[<AutoOpen>]
module Prelude =
            
    // Test parsing the string, no sanitizing occurs    
    let test_parse_raw parser input =
        match Parse.stringWith parser input with
        | Result.Ok ok  ->
            ok
        | Result.Error err ->
            let trace = err.Trace
            let msg = err.Message
            failwith msg
            
    // Test parsing the string, it is sanitized first            
    let test_parse parser input =
        let source, comments = Parse.stripComments input
        test_parse_raw parser source
        
    let (?=) a b =
        if a <> b then
            failwithf $"Values were not equal"
        
        
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
                        
        [<Extension>]
        static member AssertEmpty(seq: 't seq, msg: string) =
            match Seq.isEmpty seq with
            | true -> ()
            | false -> failwith msg
            
        [<Extension>]
        static member AssertEmpty(seq: 't seq) =
            match Seq.isEmpty seq with
            | true -> ()
            | false -> failwith "Sequence should be empty"
            
        [<Extension>]
        static member AssertNonEmpty(seq: 't seq, msg: string) =
            match Seq.isEmpty seq with
            | true -> failwith msg
            | false -> ()
            
        [<Extension>]
        static member AssertNonEmpty(seq: 't seq) =
            match Seq.isEmpty seq with
            | true -> failwith "Sequence should not be empty"
            | false -> ()
            