namespace MiniZinc.Net.Tests

open System
open Expecto
open MiniZinc.Net


module Tests =

    [<Tests>]
    let tests =
        testList "samples" [
            test "xd" {
                Expect.equal 1 1 ""
            }
        ]
