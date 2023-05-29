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
open YamlDotNet.Serialization
open YamlDotNet.Serialization.NamingConventions

type TestCase =
    { FileName: string
    ; FileInfo: FileInfo
    ; Mzn : string
    ; Spec: string
    ; Model: LoadResult }

module TestCase =
            
    [<CLIMutable>]
    type TestCaseDuration =
        { Time : string }

    [<CLIMutable>]
    type TestCaseSolution =
        { _output_item : string }

    [<CLIMutable>]
    type TestCaseResult =
        { status : string
          solution : TestCaseSolution }

    [<CLIMutable>]
    type TestCaseError =
        { Type : string
          message : string
          regex : string }

    [<CLIMutable>]
    type TestCaseTest =
        { solvers : List<string>
          check_against : List<string>
          extra_files : List<string>
          options : Dictionary<string, obj>
          expected : List<obj> }  // Use 'obj list' to handle both 'Result' and 'Error' types.

    type YamlNode =
        | String of string
        | Int of int
        | Float of float
        | List of YamlNode list
        | Map of Map<string, YamlNode>

    let rec parseYaml (parser: IParser) =
        match parser.Current with
        | :? MappingStart as x ->
            parseMap x parser
            |> YamlNode.Map
        | :? SequenceStart as x ->
            parseList x parser
            |> YamlNode.List
        | :? Scalar as x ->
            parseScalar x
            |> YamlNode.String
        | _ ->
            failwith "Unsupported YAML structure."
            
    and parseScalar (x: Scalar) =
        let tag = x.Tag
        let value = x.Value
        value
            
    and parseMap (x: MappingStart) (parser: IParser) =
        let tag = x.Tag
        let dict = Dictionary<string, YamlNode>()
        parser.MoveNext()
        while parser.Current.GetType() <> typeof<MappingEnd> do
            
            let key = (parser.Current :?> Scalar).Value
            parser.MoveNext()
            
            let value = parseYaml parser
            parser.MoveNext()
            
            dict[key] <- value
            
        
        let map =
            Map.ofDict dict
            
        map
        
    and parseList (x: SequenceStart) (parser: IParser) =
        let tag = x.Tag
        let list = ResizeArray()
        parser.MoveNext()
        while parser.Current.GetType() <> typeof<SequenceEnd> do
            let node = parseYaml parser
            parser.MoveNext()
            list.Add node
            
        Seq.toList list
    
    type YamlParser() =
        
        interface IYamlTypeConverter with

            member this.Accepts(t) =
                true

            member this.WriteYaml(emitter, value, typ) =
                ()

            member this.ReadYaml(parser, typ) =
                let node = parseYaml parser
                node
                
    
    let deserializer =
        
        let mutable x = 
            DeserializerBuilder()
                .IgnoreFields()
                .IgnoreUnmatchedProperties()
                .WithTypeConverter(YamlParser())
                
        for tag in ["Test";"Result";"SolutionSet";"Solution";"Duration"] do
            x <- x.WithTagMapping($"!{tag}", typeof<obj>)
            
        x.Build()
          
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
        let spec = StringBuilder()
                
        while (not stop) do
            let line = reader.ReadLine()
            if line = "***/" then
                stop <- true
            else
                spec.AppendLine line
                ()
                
        let mzn = reader.ReadToEnd()
        let yaml = string spec
        let json = deserializer.Deserialize<YamlNode>(new StringReader(yaml))
        let name = Path.GetFileNameWithoutExtension filename
        let testCase =
            { Spec = yaml
            ; FileName = filename
            ; FileInfo = FileInfo filename 
            ; Model = LoadResult.Reference
            ; Mzn = mzn }
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
    
        
    