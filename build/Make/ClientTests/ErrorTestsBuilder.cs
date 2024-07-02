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

            using var _ = WriteTestHeader(info);
            if (info.ErrorMessage is { } err)
                Declare("string", "errorMessage", $"\"{err}\"");
            else
                Declare("string?", "errorMessage", null);

            if (info.ErrorRegex is { } regex)
                Declare("string", "errorRegex", $"\"{regex.Replace("\\", "")}\"");
            else
                Declare("string?", "errorRegex", null);

            WriteLn("await TestError(path, solver, errorMessage, errorRegex, args);");
        }
    }
}
