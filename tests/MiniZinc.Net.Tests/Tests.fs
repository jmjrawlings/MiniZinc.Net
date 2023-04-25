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
open MiniZinc.Command

module Tests =
    
    [<Tests>]
    let arg_tests =
        testList "arg tests" [
            test "parse flag" {
                let cmd = "--a"
                let arg = Arg.parse cmd
                Expect.equal arg (Some <| Arg.Flag "--a") "?"                
            }
            
            test "parse assign equals" {
                let cmd = "--model=xd.json"
                let actual = Arg.parse cmd
                let expected = Arg.Assign("--model","xd.json")
                Expect.equal actual (Some expected) ""                
            }
            
            test "parse value" {
                let input = "asdfasdf"
                let actual = Arg.parse input
                let expected = Arg.Value input
                Expect.equal actual (Some expected) ""
            }
        ]
    
    
    [<Tests>]
    let minizinc_tests = 
        
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
                let model = Model.FromString(
                    """
                    var 0..10: x;
                    var 0..10: y;
                    constraint x < y;
                    constraint y < 3;
                    """
                    )
                let! result = MiniZinc.Solve model
                Expect.stringContains result.stdout "x" ""
            }
            
        ]
