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
module rec Client =
    
    type MiniZincClient private (executable: string, logger: ILogger) =
        
        let mutable solvers = Map.empty
        let mutable version = ""
        
        member this.Solvers =
            solvers :> IReadOnlyDictionary<string, Solver>
            
        member this.Version =
            version
        
        static member Create(executable: string, logger: ILogger) =
                    
            let executable =
                match executable with
                | path when String.IsNullOrEmpty path ->
                    "minizinc"
                | path ->
                    path
            
            let logger =
                match logger with
                | null ->
                    NullLoggerFactory.Instance.CreateLogger() :> ILogger
                | _ ->
                    logger
                    
            let client = MiniZincClient(executable, logger)
            client.GetVersion()
            client.GetSolvers()
            client
                    
        static member Create(executable: string) =
            MiniZincClient.Create(executable, null)
            
        static member Create(logger: ILogger) =
            MiniZincClient.Create(null, logger)
          
        member this.ExecutablePath =
            executable
               
        /// Create a MiniZinc command with the given arguments
        member this.Command([<ParamArray>] args: obj[]) =
            let args =
                args
                |> Seq.map string
                |> Args.parseMany
            let cmd =
                Command.Create(executable, args)
            logger.LogInformation (cmd.Statement)
            cmd

        /// Get all installed solvers
        member private this.GetSolvers () =
                        
            let result =
                this.Command "--solvers-json"
                |> Command.execSync
                |> CommandResult.stdout
                
            let options =
                let opts = JsonSerializerOptions()
                opts.PropertyNameCaseInsensitive <- true
                opts

            let map =
                JsonSerializer.Deserialize<List<Solver>>(result, options)
                |> Map.withKey (fun s -> s.Id)
                
            for solver in map.Values do
                logger.LogInformation("{Id} - {Solver} {Version}", solver.Id, solver.Name, solver.Version)
                
            solvers <- map                
            
        /// Get the installed MiniZinc version
        member private this.GetVersion() =
            let result =
                this.Command "--version"
                |> Command.execSync
                |> CommandResult.stdout
                |> Grep.match1 @"version (\d+\.\d+\.\d+)"
            version <- result
            
        override this.ToString() =
            $"MiniZinc v{version} Client"
            
            
    module MiniZincClient =
        
        /// Write the given model to a tempfile with '.mzn' extension
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
                            
        let solve (model: Model) (mz: MiniZincClient) : IAsyncEnumerable<OutputMessage> =
            
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
            
        /// Solve the given model and wait for the best solution only
        let exec (model: Model) (mz: MiniZincClient) =
            mz
            |> solve model
            |> TaskSeq.last
            
        /// Analyse the given model
        let model_types (model: Model) (mz: MiniZincClient) =
            let model_file = write_model_to_tempfile model
            let model_arg = model_arg model_file.FullName
            let command = mz.Command(model_arg, "--model-types-only")
            let result =
                command
                |> Command.exec
                |> Task.map (fun res -> res.StdOut)
                |> Task.map JsonObject.Parse
            result
            
        /// Analyse the given model
        let model_interface (model: Model) (mz: MiniZincClient) =
            let model_file = write_model_to_tempfile model
            let model_arg = model_arg model_file.FullName
            let command = mz.Command(model_arg, "--model-interface-only")
            let result = command.Exec()            
            result