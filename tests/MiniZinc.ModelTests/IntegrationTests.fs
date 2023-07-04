
namespace MiniZinc.Tests

open MiniZinc
open MiniZinc.Tests
open Xunit
open System.IO

module IntegrationTests =
   
    let test (name: string) =
        let suite = TestSuite.load name
        let model = TestSuite.parseModel suite
        model.Value.Undeclared.AssertEmpty()
        model.Value.Conflicts.AssertEmpty()
