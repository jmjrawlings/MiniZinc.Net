namespace MiniZinc.Net.Tests;

using System.Text.Json.Nodes;

public sealed record TestResult
{
    public required JsonNode? Solution { get; init; }

    // public required SolveStatus SolveStatus { get; init; }

    public string? ErrorType { get; init; }

    public string? ErrorMessage { get; init; }

    public string? ErrorRegex { get; init; }
}
