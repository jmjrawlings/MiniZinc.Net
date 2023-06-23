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
        
    interface IClassFixture<ClientFixture>