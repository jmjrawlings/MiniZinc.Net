namespace MiniZinc.Net.Tests;

using System.Text.Json.Nodes;

public sealed record TestCase
{
    public string Path { get; set; }

    public List<string>? Solvers { get; init; }

    public List<string>? ExtraFiles { get; init; }

    public JsonNode? SolveOptions { get; init; }

    public List<TestResult> Results { get; } = new();

    public override string ToString() => $"<{Path}>";
}
