namespace MiniZinc.Tests

open MiniZinc
open Xunit

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
            enum ABC = {A, B, C};
            record(int: x, bool: y): a;
            int: b;
            var int: c;
            array[1..10, ABC] of var float: d;
            """
            
        let model = Model.ParseString(mzn).Model
        let iface = client.GetModelInterface(model).Get()
        ()
        
    [<Fact>]
    member this.``test model types`` () =
        
        let mzn =
            """
            enum ABC = {A, B, C};
            record(int: x, bool: y): a;
            int: b;
            var int: c;
            array[1..10, ABC] of var float: d;
            """
            
        let model = Model.ParseString(mzn).Model
        let types = client.GetModelTypes(model).Get()
        
        let a = types.Vars["a"]
        let b = types.Vars["b"]
        let c = types.Vars["c"]
        let d = types.Vars["d"]
        
        a.Dim.AssertEquals(0)
        a.Type.StringEquals("record")
        a.FieldTypes["x"].Type.StringEquals("int")

        c.Dim.AssertEquals(0)
        c.Type.StringEquals("int")
        
        d.Dim.AssertEquals(2)
        d.Type.AssertEquals(InterfaceTypeName.Float)
        
        ()        
        
        
        
        
    interface IClassFixture<ClientFixture>