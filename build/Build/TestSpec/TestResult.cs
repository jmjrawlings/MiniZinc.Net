namespace MiniZinc.Build;

public sealed record TestResult
{
    public required TestCase TestCase { get; init; }

    public required TestSuite TestSuite { get; init; }

    // public required SolveStatus SolveStatus { get; init; }

    public required string ErrorType { get; init; }

    public required string ErrorMessage { get; init; }

    public required string ErrorRegex { get; init; }
}
