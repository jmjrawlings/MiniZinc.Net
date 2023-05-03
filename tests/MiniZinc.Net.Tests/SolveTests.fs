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
    let ``test analyze model`` () =
        let model = Model.FromString(
            """
            var 0..10: x;
            var 0..10: y;
            constraint x < y;
            constraint y < 3;
            """
            )
        
        let result =
            model
            |> MiniZinc.analyze
            
        let json = result.Result
        let vars = json["var_types"].["vars"]
        let x = vars.["x"].["type"]
        let y = vars.["y"].["type"]
        x.ShouldEqual("int")
        y.ShouldEqual("int")
