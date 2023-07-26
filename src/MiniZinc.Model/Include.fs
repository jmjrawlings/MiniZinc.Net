namespace MiniZinc

open System
open System.IO

type IncludeOption =
    | Reference = 1
    | Integrated = 2
    

[<AutoOpen>]
module rec Include =
        
    module Model =
        
        /// Include the model from the given filepath by parsing it
        /// and merging the results with the source model.
        let integrate (filepath: string) (model: Model) =
            
            let item =
                IncludeItem.Create filepath
            
            let integrated =
                item.File
                |> Option.map (fun fi -> fi.FullName)
                |> Option.defaultValue ""
                |> parseModelFile
                |> Result.mapError (fun e -> e.Message)
                |> Result.map (fun m ->
                    let merged = Model.merge model m
                    merged
                    )
                
            integrated
            
        let includeReference (path: string) (model: Model) =
            let item = IncludeItem.Create path
            match model.Includes.TryFind item.Name with
            // The model has already been included
            | Some _ ->
                model
            // Add a new inclusion
            | None ->
                { model with
                    Includes =
                        model.Includes.Add (item.Name, item) 
                   }
            
        let includeAs (option: IncludeOption) (path: string) (model: Model) =
            match option with
            | IncludeOption.Integrated ->
                integrate path model
            | IncludeOption.Reference ->
                includeReference path model
                |> Result.Ok
            | _ ->
                failwith "xd"
            
    type Model with
    
        member this.Include(filepath: string, option: IncludeOption) =
            Model.includeAs option filepath this
            
        member this.Include(filepath: string) =
             Model.includeReference filepath
             
        member this.Integrate(filepath: string) =
             Model.integrate filepath