namespace Make;

using LibMiniZinc.Tests;

public sealed class AllSolutionsTestsBuilder : ClientTestsBuilder
{
    public AllSolutionsTestsBuilder(TestSpec spec)
        : base("AllSolutionsTests", spec)
    {
        foreach (var testCase in spec.TestCases)
        {
            if (testCase.Type is not TestType.AnySolution)
                continue;

            if (GetTestInfo(testCase) is not { } info)
                continue;

            using var _ = WriteTestHeader(info);
            WriteLn("await TestAllSolutions(path, solver, solutions, args);");
        }
    }
}
