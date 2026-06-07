namespace MiniZinc.TestSpec.Model;

public sealed class TestSpec
{
    public required IReadOnlyList<TestSuite> Suites { get; init; }
    public required IReadOnlyList<TestCase> TestCases { get; init; }
}
