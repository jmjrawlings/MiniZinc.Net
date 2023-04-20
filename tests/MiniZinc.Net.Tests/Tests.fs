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
                
                let pattern =
                    @"version (\d+\.\d+\.\d+)"
                
                let cli =
                    Command.create("minizinc", "--version")
                    
                let! version =
                    cli
                    |> Command.stdout
                    |> AsyncSeq.map (Grep.match1 pattern)
                    |> AsyncSeq.firstOrDefault ""
                
                Expect.equal
                    version
                    "2.7.2"
                    "Could not determine MiniZinc version"
            }
            

        ]
