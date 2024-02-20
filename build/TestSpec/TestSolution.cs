namespace LibMiniZinc.Tests;

using System.Text.Json.Nodes;

public sealed record TestSolution
{
    public required JsonObject Vars { get; set; }

    public string? Output { get; set; }

    public JsonValue? Objective { get; set; }
}
