namespace MiniZinc.Net.Tests;

using System.Text.Json.Nodes;

public sealed class TestCase
{
    public required string TestName { get; init; }

    public required string TestPath { get; init; }

    public required List<string>? Solvers { get; init; }

    public required JsonNode? SolveOptions { get; init; }

    public required List<TestResult> Results { get; init; }

    public override string ToString() => $"<Test \"{TestName}\">";
}
