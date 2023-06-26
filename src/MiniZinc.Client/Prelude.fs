namespace MiniZinc

open System.Runtime.CompilerServices
open System.Text.Json
open System.Text.Json.Serialization
open System.Text.RegularExpressions
open System.Threading.Tasks
open FSharp.Control

[<AutoOpen>]
module Prelude =
    
    type JsonTypeName =
        | Int 
        | Record
        | Bool 
        | String
        | Float
        
    type SolveMethodConverter() =
        inherit JsonConverter<SolveMethod>()
        
        override this.Read(reader: byref<Utf8JsonReader>, _: System.Type, _: JsonSerializerOptions) =
            match reader.TokenType with
            | JsonTokenType.String ->
                match reader.GetString() with
                | "min" -> SolveMethod.Minimize
                | "max" -> SolveMethod.Maximize
                | _     -> SolveMethod.Satisfy
            | _ ->
                raise (JsonException("Expected a string."))

        override this.Write(writer: Utf8JsonWriter, value, _: JsonSerializerOptions) =
            let string =
                match value with
                | SolveMethod.Minimize -> "min"
                | SolveMethod.Maximize -> "max"
                | SolveMethod.Satisfy -> "sat"
                | _ -> "sat"
            writer.WriteStringValue(string)

    type ModelInterfaceTypeNameConverter() =
        inherit JsonConverter<JsonTypeName>()
        
        override this.Read(reader: byref<Utf8JsonReader>, _: System.Type, _: JsonSerializerOptions) =
            match reader.TokenType with
            | JsonTokenType.String ->
                match reader.GetString() with
                | "int" -> JsonTypeName.Int
                | "record" -> JsonTypeName.Record
                | "bool" -> JsonTypeName.Bool
                | "string" -> JsonTypeName.String
                | "float" -> JsonTypeName.Float
                | e -> failwith e
                
            | _ ->
                raise (JsonException("Expected a string."))

        override this.Write(writer: Utf8JsonWriter, value, _: JsonSerializerOptions) =
            let string =
                match value with
                | JsonTypeName.Int -> "int"
                | JsonTypeName.Record -> "record"
                | JsonTypeName.Bool -> "bool"
                | JsonTypeName.String -> "string"
                | JsonTypeName.Float -> "float"
                
            writer.WriteStringValue(string)


module Grep =
    
    // Returns matches for the given regex
    let matches (pattern: string) (input: string) =
                
        let regex = Regex pattern
        
        let values =
            match regex.Match input with
            | m when m.Success ->
                Seq.toList m.Groups
            | _ ->
                []
            |> List.skip 1 
            |> List.map (fun c -> c.Value)

        values
    
    
module Task =
    
    let map f a =
        task {
            let! value = a
            return f value
        }    
    
        
[<Extension>]        
type Extensions =
    
    [<Extension>]
    static member AsAsync(task: Task<'t>) =
        task
        |> Async.AwaitTask
        
    [<Extension>]
    static member Get(result: Result<'ok, 'err>) =
        Result.get result