namespace LibMiniZinc.Tests;

using System.Diagnostics;
using System.Text.Json.Nodes;

[DebuggerDisplay("{Path}")]
public sealed record TestCase
{
    /// Path of the test case relative to the test spec dir
    public required string Path { get; init; }

    /// Sequence of this test from 1..n in the same file
    public int Sequence { get; set; }

    /// Solvers for run the test on
    public List<string>? Solvers { get; set; }

    /// Solvers to check against
    public List<string>? CheckAgainstSolvers { get; set; }

    /// Any additional input files (eg: json data)
    public List<string>? InputFiles { get; set; }

    /// Solver flags to pass through
    public JsonNode? Options { get; set; }

    /// Enumeration of test type
    public TestType Type { get; set; }

    /// All solutions, present if Type == AllSolutions or AnySolution
    public List<TestSolution>? Solutions { get; set; }

    /// Any expected output files produced by the solver (eg flatzinc)
    public List<string>? OutputFiles { get; set; }

    public string? ErrorType { get; set; }

    public string? ErrorMessage { get; set; }

    public string? ErrorRegex { get; set; }

    public override string ToString() => $"<{Path}>";
}
