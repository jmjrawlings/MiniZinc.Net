namespace Make;

using LibMiniZinc.Tests;

public sealed class ClientSatisfyTestsBuilder : ClientTestsBuilder
{
    public ClientSatisfyTestsBuilder(TestSpec spec)
        : base("SatisfyTests", spec)
    {
        foreach (var testCase in spec.TestCases)
        {
            if (testCase.Type is not TestType.Satisfy)
                continue;

            if (GetTestInfo(testCase) is not { } info)
                continue;

            using var _ = WriteTestHeader(info);
            WriteLn("await TestSatisfy(path, solver, solutions, args);");
        }
    }
}
