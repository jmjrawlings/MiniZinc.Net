namespace MiniZinc.Net.Tests

open System
open Expecto
open MiniZinc.Net
open FSharp.Control
open CliWrap
open CliWrap.EventStream
open System.Reactive.Linq
open FSharp.Control
open System.Text.RegularExpressions

module Tests =

    [<Tests>]
    let tests =
        testList "minizinc" [
            
            testAsync "cli is installed" {
                
                let pattern = @"version (\d+\.\d+\.\d+)"
                let regex = Regex pattern
                
                let cli =
                    Cli.Wrap("minizinc").WithArguments("--version")
                    
                let! version =
                    cli.ListenAsync()
                    |> AsyncSeq.ofAsyncEnum
                    |> AsyncSeq.choose (fun cmd ->
                        match cmd with
                        | :? StandardOutputCommandEvent as x ->
                            Some x.Text
                        | _ ->
                            None
                        )
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
