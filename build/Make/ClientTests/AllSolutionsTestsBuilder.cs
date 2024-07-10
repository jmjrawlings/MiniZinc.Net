namespace Make;

using LibMiniZinc.Tests;

public sealed class AllSolutionsTestsBuilder : ClientTestsBuilder
{
    public AllSolutionsTestsBuilder(TestSpec spec)
        : base("AllSolutionsTests", spec)
    {
        foreach (var testCase in spec.TestCases)
        {
            if (testCase.Type is not TestType.AllSolutions)
                continue;

            if (GetTestInfo(testCase) is not { } info)
                continue;

            WriteTest(info);
        }
    }
}
