namespace MiniZinc.Tests

module CommandTests =

    open MiniZinc
    open FSharp.Control
    open MiniZinc.Command
    open MiniZinc.Tests
    open Xunit
        
    let mz = MiniZinc(logger)
    
    [<Fact>]
    let ``parse flag`` () =
        let cmd = "--a"
        let arg = Arg.parse(cmd)
        arg.StringEquals("--a")

    [<Fact>]
    let ``parse assign equals`` () =
        let arg = Arg.parse "--model=xd.json"
        arg.Flag.StringEquals "--model"
        arg.Value.StringEquals "xd.json"
        
    [<Fact>]
    let ``parse value`` () =
        let input = "asdfasdf"
        let arg = Arg.parse input 
        arg.Value.StringEquals input
        arg.Flag.StringEquals ""
        
    [<Fact>]
    let ``parse many`` () =
        let a = Args.parse "--count=1"
        let b = Arg.parse "--count"
        let args = Args.create(a, b, 1)
        args.StringEquals "--count=1 --count 1"

    [<Theory>]
    [<InlineData("org.gecode.gecode")>]
    [<InlineData("org.chuffed.chuffed")>]
    let ``test solver installed`` id =
        let solver = mz.GetSolver id
        let sid = solver.Result.Value.Id
        sid.StringEquals id
         
    [<Fact>]
    let ``test minizinc version`` () =
        task {
            let! version = mz.Version()
            version <> ""
        }