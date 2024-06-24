namespace Make;

using LibMiniZinc.Tests;
using MiniZinc.Parser;

public sealed class ClientSatisfyTestsBuilder : ClientTestsBuilder
{
    public ClientSatisfyTestsBuilder(TestSpec spec)
        : base("ClientSatisfyTests", spec)
    {
        using (Function("async Task Test", "string path", "string solver", "params string[]? args"))
        {
            WriteMessage("path");
            WriteSection();
            Var("model", "Model.FromFile(path)");
            WriteMessage("model.SourceText");
            WriteSection();
            using (ForEach("var warn in model.Warnings"))
            {
                WriteMessage("warn");
            }
            Var("options", "SolveOptions.Create(solverId:solver)");
            using (If("args is not null"))
                WriteLn("options = options.AddArgs(args);");
            Var("result", "await MiniZinc.Solve(model, options)");
            WriteLn("result.IsSuccess.Should().BeTrue();");
            WriteLn("result.Status.Should().Be(SolveStatus.Satisfied);");

            // WriteLn("string expected;");
            // foreach (var dzn in dzns)
            // {
            //     var result = Parser.ParseDataString(dzn);
            //     if (!result.Ok)
            //         continue;
            //     var expected = result.Data.Write(WriteOptions.Minimal);
            //     WriteLn($"expected = \"{expected}\";");
            //     using (If($"solution.DataString!.Equals(expected)"))
            //         Return();
            // }
            // Call("Assert.Fail", "\"Solution did not match any of the expected results\"");
        }

        foreach (var testCase in spec.TestCases)
        {
            if (testCase.Type is not TestType.Satisfy)
                continue;

            if (GetTestInfo(testCase) is not { } info)
                continue;

            WriteTest(info);
        }
    }

    void WriteTest(TestCaseInfo info)
    {
        using var _ = WriteTestHeader(info);
        var dzns = info
            .Solutions?.Select(sol => sol.Dzn)
            .Where(dzn => dzn is not null)
            .Select(dzn => dzn!);
        var test = "await Test(path, solver";
        if (dzns is not null)
            test += string.Join(",", dzns);
        test += ");";
        WriteLn(test);
    }
}
