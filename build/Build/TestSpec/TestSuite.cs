// LibMiniZinc.fs
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
namespace MiniZinc.Build;

using System;
using System.IO;
using static Prelude;

public sealed class TestSuite
{
    public required string Name { get; init; }

    public required bool? Strict { get; init; }

    public required YamlNode Options { get; init; }

    public List<string> Solvers { get; } = new();

    public List<string> IncludeGlobs { get; } = new();

    public List<FileInfo> IncludeFiles { get; } = new();

    public List<TestCase> TestCases { get; } = new();

    public override string ToString() => $"<{Name}\" ({TestCases.Count} cases)>";
}




//
//     /// Parse a TestCase from the given yaml
//     let parseTestCase (yaml: Yaml) : TestCase =
//
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
