namespace MiniZinc

open System.Collections.Generic
open System.Text.Json

/// Model Interface as returned when using the `--model-interface-only` flag
type ModelInterface =
  { Input       : JsonTypes
  ; Output      : JsonTypes
  ; SolveMethod : SolveMethod
  ; Includes    : IReadOnlyList<string>
  ; Globals     : IReadOnlyList<string> }

[<AutoOpen>]
module ModelInterface =
    open Command
        
    module MiniZincClient =
        
        /// Analyse the given model
        let modelInterface (model: Model) (client: MiniZincClient) : Result<ModelInterface, string> =
            
            let model_file =
                MiniZincClient.write_model_to_tempfile model
                
            let model_arg =
                MiniZincClient.model_arg model_file.FullName
                
            let command =
                client.Command(model_arg, "--model-interface-only")
                |> Command.runSync
                
            let options =
                let opts = JsonSerializerOptions()
                opts.PropertyNameCaseInsensitive <- true
                opts.Converters.Add(SolveMethodConverter())
                opts.Converters.Add(ModelInterfaceTypeNameConverter())
                opts
                
            let result =
                command
                |> Command.toResult
                |> Result.map (fun stdout ->
                    JsonSerializer.Deserialize<ModelInterface>(stdout, options)
                    )
                
            result

    type MiniZincClient with
                
        member this.GetModelInterface(model: Model) =
            MiniZincClient.modelInterface model this