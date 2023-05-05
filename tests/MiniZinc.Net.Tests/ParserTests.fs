namespace MiniZinc.Net.Tests

module ParserTests =

    open MiniZinc
    open FSharp.Control
    open MiniZinc.Command
    open MiniZinc.Model
    open Xunit
    open FluentAssertions
       
    [<Fact>]
    let ``test parse simple model`` () =
        let model = Model.Parse(
            """
            var 0..10: x;
            var 0..10: y;
            constraint x < y;
            constraint y < 3;
            """ 
            )
        (model.Inputs.ContainsKey "x").Should().BeTrue("")
