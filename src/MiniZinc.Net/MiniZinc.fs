namespace MiniZinc.Net

open System
open MiniZinc.Net
open FSharp.Control

module MiniZinc =
    
    let executablePath =
        "minizinc"
        
    let command () =
        executablePath
        |> Command.create
        
    let solvers () =
        command()
        |> Command.withArgs "--solvers-json"
        |> Command.execAsync
    
    let version () =
        let pattern =
            @"version (\d+\.\d+\.\d+)"
            
        async {
            let! result = 
                command()
                |> Command.withArgs "--version"
                |> Command.execAsync
                     
            let version =
                result.StandardOutput
                |> Grep.match1 pattern
            
            return version
        }