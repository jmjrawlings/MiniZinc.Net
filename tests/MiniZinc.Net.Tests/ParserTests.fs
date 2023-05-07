namespace MiniZinc.Net.Tests

open Microsoft.FSharp.Core

module ParserTests =

    open MiniZinc
    open FSharp.Control
    open MiniZinc.Command
    open MiniZinc.Model
    open Xunit
    open FluentAssertions
    
    type Model with
        member this.HasInput x =
            (this.Inputs.ContainsKey "x").Should().BeTrue($"Missing input \"{x}\"")

    let parse parser s =
        match Parse.parse parser s with
        | Result.Ok ok ->
            ok
        | Result.Error err ->
            failwith err.Message
                
    
    [<Theory>]
    [<InlineData("0..10")>]
    [<InlineData("int")>]
    [<InlineData("float")>]
    [<InlineData("bool")>]
    [<InlineData("set of string")>]
    [<InlineData("X")>]    
    let ``test parse simple type`` arg =
        let input = $"var {arg}: x;"
        let var = parse Parse.var input
        ()
        
    [<Theory>]
    [<InlineData("0..10")>]
    [<InlineData("0..0")>]
    [<InlineData("-10..x")>]
    [<InlineData("A..B")>]
    [<InlineData("'A B C' ..   4")>]
    let ``test parse range`` arg =
        let input = $"{arg}"
        let range = parse Parse.range input
        ()
        
    [<Theory>]
    [<InlineData("A,B,C,D")>]
    [<InlineData("'a b','c d','e f',g")>]
    let ``test parse enum`` arg =
        let input = $"enum X = {{{arg}}};"
        let range = parse Parse.enum input
        ()