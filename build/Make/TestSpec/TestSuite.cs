namespace LibMiniZinc.Tests;

using System.Text.Json.Nodes;

public sealed record TestSuite
{
    public string Name { get; set; }

    public bool? Strict { get; set; }

    public JsonObject? Options { get; set; }

    public List<string>? Solvers { get; set; }

    public List<string> IncludeGlobs { get; set; }

    public override string ToString() => $"<{Name}>";
}
