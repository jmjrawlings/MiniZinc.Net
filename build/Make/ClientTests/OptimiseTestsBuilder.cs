namespace Make;

using LibMiniZinc.Tests;
using MiniZinc.Parser;

public sealed class OptimiseTestsBuilder : ClientTestsBuilder
{
    public OptimiseTestsBuilder(TestSpec spec)
        : base("OptimiseTests", spec)
    {
        using (
            Function(
                "async Task TestOptimise",
                "string path",
                "string solver",
                "List<string> solutions",
                "List<string> args"
            )
        )
        {
            WriteMessage("path");
            WriteSection();
            Var("model", "Model.FromFile(path)");
            WriteMessage("model.SourceText");
            WriteSection();
            NewLine();
            Var("options", "SolveOptions.Create(solverId:solver).AddArgs(args)");
            NewLine();
            WriteSolutionCheck();
            WriteLn("result.Status.Should().Be(SolveStatus.Optimal);");
            WriteLn("allSolutions.Should().BeTrue();");
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
        WriteLn("await TestOptimise(path, solver, solutions, args);");
    }
}
