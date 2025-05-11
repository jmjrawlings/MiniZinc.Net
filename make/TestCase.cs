namespace MiniZinc.Tests;

using System.Diagnostics;
using System.Text.Json.Nodes;

[DebuggerDisplay("{Path}")]
public record TestCase
{
    public string Suite { get; set; }

    /// Path of the test case relative to the test spec dir
    public string Path { get; set; }

    /// Enumeration of test type
    public TestType Type { get; set; }

    /// Solvers to use
    public List<string>? Solvers { get; set; }

    /// Solvers to check against (for Type.CheckAgainst)
    public List<string>? CheckAgainst { get; set; }

    /// Any additional input files (eg: json data)
    public List<string>? ExtraFiles { get; set; }

    /// Extra options
    public JsonObject? Options { get; set; }

    /// Command line args
    public string? Args { get; set; }

    /// All solutions, present if Type == AllSolutions or AnySolution
    public List<string>? Solutions { get; set; }

    /// Any expected output files produced by the solver (eg flatzinc)
    public List<string>? OutputFiles { get; set; }

    /// If present, the error message should match this string
    public string? ErrorMessage { get; set; }

    /// If present, the error message should match this regex
    public string? ErrorRegex { get; set; }
}
