namespace MiniZinc.Tests

open MiniZinc
open MiniZinc.Tests
open Xunit

module ClientTests =
        
    let client = MiniZincClient.Create(logger)
        
    [<Theory>]
    [<InlineData("org.gecode.gecode")>]
    [<InlineData("org.chuffed.chuffed")>]
    [<InlineData("org.minizinc.mip.highs")>]
    let ``test solver installed`` id =
        let solver = client.Solvers[id]
        ()
