namespace MiniZinc

open System.Collections.Generic
open System.Text.Json
open System.Text.Json.Serialization

type JsonType =
  { [<JsonPropertyName("type")>]
    TypeName : JsonTypeName
    [<JsonPropertyName("dim")>]
    Dim : int
    [<JsonPropertyName("dims")>]
    Dimensions : IReadOnlyList<string>
    [<JsonPropertyName("set")>]
    IsSet : bool
    [<JsonPropertyName("opt")>]
    IsOpt : bool
    [<JsonPropertyName("field_types")>]
    FieldTypes : JsonTypes }
  
and JsonTypes =
    IReadOnlyDictionary<string, JsonType>

type ModelTypes =
  {
    Vars: JsonTypes
  }

[<AutoOpen>]
module ModelTypes =
    open Command
        
    module MiniZincClient =
        
        /// Analyse the given model
        let modelTypes (model: Model) (client: MiniZincClient) =
            
            let model_file =
                MiniZincClient.write_model_to_tempfile model
                
            let model_arg =
                MiniZincClient.model_arg model_file.FullName

            let command =
                client.Command(model_arg, "--model-types-only")
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
                    let doc = JsonDocument.Parse(stdout)
                    let root = doc.RootElement.GetProperty("var_types")
                    let types = JsonSerializer.Deserialize<ModelTypes>(root, options)
                    types
                    )
                
            result
            

    type MiniZincClient with
                
        member this.GetModelTypes(model: Model) =
            MiniZincClient.modelTypes model this
