namespace MiniZinc.Tests;

using System.Text.Json;
using System.Text.Json.Nodes;

public abstract class TestBuilder : CodeBuilder
{
    public readonly string ClassName;

    protected TestBuilder(string className)
    {
        ClassName = className;
    }

    public abstract string Build(TestSpec spec);

    protected void WriteMessage(object? msg = null)
    {
        if (msg is null)
            Call("_output.WriteLine", "\"\"");
        else
            Call("_output.WriteLine", msg.ToString()!);
    }

    protected void WriteSection() => WriteMessage("new string('-',80)");

    private string FormatDzn(string s)
    {
        var z = s.Replace("\n", "\\n");
        z = z.Replace("\"", "\\\"");
        z = $"\"{z}\"";
        return z;
    }

    private string FormatArg(string s)
    {
        // Some extra options are passed in as quoted strings
        if (s.StartsWith('"'))
            return s;

        var z = Quote(s);
        return z;
    }

    protected TestCaseInfo? GetTestInfo(TestCase testCase)
    {
        var testName = testCase.Path.Replace(".mzn", "");
        testName = testName.Replace("/", "_");
        testName = testName.Replace("-", "_");
        testName = testName.Replace(".", "");
        testName = $"test_solve_{testName}";
        if (testCase.Sequence > 1)
            testName = $"{testName}_{testCase.Sequence}";

        var testPath = testCase.Path;
        var testDir = Path.GetDirectoryName(testPath)!;
        var info = new TestCaseInfo
        {
            Path = testPath,
            Dir = testDir,
            Name = testName,
            Signature = $"async Task {testName}(string solver)",
            Options = testCase.Options,
            Sequence = testCase.Sequence,
            Solutions = testCase.Solutions,
            InputFiles = testCase.InputFiles,
            ErrorMessage = testCase.ErrorMessage,
            ErrorRegex = testCase.ErrorRegex,
            Type = testCase.Type,
            OutputFiles = testCase.OutputFiles
        };

        if (testCase.Solvers is null)
            return null;

        if (testCase.Options is JsonObject opts)
        {
            foreach (var kv in opts)
            {
                var key = kv.Key;
                var val = kv.Value!;
                var kind = val.GetValueKind();
                if (!key.StartsWith('-'))
                    key = $"--{key}";

                string arg;
                if (kind is JsonValueKind.True)
                {
                    arg = key;
                }
                else
                {
                    var value = val.ToString();
                    if (value.Contains(' '))
                        arg = $"{key} \\\"{value}\\\"";
                    else if (value.StartsWith('"'))
                        arg = $"{key} {value.Substring(1, value.Length - 2)}";
                    else
                        arg = $"{key} {value}";
                }
                info.ExtraArgs.Add(arg);
            }
        }

        if (testCase.InputFiles is { } extraFiles)
        {
            foreach (var extraFile in extraFiles)
            {
                var extraPath = Path.Combine(testDir, extraFile);
                extraPath = extraPath.Replace('\\', '/');
                info.ExtraArgs.Add($"--data \\\"{extraPath}\\\"");
            }
        }

        foreach (var solver in testCase.Solvers)
        {
            info.Solvers.Add(
                solver switch
                {
                    "cbc" => ("coin-bc", true),
                    "scip" => ("scip", false),
                    "gurobi" => ("gurobi", false),
                    "gecode_presolver" => ("gecode_presolver", false),
                    _ => (solver, true)
                }
            );
        }

        return info;
    }
}
