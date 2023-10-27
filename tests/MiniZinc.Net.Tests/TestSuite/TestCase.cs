namespace MiniZinc.Tests;

public sealed class TestCase
{
    public string TestName { get; init; }

    public FileInfo TestFile { get; init; }

    public string TestPath { get; init; }

    public string ModelString { get; init; }

    public List<FileInfo> Includes { get; init; }

    public List<string> Solvers { get; init; }

    public readonly TestSuite TestSuite;

    public readonly List<TestResult> TestResults;

    public TestCase(TestSuite suite)
    {
        TestSuite = suite;
        TestResults = new List<TestResult>();
    }

    public override string ToString() => $"<Test \"{TestFile.Name}\">";
}