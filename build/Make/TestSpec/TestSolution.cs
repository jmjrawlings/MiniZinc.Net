namespace LibMiniZinc.Tests;

public sealed record TestSolution
{
    /// <summary>
    /// Dzn data string to be tested against eg: `a = 10;`
    /// </summary>
    public string? Dzn { get; set; }

    /// <summary>
    /// Expected output string eg: `ok`
    /// </summary>
    public string? Ozn { get; set; }
}
