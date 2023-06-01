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

Example test suite:

/***
!Test
solvers: [gecode, cbc, chuffed] # List of solvers to use (omit if all solvers should be tested)
check_against: [gecode, cbc, chuffed] # List of solvers used to check results (omit if no checking is needed)
extra_files: [datafile.dzn] # Data files to use if any
options: # Options passed to minizinc-python's solve(), usually all_solutions if present
  all_solutions: true
  timeout: !Duration 10s
expected: # The obtained result must match one of these
- !Result
  status: SATISFIED # Result status
  solution: !Solution
    s: 1
    t: !!set {1, 2, 3} # The set containing 1, 2 and 3
    u: !Range 1..10 # The range 1 to 10 (inclusive)
    v: [1, 2, 3] # The array with 1, 2, 3
    x: !Unordered [3, 2, 1] # Ignore the order of elements in this array
    _output_item: !Trim |
      trimmed output item
      gets leading/trailing
      whitespace ignored 
- !Error
  type: MiniZincError # Name of the error type
  message: Exact error message # Exact error message string (avoid using this as it's generally not portable)
  regex: .*type-inst must be par set.* # Regex the start of the string must match (run with M and S flags)
***/
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
open MiniZinc.Tests

type TestSuite =
    { Name : string
    ; FilePath: string
    ; Mzn : string
    ; Tests: TestCase list }
    
and TestCase =
    { Solvers : SolverId list
      CheckAgainst : SolverId list
      ExtraFiles : string list
      SolveOptions : Map<string, Yaml>
      Expected : TestCaseResult list }
    
and TestCaseResult =
    { Status : string
      Variables : Map<string, Yaml> }

and TestCaseError =
    { Type : string
      Message : string
      Regex : string }
    
and SolverId = string        


module TestSuite =
          
    let private assembly_file =
        Assembly.GetExecutingAssembly().Location
        |> Path.GetFullPath
        |> FileInfo
        
    let examples_dir =
        assembly_file.Directory.Parent.Parent.Parent.Parent
        <//> "examples"    
    
    let rec private parseTest (yaml: Yaml) : TestCase =
        
        let solvers = yaml["solvers"].AsStringList
        let check = yaml["check_against"].AsStringList
        let extra_files = yaml["extra_files"].AsStringList
        let options = yaml["options"].AsMap
        
        let expected =
            yaml["expected"]
            |> Yaml.toList
            |> List.map (Yaml.get "!Result")
            |> List.map parseResult
        
        let testCase = 
            { Solvers = solvers
            ; CheckAgainst = check
            ; ExtraFiles = extra_files
            ; SolveOptions = options
            ; Expected = expected }
            
        testCase
        
    and private parseResult (yaml: Yaml) =

         let status =
             yaml
             |> Yaml.get "status"
             |> Yaml.toString
             |> Option.defaultValue "SATISFIED"
             
         let variables =
             yaml
             |> Yaml.get "solution"
             |> Yaml.get "!Solution"
             |> Yaml.toMap
             
         { Status = status
         ; Variables =  variables }
        
    /// Parse the model for the given TestSuite
    let parseModel (suite: TestSuite) : LoadResult =
        
        let includeOpts =
            IncludeOptions.ParseFile [examples_dir.FullName]
        
        let parseOpts =
            { ParseOptions.Default with 
                IncludeOptions =  includeOpts }
    
        let model =
            Model.parseString parseOpts suite.Mzn

        model
        
    /// Load the TestSuite from the given file
    let load (filename: string) =
        
        let filepath =
            examples_dir </> filename
            
        let modelString, suiteString =
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
            let yaml = (string yaml)
            mzn, yaml
            
        let tests =
            suiteString.Split("--- ", StringSplitOptions.RemoveEmptyEntries)
            |> Seq.map Yaml.parse
            |> Seq.map (Yaml.get "!Test")
            |> Seq.map parseTest
            |> Seq.toList

        let name =
            Path.GetFileNameWithoutExtension filename
            
        let suite =
            { Mzn = modelString
            ; FilePath = filename
            ; Name = name
            ; Tests = tests }

        suite