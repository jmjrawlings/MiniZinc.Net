namespace MiniZinc

open System.Collections.Generic
open System.Text.Json
open System.Text.Json.Serialization
    

type ModelTypes =
  { Vars: IReadOnlyDictionary<string, TypeInst> }
 

[<AutoOpen>]
module ModelTypes =
    open Command
        
    module MiniZincClient =
        
        /// Analyse the given model
        let modelTypes (model: Model) (client: MiniZincClient) : Result<ModelTypes, string> =
            
            let model_file =
                MiniZincClient.compile model
                
            let model_arg =
                MiniZincClient.model_arg model_file.FullName

            let command =
                client.Command(model_arg, "--model-types-only")
                |> Command.runSync
                
            let options =
                let opts = JsonSerializerOptions()
                opts.PropertyNameCaseInsensitive <- true
                opts.Converters.Add(SolveMethodConverter())
                opts.Converters.Add(TypeInstConverter())
                opts
                
            let result =
                command
                |> Command.map (fun result ->
                    let stdout = result.StdOut
                    let doc = JsonDocument.Parse(stdout)
                    let root = doc.RootElement.GetProperty("var_types")
                    let types = JsonSerializer.Deserialize<ModelTypes>(root, options)
                    types
                    )
                
            result
            

    type MiniZincClient with
                
        member this.GetModelTypes(model: Model) =
            MiniZincClient.modelTypes model this