namespace MiniZinc.Tests;

using System.Text.Json.Nodes;
using Command;
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
        var source = await File.ReadAllTextAsync(path);
        WriteLn(source);
        WriteSection();
        WriteLn();

        MiniZincModel? model;
        try
        {
            model = MiniZincModel.FromFile(path);
        }
        catch (Exception exn)
        {
            model = null;
            error.Should().NotBeNull(exn.Message);
            return;
        }

        var mzn = model.Write();
        WriteLn(mzn);
        WriteSection();

        var options = SolveOptions.Create(solverId: solver);

        if (args is not null)
        {
            foreach (var argString in args)
            {
                var arg = Arg.Parse(argString).First();
                if (arg.Value is { } value)
                {
                    var argFile = Path.Join(
                            Directory.GetCurrentDirectory(),
                            value.Replace("\"", "")
                        )
                        .ToFile();
                    if (argFile.Exists)
                        options = options.AddArgs($"{arg.Flag} \"{argFile.FullName}\"");
                    else
                        options = options.AddArgs(arg);
                }
                else
                {
                    options = options.AddArgs(arg);
                }
            }
        }

        var result = await MiniZinc.Solve(model, options);
        WriteLn(result.Command);

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
    public bool Check(MiniZincData expectedData, MiniZincData actualData)
    {
        foreach (var name in expectedData.Keys)
        {
            var expectedVar = expectedData[name];
            if (!actualData.TryGetValue(name, out var actualVar))
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
    public bool Check(Expr expected, Expr actual)
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
