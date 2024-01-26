namespace MiniZinc.Net.Tests;

public sealed record TestSpec
{
    public List<TestSuite> TestSuites { get; set; }

    public List<TestCase> TestCases { get; set; }
}
