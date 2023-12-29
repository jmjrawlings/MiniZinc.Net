namespace MiniZinc.Net.Tests;

using System.Text.Json.Nodes;

public sealed class TestCase
{
    public required string Name { get; init; }

    public required string Path { get; init; }

    public List<string>? Solvers { get; init; }

    public List<string>? ExtraFiles { get; init; }

    public JsonNode? SolveOptions { get; init; }

    public required List<TestResult> Results { get; init; }

    public override string ToString() => $"<Test \"{Name}\">";
}
