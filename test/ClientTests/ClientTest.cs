namespace MiniZinc.Tests;

using System.Text;
using System.Text.Json.Nodes;
using Parser.Syntax;

public class ClientTest : TestBase, IClassFixture<ClientFixture>
{
    protected readonly MiniZincClient MiniZinc;

    public ClientTest(ITestOutputHelper output, ClientFixture fixture)
        : base(output)
    {
        MiniZinc = fixture.MiniZinc;
    }

    protected async Task TestAllSolutions(
        string path,
        string solver,
        List<string> solutions,
        List<string> args,
        params SolveStatus[] statuses
    )
    {
        WriteLn(path);
        WriteSection();
        var source = File.ReadAllText(path);
        WriteLn(source);
        WriteSection();
        WriteLn();

        var model = Model.FromFile(path);
        WriteLn(source);
        WriteSection();

        var options = SolveOptions.Create(solverId: solver).AddArgs(args);
        var result = await MiniZinc.Solve(model, options);
        WriteLn(result.Command);
        result.IsSuccess.Should().BeTrue();
        if (statuses.Length > 0)
            result.Status.Should().BeOneOf(statuses);

        var actual = result.Data;
        foreach (var json in solutions)
        {
            var expected = (JsonObject)JsonNode.Parse(json)!;
            var equal = CheckSolution(expected, actual);
            equal.Should().BeTrue();
        }
    }

    protected async Task TestAnySolution(
        string path,
        string solver,
        List<string>? solutions,
        List<string> args,
        params SolveStatus[] statuses
    )
    {
        WriteLn(path);
        WriteSection();
        var source = File.ReadAllText(path);
        WriteLn(source);
        WriteSection();
        WriteLn();

        var model = Model.FromFile(path);
        WriteLn(source);
        WriteSection();

        var options = SolveOptions.Create(solverId: solver).AddArgs(args);
        var result = await MiniZinc.Solve(model, options);
        WriteLn(result.Command);
        result.IsSuccess.Should().BeTrue();

        if (statuses.Length > 0)
            result.Status.Should().BeOneOf(statuses);

        if (solutions is not { Count: > 0 })
            return;

        var actual = result.Data;
        foreach (var json in solutions)
        {
            var expected = (JsonObject)JsonNode.Parse(json)!;
            if (CheckSolution(expected, actual))
                return;
        }
        Assert.Fail("No valid solutions found");
    }

    protected async Task TestError(
        string path,
        string solver,
        string? errorMessage,
        string? errorRegex,
        List<string> args
    )
    {
        WriteSection();
        WriteLn(path);

        Model? model = null;
        try
        {
            model = Model.FromFile(path);
        }
        catch (Exception ex)
        {
            WriteLn(ex.Message);
        }

        if (model is null)
            return;

        WriteLn(model.SourceText);
        WriteSection();

        var options = SolveOptions.Create(solverId: solver);
        options = options.AddArgs(args);

        var result = await MiniZinc.Solve(model, options);
        result.IsSuccess.Should().BeFalse();

        if (errorRegex is not null)
            result.Error.Should().MatchRegex(errorRegex);
        else if (errorMessage is not null)
            result.Error.Should().Be(errorMessage);
    }

    public async Task TestOptimise(
        string path,
        string solver,
        List<string> solutions,
        List<string> args
    )
    {
        await TestAnySolution(path, solver, solutions, args, SolveStatus.Optimal);
    }

    public async Task TestSatisfy(
        string path,
        string solver,
        List<string> solutions,
        List<string> args
    )
    {
        await TestAnySolution(
            path,
            solver,
            solutions,
            args,
            SolveStatus.Satisfied,
            SolveStatus.Optimal
        );
    }

    public async Task TestUnsatisfiable(string path, string solver, List<string> args)
    {
        await TestAnySolution(path, solver, null, args, SolveStatus.Unsatisfiable);
    }

    /// <summary>
    /// Compare the solution data against the expected solution json
    /// </summary>
    public bool CheckSolution(JsonObject expectedData, DataSyntax actualData)
    {
        foreach (var kv in expectedData)
        {
            var name = kv.Key;
            var expectedVar = kv.Value;
            if (!actualData.TryGetValue(name, out var actualVar))
            {
                WriteLn($"Expected variable \"{name}\" missing from actual solution");
                return false;
            }

            if (!CheckSolution(expectedVar!, actualVar))
            {
                WriteLn();
                WriteLn("While comparing expected solution:");
                WriteLn($"{expectedVar?.ToJsonString()}");
                WriteLn();
                WriteLn("And actual solution:");
                WriteLn($"{actualVar.Write()}");
                WriteLn();
                WriteSection();
                return false;
            }
        }
        return true;
    }

