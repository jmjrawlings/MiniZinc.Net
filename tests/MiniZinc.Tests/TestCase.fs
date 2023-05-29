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
open System.IO
open System.Reflection
open System.Text
open MiniZinc

type TestCase =
    { FileName: string
    ; Mzn : string
    ; Spec: string
    ; Model: LoadResult }

module TestCase =

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
        reader.ReadLine()
                
        let mutable stop = false
        let spec = StringBuilder()
                
        while (not stop) do
            let line = reader.ReadLine()
            if line = "***/" then
                stop <- true
            else
                spec.Append line
                ()
                
        let mzn = reader.ReadToEnd()
        let spec = string spec
        let name = Path.GetFileNameWithoutExtension filename
        let testCase =
            { Spec = spec
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
    
        
    