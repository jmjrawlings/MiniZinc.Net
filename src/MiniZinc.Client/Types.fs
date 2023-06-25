namespace MiniZinc

open System.Collections.Generic
open System.Text.Json.Nodes
open System.Text.Json
open System.Text.Json.Serialization

/// An installed MiniZinc solver
type Solver = {
    Id                  : string
    Name                : string
    Version             : string
    MznLib              : string
    Executable          : string
    Tags                : IReadOnlyList<string>
    SupportsMzn         : bool
    SupportsFzn         : bool
    SupportsNL          : bool
    NeedsSolns2Out      : bool 
    NeedsMznExecutable  : bool
    NeedsStdlibDir      : bool
    NeedsPathsFile      : bool
    IsGUIApplication    : bool
    MznLibVersion       : int
    ExtraInfo           : JsonObject
    Description         : string
    StdFlags            : IReadOnlyList<string>    
    RequiredFlags       : IReadOnlyList<string>    
    ExtraFlags          : IReadOnlyList<IReadOnlyList<string>>
}

/// <summary>
/// Model Interface as returned when using the `--model-interface-only` flag
/// </summary>
type ModelInterface =
  { Input       : IReadOnlyDictionary<string, ModelInterfaceType>
  ; Output      : IReadOnlyDictionary<string, ModelInterfaceType>
  ; SolveMethod : SolveMethod
  ; Includes    : IReadOnlyList<string>
  ; Globals     : IReadOnlyList<string> }
  
and ModelInterfaceType =
  { Type : ModelInterfaceTypeName
  ; Dim  : int }
  
and ModelInterfaceTypeName =
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
    inherit JsonConverter<ModelInterfaceTypeName>()
    
    override this.Read(reader: byref<Utf8JsonReader>, _: System.Type, _: JsonSerializerOptions) =
        match reader.TokenType with
        | JsonTokenType.String ->
            match reader.GetString() with
            | "int" -> ModelInterfaceTypeName.Int
            | "record" -> ModelInterfaceTypeName.Record
            | "bool" -> ModelInterfaceTypeName.Bool
            | "string" -> ModelInterfaceTypeName.String
            | "float" -> ModelInterfaceTypeName.Float
            | e -> failwith e
            
        | _ ->
            raise (JsonException("Expected a string."))

    override this.Write(writer: Utf8JsonWriter, value, _: JsonSerializerOptions) =
        let string =
            match value with
            | ModelInterfaceTypeName.Int -> "int"
            | ModelInterfaceTypeName.Record -> "record"
            | ModelInterfaceTypeName.Bool -> "bool"
            | ModelInterfaceTypeName.String -> "string"
            | ModelInterfaceTypeName.Float -> "float"
            
        writer.WriteStringValue(string)