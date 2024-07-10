namespace Make;

using LibMiniZinc.Tests;

public sealed class UnsatisfiableTestsBuilder : ClientTestsBuilder
{
    public UnsatisfiableTestsBuilder(TestSpec spec)
        : base("UnsatisfiableTests", spec)
    {
        foreach (var testCase in spec.TestCases)
        {
            if (testCase.Type is not TestType.Unsatisfiable)
                continue;

            if (GetTestInfo(testCase) is not { } info)
                continue;

            WriteTest(info);
        }
    }
}
