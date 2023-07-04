(*

Yaml.fs

Yaml parsing types and functions to allow the proper
parsing of MiniZinc test cases which are store as comments
in yaml format.

*)

namespace MiniZinc

open MiniZinc
open System
open System.Collections.Generic
open YamlDotNet.Core
open YamlDotNet.Core.Events
open YamlDotNet.Serialization
open System.IO

#nowarn "3391"

[<AutoOpen>]
module rec Yaml =
    
    type Yaml =
        | String   of string
        | Int      of int
        | Float    of float
        | Sequence of Yaml list
        | Duration of string
        | Mapping  of Map<string, Yaml>
        | Tagged   of string * Yaml
        | Null
        
        member this.Item
            with get key = Yaml.get key this
        
        member this.AsMap =
            Yaml.toMap this

        member this.AsList =
            Yaml.toList this
            
        member this.AsString =
            Yaml.toString this
            
        member this.AsFloat =
            Yaml.toFloat this
    
        member this.AsStringList =
            Yaml.toStringList this
            
        member this.AsIntList =
            Yaml.toIntList this
            
        member this.AsFloatList =
            Yaml.toFloatList this
            
        member this.AsListOf x =
            Yaml.toListOf x this
            
        member this.Get key =
            Yaml.get key this
            
        member this.TryGet key =
            match this.Get key with
            | Yaml.Null -> None
            | x -> Some x
    
    module Yaml =
        
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
                    String value
                
                | t when t = "!Duration" ->
                    Duration value
                    
                | t when (string t ) <> "?" ->
                    String value
                    
                | t when t = "!!set" ->
                    Duration value                
                    
                | _ when Int32.TryParse(value, &int) ->
                    Int int
                    
                | _ when Double.TryParse(value, &float) ->
                    Float float
                    
                | _ ->
                    String value
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
                    Mapping map
                | false ->
                    Tagged (tag.Value, Mapping map)
                
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
                    Sequence list
                | false ->
                    Tagged (tag.Value, Sequence list)
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
                
        let parseString (input: string) : Yaml option =
            let node = deserializer.Deserialize<Yaml>(input)
            match box node with
            | null -> None
            | _ -> Some node
            
        let parseFile (file: FileInfo) =
            file.FullName
            |> File.ReadAllText
            |> parseString

        let toList (yaml: Yaml) =
            match yaml with
            | Sequence xs -> xs
            | Null -> []
            | other -> [other]

        let toListOf f (yaml: Yaml) =
            yaml
            |> toList
            |> List.choose f
            
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
            
        let toStringList =
            toListOf toString
            
        let toIntList =
            toListOf toInt
            
        let toFloatList =
            toListOf toFloat
            
        let toMap (yaml: Yaml) =
            match yaml with
            | Mapping map -> map
            | _ -> Map.empty
                    
        let get key yaml =
            match yaml with
            | Tagged (tag, node) when tag = key ->
                node
            | Mapping map when map.ContainsKey key ->
                map[key]
            | _ ->
                Null
                
        let rec toExpr yaml =
            match yaml with
            | Yaml.String s ->
                Expr.String s
            | Yaml.Sequence xs ->
                match xs.Head with
                | Yaml.Sequence _ ->
                    xs
                    |> List.choose (function
                        | Yaml.Sequence x -> Some (List.map toExpr x)
                        | _ -> None)
                    |> Expr.Array2d
                | _ ->
                    xs
                    |> List.map toExpr
                    |> Expr.Array1d
                
            | Yaml.Int i ->
                Expr.Int i
            | _ ->
                notImpl "xd"