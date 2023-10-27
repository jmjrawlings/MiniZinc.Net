namespace MiniZinc.Tests;

public sealed class TestResult
{
    public readonly TestCase TestCase;

    public readonly TestSuite TestSuite;

    public SolveStatus SolveStatus { get; init; }

    public string ErrorType { get; init; }

    public string ErrorMessage { get; init; }

    public string ErrorRegex { get; init; }

    public TestResult(TestCase testCase)
    {
        TestCase = testCase;
        TestSuite = testCase.TestSuite;
    }

    public override string ToString() => $"<TestResult \"{this.SolveStatus}\">";
}