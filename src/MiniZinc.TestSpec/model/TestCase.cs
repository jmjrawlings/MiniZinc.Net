namespace MiniZinc.TestSpec.Model;

public sealed record TestCase
{
    public required string Suite { get; init; }
    public required string Path { get; init; }
    public required TestKind Kind { get; init; }
    public required IReadOnlyList<string> Solvers { get; init; }
    public IReadOnlyList<string>? CheckAgainstSolvers { get; init; }
    public required IReadOnlyList<string> ExtraFiles { get; init; }
    public required IReadOnlyDictionary<string, string?> Options { get; init; }
    public string? Args { get; init; }
    public required IReadOnlyList<Expected> Expected { get; init; }
    public string? SkipReason { get; init; }
    public string? Name { get; init; }
    public required IReadOnlyList<string> Markers { get; init; }
}
