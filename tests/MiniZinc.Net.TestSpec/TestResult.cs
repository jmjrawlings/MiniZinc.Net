namespace MiniZinc.Net.Tests;

using System.Text.Json.Nodes;

public enum ResultType
{
    Solution,
    Error,
    FlatZinc,
    OutputModel,
    Satisfied
}

public sealed record TestResult
{
    public required ResultType Type { get; init; }
    public JsonNode? Solution { get; init; }
    public List<string>? Files { get; init; }
    public string? ErrorType { get; init; }
    public string? ErrorMessage { get; init; }
    public string? ErrorRegex { get; init; }
}
