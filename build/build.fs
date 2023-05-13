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
let obj = cwd <//> "obj"
let root = cwd.Parent
let src = root <//> "src"
let tests = root <//> "tests"
let test_proj_dir = tests <//> "MiniZinc.Net.Tests"
let test_proj_file = test_proj_dir </> "MiniZinc.Net.Tests.fsproj"
let test_examples_dir = test_proj_dir <//> "examples"


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
let download_test_examples () =
    let minizinc_repo = "https://github.com/MiniZinc/libminizinc"
    let clone_dir = obj <//> "libminizinc"
    let clone_path = "tests/spec/examples"
    
    Directory.delete clone_dir.FullName
    Directory.create clone_dir.FullName

    Directory.delete test_examples_dir.FullName
    Directory.create test_examples_dir.FullName
    
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
               test_examples_dir
               
    
    File.writeNew
        (test_examples_dir </> "README.md" |> string)
        [$"All of the files in folder were sourced from {minizinc_repo} under {clone_path}"]
     
    Trace.log $"Downloaded {mzn_files.Length} files to {test_examples_dir}"
    

let init() =

    Target.create "DownloadTestExamples" <| fun _ ->
        download_test_examples ()
   
    
[<EntryPoint>]
let main args =
    args
    |> Array.toList
    |> Context.FakeExecutionContext.Create false "build.fsx"
    |> Context.RuntimeContext.Fake
    |> Context.setExecutionContext
    
    init ()
    Target.runOrDefaultWithArguments "DownloadTestExamples"
    0