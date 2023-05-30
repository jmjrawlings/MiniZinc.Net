(*

Yaml.fs

Yaml parsing types and functions to allow the proper
parsing of MiniZinc test cases which are store as comments
in yaml format.

*)

namespace MiniZinc.Tests

open System.Security.Cryptography
open MiniZinc
open System
open System.Collections.Generic
open YamlDotNet.Core
open YamlDotNet.Core.Events
open YamlDotNet.Serialization

#nowarn "3391"

module rec Yaml =
    
    let toList (yaml: Yaml) =
        match yaml with
        | Sequence xs -> xs | _ -> []

    let toListOf (f: Yaml -> 'a option) (yaml: Yaml) =
        yaml
        |> toList
        |> List.map (fun i ->
            match f i with
            | Some v -> v
            | None -> failwith "xd")
        
    let toString (yaml: Yaml) =
        match yaml with
        | String s -> Some s
        | _ -> None
        
    let toInt (yaml: Yaml) =
        match yaml with
        | Int x -> Some x
        | _ -> None
        
    let toFloat (yaml: Yaml) =
        match yaml with
        | Float f -> Some f
        | _ -> None
        
    let toMap (yaml: Yaml) =
        match yaml with
        | Mapping map -> map
        | _ -> Map.empty
        
    let tryGet key (yaml: Yaml) =
        match yaml with
        | Tagged (tag, node) when tag = key ->
            Some node
        | Mapping map when map.ContainsKey key ->
            Some map[key]
        | _ ->
            None
                
    let get key yaml =
        match tryGet key yaml with
        | Some v -> v
        | None -> failwith "xd"
    
    type Yaml =
        | String    of string
        | Int       of int
        | Float     of float
        | Sequence  of Yaml list
        | Duration  of string
        | Mapping   of Map<string, Yaml>
        | Tagged    of string * Yaml
        
        member this.Item
            with get key = get key this
            

    /// Parse the current node
    let parseNode (parser: IParser) : Yaml =
        match parser.Current with
                            
        | :? MappingStart as x ->
            parseMapping parser x
            
        | :? SequenceStart as x ->
            parseSequence parser x
            
        | :? Scalar as x ->
            parseScalar parser x
            
        | _ ->
            failwith "Unsupported YAML structure."
            
    /// Parse a Scalar
    let parseScalar (parser: IParser) (event: Scalar) : Yaml =
        let value = event.Value
        let tag = event.Tag
        let mutable float = 0.0
        let mutable int = 0
        let scalar =
            match tag with
            
            | _ when event.IsKey ->
                Yaml.String value
            
            | t when t = "!Duration" ->
                Yaml.Duration value
                
            | _ when Int32.TryParse(value, &int) ->
                Yaml.Int int
                
            | _ when Double.TryParse(value, &float) ->
                Yaml.Float float
                
            | _ ->
                Yaml.String value
        scalar
    
    /// Parse a Map
    let parseMapping (parser: IParser) (event: MappingStart) : Yaml =
        let dict = Dictionary<string, Yaml>()
        parser.MoveNext()
        while parser.Current.GetType() <> typeof<MappingEnd> do
                           
            let key = (parser.Current :?> Scalar).Value
            parser.MoveNext()
            
            let value = parseNode parser
            parser.MoveNext()
            
            dict[key] <- value

        let map = Map.ofDict dict
        let tag = event.Tag
        let node =
            match tag.IsEmpty with
            | true ->
                Yaml.Mapping map
            | false ->
                Yaml.Tagged (tag.Value, Yaml.Mapping map)
            
        node
        
    /// Parse a Sequence            
    let parseSequence (parser: IParser) (x: SequenceStart) : Yaml =
        let array = ResizeArray()
        parser.MoveNext()
        
        while parser.Current.GetType() <> typeof<SequenceEnd> do
            let item = parseNode parser
            parser.MoveNext()
            array.Add item
            
        let list = List.ofSeq array
        let tag = x.Tag
        let node =
            match tag.IsEmpty with
            | true -> 
                Yaml.Sequence list
            | false ->
                Yaml.Tagged (tag.Value, Yaml.Sequence list)
        node
        
    type Parser() =
        
        let mutable Parser =
            Unchecked.defaultof<IParser>
        
        interface IYamlTypeConverter with
        
            member this.Accepts(t) =
                true

            member this.WriteYaml(emitter, value, typ) =
                ()

            member this.ReadYaml(parser, typ) =
                let node = parseNode parser
                parser.MoveNext()
                node


    let deserializer =
        DeserializerBuilder()
            .WithTagMapping("!Test", typeof<obj>)
            .WithTagMapping("!Result", typeof<obj>)
            .WithTagMapping("!SolutionSet", typeof<obj>)
            .WithTagMapping("!Solution", typeof<obj>)
            .WithTagMapping("!Duration", typeof<obj>)
            .WithTypeConverter(Parser())
            .Build()
            
    let parse (input: string) =
        let node = deserializer.Deserialize<Yaml>(input)
        node
