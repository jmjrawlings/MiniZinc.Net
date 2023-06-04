namespace MiniZinc.Tests

open MiniZinc
open MiniZinc.Command
open MiniZinc.Tests
open Xunit

module CommandTests =
        
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