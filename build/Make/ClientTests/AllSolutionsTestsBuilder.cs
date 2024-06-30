namespace Make;

using LibMiniZinc.Tests;

public sealed class AllSolutionsTestsBuilder : ClientTestsBuilder
{
    public AllSolutionsTestsBuilder(TestSpec spec)
        : base("AllSolutionsTests", spec)
    {
        using (
            Function(
                "async Task TestAllSolutions",
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
            WriteLn("allSolutions.Should().BeTrue();");
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
        WriteLn("await TestAllSolutions(path, solver, solutions, args);");
    }
}
