namespace MiniZinc

open System
open System.IO
open System.Text.Json
open System.Text.Json.Nodes
open FSharp.Control
open System.Collections.Generic
open Microsoft.Extensions.Logging
open Microsoft.Extensions.Logging.Abstractions
open MiniZinc.Command
open MiniZinc.Model


[<AutoOpen>]
module rec MiniZinc =
    
    type MiniZinc(executablePath: string, logger: ILogger<MiniZinc>) =
        
        let executablePath =
            match executablePath with
            | path when String.IsNullOrEmpty path ->
                "minizinc"
            | path ->
                path
        
        let logger =
            match logger with
            | null -> NullLoggerFactory.Instance.CreateLogger()
            | _ -> logger
            
        new() =
            MiniZinc("minizinc", null)
            
        new(logger) =
            MiniZinc(null, logger)
            
        new (executablePath) =
            MiniZinc(executablePath, null)
          
        member this.ExecutablePath =
            executablePath
               
        /// Create a minizinc command with the given arguments 
        member this.Command([<ParamArray>] args: obj[]) : Command =
            let args =
                args
                |> Seq.map string
                |> Args.parseMany
            Command.Create(executablePath, args)

        /// Get all installed solvers
        member this.Solvers () =
            
            task {
                let! result =
                    this.Command "--solvers-json"
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
        
        /// Find a solver by Id
        member this.GetSolver id =
            this.Solvers ()
            |> Task.map (Map.tryFind id)
            
        /// Get the installed MiniZinc version
        member this.Version() =
            this.Command "--version"
            |> Command.exec
            |> Task.map (fun x -> x.StdOut)
            |> Task.map (Grep.match1 @"version (\d+\.\d+\.\d+)")
            
        /// Execute minizinc with the given command line args
        member this.Exec([<ParamArray>] args: obj[]) =
            this.Command(args).Exec()
            
            
    module MiniZinc =
        
        // Write the given model to a tempfile with '.mzn' extension
        let write_model_to_tempfile (model: Model) : FileInfo =
            let path = Path.GetTempFileName()
            let path = Path.ChangeExtension(path, ".mzn")
            File.WriteAllText(path, "")
            let file = FileInfo path
            file
        
        /// Create a cli arg for the given model file
        let model_arg (filepath: string) : Arg =
            let uri = Uri(filepath).AbsolutePath
            let arg = Arg.parse $"--model {uri}"
            arg
                            
        let solve (model: Model) (mz: MiniZinc) : IAsyncEnumerable<OutputMessage> =
            
            let model_file = write_model_to_tempfile model
            let model_arg = model_arg model_file.FullName
            let command = mz.Command("--json-stream", model_arg)
                
            command
            |> Command.stream
            |> TaskSeq.choose (function
                | CommandMessage.Output msg ->
                    Some msg
                | _ ->
                    None)
            
        /// Solve the given model and wait
        /// for the best solution only
        let exec (model: Model) (mz: MiniZinc) =
            mz
            |> solve model
            |> TaskSeq.last
            
        /// Analyse the given model
        let model_types (model: Model) (mz: MiniZinc) =
            let model_file = write_model_to_tempfile model
            let model_arg = model_arg model_file.FullName
            let command = mz.Command(model_arg, "--model-types-only")
            task {
                let! result = command.Exec()
                let json = JsonObject.Parse(result.StdOut)
                return json
            }
            
        /// Analyse the given model
        let model_interface (model: Model) (mz: MiniZinc) =
            let model_file = write_model_to_tempfile model
            let model_arg = model_arg model_file.FullName
            let result = mz.Exec(model_arg, "--model-interface-only")
            result