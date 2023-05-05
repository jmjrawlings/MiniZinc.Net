namespace MiniZinc.Net.Tests

module SolveTests =

    open MiniZinc
    open FSharp.Control
    open MiniZinc.Command
    open Xunit
    open FluentAssertions
       
    [<Fact>]
    let ``test solve simple model`` () =
        let model = Model.FromString(
            """
            var 0..10: x;
            var 0..10: y;
            constraint x < y;
            constraint y < 3;
            """ 
            )
        let result = MiniZinc.exec model
        let stdout = result.Result.Text
        stdout.Should().Contain("x","")

    [<Fact>]
    let ``test stream simple model`` () =
        let model = Model.FromString(
            """
            var 0..10: x;
            var 0..10: y;
            constraint x < y;
            constraint y < 3;
            """
            )
        let sol =
            model
            |> MiniZinc.solve
            |> TaskSeq.toList
            |> List.last
            
        let msg = sol.Text
            
        msg.Should().Contain("x","")


    [<Fact>]
    let ``test analyze int`` () =
        let model = Model.FromString("var int: x;")
        let result = MiniZinc.model_types model
        let json = result.Result
        json["var_types"].["vars"].["x"].["type"].ShouldEqual("int")

    [<Fact>]
    let ``test analyze bool`` () =
        let model = Model.FromString("var bool: x;")
        let result = MiniZinc.model_types model
        let json = result.Result
        json["var_types"].["vars"].["x"].["type"].ShouldEqual("bool")
        
    [<Fact>]
    let ``test analyze enum`` () =
        let model = Model.FromString("enum Letter = {\"A\",\"B\",\"C\"}; var Letter: x;")
        let result = MiniZinc.model_types model
        let json = result.Result
        let a = 1
        ()
        
    [<Fact>]
    let ``test model interface`` () =
        let model = Model.FromString("int: a; int: b; var int: c; constraint c > b; var {1,2,3}: d;")
        let result = MiniZinc.model_interface model
        let json = result.Result
        let a = 1
        ()

