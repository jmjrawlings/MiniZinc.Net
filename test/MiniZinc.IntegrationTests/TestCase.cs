namespace MiniZinc.Tests;

using System.Diagnostics;
using System.Text.Json.Nodes;
using Client;

[DebuggerDisplay("{Path}")]
public record TestCase
{
    public string Slug { get; set; }

    /// Path of the test case relative to the test spec dir
    public FileInfo File { get; set; }

    /// Sequence of this test from 1..n in the same file
    public int Sequence { get; set; }

    /// Solvers for run the test on
    public List<string>? Solvers { get; set; }

    /// Any additional input files (eg: json data)
    public List<string>? InputFiles { get; set; }

    /// Solver flags to pass through
    public JsonNode? Options { get; set; }

    /// Enumeration of test type
    public TestType Type { get; set; }

    /// All solutions, present if Type == AllSolutions or AnySolution
    public List<string>? Solutions { get; set; }

    /// Any expected output files produced by the solver (eg flatzinc)
    public List<string>? OutputFiles { get; set; }

    /// If present, the error message should match this string
    public string? ErrorMessage { get; set; }

    /// If present, the error message should match this regex
    public string? ErrorRegex { get; set; }
}
