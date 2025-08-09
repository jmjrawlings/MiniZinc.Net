namespace MiniZinc.Command;

using System.Collections;
using System.Text.RegularExpressions;

/// <summary>
/// A command line argument
/// </summary>
public readonly partial struct Arg
{
    /// <summary>
    /// eg: --output, -a
    /// </summary>
    public readonly string? Flag;

    /// <summary>
    /// The non-flag part of the arg
    /// </summary>
    public readonly string? Value;

    /// <summary>
    /// True if equals assignment was used
    /// </summary>
    public readonly ArgType ArgType;

    /// <summary>
    /// The full string
    /// </summary>
    public readonly string String;

    /// <summary>
    /// Create an Arg from a flag, value, or both
    /// </summary>
    /// <param name="flag"></param>
    /// <param name="value"></param>
    /// <param name="eq"></param>
    /// <exception cref="ArgumentException"></exception>
    public Arg(string? flag, string? value, bool eq = false)
    {
        switch (flag, value)
        {
            case (null, null):
                throw new ArgumentException("One of 'flag' or 'value' must be provided");

            case (not null, null):
                String = flag;
                ArgType = ArgType.FlagOnly;
                break;
            case (null, not null):
                String = value;
                ArgType = ArgType.ValueOnly;
                break;
            default:
                ArgType = eq ? ArgType.FlagOptionEqual : ArgType.FlagOptionSpace;
                String = eq ? $"{flag}={value}" : $"{flag} {value}";
                break;
        }
        Flag = flag;
        Value = value;
    }

    /// <summary>
    /// Regex pattern used to match command line arguments
    /// </summary>
    const string RegexPattern = """(-{1,2}[a-zA-Z][a-zA-Z0-9_-]*)?\s*(=)?\s*("[^"]*"|[^\s]+)?""";

    [GeneratedRegex(RegexPattern)]
    internal static partial Regex Regex();

    /// <summary>
    /// Parse args from the given string
    /// </summary>
    public static IEnumerable<Arg> Parse(string s)
    {
        var regex = Regex();
        var matches = regex.Matches(s.Trim());
        foreach (Match m in matches)
        {
            if (m.Length <= 0)
                continue;

            Group g;
            g = m.Groups[1];
            var flag = g.Success ? g.Value : null;

            g = m.Groups[2];
            var eq = g.Success;

            g = m.Groups[3];
            var value = g.Success ? g.Value : null;
            var arg = new Arg(flag, value, eq);
            yield return arg;
        }
    }

    public static implicit operator string(Arg a) => a.String;

    ///
    public override string ToString()
    {
        return String;
    }
}
