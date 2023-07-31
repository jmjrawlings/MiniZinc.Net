namespace MiniZinc

open System
open System.IO
open System.Text.Json
open System.Text.Json.Nodes
open System.Collections.Generic
open Microsoft.Extensions.Logging
open Microsoft.Extensions.Logging.Abstractions

open Command

/// <summary>
/// A backend Solver supported by MiniZinc
/// </summary>
/// <remarks>
/// Reflects the information provided by `minizinc --solvers`
/// </remarks>
type Solver =
    { Id                 : string
    ; Name               : string
    ; Description        : string
    ; Version            : string
    ; MznLib             : string
    ; MznLibVersion      : int
    ; Executable         : string
    ; SupportsMzn        : bool
    ; SupportsFzn        : bool
    ; SupportsNL         : bool
    ; NeedsSolns2Out     : bool 
    ; NeedsMznExecutable : bool
    ; NeedsStdlibDir     : bool
    ; NeedsPathsFile     : bool
    ; IsGUIApplication   : bool
    ; ExtraInfo          : JsonObject
    ; Tags               : IReadOnlyList<string>
    ; StdFlags           : IReadOnlyList<string>    
    ; RequiredFlags      : IReadOnlyList<string>    
    ; ExtraFlags         : IReadOnlyList<IReadOnlyList<string>> }

type CompiledModel =
    { Model       : Model
    ; ModelString : string
    ; ModelFile   : FileInfo
    ; ModelArg    : Arg
    ; Warnings    : string list
    ; Errors      : string list }

[<AutoOpen>]
module rec Client =
    
    /// <summary>
    /// A MiniZinc CLI Client 
    /// </summary>
    type MiniZincClient internal (
            executable: string,
            path: FileInfo,
            version:string,
            logger: ILogger<MiniZincClient>) as this =
            
        let mutable solvers = Map.empty
        
        do
            this.GetSolvers()

        member this.Logger =
            logger
                
        /// The MiniZinc executable 
        member this.Executable =
            executable
            
        /// The full path to the executable 
        member this.Path =
            path
            
        /// The executable directory
        member this.Directory =
            path.Directory
                    
        /// The MiniZinc CLI Version
        member this.Version =
            version
                     
        /// Installed solvers by their ID
        member this.Solvers =
            solvers :> IReadOnlyDictionary<string, Solver>
            
        /// <summary>
        /// Create a new Client
        /// </summary>
        /// <param name="executable">The MiniZinc executable path</param>
        /// <param name="logger">A logger to write to</param>
        static member Create(executable: string, logger: ILogger<MiniZincClient>) =
            match MiniZincClient.create logger executable with
            | Result.Ok client ->
                client
            | Result.Error err ->
                let exn = MiniZincException("Could not create a miniznc client", err)
                raise exn

        /// <summary>
        /// Create a new Client
        /// </summary>
        /// <param name="executable">The MiniZinc executable path</param>        
        static member Create(executable: string) =
            MiniZincClient.Create(executable, null)

        /// <summary>
        /// Create a new Client 
        /// </summary>
        static member Create(logger: ILogger<MiniZincClient>) =
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
                
            let command =
                Command.Create(executable, args)
                
            logger.LogInformation(command.Statement)
                
            command

        /// Run a command with the given arguments
        member this.Run([<ParamArray>] args: obj[]) =
            let command = this.Command(args)
            let result = command.Run()
            result
            
        /// Run a command with the given arguments
        member this.RunSync([<ParamArray>] args: obj[]) =
            let command = this.Command(args)
            let result = command.RunSync()
            result
        
        /// Get all installed solvers
        member public this.GetSolvers () =
                        
            let stdout =
                this.RunSync("--solvers-json").StdOut
               
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
            
        member this.Compile(model: Model) =
            MiniZincClient.compile model this
            
        override this.ToString() =
            $"MiniZinc Client v{version}"
            
    module MiniZincClient =
        
        /// Create a new MiniZinc client from the given executable and logger
        let create (logger: ILogger<MiniZincClient>) (executable: string) : Result<MiniZincClient, string> =
                            
            let logger =
                match logger with
                | null ->
                    NullLoggerFactory.Instance.CreateLogger<MiniZincClient>()
                | log -> log
                
            let executable =
                match executable with
                | _ when String.IsNullOrEmpty(executable) ->
                    "minizinc"
                | other ->
                    other
                    
            let path =
                FileInfo executable
                       
            let client =
                Command.Create(executable, "--version")
                |> Command.runSync
                |> Command.map (fun result ->
                    
                    let version =
                        result.StdOut
                        |> Grep.matches @"version (\d+\.\d+\.\d+)"
                        |> List.head
                        
                    MiniZincClient(executable, path, version, logger)
                )
                    
            client
        
        /// Compile a model
        let compile (model: Model) (client: MiniZincClient) : CompiledModel =
                                
            let nameSpaceBindings = model.NameSpace.Bindings
            client.Logger.LogInformation(
                "Compiling model {Name} with {Bindings} bindings and {Constraints} constraints",
                model.Name,
                model.NameSpace.Bindings.Count,
                model.Constraints.Length)
                
            let result =
                model.Compile()
                
            client.Logger.LogInformation(
                "Compilation produced {Warns} warnings and {Errors} errors",
                result.Warnings.Length,
                result.Errors.Length)

            let modelFile =
                let tmp = Path.GetTempFileName()
                let mzn = Path.ChangeExtension(tmp, ".mzn")
                File.WriteAllText (mzn,result.ModelString)
                FileInfo mzn
                
            let arg =
                let uri = Uri(modelFile.FullName).AbsolutePath
                Arg.parse $"--model {uri}"
                
            client.Logger.LogInformation("Compiled model to {Path}", modelFile.FullName)
                
            let compiled =
                { Model = model
                ; ModelString = result.ModelString
                ; ModelFile = modelFile
                ; ModelArg = arg
                ; Warnings = result.Warnings
                ; Errors = result.Errors }
                
            compiled