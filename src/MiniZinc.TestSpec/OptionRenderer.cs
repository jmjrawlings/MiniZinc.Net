namespace MiniZinc.TestSpec;

using System.Globalization;
using System.Text;

/// <summary>
/// Deterministic <c>options</c> dict → CLI flags rendering.
///
/// Rules:
///  - alphabetical key order (Ordinal)
///  - <c>InvariantCulture</c> formatting
///  - <c>all_solutions: true</c> is consumed (handled elsewhere as a kind promotion).
///  - Boolean true with leading <c>-</c>: emitted bare (<c>-O3</c>).
///  - Boolean true without leading <c>-</c>: emitted with <c>--</c> prefix.
///  - Other values: <c>--key value</c> or <c>-key value</c>.
/// </summary>
public static class OptionRenderer
{
    public static string? Render(IReadOnlyDictionary<string, string?> options)
    {
        if (options.Count == 0)
            return null;

        var sb = new StringBuilder();
        foreach (var key in options.Keys.OrderBy(k => k, StringComparer.Ordinal))
        {
            if (key == "all_solutions")
                continue;
            var raw = options[key];
            if (sb.Length > 0)
                sb.Append(' ');
            bool dashKey = key.StartsWith('-');
            if (string.Equals(raw, "true", StringComparison.OrdinalIgnoreCase))
            {
                sb.Append(dashKey ? key : $"--{key}");
                continue;
            }
            if (string.Equals(raw, "false", StringComparison.OrdinalIgnoreCase))
            {
                // skip negated bool flags
                continue;
            }
            string val = raw ?? "";
            // Time-limit special: take int value to milliseconds.
            if (key == "time_limit")
            {
                if (int.TryParse(val, NumberStyles.Integer, CultureInfo.InvariantCulture, out int ms))
                {
                    sb.Append("--time-limit ").Append(ms.ToString(CultureInfo.InvariantCulture));
                    continue;
                }
            }
            sb.Append(dashKey ? key : $"--{key}").Append(' ').Append(val);
        }

        if (sb.Length == 0)
            return null;
        return sb.ToString();
    }
}
