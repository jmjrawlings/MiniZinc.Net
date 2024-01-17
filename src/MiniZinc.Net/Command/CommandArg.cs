namespace MiniZinc.Net;

using System.Text.RegularExpressions;

internal readonly record struct CommandArg
{
    public readonly string? Flag;
    public readonly string? Value;
    public readonly bool Eq;
    public readonly string String;

    public CommandArg(string? flag, bool? eq, string? value)
    {
        Flag = flag;
        Eq = eq ?? false;
        Value = value;
        if (value is null)
            String = flag!;
        else if (flag is null)
            String = value;
        else if (Eq)
            String = $"{Flag}={Value}";
        else
            String = $"{Flag} {Value}";
    }

    public override string ToString() => String;

    internal static CommandArg Parse(Match m)
    {
        Group g;
        g = m.Groups[1];
        var flag = g.Success ? g.Value : null;

        g = m.Groups[2];
        var eq = g.Success;

        g = m.Groups[3];
        var value = g.Success ? g.Value : null;
        var arg = new CommandArg(flag, eq, value);
        return arg;
    }

    /// <summary>
    /// Parse a command line argument
    /// </summary>
    public static CommandArg? Parse(string s)
    {
        var m = CommandArgs.Regex().Match(s);
        if (!m.Success)
            return null;
        var arg = Parse(m);
        return arg;
    }
}
