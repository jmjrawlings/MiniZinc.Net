namespace MiniZinc

open System.Collections.Generic
open System.Text.Json
open Command

type ModelTypes =
  { Vars: IReadOnlyDictionary<string, TypeInst> }

[<AutoOpen>]
module ModelTypes =
    
    /// Analyse the given model
    let getModelTypes (model: Model) (client: MiniZincClient) : Result<ModelTypes, string> =
        
        let compiled =
            client.Compile model

        let command =
            client.Command(compiled.ModelArg, "--model-types-only")
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
        
    module MiniZincClient =
        
        let modelTypes =
            getModelTypes

    type MiniZincClient with
        
        /// Get the models types as returned by the `--model-types-only` command
        member this.GetModelTypes(model: Model) =
            MiniZincClient.modelTypes model this