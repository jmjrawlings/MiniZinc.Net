namespace Make;

using LibMiniZinc.Tests;

/// <summary>
/// Extends the <see cref="TestCase"/> with post processed
/// information used to generate tests
/// </summary>
public sealed record TestCaseInfo : TestCase
{
    public string Dir { get; init; }

    public string Name { get; init; }

    public string Signature { get; init; }

    public List<string> ExtraArgs { get; } = new();

    public new List<(string solver, bool enabled)> Solvers { get; } = new();
}
