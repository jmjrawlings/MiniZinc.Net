(* 
ModelTests.fs

Generates XUnit parsing tests for each test in the
libminizinc test suite
*)

namespace MiniZinc
open Fake.IO
open MiniZinc.Tests

module ParserTests =
    
    let project_dir =
        test_dir <//> model_tests_name

    let project_file =
        project_dir </> $"{model_tests_name}.fsproj"

    let tests_file =
        project_dir </> "IntegrationTests.fs"
    
    let create () =
                    
        let testCases =
            LibMiniZinc.testSpec
            |> Map.values
            |> Seq.collect (fun suite -> suite.TestCases)
            |> Seq.distinctBy (fun case -> case.TestName)
            |> Seq.toList
        
        let createTest (testCase: TestCase) =
            
            let testFile =
                testCase.TestFile.RelativeTo(LibMiniZinc.testDir)
                    
            let body = $"""
        [<Fact>]
        let ``test {testCase.TestName}`` () =
            testParseFile @"{testFile}"
    """
            body
                
        let mutable source = """namespace MiniZinc.Tests

    open MiniZinc
    open MiniZinc.Tests
    open Xunit
    open System.IO

    module IntegrationTests =
            
        let testParseFile filePath =
            let file = LibMiniZinc.testDir </> filePath
            let result = parseModelFile file.FullName
            match result with
            | Result.Ok model ->
                ()
            | Result.Error err ->
                Assert.Fail(err.Message)            
    """
        
        for testCase in testCases do
            let body = createTest testCase
            source <- source + body
           
        source
        
        File.writeString
            false
            tests_file.FullName
            source