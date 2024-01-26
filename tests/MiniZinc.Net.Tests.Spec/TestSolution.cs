namespace MiniZinc.Net.Tests;

using System.Text.Json.Nodes;

public sealed record TestSolution
{
    public JsonObject Vars { get; init; }

    public string? Output { get; init; }

    public JsonValue? Objective { get; init; }
}
