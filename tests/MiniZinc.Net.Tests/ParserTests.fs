namespace MiniZinc.Net.Tests

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
        model.HasInput "x"
        model.HasInput "y"

    [<Fact>]
    let ``test parse sets`` () =
        let model = Model.Parse(
            """
            {1}: a;
            var {3,4,5}: b;
            par set of int: c;
            """ 
            )
        model.HasInput "a"
        model.HasInput "b"
        model.HasInput "c"
