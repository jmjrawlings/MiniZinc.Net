namespace MiniZinc.Build;

public sealed record TestResult
{
    public required YamlNode Expected { get; init; }

    // public required SolveStatus SolveStatus { get; init; }

    public string? ErrorType { get; init; }

    public string? ErrorMessage { get; init; }

    public string? ErrorRegex { get; init; }
}
