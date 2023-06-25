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
        
        
        
        
    interface IClassFixture<ClientFixture>