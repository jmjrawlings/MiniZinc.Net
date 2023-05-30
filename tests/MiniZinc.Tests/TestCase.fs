(*
TestCase.fs

A module to load the official test suite to use
as integration tests.

TODO/JR

These example files should really be included as EmbeddedResources into
this assembly but I cannot for the life of me figure out why the '.mzn' files
come through properly but the '.model' files do not.

This does not work:
    <EmbeddedResource Include="../examples/*.*"/>
    
I cant be fucked anymore so we're just going to link back up the directory chain
to the examples folder.      

*)
namespace MiniZinc.Tests

#nowarn "0025"

open System
open System.Collections.Generic
open System.IO
open System.Reflection
open System.Text
open System.Text.Json
open MiniZinc
open YamlDotNet
open YamlDotNet.Core
open YamlDotNet.Core.Events
open YamlDotNet.RepresentationModel
open YamlDotNet.Serialization
open YamlDotNet.Serialization.NamingConventions

module rec TestCase =
    
    type TestCase =
        { Name : string
        ; Mzn : string
        ; Yaml : string
        ; FileName: string
        ; Tests: TestCaseTest list
        ; Model: LoadResult }
        
    type TestCaseSolution =
        Map<string, Yaml>

    type TestCaseResult =
        { Status : string
          Solutions : TestCaseSolution list }

    type TestCaseError =
        { Type : string
          Message : string
          Regex : string }
        
    type SolverId = string        

    type TestCaseTest =
        { Solvers : SolverId list
          CheckAgainst : SolverId list
          ExtraFiles : string list
          SolveOptions : Map<string, Yaml>
          Expected : TestCaseResult list }

    type MapNode =
        { Map : Map<string, Yaml> }
        
        member this.Get key =
            this.Map.Get(key)
            
        member this.TryGet key =
            this.Map.TryGet(key)
            
        member this.GetList key =
            match this.TryGet key with
            | Some (Yaml.YList xs) -> xs.List
            | _ -> []
        
        member this.GetMap key =
            match this.TryGet key with
            | Some (Yaml.NMap xs) -> xs.Map 
            | _ -> Map.empty
            
        member this.GetString key =
            match this.TryGet key with
            | Some (Yaml.YString x) -> x 
            | _ -> ""
            
        member this.GetInt key =
            match this.TryGet key with
            | Some (Yaml.YInt x) -> x 
            | _ -> 0
    
    type YamlList =
        | YList of Yaml list
        
        member this.List =
            match this with
            | YList xs -> xs
    
    type Yaml =
        | YString of string
        | YInt of int
        | YFloat of float
        | YDur of string
        | YList of YamlList
        | NMap of MapNode 
        | NTest of TestCaseTest
        | NSolution of TestCaseSolution
        | NResult of TestCaseResult
        
        member this.String =
            match this with | YString s -> s
            
        member this.Int =
            match this with | YInt s -> s
            
        member this.Float =
            match this with | YFloat s -> s
        
        member this.List =
            match this with | YList s -> s
            
        member this.Map =
            match this with | NMap s -> s
            
        member this.Solution =
            match this with | NSolution s -> s
            
        member this.Result =
            match this with | NResult s -> s
        

    /// Parse the current node
    let parseNode (parser: IParser) : Yaml =
        match parser.Current with
                            
        | :? MappingStart as x ->
            parseMap parser x
            
        | :? SequenceStart as x ->
            parseList parser x
            
        | :? Scalar as x ->
            parseScalar parser x
            
        | _ ->
            failwith "Unsupported YAML structure."
            
    /// Parse a Scalar
    let parseScalar (parser: IParser) (x: Scalar) : Yaml =
        let tag = x.Tag
        let value = x.Value
        let mutable float = 0.0
        let mutable int = 0
        
        let node =
            match string tag  with
            | "!Duration" ->
                Yaml.YDur value
                
            | _ when Int32.TryParse(value, &int) ->
                Yaml.YInt int
                
            | _ when Double.TryParse(value, &float) ->
                Yaml.YFloat float
                
            | _ ->
                Yaml.YString value
        node
    
    /// Parse a Map
    let parseMap (parser: IParser) (x: MappingStart) : Yaml =
        let dict = Dictionary<string, Yaml>()
        parser.MoveNext()
        while parser.Current.GetType() <> typeof<MappingEnd> do
                           
            let key = (parser.Current :?> Scalar).Value
            parser.MoveNext()
            
            let value = parseNode parser
            parser.MoveNext()
            
            dict[key] <- value
            
        let tag = x.Tag
        let map = { Map = Map.ofDict dict }
        let node =
            match tag.Value with
            | "!Test" ->
                parseTest map
            | "!Solution" ->
                Yaml.NSolution (parseSolution map)
            | "!Result" ->
                Yaml.NResult (parseResult map)
            | other ->
                Yaml.NMap map
        node
        
    let parseResult (map: MapNode) =
        
        let status =
            map.TryGet "status"
            |> Option.map (fun s -> s.String)
            |> Option.defaultValue "SATISFIED"
            
        let solution =
            map.Get "solution"
            
        let solutions = ResizeArray()
        
        let rec loop node =
            match node with
            | Yaml.NSolution sol ->
                solutions.Add sol
            | Yaml.YList xs ->
                for x in xs.List do
                    loop x
            | _ ->
                ()

        loop solution
        let solutions = Seq.toList solutions
        let result = 
            { Status = status
            ; Solutions = solutions }
            
        result            
        
    let parseTest (map: MapNode) =
        
        let solvers =
            map.GetList("solvers")
            |> List.map (fun f -> f.String)
        
        let checkAgainst =
            map.GetList("checkAgainst")
            |> List.map (fun x -> x.String)
            
        let extraFiles =
            map.GetList("extraFiles")
            |> List.map (fun x -> x.String)
        
        let solveOpts =
            map.GetMap("options")

        let expected =
            map.GetList("expected")
            |> List.map (function | Yaml.NResult res -> res )
        
        let node =
            { Solvers = solvers
            ; CheckAgainst = checkAgainst
            ; ExtraFiles = extraFiles
            ; Expected =  expected
            ; SolveOptions = solveOpts }
            |> Yaml.NTest
            
        node
        
    let parseSolution (map: MapNode) =
        map.Map
       
        
    /// Parse a Sequence            
    let parseList (parser: IParser) (x: SequenceStart) : Yaml =
        let tag = x.Tag
        let array = ResizeArray()
        parser.MoveNext()
        
        while parser.Current.GetType() <> typeof<SequenceEnd> do
            let item = parseNode parser
            parser.MoveNext()
            array.Add item
            
        let list = List.ofSeq array
        let node = Yaml.YList (YamlList.YList list)
        node
        
    type TestCaseParser() =
        
        let mutable Parser =
            Unchecked.defaultof<IParser>
        
        interface IYamlTypeConverter with
        
            member this.Accepts(t) =
                true

            member this.WriteYaml(emitter, value, typ) =
                ()

            member this.ReadYaml(parser, typ) =
                let node = parseNode parser
                node
                
    
    let deserializer =

        DeserializerBuilder()
            .WithTagMapping("!Test", typeof<obj>)
            .WithTagMapping("!Result", typeof<obj>)
            .WithTagMapping("!SolutionSet", typeof<obj>)
            .WithTagMapping("!Solution", typeof<obj>)
            .WithTagMapping("!Duration", typeof<obj>)
            .WithTypeConverter(TestCaseParser())
            .Build()
            
          
    let assembly_file =
        Assembly.GetExecutingAssembly().Location
        |> Path.GetFullPath
        |> FileInfo
        
    let examples_dir =
        assembly_file.Directory.Parent.Parent.Parent.Parent
        <//> "examples"


    /// Read the testcase from the given file
    let read (filename: string) =
        let filepath = examples_dir </> filename
        use reader = new StreamReader(filepath)        
        
        let header = reader.ReadLine()
        assert (header = "/***")
                
        let mutable stop = false
        let yaml = StringBuilder()
                
        while (not stop) do
            let line = reader.ReadLine()
            if line = "***/" then
                stop <- true
            else
                yaml.AppendLine line
                ()
                
        let mzn = reader.ReadToEnd()
        let yaml = string yaml
        let testCase = deserializer.Deserialize<TestCaseTest>(new StringReader(yaml))
        let name = Path.GetFileNameWithoutExtension filename
        let testCase =
            { Yaml = yaml
              Mzn = mzn
              FileName = filename
              Name = name
              Model = LoadResult.Reference
              Tests = [testCase] }

        testCase
        
        
    // Parse the testcase
    let parse (testCase: TestCase) =
        
        let includeOpts =
            IncludeOptions.ParseFile [examples_dir.FullName]
        
        let parseOpts =
            { ParseOptions.Default with 
                IncludeOptions =  includeOpts }
    
        let model =
            Model.parseString parseOpts testCase.Mzn

        { testCase with Model = model }
    
        
    