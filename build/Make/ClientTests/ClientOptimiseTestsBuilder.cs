namespace Make;

using LibMiniZinc.Tests;
using MiniZinc.Parser;

public sealed class ClientOptimiseTestsBuilder : ClientTestsBuilder
{
    public ClientOptimiseTestsBuilder(TestSpec spec)
        : base("ClientOptimiseTests", spec)
    {
        using (Function("async Task Test", "string path", "string solver", "string expected"))
        {
            WriteMessage("path");
            WriteSection();
            Var("model", "Model.FromFile(path)");
            WriteMessage("model.SourceText");
            WriteSection();
            Var("options", "SolveOptions.Create(solverId:solver)");
            Var("result", "await MiniZinc.Solve(model, options)");
            WriteLn("result.IsSuccess.Should().BeTrue();");
            WriteLn("result.Status.Should().Be(SolveStatus.Optimal);");
            WriteLn("result.DataString.Should().Be(expected);");
        }
        foreach (var testCase in spec.TestCases)
        {
            if (testCase.Type is not TestType.Optimise)
                continue;

            if (GetTestInfo(testCase) is not { } info)
                continue;

            WriteTest(info);
        }
    }

    void WriteTest(TestCaseInfo info)
    {
        using var _ = WriteTestHeader(info);
        var dzn = info
            .Solutions.Select(sol => sol.Dzn)
            .Where(dzn => dzn is not null)
            .FirstOrDefault();

        if (dzn is null)
            return;

        Var("expected", Quote(dzn));
        Write("await Test(path, solver, expected);");
        NewLine();
    }
}
