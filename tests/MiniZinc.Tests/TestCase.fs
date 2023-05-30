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
open MiniZinc.Tests.Yaml

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
        let node = Yaml.parse (string yaml)
        let test = node["!Test"]
        let name = Path.GetFileNameWithoutExtension filename
        let testCase =
            { Yaml = ""
              Mzn = mzn
              FileName = filename
              Name = name
              Model = LoadResult.Reference
              Tests = [] }

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
    
        
    