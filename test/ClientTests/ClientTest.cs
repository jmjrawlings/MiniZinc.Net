using System.Text;

namespace MiniZinc.Tests;

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
        Write(path);
        WriteSection();

        var model = Model.FromFile(path);
        Write(model.SourceText);
        WriteSection();

        var options = SolveOptions.Create(solverId: solver).AddArgs(args);
        var result = await MiniZinc.Solve(model, options);
        Write(result.Command);
        result.IsSuccess.Should().BeTrue();
        if (statuses.Length > 0)
            result.Status.Should().BeOneOf(statuses);

        foreach (var json in solutions)
        {
            var node = (JsonObject)JsonNode.Parse(json)!;
            var equal = CheckSolution(result.Data, node);
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
        Write(path);
        WriteSection();

        var model = Model.FromFile(path);
        Write(model.SourceText);
        WriteSection();

        var options = SolveOptions.Create(solverId: solver).AddArgs(args);
        var result = await MiniZinc.Solve(model, options);
        Write(result.Command);
        result.IsSuccess.Should().BeTrue();

        if (statuses.Length > 0)
            result.Status.Should().BeOneOf(statuses);

        if (solutions is null)
            return;

        foreach (var json in solutions)
        {
            var node = (JsonObject)JsonNode.Parse(json)!;
            var equal = CheckSolution(result.Data, node);
            if (equal)
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
        Write(path);

        Model? model = null;
        try
        {
            model = Model.FromFile(path);
        }
        catch (Exception ex)
        {
            Write(ex.Message);
        }

        if (model is null)
            return;

        Write(model.SourceText);
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
    public bool CheckSolution(DataSyntax data, JsonObject json)
    {
        WriteSection();
        Write("Checking actual solution:");
        Write(data.ToString());
        Write();
        Write("Against expected solution:");
        Write(json.ToString());
        Write();

        foreach (var assign in data)
        {
            var name = assign.Key;
            var actual = assign.Value;
            if (!json.TryGetPropertyValue(name, out var expected))
            {
                Write($"Could not find \"{name}\" in expected solution");
                return false;
            }

            if (!CheckSolution(actual, expected!))
                return false;
        }
        return true;
    }

    /// <summary>
    /// Compare the solution against the json node
    /// </summary>
    public bool CheckSolution(ExpressionSyntax expr, JsonNode node)
    {
        int i = 0;
        switch (node, expr)
        {
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
                foreach (var z in arr)
                {
                    var a = 2;
                }
                return false;

            case (JsonObject sobj, _) when sobj.ContainsKey("_set_"):
                var expectedDzn = sobj["_set_"]!.ToString();
                string actualDzn = "";
                var sb = new StringBuilder();
                switch (expr)
                {
                    case RangeLiteralSyntax rng:
                        sb.Append('{');
                        int lower = rng.Lower.Start.IntValue;
                        int upper = rng.Upper.Start.IntValue;
                        for (int j = lower; j <= upper; j++)
                        {
                            if (++i < 1)
                                sb.Append(',');
                            sb.Append(j);
                        }

                        sb.Append('}');
                        actualDzn = sb.ToString();
                        break;
                    default:
                        actualDzn = expr.ToString();
                        break;
                }
                if (expectedDzn != actualDzn)
                    return Error(expectedDzn, actualDzn);
                break;

            case (JsonObject obj, RecordLiteralSyntax rec):
                foreach (var (id, fieldExpr) in rec.Fields)
                {
                    if (!obj.TryGetPropertyValue(id.Name, out var fieldNode))
                        return Error("Actual record field \"{id}\" missing from expected");
                    if (CheckSolution(fieldExpr, fieldNode!))
                        return false;
                }
                break;
            default:
                Error($"Could not compare {node} and {expr}");
                return false;
        }

        return true;
    }

    public bool Error(string msg)
    {
        Write(msg);
        return false;
    }

    public bool Error(object expected, object actual)
    {
        Write($"Expected {expected} but got {actual}");
        return false;
    }
}
