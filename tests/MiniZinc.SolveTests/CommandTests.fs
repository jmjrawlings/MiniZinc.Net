namespace MiniZinc.Solve.Tests

module CommandTests =

    open MiniZinc
    open FSharp.Control
    open MiniZinc.Command
    open Xunit
    
    [<Fact>]
    let ``parse flag`` () =
        let cmd = "--a"
        let arg = Arg.parse(cmd)
        arg.ShouldEqual("--a")

    [<Fact>]
    let ``parse assign equals`` () =
        let arg = Arg.parse "--model=xd.json"
        arg.Flag.ShouldEqual "--model"
        arg.Value.ShouldEqual "xd.json"
        
    [<Fact>]
    let ``parse value`` () =
        let input = "asdfasdf"
        let arg = Arg.parse input 
        arg.Value.ShouldEqual input
        arg.Flag.ShouldEqual ""
        
    [<Fact>]
    let ``parse many`` () =
        let a = Args.parse "--count=1"
        let b = Arg.parse "--count"
        let args = Args.Create(a, b, 1)
        args.ShouldEqual "--count=1 --count 1"

    [<Theory>]
    [<InlineData("org.gecode.gecode")>]
    [<InlineData("org.chuffed.chuffed")>]
    let ``test solver installed`` id =
        let solver =
            MiniZinc.GetSolver id
        let sid = solver.Result.Value.Id
        sid.ShouldEqual id
        
    [<Fact>]
    let ``test minizinc version`` () =
        task {
            let! version = MiniZinc.Version ()
            version.ShouldEqual "2.7.3"
        }
