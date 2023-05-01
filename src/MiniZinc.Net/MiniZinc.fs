namespace MiniZinc

open System
open System.Diagnostics
open System.IO
open System.Text
open System.Text.Json
open System.Threading.Tasks
open FSharp.Control
open System.Collections.Generic
open System.Threading.Channels
open MiniZinc.Command

[<AutoOpen>]
module rec Net =
    
    let  [<Literal>] SOLUTION_SEP ="----------"
    let [<Literal>] UNSAT_MSG = "=====UNSATISFIABLE====="
    let [<Literal>] UNSAT_OR_UNBOUNDED_MSG = "=====UNSATorUNBOUNDED====="
    let [<Literal>] UNBOUNDED_MSG = "==UNBOUNDED====="
    let [<Literal>] UNKNOWN_MSG = "=====UNKNOWN====="
    let [<Literal>] ERROR_MSG = "=====ERROR====="
    let [<Literal>] COMPLETE_MSG = "=========="

    type Model =
        internal
        | String of string
        | Mzn of FileInfo
        
        static member FromString (s: string) =
            Model.String s
        
        static member FromFile (file: string) =
            Model.FromFile(FileInfo file)
            
        static member FromFile (file: FileInfo) =
            Model.Mzn file

    type MiniZinc() =
            
        static let mutable executablePath =
            "minizinc"
          
        static member ExecutablePath
            with get() =
                executablePath
            and set value =
                executablePath <- value
                
        /// <summary>
        /// Create a minizinc command with the given arguments 
        /// </summary>
        static member Command([<ParamArray>] args: obj[]) =
            MiniZinc.command.AddArgs(args)

        /// <summary>
        /// Get all installed solvers
        /// </summary>
        static member Solvers () =
            
            task {
                let! result =
                    MiniZinc.Command "--solvers-json"
                    |> Command.exec
                
                let options =
                    let opts = JsonSerializerOptions()
                    opts.PropertyNameCaseInsensitive <- true
                    opts

                let solvers =
                    JsonSerializer.Deserialize<List<Solver>>(result.StdOut, options)
                    |> Map.withKey (fun s -> s.Id)
                
                return solvers
            }
        
        /// <summary>
        /// Find a solver by Id
        /// </summary>
        static member GetSolver id =
            MiniZinc.Solvers ()
            |> Task.map (Map.tryFind id)
            
        /// <summary>
        /// Get the installed MiniZinc version
        /// </summary>
        static member Version() =
            MiniZinc.Command "--version"
            |> Command.exec
            |> Task.map (fun x -> x.StdOut)
            |> Task.map (Grep.match1 @"version (\d+\.\d+\.\d+)")
            
        static member Stream(model: Model) =
            let command = MiniZinc.Command(model)
            taskSeq {
                for msg in Command.stream command do
                    yield msg
            }
            
    module MiniZinc =
                
        let command : Command =
            Command.create MiniZinc.ExecutablePath Args.empty
            
        let solve (model: Model) =
            let model_file =
                match model with
                | Model.String s ->
                    let mzn = Path.GetTempFileName()
                    let mzn = Path.ChangeExtension(mzn, ".mzn")
                    File.WriteAllText(mzn, s)
                    mzn
                | Model.Mzn file ->
                    file.FullName

            let model_uri =
                Uri(model_file).AbsolutePath

            let command =
                MiniZinc.Command("--json-stream", "--model", model_uri)
            
            let result =
                Command.exec command
                
            result