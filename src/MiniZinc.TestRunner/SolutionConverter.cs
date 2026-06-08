namespace MiniZinc.TestRunner;

using MiniZinc.Parser;
using MiniZinc.TestSpec;
using MiniZinc.TestSpec.Model;

/// <summary>
/// Turns an expected <see cref="Solution"/> (parsed from the libminizinc YAML
/// spec) into a <see cref="MiniZincData"/> — the same representation the client
/// produces for actual solver output — so the two can be compared homogeneously
/// by <see cref="SolutionComparer"/>.
///
/// Implemented as a DZN round-trip (<see cref="SolutionDzn.Render"/> →
/// <see cref="Parser.TryParseDataString"/>): the expected and actual sides then
/// come out of the very same parser, so the comparer never has to bridge two
/// different node-shape conventions. The <c>!Approx</c>/<c>!Unordered</c>
/// annotations are intentionally flattened by this path — the comparer applies
/// float-tolerance and unordered-set semantics universally instead.
/// </summary>
public static class SolutionConverter
{
    /// <summary>
    /// Convert an expected <see cref="Solution"/> to <see cref="MiniZincData"/>.
    /// Returns false (with a reason) if the solution has nothing renderable or the
    /// rendered DZN fails to parse.
    /// </summary>
    public static bool TryFromSolution(
        Solution solution,
        out MiniZincData data,
        out string? error
    )
    {
        string? dzn = SolutionDzn.Render(solution);
        if (dzn is null)
        {
            data = new MiniZincData();
            error = "solution has no renderable variables";
            return false;
        }

        if (!Parser.TryParseDataString(dzn, out MiniZincData? parsed, out string? err, out string? trace, out _))
        {
            data = new MiniZincData();
            error = $"rendered expected DZN failed to parse: {err ?? trace}\n{dzn}";
            return false;
        }

        data = parsed;
        error = null;
        return true;
    }

    /// <summary>
    /// Convert an expected <see cref="Solution"/> to <see cref="MiniZincData"/>,
    /// throwing if it cannot be rendered/parsed. Prefer
    /// <see cref="TryFromSolution"/> in the harness so a bad expected value
    /// surfaces as a real failure rather than an exception.
    /// </summary>
    public static MiniZincData FromSolution(Solution solution)
    {
        if (!TryFromSolution(solution, out MiniZincData data, out string? error))
            throw new FormatException(error);
        return data;
    }
}
