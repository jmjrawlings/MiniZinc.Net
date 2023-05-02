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
        let result = MiniZinc.solve model
        let stdout = result.Exec().Result.StdOut
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
            |> MiniZinc.Stream
            |> TaskSeq.choose (function
                | CommandMessage.Output out -> Some out
                | _ -> None
                )
            |> TaskSeq.last
            
        let msg = sol.Result.Text                    
            
        msg.Should().Contain("x","")
