namespace MiniZinc.Solve.Tests

module CommandTests =

    open MiniZinc
    open FSharp.Control
    open MiniZinc.Tests
    open MiniZinc.Command
    open Xunit
    
    [<Fact>]
    let ``parse flag`` () =
        let cmd = "--a"
        let arg = Arg.parse(cmd)
        arg.ToString().AssertEquals("--a")

    [<Fact>]
    let ``parse assign equals`` () =
        let arg = Arg.parse "--model=xd.json"
        arg.Flag.AssertEquals "--model"
        arg.Value.AssertEquals "xd.json"
        
    [<Fact>]
    let ``parse value`` () =
        let input = "asdfasdf"
        let arg = Arg.parse input 
        arg.Value.AssertEquals input
        arg.Flag.AssertEquals ""
        
    [<Fact>]
    let ``parse many`` () =
        let a = Args.parse "--count=1"
        let b = Arg.parse "--count"
        let args = Args.Create(a, b, 1)
        args.ToString().AssertEquals "--count=1 --count 1"

    [<Theory>]
    [<InlineData("org.gecode.gecode")>]
    [<InlineData("org.chuffed.chuffed")>]
    let ``test solver installed`` id =
        let solver =
            MiniZinc.GetSolver id
        let sid = solver.Result.Value.Id
        sid.AssertEquals id
        
    [<Fact>]
    let ``test minizinc version`` () =
        task {
            let! version = MiniZinc.Version ()
            assert (version <> "")
        }
