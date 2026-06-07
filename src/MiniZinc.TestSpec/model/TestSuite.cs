namespace MiniZinc.TestSpec.Model;

public sealed record TestSuite(
    string Name,
    IReadOnlyList<string> IncludeGlobs,
    IReadOnlyList<string>? Solvers,
    IReadOnlyDictionary<string, string?>? Options,
    bool Strict
);
