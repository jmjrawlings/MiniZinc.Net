namespace MiniZinc

open MiniZinc.Parser

[<AutoOpen>]
module Include =
        
    module Model =
        
        /// Include the model from the given filepath by parsing it
        /// and merging the results with the source model.
        let includeFrom (options: ParseOptions) (filepath: string)  (model: Model) =
            
            let item =
                IncludeItem.Create filepath
            
            let integrated =
                item.File
                |> Option.map (fun fi -> fi.FullName)
                |> Option.defaultValue ""
                |> parseModelFromFile options
                |> Result.mapError (fun e -> e.Message)
                |> Result.map (fun m ->
                    let merged = Model.merge model m
                    merged
                    )
                
            integrated
                
    type Model with

        member this.Include(filepath: string) =
             Model.includeFrom ParseOptions.Default filepath
             
        member this.Include(filepath: string, parseOptions: ParseOptions) =
             Model.includeFrom parseOptions filepath