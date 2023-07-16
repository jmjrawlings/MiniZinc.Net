(* 
ModelTests.fs

Generates XUnit parsing tests for each test in the
libminizinc test suite
*)

namespace MiniZinc
open System.Text
open Fake.IO
open MiniZinc.Tests
open Humanizer

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
            
        let testGroups =
            testCases
            |> List.groupBy (fun testCase -> testCase.TestFile.Directory.Name)
            
        let source = StringBuilder()
        
        let mutable tab = 0
        
        let indent () =
            tab <- tab + 1
            
        let dedent () =
            tab <- tab - 1
            
        let write (msg: string) =
            for i in 0 .. tab do
                source.Append "    "
            source.AppendLine msg
            ()
            
        let writeln() =
            source.AppendLine()
            
        let writei msg =
            write msg
            indent()
                
        write "namespace MiniZinc.Tests"
        write "open MiniZinc"
        write "open MiniZinc.Tests"
        write "open Xunit"
        write "open System.IO"
        writeln()
        writei "module IntegrationTests ="
        writeln()
        writei "let test filePath ="
        write "let file = LibMiniZinc.testDir </> filePath"
        write "let result = parseModelFile file.FullName"
        write "match result with"
        write "| Result.Ok model -> ()"
        write "| Result.Error err -> Assert.Fail(err.Message)"
        dedent()
        writeln()
        
        for group, tests in testGroups do
            let moduleName = group.Humanize(LetterCasing.Title)
            writei $"module ``{moduleName}`` ="
            writeln()
            for testCase in tests do
                let testFile =
                    testCase.TestFile.RelativeTo(LibMiniZinc.testDir)
                let testName =
                    testCase.TestName
                write "[<Fact>]"
                writei $"let ``test {testName}`` () ="
                write $"test @\"{testFile}\""
                writeln()
                dedent()
            dedent()

        let source = string source
                
        File.writeString
            false
            tests_file.FullName
            source