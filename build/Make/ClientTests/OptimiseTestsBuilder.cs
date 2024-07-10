namespace Make;

using LibMiniZinc.Tests;

public sealed class OptimiseTestsBuilder : ClientTestsBuilder
{
    public OptimiseTestsBuilder(TestSpec spec)
        : base("OptimiseTests", spec)
    {
        foreach (var testCase in spec.TestCases)
        {
            if (testCase.Type is not TestType.Optimise)
                continue;

            if (GetTestInfo(testCase) is not { } info)
                continue;

            WriteTest(info);
        }
    }
}
