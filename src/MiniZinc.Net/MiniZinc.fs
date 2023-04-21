namespace MiniZinc.Net

open System
open System.Text.Json.Serialization
open MiniZinc.Net
open FSharp.Control
open System.Text.Json

module MiniZinc =
    
    let executablePath =
        "minizinc"
        
    let command () =
        executablePath
        |> Command.create
        
    /// <summary>
    /// Get all installed solvers
    /// </summary>
    let solvers () =
        async {
            let! result =
                command()
                |> Command.withArgs "--solvers-json"
                |> Command.execAsync

            let json = result.StandardOutput
            let options = JsonSerializerOptions()
            options.PropertyNameCaseInsensitive <- true
            let solvers =
                JsonSerializer.Deserialize<List<Solver>>(json, options)
                |> Map.withKey (fun s -> s.Id)
            
            return solvers
        }

    /// <summary>
    /// Find a solver by Id
    /// </summary>
    let solver id =
        solvers ()
        |> Async.map (Map.tryFind id)
    
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