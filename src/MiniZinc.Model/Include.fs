namespace MiniZinc

open System
open System.IO

type IncludeOption =
    | Reference = 1
    | Isolated = 2
    | Integrated = 3

[<AutoOpen>]
module rec Include =
    
    /// Include the model from the given filepath via its name only
    let includeModelAsReference (filepath: string) (model: Model) =
        
        match model.Includes.TryFind filepath with
        | Some _ ->
            model
        | None ->
            { model with
                Includes =
                    Map.add filepath IncludedModel.Reference model.Includes  }

    /// Include the model from the given filepath by parsing it
    /// and storing it in isolation within the parent model.
    let includeModelAsIsolated (filepath: string) (model: Model) =
        
        let result =
            match parseModelFile filepath with
            | Result.Ok m ->
                let included = IncludedModel.Isolated m
                let includes = Map.add filepath included model.Includes 
                { model with Includes = includes }
            | Result.Error err ->
                failwith $"{err.Message}"
                
        result
        
    /// Include the model from the given filepath by parsing it
    /// and merging the results with the source model.
    let includeModelAsIntegrated (filepath: string) (model: Model) =
        
        let result =
            match parseModelFile filepath with
            | Result.Ok m ->
                let merged = Model.merge model m
                let included = IncludedModel.Integrated m
                let includes = Map.add filepath included merged.Includes
                { merged with Includes = includes }
            | Result.Error err ->
                failwith $"{err.Message}"

        result
        
    let includeModel (option: IncludeOption) filepath model =
        match option with
        | IncludeOption.Isolated ->
            includeModelAsIsolated filepath model
        | IncludeOption.Integrated ->
            includeModelAsIntegrated filepath model
        | _ ->
            includeModelAsReference filepath model
        
        
    module Model =
            
        let includeAs option filepath model =
            includeModel option filepath model
            
            
    type Model with
    
        member this.Include(filepath: string, option: IncludeOption) =
            let result = includeModel option filepath this
            result
            
        member this.Include(filepath: string) =
            includeModelAsReference filepath this