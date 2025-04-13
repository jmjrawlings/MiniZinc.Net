namespace MiniZinc.Tests;

using System.Text.Json.Nodes;

public sealed class TestSuite
{
    public required string Name { get; init; }

    public required bool? Strict { get; init; }

    public required JsonObject? Options { get; init; }

    public required IReadOnlyList<string>? Solvers { get; init; }

    public required IReadOnlyList<TestCase> TestCases { get; init; }

    public required IReadOnlyList<string> IncludeGlobs { get; init; }

    public override string ToString() => $"<{Name}>";
}
