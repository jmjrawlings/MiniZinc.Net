namespace Make;

using LibMiniZinc.Tests;

public sealed class AnySolutionTestsBuilder : ClientTestsBuilder
{
    public AnySolutionTestsBuilder(TestSpec spec)
        : base("AnySolutionTests", spec)
    {
        foreach (var testCase in spec.TestCases)
        {
            if (testCase.Type is not TestType.AnySolution)
                continue;

            if (GetTestInfo(testCase) is not { } info)
                continue;
            using var _ = WriteTestHeader(info);
            WriteLn("await TestAnySolution(path, solver, solutions, args);");
        }
    }
}
