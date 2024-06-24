using LibMiniZinc.Tests;

namespace Make;

public sealed class ClientAllSolutionsTestsBuilder : ClientTestsBuilder
{
    public ClientAllSolutionsTestsBuilder(TestSpec spec)
        : base("ClientAllSolutionsTests", spec)
    {
        foreach (var testCase in spec.TestCases)
        {
            if (testCase.Type is not TestType.AllSolutions)
                continue;

            if (GetTestInfo(testCase) is not { } info)
                continue;

            MakeTest(info);
        }
    }

    void MakeTest(TestCaseInfo info)
    {
        using var _ = WriteTestHeader(info);
        Var("solution", "await Solve(model, options, SolveStatus.Satisfied)");
    }
}
