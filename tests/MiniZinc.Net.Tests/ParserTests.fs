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
                