namespace MiniZinc.Build;

using System.IO;

public sealed class TestSuite
{
    public required string Name { get; init; }

    public required bool? Strict { get; init; }

    // public required YamlNode Options { get; init; }

    public List<string> Solvers { get; } = new();

    public List<string> IncludeGlobs { get; } = new();

    public List<string> IncludeFiles { get; } = new();

    public List<TestCase> TestCases { get; } = new();

    public override string ToString() => $"<{Name}\" ({TestCases.Count} cases)>";
}
