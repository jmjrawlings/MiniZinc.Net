namespace MiniZinc
open System
open System.IO
open Humanizer
open Fake.IO
open Fake.Core
open System.Text

module ClientTests =
        
    let project_dir =
        test_dir <//> client_tests_name

    let project_file =
        project_dir </> $"{client_tests_name}.fsproj"
                                            
    let tests_file =
        project_dir </> "IntegrationTests.fs"
                
    let create (suites: TestSuite list) =
                
        let testSuites =
            parseTestSuites ()
            |> Seq.filter (fun suite -> suite.SuiteName <> "default")
            //|> Seq.take 1
            |> Seq.toList
            
        let mutable source = """namespace MiniZinc.Tests

open MiniZinc
open MiniZinc.Tests
open Xunit
open System.IO

module ``Integration Tests`` =

    let cwd =
        Directory.GetCurrentDirectory()
        |> DirectoryInfo
        
    let projectDir =
        cwd.Parent.Parent.Parent.Parent.Parent
        
    let libminizincDir =
        projectDir <//> "tests" <//> "libminizinc"
       
    type IntegrationTestSuite(fixture: ClientFixture) =
    
        let client = fixture.Client
                
        let fail msg =
            Assert.Fail(msg)
            failwith ""
            
        interface IClassFixture<ClientFixture>            
            
        member _.test filePath solver =
                
            let file =
                libminizincDir </> filePath
            
            let options =
                SolveOptions.create solver
                
            let model =
                match parseModelFile file.FullName with
                | Result.Ok model ->
                    model
                | Result.Error err ->
                    fail err.Message
                    
            let solution =
                client.SolveSync(model, options)
                
            ()
"""

        for testSuite in testSuites do

            let className =
                testSuite.SuiteName.Humanize(LetterCasing.Title)
                
            let mutable suiteSource = $"""
            
    type ``{className}``(fixture: ClientFixture) =
        inherit IntegrationTestSuite(fixture)                
"""

            let mutable lastName = ""
            let mutable count = 1
             
            for testCase in testSuite.TestCases do
                                
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
                    
                let testSource = StringBuilder()
                let write tabs (msg: string) =
                    for i in 0 .. tabs do
                        testSource.Append "    "
                    testSource.AppendLine msg                    
                    ()
                
                write 2 ""
                write 2 "[<Theory>]"
                for solver in solvers do
                    write 2 $"[<InlineData(\"{solver}\")>]"
                write 2 $"member this.``{methodName}`` solver ="
                write 3 $"this.test @\"{testFile}\" solver"
                suiteSource <- $"{suiteSource}{string testSource}"
            
            source <- source + "\n" + suiteSource
        source
        
        File.writeString
            false
            tests_file.FullName
            source