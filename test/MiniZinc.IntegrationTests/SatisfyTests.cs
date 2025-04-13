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
    [Test]
    public async Task TestParseTests()
    {
        var source = "./spec/suites.yml".ToFile();
        var spec = TestParser.ParseTestSpecFile(source);
        spec.TestSuites.ShouldNotBeEmpty();
    }

    public static IEnumerable<SatisfyTestCase> GetSatisfyTests()
    {
        var client = MiniZincClient.Autodetect();
        var source = "./spec/suites.yml".ToFile();
        var testSpec = TestParser.ParseTestSpecFile(source);
        foreach (var testSuite in testSpec.TestSuites)
        {
            foreach (var testCase in testSuite.TestCases)
            {
                if (testCase.Type is not TestType.TEST_SATISFY)
                    continue;

                if (testSuite.Solvers is null)
                    continue;

                foreach (var solver in testSuite.Solvers)
                {
                    SatisfyTestCase tcase = new();
                    tcase.Client = client;
                    tcase.Options = testSuite.Options;
                    tcase.Path = source.Directory!.JoinFile(testCase.Path);
                    tcase.Sequence = testCase.Sequence;
                    tcase.InputFiles = testCase.InputFiles;
                    tcase.Solver = solver;
                    tcase.Suite = testSuite;
                    yield return tcase;
                }
            }
        }
    }

    public struct SatisfyTestCase
    {
        public MiniZincClient Client;
        public FileInfo Path;
        public int Sequence;
        public string? Solver;
        public List<string>? InputFiles;
        public JsonNode? Options;
        public TestSuite Suite;
    }

    [Test]
    [MethodDataSource(typeof(SatisfyTests), nameof(GetSatisfyTests))]
    [ArgumentDisplayFormatter<TestNameFormatter>]
    public async Task RunTest(SatisfyTestCase test)
    {
        var client = test.Client;
        WriteLine($"Test model: {test.Path}");
        WriteLine("--------------------------------------");
        var source = await File.ReadAllTextAsync(test.Path.ToString());
        WriteLine(source);
        WriteLine("--------------------------------------");

        MiniZincModel? model;
        try
        {
            model = MiniZincModel.FromFile(test.Path);
        }
        catch (Exception exn)
        {
            model = null;
            return;
        }

        var mzn = model.Write();

        WriteLine($"Parsed model: ");
        WriteLine("--------------------------------------");
        WriteLine(mzn);
        WriteLine("--------------------------------------");

        var solver = test.Solver;
        var options = new MiniZincOptions(solver);
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
            return value is SatisfyTestCase;
        }

        public override string FormatValue(object? value)
        {
            var tcase = (SatisfyTestCase)value;
            return $"{tcase.Suite.Name} - {tcase.Path}";
        }
    }
}