    /// <summary>
    /// Compare the solution against the json node
    /// </summary>
    public bool CheckSolution(JsonNode? expected, ExpressionSyntax actual)
    {
        int i = 0;
        switch (expected, actual)
        {
            case (null, EmptyLiteralSyntax):
                break;

            case (JsonValue val, IdentifierSyntax id):
                if (val.ToString() != id.Name)
                    return Error(val, id);
                break;

            case (JsonValue val, IntLiteralSyntax iexpr):
                if (!val.TryGetValue(out i))
                    return Error(val, iexpr);
                if (i != iexpr.Value)
                    return Error(i, iexpr);
                break;

            case (JsonValue val, FloatLiteralSyntax dexpr):
                if (!val.TryGetValue(out decimal d))
                    return Error(val, dexpr);
                if (d != dexpr.Value)
                    return Error(d, dexpr);
                break;

            case (JsonValue val, BoolLiteralSyntax bexpr):
                if (!val.TryGetValue(out bool b))
                    return Error(val, bexpr);
                if (b != bexpr.Value)
                    return Error(b, bexpr);
                break;

            case (JsonValue val, StringLiteralSyntax sexpr):
                if (val.ToString() != sexpr.Value)
                    return Error(val, sexpr);
                break;

            case (JsonArray arr, ArraySyntax aexpr):
                if (!CheckSolution(arr, aexpr.Elements))
                    return false;
                break;

            case (JsonArray arr, TupleLiteralSyntax tuple):
                if (!CheckSolution(arr, tuple.Fields))
                    return false;
                break;

            case (JsonArray arr, CallSyntax { Name: "array1d" } arr1d):
                if (!CheckSolution(arr, arr1d.Args![1]))
                    return false;
                break;

            case (JsonArray arr, CallSyntax { Name: "array2d" } arr1d):
                if (!CheckSolution(arr, arr1d.Args![2]))
                    return false;
                break;

            case (JsonArray arr, CallSyntax { Name: "array3d" } arr1d):
                if (!CheckSolution(arr, arr1d.Args![3]))
                    return false;
                break;

            case (JsonObject sobj, _) when sobj.TryGetPropertyValue("_set_", out var set):
                /* Sets and Ranges are unified into set literal dzn syntax
                 * and compared as strings for now */
                var expectedDzn = set!.ToString();
                string actualDzn = "";
                var sb = new StringBuilder();
                switch (actual)
                {
                    case RangeLiteralSyntax rng:
                        sb.Append('{');
                        int lower = rng.Lower!.Start.IntValue;
                        int upper = rng.Upper!.Start.IntValue;
                        for (int j = lower; j <= upper; j++)
                        {
                            if (++i > 1)
                                sb.Append(',');
                            sb.Append(j);
                        }

                        sb.Append('}');
                        actualDzn = sb.ToString();
                        break;
                    default:
                        actualDzn = actual.ToString();
                        break;
                }
                if (expectedDzn != actualDzn)
                    return Error(expectedDzn, actualDzn);
                break;

            case (JsonObject obj, RecordLiteralSyntax rec):
                foreach (var kv in obj)
                {
                    var fieldName = kv.Key;
                    var fieldNode = kv.Value;
                    ExpressionSyntax? field = null;
                    foreach (var (name, f) in rec.Fields)
                        if (fieldName == name.Name)
                        {
                            field = f;
                            break;
                        }

                    if (field is null)
                        return Error($"Expected record field \"{fieldName}\" missing from actual");

                    if (!CheckSolution(fieldNode, field))
                        return false;
                }
                break;
            default:
                Error($"Could not compare {expected} and {actual}");
                return false;
        }

        return true;
    }

    bool CheckSolution(JsonArray expectedList, List<ExpressionSyntax> actualList)
    {
        var expectedCount = expectedList.Count;
        var actualCount = actualList.Count;
        if (expectedCount != actualCount)
            return Error($"Expected {expectedCount} items found {actualCount}");

        for (int i = 0; i < expectedCount; i++)
        {
            if (i >= actualCount)
                return Error($"Item {i} does not exist in actual value");

            var expected = expectedList[i];
            var actual = actualList[i];
            if (!CheckSolution(expected, actual))
                return false;
        }

        return true;
    }

    public bool Error(string msg)
    {
        WriteLn(msg);
        return false;
    }

    public bool Error(object expected, object actual)
    {
        WriteLn($"Expected {expected} but got {actual}");
        return false;
    }
}
