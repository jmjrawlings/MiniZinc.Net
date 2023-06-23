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
    
    /// <summary>
    /// A MiniZinc CLI Client 
    /// </summary>
    type MiniZincClient private (executable: string, logger: ILogger) =
        
        let mutable solvers = Map.empty
        
        let mutable version = ""
                
        /// Installed solvers by their ID
        member this.Solvers =
            solvers :> IReadOnlyDictionary<string, Solver>
            
        /// The MiniZinc CLI Version
        member this.Version =
            version
        
        /// The MiniZinc executable path
        member this.ExecutablePath =
            executable
            
        /// <summary>
        /// Create a new Client
        /// </summary>
        /// <param name="executable">The MiniZinc executable path</param>
        /// <param name="logger">A logger to write to</param>
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
                    
        /// <summary>
        /// Create a new Client
        /// </summary>
        /// <param name="executable">The MiniZinc executable path</param>        
        static member Create(executable: string) =
            MiniZincClient.Create(executable, null)

        /// <summary>
        /// Create a new Client 
        /// </summary>
        /// <param name="logger">A logger to write to</param>            
        static member Create(logger: ILogger) =
            MiniZincClient.Create(null, logger)
               
        /// Create a command from the given arguments
        member this.Command([<ParamArray>] args: obj[]) =
            
            let args =
                args
                |> Seq.map string
                |> Args.parseMany
                
            let cmd =
                Command.Create(executable, args)
                
            cmd

        /// Get all installed solvers
        member private this.GetSolvers () =
                        
            let stdout =
                this.Command "--solvers-json"
                |> Command.execSync
                |> CommandResult.stdout
               
            let options =
                let opts = JsonSerializerOptions()
                opts.PropertyNameCaseInsensitive <- true
                opts
                
            let result =
                JsonSerializer.Deserialize<List<Solver>>(stdout, options)
                |> Map.withKey (fun s -> s.Id)
                
            for solver in result.Values do
                logger.LogInformation("Found solver {Id}: {Solver} {Version}", solver.Id, solver.Name, solver.Version)
                
            solvers <- result                
            
        /// Get the installed MiniZinc version
        member private this.GetVersion() =
                        
            let result =
                this.Command "--version"
                |> Command.execSync
                |> CommandResult.stdout
                |> Grep.matches @"version (\d+\.\d+\.\d+)"
                |> List.head
                
            logger.LogInformation("MiniZinc is v{Version}", result)
            version <- result
        
        /// Solve the given model      
        member this.Solve(model: Model) =
            MiniZincClient.solve model this
            
        /// Solve the given model and wait for the last solution            
        member this.SolveAndWait(model: Model) =
            MiniZincClient.solveAndWait model this
            
        member this.GetModelTypes(model: Model) =
            MiniZincClient.model_types model this
            
        member this.GetModelInterface(model: Model) =
            MiniZincClient.model_interface model this
            
        override this.ToString() =
            $"MiniZinc v{version} Client"
            
            
    module MiniZincClient =
        
        /// Write the given model to a tempfile with '.mzn' extension
        let write_model_to_tempfile (model: Model) : FileInfo =
            let path = Path.GetTempFileName()
            let path = Path.ChangeExtension(path, ".mzn")
            let mzn = model.Encode()
            File.WriteAllText(path, mzn)
            let file = FileInfo path
            file
        
        /// Create a cli arg for the given model file
        let model_arg (filepath: string) : Arg =
            let uri = Uri(filepath).AbsolutePath
            let arg = Arg.parse $"--model {uri}"
            arg
               
        /// Solve the given model                            
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
        let solveAndWait (model: Model) (client: MiniZincClient) =
            
            let task =
                client
                |> solve model
                |> TaskSeq.last
                
            let result =
                task.Result
                
            result                
            
        /// Analyse the given model
        let model_types (model: Model) (client: MiniZincClient) =
            let model_file = write_model_to_tempfile model
            let model_arg = model_arg model_file.FullName
            let command = client.Command(model_arg, "--model-types-only")
            let result =
                command
                |> Command.exec
                |> Task.map (fun res -> res.StdOut)
                |> Task.map JsonObject.Parse
            result
            
        /// Analyse the given model
        let model_interface (model: Model) (client: MiniZincClient) =
            let model_file = write_model_to_tempfile model
            let model_arg = model_arg model_file.FullName
            let command = client.Command(model_arg, "--model-interface-only")
            let result =
                command
                |> Command.exec
                |> Task.map (fun res -> res.StdOut)
                |> Task.map JsonObject.Parse
            result