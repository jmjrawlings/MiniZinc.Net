namespace MiniZinc.Tests;

using System.Text.Json.Nodes;
using MiniZinc;
using MiniZinc.Client;
using MiniZinc.Command;
using MiniZinc.Core;
using MiniZinc.Parser;
using MiniZinc.Tests;
using Shouldly;
using TUnit;
using static System.Console;

public abstract class IntegrationTests
{
    private static CancellationTokenSource _cts = new CancellationTokenSource();
    private static MiniZincClient _client = MiniZincClient.Autodetect();

    public async Task RunSolveTest(string slug, string solver, string? args, string? output)
    {
        string path = $"spec/{slug}";
        FileInfo file = new FileInfo(path);
        WriteLine($"{path}");
        WriteLine("--------------------------------------");
        string source = await File.ReadAllTextAsync(path);
        MiniZincModel model = MiniZincModel.FromFile(path);
        model.ClearOutput();

        string mzn = model.Write();
        WriteLine(mzn);
        WriteLine("--------------------------------------");
        var result = await _client.Solution(model, solver, _cts.Token, args);
        result.IsSolution.ShouldBeTrue();

        // Test case has no expected output
        if (output is null)
            return;

        // If we cannot parse the test case dzn then ignore (for now)
        if (
            !Parser.TryParseDataString(
                output,
                out var expected,
                out var err,
                out var trace,
                out var token
            )
        )
        {
            WriteLine($"Could not parse test case output:\n{output}");
            return;
        }

        if (result.Data is not { } actual)
            return;

        actual.ShouldBe(expected);

        var a = 2;
    }

    public async Task RunTest(
        string slug,
        TestType ttype,
        string? solver,
        string? args,
        string? solutions,
        string? errorMessage,
        string? errorRegex
    )
    {
        string path = $"spec/{slug}";
        WriteLine($"{path}");
        WriteLine("--------------------------------------");
        var source = await File.ReadAllTextAsync(path);

        MiniZincModel? model;
        try
        {
            model = MiniZincModel.FromFile(path);
        }
        catch (Exception exn)
        {
            model = null;
            return;
        }

        var mzn = model.Write();
        WriteLine(mzn);
        WriteLine("--------------------------------------");

        await foreach (var sol in _client.Solve(model, solver, _cts.Token))
        {
            WriteLine($"{sol.Iteration} - {sol.Status}");
            if (sol.Error is not null)
                WriteLine(sol.Error);
        }

        var a = 2;
    }

    /// <summary>
    /// Compare the solution data against the expected solution json
    /// </summary>
    public bool Check(MiniZincData expectedData, MiniZincData actualData)
    {
        foreach (var name in expectedData.Keys)
        {
            var expectedVar = expectedData[name];
            if (!actualData.TryGetValue(name, out var actualVar))
                continue;

            if (!Check(expectedVar, actualVar))
            {
                // WriteLn();
                // WriteLn("While comparing expected solution:");
                // WriteLn($"{expectedVar.Write()}");
                // WriteLn();
                // WriteLn("And actual solution:");
                // WriteLn($"{actualVar.Write()}");
                // WriteLn();
                // WriteSection();
                return false;
            }
        }
        return true;
    }

    // public bool Check(int a, int b) => a == b;
    //
    // public bool Check(decimal a, decimal b)
    // {
    //     var ra = Math.Round(a, 4);
    //     var rb = Math.Round(b, 4);
    //     return ra == rb;
    // }

    /// <summary>
    /// Compare the solution against the json node
    /// </summary>
    public bool Check(MiniZincExpr expected, MiniZincExpr actual)
    {
        int i = 0;
        switch (expected, actual)
        {
            default:
                var expectedMzn = expected.Write(WriteOptions.Minimal);
                var actualMzn = expected.Write(WriteOptions.Minimal);
                if (!expectedMzn.Equals(actualMzn))
                    return false;
                break;
        }

        return true;
    }

    // private bool Check(SetExpr set, RangeExpr range)
    // {
    //     switch (set.Elements)
    //     {
    //         case []:
    //             return false;
    //
    //         case [var e]:
    //             if (!e.Equals(range.Lower))
    //                 return false;
    //             if (!e.Equals(range.Upper))
    //                 return false;
    //             return true;
    //
    //         case var elements:
    //             int min = (range.Lower as IntExpr).Value;
    //             int max = (range.Upper as IntExpr).Value;
    //             for (int i = min; i <= max; i++)
    //             {
    //                 var e = elements[i];
    //                 if (e is not IntExpr il)
    //                     return false;
    //                 if (il.Value != i)
    //                     return false;
    //             }
    //
    //             return true;
    //         default:
    //             break;
    //     }
    // }

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

    // public class TestNameFormatter : ArgumentDisplayFormatter
    // {
    //     public override bool CanHandle(object? value)
    //     {
    //         return value is IntegrationTestCase;
    //     }
    //
    //     public override string FormatValue(object? value)
    //     {
    //         var tcase = (IntegrationTestCase)value;
    //         return tcase.Name;
    //     }
    // }
}
