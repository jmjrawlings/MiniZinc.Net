namespace MiniZinc.Net.Tests

open Expecto
open MiniZinc.Net
open FSharp.Control
open CliWrap
open CliWrap.EventStream
open System.Text.RegularExpressions


module Tests =

    [<Tests>]
    let tests =
        testList "minizinc" [
            
            testAsync "cli is installed" {
                
                let pattern = @"version (\d+\.\d+\.\d+)"
                let regex = Regex pattern
                
                let cli =
                    Command.create("minizinc", "--version")
                    
                let! version =
                    cli
                    |> Command.listen
                    |> AsyncSeq.choose (function
                        | CommandEvent.Output msg -> Some msg
                        | _ -> None)
                    |> AsyncSeq.choose (fun msg ->
                        let result = regex.Match msg
                        if result.Success then
                            Some result.Groups.[1].Value
                        else
                            None
                        )
                    |> AsyncSeq.firstOrDefault ""
                
                Expect.equal version "2.7.2" "Could not determine MiniZinc version"
            }
            

        ]
