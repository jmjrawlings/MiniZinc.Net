(*
ClientTests.fs

Generates XUnit test classes from the LibMiniZinc test suite.
*)

namespace MiniZinc
open Humanizer
open Fake.IO
open Fake.Core
open System.Text
open MiniZinc.Tests

module ClientTests =
        
    let project_dir =
        test_dir <//> client_tests_name

    let project_file =
        project_dir </> $"{client_tests_name}.fsproj"
                                            
    let tests_file =
        project_dir </> "IntegrationTests.fs"
                
    let create () =
                        
        let testSuites =
            LibMiniZinc.testSpec
            |> Map.values
            |> Seq.filter (fun suite -> suite.SuiteName <> "default")
            //|> Seq.take 1
            |> Seq.toList
            
        let source = StringBuilder()
        let mutable tab = 0
        let indent () = tab <- tab + 1
        let dedent () = tab <- tab - 1
        let write (msg: string) =
            for i in 0 .. tab do
                source.Append "    "
            source.AppendLine msg
            ()
        let writeln() =
            source.AppendLine()
            
        write "namespace MiniZinc.ClientTests"
        write "open MiniZinc.Tests"
        write "open Xunit"
        writeln()
        write "module ``Integration Tests`` = "
        indent()

        for testSuite in testSuites do
            
            let className =
                testSuite.SuiteName.Humanize(LetterCasing.Title)
                
            write $"type ``{className}``(fixture: ClientFixture) ="
            indent()
            write "inherit IntegrationTestSuite(fixture)"
            writeln()
            
            write $"override this.Name with get() ="
            indent()
            write $"\"{testSuite.SuiteName}\""
            dedent()
            writeln()

            let mutable lastName = ""
            let mutable count = 1
             
            for i, testCase in Seq.indexed testSuite.TestCases do
                                
                let testFile =
                    testCase.TestFile.RelativeTo(LibMiniZinc.test_suite_dir)
                
                let solvers =
                    match testCase.Solvers with
                    | [] -> [ "gecode" ]
                    | xs -> xs
                    
                let mutable methodName =
                    testCase.TestName
                
                if methodName = lastName then
                    count <- count + 1
                    methodName <- $"{methodName} {count}"
                else
                    count <- 1
                    
                lastName <- methodName
                Trace.log methodName
                
                write "[<Theory>]"
                for solver in solvers do
                    write $"[<InlineData(\"{solver}\")>]"
                write $"member this.``{methodName}`` solver ="
                indent()
                write $"let testCase = this.Suite.TestCases[{i}]"
                write $"this.test testCase solver"
                writeln()
                dedent()
            dedent()
            writeln()
        source
        
        let source = string source 
        
        File.writeString
            false
            tests_file.FullName
            source