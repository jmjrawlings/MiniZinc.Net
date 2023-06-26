namespace MiniZinc

open System
open System.IO
open System.Text.Json
open System.Text.Json.Nodes
open System.Collections.Generic
open Microsoft.Extensions.Logging
open Microsoft.Extensions.Logging.Abstractions

open Command

/// An installed MiniZinc solver
type Solver =
    { Id                 : string
    ; Name               : string
    ; Version            : string
    ; MznLib             : string
    ; Executable         : string
    ; Tags               : IReadOnlyList<string>
    ; SupportsMzn        : bool
    ; SupportsFzn        : bool
    ; SupportsNL         : bool
    ; NeedsSolns2Out     : bool 
    ; NeedsMznExecutable : bool
    ; NeedsStdlibDir     : bool
    ; NeedsPathsFile     : bool
    ; IsGUIApplication   : bool
    ; MznLibVersion      : int
    ; ExtraInfo          : JsonObject
    ; Description        : string
    ; StdFlags           : IReadOnlyList<string>    
    ; RequiredFlags      : IReadOnlyList<string>    
    ; ExtraFlags         : IReadOnlyList<IReadOnlyList<string>> }


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
                |> Command.runSync
                |> Command.stdout
               
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
                |> Command.runSync
                |> Command.stdout
                |> Grep.matches @"version (\d+\.\d+\.\d+)"
                |> List.head
                
            logger.LogInformation("MiniZinc is v{Version}", result)
            version <- result
            
        override this.ToString() =
            $"MiniZinc v{version} Client"
            
            
    module MiniZincClient =
        
        /// Write the given model to a tempfile with '.mzn' extension
        let write_model_to_tempfile (model: Model) : FileInfo =
            
            let path =
                let tmp = Path.GetTempFileName()
                Path.ChangeExtension(tmp, ".mzn")
                
            let mzn = model.Encode()
            File.WriteAllText(path, mzn)
            FileInfo path
        
        /// Create a cli arg for the given model file
        let model_arg (filepath: string) : Arg =
            let uri = Uri(filepath).AbsolutePath
            let arg = Arg.parse $"--model {uri}"
            arg