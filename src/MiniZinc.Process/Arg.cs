namespace MiniZinc.Process;

using System.Text.RegularExpressions;

/// <summary>
/// A command line argument
/// </summary>
public readonly partial record struct Arg
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
    public readonly String String;

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
    public const string RegexPattern =
        """(-{1,2}[a-zA-Z][a-zA-Z0-9_-]*)?\s*(=)?\s*("[^"]*"|[^\s]+)?""";

    /// <summary>
    /// Parse the given string into many arguments
    /// </summary>
    public static IEnumerable<Arg> Parse(string s)
    {
        var regex = Regex();
        var matches = regex.Matches(s);
        foreach (Match m in matches)
        {
            if (m.Length <= 0)
                continue;
            var arg = FromMatch(m);
            yield return arg;
        }
    }

    /// <summary>
    /// Parse a single argument from the given string
    /// </summary>
    public static Arg ParseSingle(string s)
    {
        var regex = Regex();
        var match = regex.Match(s);
        if (!match.Success)
        {
            throw new Exception($"Could not parse {s} as an arg");
        }

        if (match.Length < s.Length)
        {
            throw new Exception($"Could not parse {s} as an arg");
        }

        var arg = FromMatch(match);
        return arg;
    }

    private static Arg FromMatch(Match m)
    {
        Group g;
        g = m.Groups[1];
        var flag = g.Success ? g.Value : null;

        g = m.Groups[2];
        var eq = g.Success;

        g = m.Groups[3];
        var value = g.Success ? g.Value : null;
        var arg = new Arg(flag, value, eq);
        return arg;
    }

    /// <summary>
    /// Parse command line arguments from the given strings
    /// </summary>
    public static IEnumerable<Arg> Parse(params string[] inputs)
    {
        foreach (var input in inputs)
        {
            foreach (var arg in Parse(input))
            {
                yield return arg;
            }
        }
    }

    [GeneratedRegex(RegexPattern)]
    internal static partial Regex Regex();

    public override string ToString()
    {
        return String;
    }
}
