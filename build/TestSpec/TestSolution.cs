namespace LibMiniZinc.Tests;

using System.Text.Json.Nodes;

public sealed record TestSolution
{
    public required string Dzn { get; set; }

    public string? Output { get; set; }
}
