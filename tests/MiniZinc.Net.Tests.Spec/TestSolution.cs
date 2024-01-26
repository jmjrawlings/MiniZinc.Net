namespace MiniZinc.Net.Tests;

using System.Text.Json.Nodes;

public sealed record TestSolution
{
    public JsonObject Vars { get; set; }

    public string? Output { get; set; }

    public JsonValue? Objective { get; set; }
}
