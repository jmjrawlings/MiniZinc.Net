module Tests

open MiniZinc

open FSharp.Control
open MiniZinc.Command
open Xunit
open FluentAssertions


[<Fact>]
let ``parse flag`` () =
    let cmd = "--a"
    let arg = Arg.parse(cmd)
    arg.Flag.Should().Be("--a","")

[<Fact>]
let ``parse assign equals`` () =
    let arg = Arg.parse "--model=xd.json"
    arg.Flag.Should().Be("--model","")
    arg.Value.Should().Be("xd.json","")
    
[<Fact>]
let ``parse value`` () =
    let input = "asdfasdf"
    let arg = Arg.parse input 
    arg.Value.Should().BeSameAs(input,null)
    arg.Flag.Should().BeEmpty("")
    
[<Fact>]
let ``parse many`` () =
    let a = Args.parse("--count=1")
    let b = Arg.parse("--count")
    let args = Args.Create(a, b, 1)
    args.ToString().Should().Be("--count=1 --count 1","")


[<Theory>]
[<InlineData("org.gecode.gecode")>]
[<InlineData("org.chuffed.chuffed")>]
let ``test solver installed`` id =
    let solver =
        MiniZinc.GetSolver id
    let sid = solver.Result.Value.Id
    id.Should().Be(id,"")
    
[<Fact>]
let ``test minizinc version`` () =
    task {
        let! version = MiniZinc.Version ()
        version.Should().Be("2.7.2","")
    }
   
//
// [<Tests>]
// let solve_tests =
//     testList "solver tests" [
//                     
//         testTask "simple model solve" {
//             let model = Model.FromString(
//                 """
//                 var 0..10: x;
//                 var 0..10: y;
//                 constraint x < y;
//                 constraint y < 3;
//                 """
//                 )
//             let! result = MiniZinc.solve model
//             Expect.stringContains result.StdOut "x" ""
//         }
//         
//     ]
