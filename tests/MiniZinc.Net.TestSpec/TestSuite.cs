using System.Text.Json.Nodes;

namespace MiniZinc.Net.Tests;

using System.IO;

public sealed class TestSuite
{
    public required string Name { get; init; }

    public bool? Strict { get; init; }

    public required JsonObject? Options { get; init; }

    public required List<string> Solvers { get; init; }

    public required List<string> IncludeGlobs { get; init; }

    // public List<string> IncludeFiles { get; } = new();

    public required List<TestCase> TestCases { get; init; }

    public override string ToString() => $"<{Name}\" ({TestCases.Count} cases)>";
}
