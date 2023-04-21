namespace MiniZinc.Net.Tests

open Expecto
open MiniZinc.Net
open FSharp.Control
open CliWrap

module Tests =

    [<Tests>]
    let tests =
        testList "minizinc" [
            
            testAsync "cli is installed" {
                    
                let! version = MiniZinc.version ()
                Expect.equal
                    version
                    "2.7.2"
                    "Could not determine MiniZinc version"
            }
            

        ]
