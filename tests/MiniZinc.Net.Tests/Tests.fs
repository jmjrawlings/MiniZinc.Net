namespace MiniZinc.Net.Tests

open Expecto
open MiniZinc.Net
open System.Threading.Tasks

module Tests =
    
    [<Tests>]
    let tests =
        
        let test_solver_installed id =
            testTask $"{id} installed" {
                let! solver = MiniZinc.GetSolver id
                Expect.isSome solver $"{id} was not installed"
            }

        testList "minizinc installation" [
            
            testTask "version is correct" {
                let! version = MiniZinc.Version ()
                Expect.equal version "2.7.2" "Could not determine MiniZinc version"
            }
            
            testList "solvers are installed" [
                test_solver_installed "org.gecode.gecode"
                test_solver_installed "org.chuffed.chuffed"
            ]

        ]
