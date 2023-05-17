open System
open System.Diagnostics
open Fake.Core
open Fake.DotNet
open Fake.IO
open Fake.IO.FileSystemOperators
open Fake.IO.Globbing.Operators
open Fake.Core.TargetOperators
open Fake.Api
open Fake.BuildServer
open Fake.Tools
open MiniZinc.Build

let cwd = di __SOURCE_DIRECTORY__
let root = cwd.Parent

let src_name = "MiniZinc.Net"
let src_dir = root <//> "src"
let src_proj_dir = src_dir <//> src_name
let src_proj_file = src_proj_dir </> $"{src_name}.fs"

let test_name = "MiniZinc.Net.Tests"
let test_dir = root <//> "tests"
let test_proj_dir = test_dir <//> test_name
let test_proj_file = test_proj_dir </> "MiniZinc.Net.Tests.fsproj"

let obj = cwd <//> "obj"
let examples_dir = test_proj_dir <//> "examples"
let example_tests_file = test_proj_dir </> "ExampleTests.fs"



/// <summary>
/// Download MiniZinc test examples
/// </summary>
/// <remarks>
/// We use the official MiniZinc repository
/// as a source for our tests.  This task
/// will download the 'test' branch of the repo
/// and copy all of the example tests into
/// this repo.
/// </remarks>
let download_test_models () =
    let minizinc_repo = "https://github.com/MiniZinc/libminizinc"
    let clone_dir = obj <//> "libminizinc"
    let clone_path = "tests/spec/examples"
    
    Directory.delete clone_dir.FullName
    Directory.create clone_dir.FullName

    Directory.delete examples_dir.FullName
    Directory.create examples_dir.FullName
    
    let git = git clone_dir

    git "init"
    git $"remote add origin {minizinc_repo}.git"
    git $"sparse-checkout set {clone_path}"
    git "fetch origin master"
    git "checkout master"
    
    let mzn_files =    
        clone_dir <//> clone_path
        |> DirectoryInfo.copyRecursiveToWithFilter
               true
               (fun dir file -> file.Extension = ".mzn")
               examples_dir
    
    File.writeNew
        (examples_dir </> "README.md" |> string)
        [$"All of the files in folder were sourced from {minizinc_repo} under {clone_path}"]
     
    Trace.log $"Downloaded {mzn_files.Length} files to {examples_dir}"
    
/// <summary>
/// Create a test suite from MiniZinc Examples
/// </summary>
/// <remarks>
/// For each model we downloaded from the MiniZinc
/// test suite we create our own test in a dedicated
/// module. 
/// </remarks>
let create_test_suite() =
        
    let mutable code = """module MiniZinc.Net.Tests.ExampleTests

open MiniZinc
open Xunit
open System.IO

let example_dir =
    $"{__SOURCE_DIRECTORY__}/examples"

type TestSpec =
    { Name   : string 
    ; File   : FileInfo
    ; String : string }
    
    static member create name =
        let file = FileInfo $"{example_dir}/{name}.mzn"
        let string = File.ReadAllText file.FullName
        let spec = { Name = name ; File = file; String = string }
        spec
    
let test (spec: TestSpec) =
    let result = test_parse Parsers.model spec.String
    result
    

"""

    let examples =
        examples_dir
        |> DirectoryInfo.getMatchingFiles "*.mzn"

    for example in examples do
        let name = example.NameWithoutExtension
        let test_case = $"""
[<Fact>]
let ``test {name}`` () =
    let spec = TestSpec.create "{name}" 
    let result = test spec
    ()
    """
        code <- code + "\n" + test_case
    
    File.writeString
        false
        example_tests_file.FullName
        code
        
let run_tests() =
    test_proj_file.FullName
    |> DotNet.test (fun opts ->
        { opts with
            Configuration = DotNet.BuildConfiguration.Release }
        )
 
let init() =

    Target.create "DownloadTestModels" <| fun _ ->
        download_test_models ()
        
    Target.create "CreateTestSuite" <| fun _ ->
        create_test_suite ()
        
    Target.create "RunTests" <| fun _ ->
        run_tests()        
        
    Target.create "All" <| fun _ ->
        ()        

    "DownloadTestModels"
        ==> "CreateTestSuite"
        
    "RunTests"
        ==> "All"
        
    
[<EntryPoint>]
let main args =
    args
    |> Array.toList
    |> Context.FakeExecutionContext.Create false "build.fsx"
    |> Context.RuntimeContext.Fake
    |> Context.setExecutionContext
    
    init ()
    Target.runOrDefaultWithArguments "All"
    0