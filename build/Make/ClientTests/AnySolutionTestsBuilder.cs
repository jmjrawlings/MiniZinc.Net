namespace Make;

using LibMiniZinc.Tests;

public sealed class AnySolutionTestsBuilder : ClientTestsBuilder
{
    public AnySolutionTestsBuilder(TestSpec spec)
        : base("AnySolutionTests", spec)
    {
        using (
            Function(
                "async Task TestAnySolution",
                "string path",
                "string solver",
                "List<string> solutions",
                "List<string> args"
            )
        )
        {
            WriteMessage("path");
            WriteSection();
            NewLine();
            Var("model", "Model.FromFile(path)");
            WriteMessage("model.SourceText");
            WriteSection();
            NewLine();
            Var("options", "SolveOptions.Create(solverId:solver).AddArgs(args);");
            NewLine();
            WriteSolutionCheck();
            WriteLn("anySolution.Should().BeTrue();");
        }

        foreach (var testCase in spec.TestCases)
        {
            if (testCase.Type is not TestType.AnySolution)
                continue;

            if (GetTestInfo(testCase) is not { } info)
                continue;

            WriteTest(info);
        }
    }

    void WriteTest(TestCaseInfo info)
    {
        using var _ = WriteTestHeader(info);
        WriteLn("await TestAnySolution(path, solver, solutions, args);");
    }
}
