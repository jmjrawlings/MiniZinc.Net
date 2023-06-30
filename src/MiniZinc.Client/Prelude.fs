namespace MiniZinc

open System.Collections.Generic
open System.Runtime.CompilerServices
open System.Text.Json
open System.Text.Json.Nodes
open System.Text.Json.Serialization
open System.Text.RegularExpressions
open System.Threading.Tasks
open FSharp.Control
open System

[<AutoOpen>]
module Prelude =
    
    [<Extension>]        
    type Extensions =
        
        [<Extension>]
        static member AsAsync(task: Task<'t>) =
            task
            |> Async.AwaitTask
            
        [<Extension>]
        static member Get(result: Result<'ok, 'err>) =
            Result.get result
            
        [<Extension>]
        static member OrElse(opt, backup) =
            Option.defaultValue backup opt
    
    type JsonTypes =
        IReadOnlyDictionary<string, TypeInst>
    
    module Json =
        let tryGetValue<'t> (node: JsonNode) =
            let value = ref Unchecked.defaultof<'t>
            if node.AsValue().TryGetValue(value) then
                Some value.Value
            else
                None
                
        let tryGetValueOr<'t> backup node =
            node
            |> tryGetValue<'t>
            |> Option.defaultValue backup
            
        let tryGetBool node =
            tryGetValueOr false node
            
        
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

    type TypeInstConverter() =
        inherit JsonConverter<TypeInst>()
        
        override this.Read(reader: byref<Utf8JsonReader>, _: System.Type, _: JsonSerializerOptions) =
            let dict = JsonSerializer.Deserialize<JsonObject>(&reader)
            
            let rec parse (obj: JsonObject) : TypeInst =
                
                let inst =
                    Inst.Par
                
                let isSet =
                    obj.TryGet("set")
                    |> Option.bind Json.tryGetValue<bool>
                    |> Option.defaultValue false
                    
                let isOpt =
                    obj.TryGet("opt")
                    |> Option.bind Json.tryGetValue<bool>
                    |> Option.defaultValue false
                    
                let type' =
                    match obj["type"].ToString() with
                    | "int" ->
                        Type.Int
                    | "bool" ->
                        Type.Bool
                    | "record" when obj.ContainsKey "field_types" -> 
                        let fields =
                            obj["field_types"].AsObject()
                            |> Seq.map (fun x ->
                                let fieldName = x.Key
                                let fieldType = x.Value.AsObject()
                                let fieldTi = parse fieldType
                                (fieldName, fieldTi))
                            |> Seq.toList
                            
                        Type.Record { Fields = fields }
                    | "record" ->
                        Type.Record { Fields = [] }
                    | "float" -> 
                        Type.Float
                    | "string" ->
                        Type.String
                    | x ->
                        failwith $"{x}"
                
                let isArray =
                    obj.TryGet("dim")
                    |> Option.bind Json.tryGetValue<int>
                    |> Option.map (fun n -> n > 0)
                    |> Option.defaultValue false
                    
                let arrayDims =
                    obj.TryGet("dims")
                    |> Option.map (fun node -> Seq.toList <| node.AsArray())
                    |> Option.defaultValue []
                    |> List.map (fun node ->
                        match string node with
                        | "int" -> Type.Int
                        | x -> Type.Id x
                        )
                        
                let baseTi =
                    { IsSet = isSet
                    ; IsOptional = isOpt
                    ; IsArray = false 
                    ; Inst = inst
                    ; Type = type' }
                        
                let ti =
                    match isArray with
                    | true ->
                        { IsSet = false
                        ; IsOptional = false
                        ; IsArray = true 
                        ; Inst = inst
                        ; Type = Type.Array { Elements = baseTi; Dimensions=[] } }
                    | false ->
                        baseTi
                ti
                
            let ti = parse dict
            ti

        override this.Write(writer: Utf8JsonWriter, value, _: JsonSerializerOptions) =
            writer.WriteStringValue("")


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

type TimePeriod =
        { StartTime : DateTimeOffset
        ; EndTime   : DateTimeOffset
        ; Duration  : TimeSpan }
        
        static member Since (startTime: DateTimeOffset) =
            let now = DateTimeOffset.Now
            let elapsed = now - startTime
            { StartTime = startTime
            ; EndTime = now
            ; Duration = elapsed }
            
        static member Since (period: TimePeriod) =
            TimePeriod.Since(period.EndTime)
            
        static member At time =
            { StartTime = time
            ; EndTime = time
            ; Duration = TimeSpan.Zero }
            
        static member Now =
            TimePeriod.At DateTimeOffset.Now
            
        static member Create(startTime, endTime) =
            if startTime <= endTime then
                { StartTime = startTime
                  EndTime = endTime
                  Duration = endTime - startTime  }        
            else
                { StartTime = endTime
                  EndTime = startTime
                  Duration = startTime - endTime  }
