namespace MiniZinc.Tests;

using System.Text.Json.Nodes;
using MiniZinc;
using MiniZinc.Client;
using MiniZinc.Command;
using MiniZinc.Core;
using MiniZinc.Parser;
using MiniZinc.Tests;
using Shouldly;
using Xunit;
using static System.Console;

public abstract class IntegrationTests
{
    private static CancellationTokenSource _cts = new CancellationTokenSource();
    private static MiniZincClient _client = MiniZincClient.Autodetect();

    /// <summary>
    /// Wire up an extra data/model file for the test. MiniZinc-native, JSON-based
    /// formats (.json data files and .mpc parameter-configuration files) are
    /// passed to the solver as a separate file argument and returned here: the
    /// parser only understands MiniZinc/DZN syntax, and JSON data coercion is
    /// type-directed (the same JSON string can be an enum member or a string
    /// depending on the declared type), so it must be done by minizinc itself.
    /// Plain MiniZinc data (.dzn/.mzn) is valid syntax and is inlined into the
    /// model via <see cref="MiniZincModel.AddFile(string)"/>.
    /// </summary>
    private static string? AddExtraFile(MiniZincModel model, string path, string? extraFile)
    {
        if (extraFile is null)
            return null;

        if (
            extraFile.EndsWith(".json", StringComparison.OrdinalIgnoreCase)
            || extraFile.EndsWith(".mpc", StringComparison.OrdinalIgnoreCase)
        )
        {
            string? dir = Path.GetDirectoryName(path);
            string dataPath = Path.GetFullPath(
                dir is null ? extraFile : Path.Combine(dir, extraFile)
            );
            return $"\"{dataPath}\"";
        }

        model.AddFile(extraFile);
        return null;
    }

    public async Task RunSolveTest(
        string slug,
        string solver,
        string? args,
        string? extraFile,
        string? solution
    )
    {
        string path = $"spec/{slug}";
        FileInfo file = new FileInfo(path);
        WriteLine($"{path}");
        WriteLine("--------------------------------------");
        string source = await File.ReadAllTextAsync(path);
        MiniZincModel model = MiniZincModel.FromFile(path);
        model.ClearOutput();

        string? dataArg = AddExtraFile(model, path, extraFile);

        string mzn = model.Write();
        WriteLine(mzn);
        WriteLine("--------------------------------------");
        var result = await _client.Solution(model, solver, _cts.Token, args, dataArg);
        result.IsSolution.ShouldBeTrue(result.Error);

        // Test case has no expected output
        if (solution is null)
            return;

        // If we cannot parse the test case dzn then ignore (for now)
        if (
            !Parser.TryParseDataString(
                solution,
                out var expectedData,
                out var err,
                out var trace,
                out var token
            )
        )
        {
            WriteLine($"Could not parse test case output:\n{solution}");
            return;
        }

        if (result.Data is not { } actualData)
            return;

        Check(expectedData, actualData).ShouldBeTrue();
    }

    public async Task RunAnySolutionTest(
        string slug,
        string solver,
        string? args,
        string? extraFile,
        string[] solutions
    )
    {
        string path = $"spec/{slug}";
        FileInfo file = new FileInfo(path);
        WriteLine($"{path}");
        WriteLine("--------------------------------------");
        string source = await File.ReadAllTextAsync(path);
        MiniZincModel model = MiniZincModel.FromFile(path);
        model.ClearOutput();

        string? dataArg = AddExtraFile(model, path, extraFile);

        string mzn = model.Write();
        WriteLine(mzn);
        WriteLine("--------------------------------------");
        var result = await _client.Solution(model, solver, _cts.Token, args, dataArg);
        result.IsSolution.ShouldBeTrue(result.Error);
        if (result.Data is not { } actualData)
            return;

        foreach (var solution in solutions)
        {
            // If we cannot parse the test case dzn then ignore (for now)
            if (
                !Parser.TryParseDataString(
                    solution,
                    out var expectedData,
                    out var err,
                    out var trace,
                    out var token
                )
            )
            {
                WriteLine($"Could not parse test case output:\n{solution}");
                return;
            }

            if (Check(expectedData, actualData))
                return;
        }

        Assert.Fail($"The actual solution did not match any of the expected solutions");
    }

