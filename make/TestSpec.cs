namespace MiniZinc.Tests;

using System.Diagnostics;
using System.Text.Json.Nodes;

public sealed class TestSpec
{
    public List<TestCase> TestCases = new();
}

[DebuggerDisplay("{Path} - {Type} - {Args}")]
public record TestCase
{
    public string Suite;

    /// Path of the test case relative to the test spec dir
    public string Path;

    /// Enumeration of test type
    public TestType Type;

    /// Solvers to use
    public List<string>? Solvers;

    /// Solvers to check against (for Type.CheckAgainst)
    public List<string>? CheckAgainstSolvers;

    /// Any additional input files (eg: json data)
    public List<string>? ExtraFiles;

    /// Extra options
    public JsonObject? Options;

    /// Command line args
    public string? Args;

    /// All solutions, present if Type == AllSolutions or AnySolution
    public List<string>? Solutions;

    /// Any expected output files produced by the solver (eg flatzinc)
    public List<string>? OutputFiles;

    /// If present, the error message should match this string
    public string? ErrorMessage;

    /// If present, the error message should match this regex
    public string? ErrorRegex;
}

public enum TestType
{
    SOLVE,
    COMPILE,
    OUTPUT_MODEL,
    CHECK_AGAINST,
    ALL_SOLUTIONS,
    ANY_SOLUTION,
    UNSATISFIABLE,
    ERROR,
    ASSERTION_ERROR,
    EVALUATION_ERROR,
    MINIZINC_ERROR,
    TYPE_ERROR,
    SYNTAX_ERROR,
    OUTPUT
}
