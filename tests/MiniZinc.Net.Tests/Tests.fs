namespace MiniZinc.Net.Tests

open Expecto
open MiniZinc
open MiniZinc.Net

open FSharp.Control
open System.Threading.Tasks
open System.Threading.Tasks
open FSharp.Control
open System.Collections.Generic
open System.Threading.Channels

module Tests =
    
    [<Tests>]
    let install_tests =
        
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
        
    [<Tests>]
    let solve_tests =
        testList "solver tests" [
                        
            testTask "simple model solve" {
                let model = Model.FromString("int: x = 3; int: y = 5; var int: sum = x + y; solve satisfy;")
                let! result = MiniZinc.Solve model
                Expect.stringContains result.stdout "x" ""
            }
        ]