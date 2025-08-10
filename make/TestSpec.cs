namespace MiniZinc.Tests;

using System.Text.Json;
using System.Text.Json.Serialization;
using Core;

public sealed record TestSpec
{
    public List<TestCase> TestCases { get; set; } = [];
}
