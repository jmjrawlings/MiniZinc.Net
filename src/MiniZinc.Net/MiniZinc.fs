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
open MiniZinc
open MiniZinc.Command

module rec Net =
    
    module private Constants =
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
        /// Execute the 'minizinc' command line tool
        /// with the given arguments 
        /// </summary>
        static member Exec([<ParamArray>] args: string[]) : Task<CommandResult> =
            Command.Exec(MiniZinc.ExecutablePath, args)
            
        /// <summary>
        /// Execute the 'minizinc' command line tool
        /// with the given arguments 
        /// </summary>
        static member Exec([<ParamArray>] args: string seq) : Task<CommandResult> =
            Command.Exec(MiniZinc.ExecutablePath, args)
        
        /// <summary>
        /// Get all installed solvers
        /// </summary>
        static member Solvers () =
            task {
                let! result =
                    MiniZinc.Exec "--solvers-json"
                
                let options =
                    let opts = JsonSerializerOptions()
                    opts.PropertyNameCaseInsensitive <- true
                    opts

                let solvers =
                    JsonSerializer.Deserialize<List<Solver>>(result.stdout, options)
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
            MiniZinc.Exec "--version"
            |> Task.map (fun x -> x.stdout)
            |> Task.map (Grep.match1 @"version (\d+\.\d+\.\d+)")
            
        /// <summary>
        /// Create a Command for the given Model
        /// </summary>
        static member Command(model: Model) =
            
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

            let flags = 
                [ "--json-stream"
                 ; "--model"; model_uri ]
                
            Command.Create(MiniZinc.ExecutablePath, flags)
            
            
        static member Solve(model: Model) =
            let command = MiniZinc.Command(model)
            let result = Command.Exec(command)
            result
            
        static member Analyze(model: Model) =
            let command = MiniZinc.Command(model)
            let result = Command.Exec(command)
            result            
            
        static member Stream(model: Model) =
            let command = MiniZinc.Command(model)
            taskSeq {
                for msg in Command.Stream(command) do
                    yield msg
            }