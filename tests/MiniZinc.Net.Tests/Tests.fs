namespace MiniZinc.Net.Tests

open Expecto
open MiniZinc.Net
open FSharp.Control
open CliWrap

module Tests =
    
    let test_solver_installed id =
        testAsync $"{id} installed" {
            let! solver = MiniZinc.solver id
            Expect.isTrue solver.IsSome $"{id} was not installed"
        }
        

    [<Tests>]
    let tests =
        testList "minizinc" [
            
            testAsync "cli version" {
                let! version = MiniZinc.version ()
                Expect.equal version "2.7.2" "Could not determine MiniZinc version"
            }
            
            testList "solvers installed" [
                test_solver_installed "org.gecode.gecode"
                test_solver_installed "org.chuffed.chuffed"
            ]

        ]
