namespace MiniZinc.Net.Tests;

using System.Text.Json.Nodes;

public sealed record TestSuite
{
    public required string Name { get; init; }

    public bool? Strict { get; init; }

    public JsonObject? Options { get; init; }

    public List<string>? Solvers { get; init; }

    public required List<string> IncludeGlobs { get; init; }

    public override string ToString() => $"<{Name}>";
}
