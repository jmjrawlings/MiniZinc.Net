namespace MiniZinc.Command;

using System.Collections;
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

    internal static Arg FromMatch(Match m)
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

    [GeneratedRegex(RegexPattern)]
    internal static partial Regex Regex();

    ///
    public override string ToString()
    {
        return String;
    }

    /// <summary>
    /// Parse args from the given string
    /// </summary>
    public static IEnumerable<Arg> Parse(string s)
    {
        var regex = Arg.Regex();
        var matches = regex.Matches(s);
        foreach (Match m in matches)
        {
            if (m.Length <= 0)
                continue;
            var arg = Arg.FromMatch(m);
            yield return arg;
        }
    }

    /// <summary>
    /// Parse args from the given parameters
    /// </summary>
    public static IEnumerable<Arg> Parse(params object[] inputs)
    {
        foreach (var input in inputs)
        {
            switch (input)
            {
                case Arg a:
                    yield return a;
                    break;
                case string s:
                    foreach (var arg in Parse(s))
                        yield return arg;
                    break;
                case IEnumerable e:
                    foreach (var item in e)
                    foreach (var arg in Parse(item))
                        yield return arg;
                    break;
                default:
                    var x = input.ToString();
                    foreach (var arg in Parse(x))
                        yield return arg;
                    break;
            }
        }
    }
}
