namespace MiniZinc.Net.Tests;

using System.Text.Json.Nodes;

public enum TestResultType
{
    Solve,
    Error,
    Compile,
    Output,
    Satisfy,
    Unsatisfiable
}

public sealed record TestResult
{
    public required TestResultType Type { get; init; }
    public JsonNode? Solution { get; set; }
    public List<string>? Files { get; set; }
    public string? ErrorType { get; set; }
    public string? ErrorMessage { get; set; }
    public string? ErrorRegex { get; set; }
}