    public async Task RunUnsatisfiableTest(
        string slug,
        string solver,
        string? args,
        string? extraFile
    )
    {
        string path = $"spec/{slug}";
        WriteLine($"{path}");
        WriteLine("--------------------------------------");
        MiniZincModel model = MiniZincModel.FromFile(path);
        model.ClearOutput();

        string? dataArg = AddExtraFile(model, path, extraFile);

        string mzn = model.Write();
        WriteLine(mzn);
        WriteLine("--------------------------------------");
        var result = await _client.Solution(model, solver, _cts.Token, args, dataArg);
        result.Status.ShouldBe(SolveStatus.Unsatisfiable, result.Error);
    }

    /// Compare the solution data against the expected solution json
    public bool Check(MiniZincData expected, MiniZincData actual)
    {
        foreach (var name in expected.Keys)
        {
            var evar = expected[name];
            if (!actual.TryGetValue(name, out var avar))
                continue;

            if (!Check(evar, avar))
            {
                WriteLine();
                WriteLine("While comparing expected solution:");
                WriteLine($"{evar.Write()}");
                WriteLine();
                WriteLine("And actual solution:");
                WriteLine($"{avar.Write()}");
                return false;
            }
        }
        return true;
    }

    /// Compare the solution against the json node
    public static bool Check(MiniZincExpr exp, MiniZincExpr act)
    {
        int i = 0;
        switch (exp, act)
        {
            case (IntExpr e, IntExpr a):
                return e.Value == a.Value;

            case (IntExpr e, FloatExpr a):
                return e.Value == a.Value;

            case (FloatExpr e, FloatExpr a):
                return e.Value == a.Value;

            case (FloatExpr e, IntExpr a):
                return e.Value == a.Value;

            case (BoolExpr a, BoolExpr b):
                return a.Value == b.Value;

            case (StringExpr a, StringExpr b):
                return a.Value == b.Value;

            case (TupleExpr a, TupleExpr b):
                return CheckSeq(a.Fields, b.Fields);

            case (TupleExpr a, Array1dExpr b):
                return CheckSeq(a.Fields, b.Elements);

            case (Array1dExpr a, TupleExpr b):
                return CheckSeq(a.Elements, b.Fields);

            case (Array1dExpr a, Array1dExpr b):
                return CheckSeq(a.Elements, b.Elements);

            case (Array1dExpr a, Array2dExpr b):
                return CheckSeq(a.Elements, b.Elements);

            case (Array1dExpr a, Array3dExpr b):
                return CheckSeq(a.Elements, b.Elements);

            case (Array1dExpr a, CallExpr { Name: { StringValue: "array3d" } } b):
                return Check(a, b.Args[3]);

            case (Array1dExpr a, CallExpr { Name: { StringValue: "array2d" } } b):
                return Check(a, b.Args[2]);

            case (SetExpr a, SetExpr b):
                return CheckSeq(a.Elements, b.Elements);

            case (RecordExpr a, RecordExpr b):
                return CheckRecord(a, b);

            case (IndexedExpr a, IndexedExpr b):
                return Check(a.Index, b.Index) && Check(a.Value, b.Value);

            case (var a, IndexedExpr b):
                return Check(a, b.Value);

            default:
                return false;
        }

        return true;
    }

    public static bool CheckRecord(RecordExpr a, RecordExpr b)
    {
        if (a.Fields.Count != b.Fields.Count)
            return false;

        foreach (var (akey, aval) in a.Fields)
        {
            bool found = false;
            foreach (var (bkey, bval) in b.Fields)
            {
                if (akey.Equals(bkey))
                {
                    found = true;
                    if (!Check(aval, bval))
                        return false;
                }

                if (found)
                    break;
            }
        }

        return true;
    }

    public static bool CheckSeq(IReadOnlyList<MiniZincExpr>? a, IReadOnlyList<MiniZincExpr>? b)
    {
        // Empty arrays/sets carry a null element list rather than an empty one.
        int countA = a?.Count ?? 0;
        int countB = b?.Count ?? 0;
        if (countA != countB)
            return false;

        for (int i = 0; i < countA; i++)
        {
            var ia = a![i];
            var ib = b![i];
            if (!Check(ia, ib))
                return false;
        }

        return true;
    }

    private IEnumerable<JsonNode?> Flatten(JsonArray arr)
    {
        foreach (var node in arr)
        {
            if (node is JsonArray inner)
                foreach (var x in Flatten(inner))
                    yield return x;
            else
                yield return node;
        }
    }
}
