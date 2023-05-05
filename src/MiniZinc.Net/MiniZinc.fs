namespace MiniZinc

open System
open System.Diagnostics
open System.IO
open System.Text
open System.Text.Json
open System.Text.Json.Nodes
open System.Threading.Tasks
open FSharp.Control
open System.Collections.Generic
open System.Threading.Channels
open MiniZinc.Command
open MiniZinc.Model

[<AutoOpen>]
module rec Net =

    
    type MiniZinc() =
            
        static let mutable executablePath =
            "minizinc"
          
        static member ExecutablePath
            with get() =
                executablePath
            and set value =
                executablePath <- value
        
        /// <summary>
        /// Create a minizinc command 
        /// </summary>
        static member Command() =
            Command.Create(executablePath)
            
        /// <summary>
        /// Create a minizinc command with the given model
        /// and extra args 
        /// </summary>
        static member Command(model: Model, [<ParamArray>] args: obj[]) =
            let model_file = MiniZinc.write_model_to_tempfile model
            let model_arg = MiniZinc.model_arg model_file
            let command = MiniZinc.Command().AddArgs(model_arg)
            command
               
        /// <summary>
        /// Create a minizinc command with the given arguments 
        /// </summary>
        static member Command([<ParamArray>] args: obj[]) =
            MiniZinc.Command().AddArgs(args)

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
            
    module MiniZinc =
        
        // Write the given model to a tempfile with '.mzn' extension
        let internal write_model_to_tempfile (model: Model) : FileInfo =
            let path = Path.GetTempFileName()
            let path = Path.ChangeExtension(path, ".mzn")
            File.WriteAllText(path, model.String)
            let file = FileInfo path
            file
        
        // Create a cli Arg for the given model file
        let internal model_arg (file: FileInfo) : Arg =
            let uri = Uri(file.FullName).AbsolutePath
            let arg = Arg.parse $"--model {uri}"
            arg
                            
        let solve (model: Model) : IAsyncEnumerable<OutputMessage> =
            
            let args = Args.Create("--json-stream")
            let model_file = write_model_to_tempfile model
            let model_arg = model_arg model_file
            let args = args.Append(model_arg)
            let command = MiniZinc.Command(args)
                
            command
            |> Command.stream
            |> TaskSeq.choose (function
                | CommandMessage.Output msg ->
                    Some msg
                | _ ->
                    None)
            
        /// <summary>
        /// Solve the given model and wait
        /// for the best solution only
        /// </summary>
        let exec (model: Model) =
            model
            |> solve
            |> TaskSeq.last
            
        /// <summary>
        /// Analyse the given model
        /// </summary>
        let model_types (model: Model) =
            let command =
                MiniZinc.Command(model).AddArgs("--model-types-only")
            task {
                let! result = command.Exec()
                let json = JsonObject.Parse(result.StdOut)
                return json
            }
            
        /// <summary>
        /// Analyse the given model
        /// </summary>
        let model_interface (model: Model) =
            let command = MiniZinc.Command(model).AddArgs("--model-interface-only")
            let result = command.Exec()
            result
