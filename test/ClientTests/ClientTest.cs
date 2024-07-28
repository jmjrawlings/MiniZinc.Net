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

    protected async Task Test(
        string path,
        string solver,
        List<string>? solutions = null,
        List<string>? args = null,
        string? error = null,
        bool allSolutions = false,
        List<SolveStatus>? statuses = null
    )
    {
        WriteLn(path);
        WriteSection();
        var source = File.ReadAllText(path);
        WriteLn(source);
        WriteSection();
        WriteLn();

        Model? model;
        try
        {
            model = Model.FromFile(path);
        }
        catch (Exception)
        {
            model = null;
            error.Should().NotBeNull();
            return;
        }

        var mzn = model.Write();
        WriteLn(mzn);
        WriteSection();

        var options = SolveOptions.Create(solverId: solver);

        if (args is not null)
            options = options.AddArgs(args);

        var result = await MiniZinc.Solve(model, options);
        WriteLn(result.Command);
        result.IsSuccess.Should().BeTrue();

        if (statuses is { Count: > 0 })
            result.Status.Should().BeOneOf(statuses);

        if (error is not null)
        {
            result.IsError.Should().BeTrue();
            return;
        }

        if (solutions is not { Count: > 0 })
            return;

        var actual = result.Data;
        foreach (var dzn in solutions)
        {
            var parsed = Parser.Parser.ParseDataString(dzn, out var expected);
            var ok = Check(expected, actual);
            switch (ok, allSolutions)
            {
                case (true, true):
                    return;
                case (false, true):
                    break;
                case (false, false):
                    break;
                case (true, false):
                    return;
            }
        }

        Assert.Fail("No solution found");
    }

    /// <summary>
    /// Compare the solution data against the expected solution json
    /// </summary>
    public bool Check(DataSyntax expectedData, DataSyntax actualData)
    {
        foreach (var kv in expectedData.Values)
        {
            var name = kv.Key;
            var expectedVar = kv.Value;
            if (!actualData.Values.TryGetValue(name, out var actualVar))
                continue;

            if (!Check(expectedVar, actualVar))
            {
                WriteLn();
                WriteLn("While comparing expected solution:");
                WriteLn($"{expectedVar.Write()}");
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
    public bool Check(ExpressionSyntax expected, ExpressionSyntax actual)
    {
        int i = 0;
        switch (expected, actual)
        {
            case (TupleLiteralSyntax expectedTuple, TupleLiteralSyntax actualTuple):
                for (i = 0; i < expectedTuple.Fields.Count; i++)
                {
                    var expectedItem = expectedTuple.Fields[i];
                    var actualItem = actualTuple.Fields[i];
                    if (!Check(expectedItem, actualItem))
                        return false;
                }
                break;

            case (ArraySyntax expectedArray, ArraySyntax actualArray):
                for (i = 0; i < expectedArray.Elements.Count; i++)
                {
                    var expectedItem = expectedArray.Elements[i];
                    var actualItem = actualArray.Elements[i];
                    if (!Check(expectedItem, actualItem))
                        return false;
                }
                break;

            case (
                ArraySyntax expectedArray,
                CallSyntax { Name: "array1d", Args: [_, Array1dSyntax actualArray] }
            ):
                if (!Check(expectedArray, actualArray))
                    return false;
                break;

            case (
                ArraySyntax expectedArray,
                CallSyntax { Name: "array2d", Args: [_, _, Array1dSyntax actualArray] }
            ):
                if (!Check(expectedArray, actualArray))
                    return false;
                break;

            case (
                ArraySyntax expectedArray,
                CallSyntax { Name: "array3d", Args: [_, _, _, Array1dSyntax actualArray] }
            ):
                if (!Check(expectedArray, actualArray))
                    return false;
                break;

            case (ArraySyntax expectedArray, TupleLiteralSyntax actualTuple):
                for (i = 0; i < expectedArray.Elements.Count; i++)
                {
                    var expectedItem = expectedArray.Elements[i];
                    var actualItem = actualTuple.Fields[i];
                    if (!Check(expectedItem, actualItem))
                        return false;
                }
                break;

            case (RecordLiteralSyntax expectedRecord, RecordLiteralSyntax actualRecord):
                foreach (var (fieldName, expectedField) in expectedRecord.Fields)
                {
                    ExpressionSyntax? field = null;
                    foreach (var (name, actualField) in actualRecord.Fields)
                        if (fieldName.Name == name.Name)
                        {
                            field = actualField;
                            break;
                        }

                    if (field is null)
                        Error($"Expected record field \"{fieldName}\" missing from actual");
                    else if (!Check(expectedField, field))
                        return false;
                }
                break;

            case (SetLiteralSyntax set, RangeLiteralSyntax range):
                if (!Check(set, range))
                    return false;
                break;
            case (RangeLiteralSyntax range, SetLiteralSyntax set):
                if (!Check(set, range))
                    return false;
                break;
            default:
                if (!expected.Equals(actual))
                    return false;
                break;
        }

        return true;
    }

    private bool Check(SetLiteralSyntax set, RangeLiteralSyntax range)
    {
        switch (set.Elements)
        {
            case []:
                return false;

            case [var e]:
                if (!e.Equals(range.Lower))
                    return false;
                if (!e.Equals(range.Upper))
                    return false;
                return true;

            case var elements:
                int min = (range.Lower as IntLiteralSyntax).Value;
                int max = (range.Upper as IntLiteralSyntax).Value;
                for (int i = min; i <= max; i++)
                {
                    var e = elements[i];
                    if (e is not IntLiteralSyntax il)
                        return false;
                    if (il.Value != i)
                        return false;
                }

                return true;
            default:
                break;
        }
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
