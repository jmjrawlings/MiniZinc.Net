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

public sealed class SatisfyTests
{
    private static CancellationTokenSource _cts = new CancellationTokenSource();

    public static IEnumerable<IntegrationTestCase> GetSatisfyTests()
    {
        var client = MiniZincClient.Autodetect();
        var source = "spec.json".ToFile();
        var testSpec = TestSpec.FromJsonFile(source);
        foreach (var testCase in testSpec.TestCases)
        {
            if (testCase.Suite != "optimize-0")
                continue;

            var solvers = testCase.Solvers ?? [MiniZincSolver.GECODE];

            foreach (var solver in solvers)
            {
                IntegrationTestCase tcase = new();
                tcase.Client = client;
                tcase.Name = $"{testCase.Path} {testCase.Suite}";
                tcase.Options = testCase.Options;
                tcase.File = "spec".ToDirectory().JoinFile(testCase.Path);
                tcase.InputFiles = testCase.InputFiles;
                tcase.Solver = solver;
                // tcase.Suite = testCase.s;
                tcase.Path = testCase.Path;
                yield return tcase;
            }
        }
    }

    public struct IntegrationTestCase
    {
        public MiniZincClient Client;
        public FileInfo File;
        public string Path;
        public string Name;
        public int Sequence;
        public string? Solver;
        public List<string>? InputFiles;
        public JsonNode? Options;
    }

    [Test]
    [MethodDataSource(typeof(SatisfyTests), nameof(GetSatisfyTests))]
    [ArgumentDisplayFormatter<TestNameFormatter>]
    public async Task RunTest(IntegrationTestCase test)
    {
        WriteLine($"{test.Path}");
        WriteLine("--------------------------------------");
        var source = await File.ReadAllTextAsync(test.File.ToString());
        // WriteLine(source);
        // WriteLine("--------------------------------------");

        MiniZincModel? model;
        try
        {
            model = MiniZincModel.FromFile(test.File);
        }
        catch (Exception exn)
        {
            model = null;
            return;
        }

        var mzn = model.Write();
        WriteLine(mzn);
        WriteLine("--------------------------------------");

        var solver = test.Solver;
        var client = test.Client;
        WriteLine("--------------------------------------");

        await foreach (var sol in client.Solve(model, _cts.Token, solver))
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

    public class TestNameFormatter : ArgumentDisplayFormatter
    {
        public override bool CanHandle(object? value)
        {
            return value is IntegrationTestCase;
        }

        public override string FormatValue(object? value)
        {
            var tcase = (IntegrationTestCase)value;
            return tcase.Name;
        }
    }
}
