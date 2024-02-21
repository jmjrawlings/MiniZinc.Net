namespace MiniZinc.Net.Process;

using System.Text.RegularExpressions;

public static partial class Args
{
    const string RegexPattern = """(-{1,2}[a-zA-Z]\w*)?\s*(=)?\s*("[^"]*"|[^\s]+)?""";

    /// <summary>
    /// Parse command line arguments from the given strings
    /// </summary>
    public static IEnumerable<Arg> Parse(params string[] inputs)
    {
        var regex = Regex();
        foreach (var input in inputs)
        {
            var matches = regex.Matches(input);
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
                var arg = new Arg(flag, eq, value);
                yield return arg;
            }
        }
    }

    [GeneratedRegex(RegexPattern)]
    internal static partial Regex Regex();
}
