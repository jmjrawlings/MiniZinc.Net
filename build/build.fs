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

let src_dir = root <//> "src"

let model_proj_name = "MiniZinc.Model"
let model_proj_dir = src_dir <//> model_proj_name
let model_proj_file = model_proj_dir </> $"{model_proj_name}.fs"

let solve_proj_name = "MiniZinc.Solve"
let solve_proj_dir = src_dir <//> solve_proj_name
let solve_proj_file = solve_proj_dir </> $"{solve_proj_name}.fs"


let test_dir = root <//> "tests"

let model_tests_name = "MiniZinc.ModelTests"
let model_test_proj_dir = test_dir <//> model_tests_name
let model_test_proj_file = model_test_proj_dir </> $"{model_tests_name}.fsproj"

let solve_tests_name = "MiniZinc.SolveTests"
let solve_test_proj_dir = test_dir <//> solve_tests_name
let solve_test_proj_file = solve_test_proj_dir </> $"{solve_tests_name}.fsproj"


let obj = cwd <//> "obj"
let examples_dir = test_dir <//> "examples"


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
let download_libminizinc_test_suite () =
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
               (fun dir file -> List.contains file.Extension [".mzn"; ".model"])
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
let create_integration_tests () =
        
    let mutable code = """
namespace MiniZinc.Model.Tests

open MiniZinc
open MiniZinc.Tests
open Xunit
open System.IO

module IntegrationTests =
   
    let test (name: string) =
        let suite = TestSuite.load name
        let model = TestSuite.parseModel suite
        model.Value.Undeclared.AssertEmpty()
        model.Value.Conflicts.AssertEmpty()
"""
    
    let examples =
        examples_dir
        |> DirectoryInfo.getMatchingFiles "*.mzn"

    for example in examples do

        let name =
            example.NameWithoutExtension.Replace("_", " ")
            
        let test_case = $"""
    [<Fact>]
    let ``test {name}`` () =
        test "{example.Name}"
"""

        code <- code + "\n" + test_case
    
    let destination =
        model_test_proj_dir </> "IntegrationTests.fs"
    
    File.writeString
        false
        destination.FullName
        code
        
let run_tests() =
    model_test_proj_file.FullName
    |> DotNet.test (fun opts ->
        { opts with
            Configuration = DotNet.BuildConfiguration.Release }
        )
 
let init() =

    Target.create "DownloadTestSuite" <| fun _ ->
        download_libminizinc_test_suite ()
        
    Target.create "CreateIntegrationTests" <| fun _ ->
        create_integration_tests ()
        
    Target.create "RunTests" <| fun _ ->
        run_tests()        
        
    Target.create "All" <| fun _ ->
        ()

    "DownloadTestSuite"
        ==> "CreateIntegrationTests"
        
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