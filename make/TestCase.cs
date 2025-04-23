namespace MiniZinc.Tests;

using System.Diagnostics;
using System.Text.Json.Nodes;

[DebuggerDisplay("{Path}")]
public record TestCase
{
    /// Name of the test suite this belongs to
    public string Suite { get; set; }

    /// Path of the test case relative to the test spec dir
    public string Path { get; set; }

    /// Solvers for run the test on
    public List<string>? Solvers { get; set; }

    /// Any additional input files (eg: json data)
    public List<string>? InputFiles { get; set; }

    /// Extra options
    public JsonObject? Options { get; set; }

    /// Enumeration of test type
    public TestType Type { get; set; }

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
