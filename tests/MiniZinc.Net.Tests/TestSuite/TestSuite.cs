﻿// LibMiniZinc.fs
//
// Load and parse the suite of tests used by libminizinc which
// can be found [here](https://github.com/MiniZinc/libminizinc/tree/master/tests/spec)
//
// The tests are detailed in both a standalone yaml file `suites.yaml` as well
// as yaml structures embedded inside minizinc models as top level comments.
//
// It's quite tricky to parse all of these correctly, and from what I can tell
// there isn't a great yaml parsing library for dotnet.  That being said the extra
// effort is well worth it to ensure MiniZinc.Net works exactly as expected.
//
// Example test suite:

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

namespace MiniZinc.Tests;

using System.Text.Json.Nodes;
using System.Reflection;
using System;
using System.IO;
using static Prelude;

public sealed class TestSuite
{
    public string SuiteName { get; init; }

    public List<string> IncludeGlobs { get; } = new();

    public List<FileInfo> IncludeFiles { get; } = new();

    public List<TestCase> TestCases { get; } = new();
    
    public override string ToString() => $"<{SuiteName}\" ({this.TestCases.Count} cases)>";

    public static IEnumerable<TestSuite> Load(FileInfo file)
    {
        var yaml = Yaml.ParseFile(file) as YamlMap;
        return Enumerable.Empty<TestSuite>();
        }
    



    //         let includeGlobs =
//             yaml.Get "includes"
//             |> Yaml.toList
//             |> List.choose Yaml.toString
//
//         let includeFiles =
//             includeGlobs
//             |> Seq.collect (fun glob -> Directory.GetFiles(directory.FullName, glob, SearchOption.AllDirectories))
//             |> Seq.map FileInfo
//             |> Seq.filter (fun fi -> fi.Extension = ".mzn")
//             |> Seq.toList
//
//         let solveOptions =
//             yaml.TryGet "options"
//             |> Option.map Yaml.toMap
//             |> Option.defaultValue Map.empty
//
//         let testCases =
//             includeFiles
//             |> Seq.toArray
//             |> Seq.collect parseTestCases
//             |> Seq.map (fun case ->
//                 {case with
//                     SolveOptions = solveOptions
//                     SuiteName = suiteName })
//             |> Seq.toArray
//
//         let suite =
//             { SuiteName = suiteName
//             ; TestCases = testCases
//             ; IncludeGlobs = includeGlobs
//             ; IncludeFiles = includeFiles
//             ; SolveOptions = solveOptions }
//

    [Fact]
    public void Test_Load_Test_Suite()
    {
        var yaml = Yaml.ParseFile(Prelude.TestSuiteFile).Map;
        foreach (var kv in yaml.Dict)
        {
            var suiteName = kv.Key;
            var suiteYaml = kv.Value;
            var includes = suiteYaml.Get("includes")?.List(x => x.String);
            var strict = suiteYaml.Get("strict")?.Bool;
            var options = suiteYaml.Get("options")?.Map;
            var solvers = suiteYaml.Get("solvers")?.List(x => x.String);
            var x = 1;
        }
    }
}



//
//     /// Parse a TestCase from the given yaml
//     let parseTestCase (yaml: Yaml) : TestCase =
//
//         let solvers =
//             yaml["solvers"].AsStringList
//
//         let check =
//             yaml["check_against"].AsStringList
//
//         let extraFiles =
//             yaml["extra_files"]
//             |> Yaml.toStringList
//             |> List.map FileInfo
//
//         let options =
//             yaml["options"].AsMap
//
//         let results =
//             yaml["expected"]
//             |> Yaml.toList
//             |> List.map parseTestResult
//
//         let testCase =
//             { SuiteName = ""
//             ; TestName = ""
//             ; TestFile = FileInfo "."
//             ; TestPath = ""
//             ; ModelString = ""
//             ; Solvers = solvers
//             ; Includes = extraFiles
//             ; SolveOptions = options
//             ; Results = results }
//
//         testCase
//
//     /// Parse the TestCaseResult from the given suite yaml
//     let parseTestResult (yaml: Yaml) : TestResult =
//
//         let statusType =
//             yaml
//             |> Yaml.get "status"
//             |> Yaml.toString
//             |> Option.defaultValue "SATISFIED"
//             |> StatusType.parse
//
//         let variables, objective =
//             yaml
//             |> Yaml.get "solution"
//             |> Yaml.get "!Solution"
//             |> Yaml.toMap
//             |> Map.map (fun _ -> Yaml.toExpr)
//             |> function
//                 | m when m.ContainsKey "objective" ->
//                     (Map.remove "objective" m), (Some m["objective"])
//                 | m ->
//                     m, None
//
//         { StatusType = statusType
//         ; Objective = objective
//         ; Variables =  variables
//         ; ErrorType = ""
//         ; ErrorMessage = ""
//         ; ErrorRegex = "" }
//
/// Load the TestSuite for the given filename
// let parseTestCases (testFile: FileInfo) : TestCase list =
//
//     let testName =
//         Path.GetFileNameWithoutExtension testFile.FullName
//
//     let modelString, yamlString =
//         use reader = new StreamReader(testFile.FullName)
//         let yml = StringBuilder()
//         let header = reader.ReadLine()
//         let mutable stop = header <> "/***"
//         let mutable i = 0
//
//         while (not stop) do
//             i <- i + 1
//             let line = reader.ReadLine()
//             match line with
//             | _ when line.Contains "***/" ->
//                 stop <- true
//             | null ->
//                 stop <- true
//             | line ->
//                 yml.AppendLine line
//                 ()
//
//         let mzn = reader.ReadToEnd()
//         let yml = (string yml)
//         mzn, yml
//
//     let testYamls =
//         yamlString.Split("---", StringSplitOptions.RemoveEmptyEntries)
//         |> Seq.map (fun s -> s.Trim())
//         |> Seq.toList
//
//     let testCases =
//         testYamls
//         |> Seq.choose parseYamlString
//         |> Seq.map (Yaml.get "!Test")
//         |> Seq.filter (fun yml -> yml <> Null)
//         |> Seq.map parseTestCase
//         |> Seq.map (fun case ->
//
//             let includes =
//                 case.Includes
//                 |> List.map (fun fi -> testFile.Directory </> fi.Name)
//
//             { case with
//                 Includes = includes
//                 TestName = testName
//                 TestFile = testFile
//                 ModelString = modelString })
//         |> Seq.toList
//
//     testCases
