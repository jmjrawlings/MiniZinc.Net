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
    type MiniZincClient internal (executable: string, version:string, logger: ILogger) as this =
                
        let logger =
            match logger with
            | null ->
                NullLoggerFactory.Instance.CreateLogger() :> ILogger
            | log -> log
            
        let mutable solvers = Map.empty
        
        do this.GetSolvers()
                     
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
                    
            let result =
                MiniZincClient.create logger executable
                
            let client =
                result.Get()
                
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
        static member Create(logger: ILogger) =
            MiniZincClient.Create(null, logger)
            
        /// Create a new Client 
        static member Create() =
            MiniZincClient.Create(null, null)
               
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
        member public this.GetSolvers () =
                        
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
            
        override this.ToString() =
            $"MiniZinc v{version} Client"
            
            
    module MiniZincClient =
        
        /// Create a new MiniZinc client from the given executable
        let create (logger: ILogger) (executable: string) : Result<MiniZincClient, string> =
                       
            let client =
                match Command.Create(executable, "--version").RunSync().ToResult() with
                | Result.Ok stdout ->
                    
                    let version =
                        stdout
                        |> Grep.matches @"version (\d+\.\d+\.\d+)"
                        |> List.head
                        
                    MiniZincClient(executable, version, logger)
                    |> Result.Ok
                    
                | Result.Error err -> Result.Error err
                    
            client
        
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