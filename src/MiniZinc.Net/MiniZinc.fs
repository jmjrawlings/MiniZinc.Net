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

[<AutoOpen>]
module rec Net =

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
        /// Create a minizinc command 
        /// </summary>
        static member Command() =
            Command.Create(executablePath)
               
        /// <summary>
        /// Create a minizinc command with the given arguments 
        /// </summary>
        static member Command([<ParamArray>] args: obj[]) =
            Command.Create(executablePath, args)

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
        
        let private model_arg (model: Model) =

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
                
            let arg = Arg.parse $"--model {model_uri}"
            arg
                            
        let solve (model: Model) : IAsyncEnumerable<OutputMessage> =
            
            let args = Args.Create("--json-stream")
            let model_arg = model_arg model
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
        let analyze (model: Model) =
            let model_arg = model_arg model
            let command = MiniZinc.Command("--model-types-only", model_arg)
            task {
                let! result = command.Exec()
                let json = JsonObject.Parse(result.StdOut)
                return json
            }