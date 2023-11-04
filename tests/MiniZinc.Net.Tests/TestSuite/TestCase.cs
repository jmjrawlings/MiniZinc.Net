namespace MiniZinc.Tests;

public sealed class TestCase
{
    public required TestSuite TestSuite { get; init; }
    public required string TestName { get; init; }
    public required string TestPath { get; init; }
    public required FileInfo TestFile { get; init; }
    public required List<FileInfo> Includes { get; init; }
    public required List<string> Solvers { get; init; }
    public required YamlNode SolveOptions { get; init; }
    public List<TestResult> TestResults { get; } = new();

    public override string ToString() => $"<Test \"{TestFile.Name}\">";
}
