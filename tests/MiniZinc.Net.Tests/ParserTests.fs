namespace MiniZinc.Net.Tests

open Microsoft.FSharp.Core

module ParserTests =

    open MiniZinc
    open FSharp.Control
    open MiniZinc.Command
    open MiniZinc.Model
    open Xunit
    open FluentAssertions

    let parse p s =
        match Parse.parseLines p s with
        | Result.Ok ok ->
            ok
        | Result.Error err ->
            let trace = err.Trace
            let msg = err.Message
            failwith msg
            
    [<Theory>]
    [<InlineData("int")>]
    [<InlineData("var int")>]
    [<InlineData("bool")>]
    [<InlineData("var opt bool")>]
    [<InlineData("par set of int")>]
    let ``test type inst`` arg =
        let input = $"{arg};"
        parse Parse.ti_expr input