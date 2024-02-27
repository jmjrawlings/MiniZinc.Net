namespace LibMiniZinc.Tests;

using System.Text.Json.Nodes;

public sealed record TestSuite
{
    public required string Name { get; init; }

    public bool? Strict { get; set; }

    public JsonObject? Options { get; set; }

    public List<string>? Solvers { get; set; }

    public required List<string> IncludeGlobs { get; init; }

    public override string ToString() => $"<{Name}>";
}
