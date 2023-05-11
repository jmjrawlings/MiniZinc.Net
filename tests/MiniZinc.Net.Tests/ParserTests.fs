namespace MiniZinc.Net.Tests

open Microsoft.FSharp.Core

module ParserTests =

    open MiniZinc
    open FSharp.Control
    open MiniZinc.Command
    open MiniZinc.Model
    open Xunit
    open FluentAssertions

    let parseLines p s =
        match Parse.parseLines p s with
        | Result.Ok ok ->
            ok
        | Result.Error err ->
            let trace = err.Trace
            let msg = err.Message
            failwith msg
            
    let parseLine p s =
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
    let ``test base type inst`` arg =
        let input = arg
        let output = parseLine Parse.base_ti_expr input
        ()
        
    [<Theory>]
    [<InlineData("array[int] of int")>]
    [<InlineData("array[int,int] of var float")>]
    let ``test array type inst`` arg =
        let input = arg
        let output = parseLine Parse.array_ti_expr input
        ()
        
        
        
        
        
        