namespace Make;

using LibMiniZinc.Tests;

public sealed class ErrorTestsBuilder : ClientTestsBuilder
{
    public ErrorTestsBuilder(TestSpec spec)
        : base("ErrorTests", spec)
    {
        foreach (var testCase in spec.TestCases)
        {
            var isErr = testCase.Type switch
            {
                TestType.Error => true,
                TestType.AssertionError => true,
                TestType.EvaluationError => true,
                TestType.SyntaxError => true,
                TestType.TypeError => true,
                TestType.MiniZincError => true,
                _ => false
            };

            if (!isErr)
                continue;

            if (GetTestInfo(testCase) is not { } info)
                continue;

            WriteTest(info);
        }
    }
}
