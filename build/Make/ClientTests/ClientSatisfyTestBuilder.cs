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
            NewLine();
            Var("model", "Model.FromFile(path)");
            Call("model.Satisfy");
            WriteMessage("model.SourceText");
            WriteSection();
            NewLine();
            Var("options", "SolveOptions.Create(solverId:solver)");
            using (If("args is not null"))
                WriteLn("options = options.AddArgs(args);");
            Var("result", "await MiniZinc.Solve(model, options)");
            WriteLn("result.IsSuccess.Should().BeTrue();");
            WriteLn("result.Status.Should().Be(SolveStatus.Satisfied);");
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
