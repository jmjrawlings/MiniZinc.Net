namespace MiniZinc.Build;

public sealed class TestCase
{
    public required string TestName { get; init; }

    public required string TestPath { get; init; }

    public required List<string> Includes { get; init; }

    public required List<string> Solvers { get; init; }

    //public required YamlNode SolveOptions { get; init; }

    public List<TestResult> Results { get; } = new();

    public override string ToString() => $"<Test \"{TestName}\">";
}
