namespace MiniZinc.Tests

open MiniZinc
open Xunit
open FSharp.Control

type ``Client Tests``(fixture: ClientFixture) =
    
    let client = fixture.Client
    
    [<Theory>]
    [<InlineData("org.gecode.gecode")>]
    [<InlineData("org.chuffed.chuffed")>]
    member this.``test solver installed`` id =
        let solver = client.Solvers[id]
        ()
        
    [<Fact>]
    member this.``test model interface`` () =
        
        let mzn =
            """
            enum A = {A1, A2, A3};
            set of int: B = {1, 2, 3};
            record(int: x, bool: y): c;
            float: d;
            array[1..10, B] of var float: e;
            """
            
        let model = Model.ParseString(mzn).Get()
        let iface = client.GetModelInterface(model).Get()
        
        let c = iface.Input["c"]
        let d = iface.Input["d"]
        
        let e = iface.Output["e"]
        e.IsArray.AssertEquals(true)
        
        ()
        
    [<Fact>]
    member this.``test model types`` () =
        
        let mzn =
            """
            enum A = {A1, A2, A3};
            set of int: B = {1, 2, 3};
            record(int: x, bool: y): c;
            float: d;
            array[1..10, B] of var float: e;
            """
            
        let model = Model.ParseString(mzn).Get()
        let types = client.GetModelTypes(model).Get()
        
        let a = types.Vars["B"]
        let b = types.Vars["c"]
        let c = types.Vars["d"]
        let d = types.Vars["e"]
        d.IsArray.AssertEquals(true)
        
        ()
        
    [<Fact>]
    member this.``test solve simple`` () =
        let mzn =
            """
            var 0..10: a;
            var 0..10: b;
            constraint a < b;
            solve maximize abs(a-b);
            """
        let sol = client.SolveSync(mzn)
        sol.Outputs["a"].AssertEquals(Expr.Int 0)
        sol.Outputs["b"].AssertEquals(Expr.Int 10)
        ()
        
    [<Fact>]
    member this.``test solve unsatisfiable`` () =
        let mzn =
            """
            var 0..5: a;
            var 6..10: b;
            constraint a > b;
            """
        let sol = client.SolveSync(mzn)
        sol.StatusType.AssertEquals(StatusType.Unsatisfiable)
        sol.Outputs.Count.AssertEquals(0)
        
    interface IClassFixture<ClientFixture>