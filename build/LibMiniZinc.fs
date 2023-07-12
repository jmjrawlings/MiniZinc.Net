namespace MiniZinc

open Fake.IO
open Fake.Core
open MiniZinc.Tests

module LibMiniZinc =
    
    let repo_url = "https://github.com/MiniZinc/libminizinc"
            
    let clone_dir = "obj/libminizinc"
        
    let clone_path = "tests/spec"
               
    let test_suite_dir = LibMiniZinc.testDir
                            
    /// Download  
    let downloadTests () =
            
        Directory.delete clone_dir
        Directory.create clone_dir
                
        Directory.delete test_suite_dir.FullName
        Directory.create test_suite_dir.FullName

        let git = git clone_dir

        git "init"
        git $"remote add origin {repo_url}.git"
        git $"sparse-checkout set {clone_path}"
        git "fetch origin master"
        git "checkout master"

        let mzn_files =    
            clone_dir <//> clone_path
            |> DirectoryInfo.copyRecursiveToWithFilter
                true
                (fun dir file -> List.contains file.Extension [".mzn"; ".model"; ".dzn"; ".yaml"])
                test_suite_dir

        File.writeNew
            (test_suite_dir </> "README.md" |> string)
            [$"All of the files in folder were sourced from {repo_url} under {clone_path}"]
         
        Trace.log
            $"Downloaded {mzn_files.Length} files to {test_suite_dir}"
