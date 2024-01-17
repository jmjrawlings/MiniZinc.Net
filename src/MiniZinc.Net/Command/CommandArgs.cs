namespace MiniZinc.Net;

using System.Text.RegularExpressions;

internal static partial class CommandArgs
{
    private const string RegexPattern = """(-{1,2}[a-zA-Z]\w*)?\s*(=)?\s*("[^"]*"|[^\s]+)?""";

    /// <summary>
    /// Parse command line arguments from the given strings
    /// </summary>
    public static IEnumerable<CommandArg> Parse(params string[] args)
    {
        var regex = Regex();
        foreach (var arg in args)
        {
            var matches = regex.Matches(arg);
            foreach (Match m in matches)
            {
                if (m.Length <= 0)
                    continue;

                var a = CommandArg.Parse(m);
                yield return a;
            }
        }
    }

    [GeneratedRegex(RegexPattern)]
    internal static partial Regex Regex();
}
