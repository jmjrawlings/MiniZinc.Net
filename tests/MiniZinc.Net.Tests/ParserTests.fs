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
        let error =
            match Parse.parse parser s with
            | Result.Ok ok ->
                ""
            | Result.Error err ->
                err.Message
        error.Should().BeEmpty("")
    
    [<Theory>]
    [<InlineData("0..10")>]
    [<InlineData("int")>]
    [<InlineData("float")>]
    [<InlineData("bool")>]
    [<InlineData("set of string")>]
    [<InlineData("X")>]    
    let ``test parse simple model`` typ =
        let string = $"var {typ}: x;"
        parse Parse.var string
