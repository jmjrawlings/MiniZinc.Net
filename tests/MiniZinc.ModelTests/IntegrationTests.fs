namespace MiniZinc.Tests

    open MiniZinc
    open MiniZinc.Tests
    open Xunit
    open System.IO

    module IntegrationTests =

        let cwd =
            Directory.GetCurrentDirectory()
            |> DirectoryInfo
            
        let projectDir =
            cwd.Parent.Parent.Parent.Parent.Parent
            
        let specDir =
            projectDir <//> "tests" <//> "libminizinc"
            
        let testParseFile filePath =
            let file = projectDir </> filePath
            let result = parseModelFile file.FullName
            match result with
            | Result.Ok model ->
                ()
            | Result.Error err ->
                Assert.Fail(err.Message)            
    
        [<Fact>]
        let ``test test-globals-float`` () =
            testParseFile @"libminizinc\unit\test-globals-float.mzn"
    