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
            
            let compiled = client.Compile(model)
            let command =
                client.Command(compiled.ModelArg, "--model-interface-only")
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
                    let iface = JsonSerializer.Deserialize<ModelInterface>(stdout, options)
                    iface
                    )
                
            result                


    type MiniZincClient with
                
        member this.GetModelInterface(model: Model) =
            MiniZincClient.modelInterface model this