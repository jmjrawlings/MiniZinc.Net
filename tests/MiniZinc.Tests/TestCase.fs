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

    type CustomTagConverter<'t>() =
        interface IYamlTypeConverter with
            member this.Accepts(t) = t = typeof<'t>
            member this.WriteYaml(emitter, value, typ) = ()
            member this.ReadYaml(parser, typ) =
                
                let rec parseNested (parser: IParser) =
                    match parser.Current with
                    | :? MappingStart ->
                        let dict = new Dictionary<string, obj>()
                        parser.MoveNext() // Move past the start of the mapping.
                        while parser.Current.GetType() <> typeof<MappingEnd> do
                            parser.MoveNext() // Move to the key.
                            let key = (parser.Current :?> Scalar).Value
                            parser.MoveNext() // Move to the value.
                            dict.Add(key, parseNested parser)
                            parser.MoveNext() // Move to the next key or the end of the mapping.
                        dict :> obj
                    | :? SequenceStart ->
                        let list = new List<obj>()
                        parser.MoveNext() // Move past the start of the sequence.
                        while parser.Current.GetType() <> typeof<SequenceEnd> do
                            list.Add(parseNested parser)
                            parser.MoveNext() // Move to the next item or the end of the sequence.
                        list :> obj
                    | :? Scalar ->
                        (parser.Current :?> Scalar).Value :> obj
                    | _ ->
                        failwith "Unsupported YAML structure."

                parseNested parser    
    
    let deserializer =
        DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .WithTypeConverter(CustomTagConverter<TestCaseSolution>())
            .WithTypeConverter(CustomTagConverter<TestCaseDuration>())
            .WithTypeConverter(CustomTagConverter<TestCaseError>())
            .WithTypeConverter(CustomTagConverter<TestCaseResult>())
            .WithTypeConverter(CustomTagConverter<TestCaseTest>())
            .WithTagMapping("!Solution", typeof<TestCaseSolution>)
            .WithTagMapping("!Test", typeof<TestCaseTest>)
            .WithTagMapping("!Duration", typeof<TestCaseDuration>)
            .WithTagMapping("!Result", typeof<TestCaseResult>)
            .Build()
          
    let assembly_file =
        Assembly.GetExecutingAssembly().Location
        |> Path.GetFullPath
        |> FileInfo
        
    let examples_dir =
        assembly_file.Directory.Parent.Parent.Parent.Parent
        <//> "examples"


    // Read the testcase from the given file
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
        let json = deserializer.Deserialize<Dictionary<string, obj>>(new StringReader(yaml))
        let name = Path.GetFileNameWithoutExtension filename
        let testCase =
            { Spec = yaml
            ; FileName = filename
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
    
        
    