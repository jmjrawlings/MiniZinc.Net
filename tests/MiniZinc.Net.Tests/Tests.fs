namespace MiniZinc.Net.Tests

open System
open Expecto
open MiniZinc.Net
open FSharp.Control
open System.Reactive
open System.Reactive.Linq
open FSharp.Control.Reactive


module Tests =

    [<Tests>]
    let tests =
        testList "minizinc" [
            
            test "cli is installed" {
                let obs = 
                    Proc.create("minizinc", "--version")
                    |> Proc.exec

                let a = obs.ToList()
                let b = a.Wait()

                // let x = obs.LastAsync().Wait()
                Expect.equal 1 1 ""
            }
            

        ]
