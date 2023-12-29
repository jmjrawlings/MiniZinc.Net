namespace MiniZinc.Net.Tests;

using System.IO;
using System.Text.Json.Nodes;

public sealed class TestSuite
{
    public required string Name { get; init; }

    public bool? Strict { get; init; }

    public JsonObject? Options { get; init; }

    public List<string>? Solvers { get; init; }

    public required List<string> IncludeGlobs { get; init; }

    public required List<TestCase> TestCases { get; init; }

    public override string ToString() => $"<{Name}\" ({TestCases.Count} cases)>";
}
