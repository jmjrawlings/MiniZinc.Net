namespace MiniZinc.Command;

using System.Text;

/// <summary>
/// An ordered, faithful list of command line arguments.
///
/// Arguments are constructed explicitly via the typed Add methods — there is no
/// string parsing, so <see cref="Values"/> and <see cref="Tokens"/> are an exact
/// record of what will be passed to the process. Use <see cref="AddCommandLine"/>
/// for the one case where a raw, user-supplied argument string must be tokenised.
/// </summary>
public sealed class Args
{
    private List<Arg>? _args;

    public static Args Empty => new Args();

    public int Count => _args?.Count ?? 0;

    /// <summary>
    /// The arguments as specified.
    /// </summary>
    public IEnumerable<Arg> Values => _args ?? Enumerable.Empty<Arg>();

    /// <summary>
    /// The flattened process-level tokens (argv), in order.
    /// </summary>
    public IEnumerable<string> Tokens
    {
        get
        {
            if (_args is null)
                yield break;

            foreach (Arg arg in _args)
                foreach (string token in arg.Tokens)
                    yield return token;
        }
    }

    /// <summary>
    /// Add a single argument.
    /// </summary>
    public void Add(Arg arg)
    {
        _args ??= new List<Arg>();
        _args.Add(arg);
    }

    /// <summary>
    /// Append all arguments from another set.
    /// </summary>
    public void Add(Args args)
    {
        foreach (Arg arg in args.Values)
            Add(arg);
    }

    /// <summary>
    /// Add one or more literal tokens. Each string becomes exactly one argument
    /// (no splitting); a leading '-' marks it as a flag, otherwise a value.
    /// Nulls are skipped.
    /// </summary>
    public void Add(params string?[] tokens)
    {
        foreach (string? token in tokens)
        {
            if (token is null)
                continue;

            if (token.StartsWith('-'))
                Add(new Arg(token, null));
            else
                Add(new Arg(null, token));
        }
    }

    /// <summary>
    /// Add a bare flag, eg "--all-solutions".
    /// </summary>
    public void AddFlag(string flag) => Add(new Arg(flag, null));

    /// <summary>
    /// Add a positional value, eg a model file path.
    /// </summary>
    public void AddValue(string value) => Add(new Arg(null, value));

    /// <summary>
    /// Add a flag paired with a value, eg ("--solver", "gecode"). The pairing is
    /// preserved, so it can be looked up later with <see cref="TryGetOption"/>.
    /// </summary>
    public void AddOption(string flag, string value, bool eq = false) =>
        Add(new Arg(flag, value, eq));

    /// <summary>
    /// Tokenise a raw command line string (respecting double quotes) and append
    /// each token. For passing through user-supplied argument strings.
    /// </summary>
    public void AddCommandLine(string commandLine)
    {
        foreach (string token in Tokenize(commandLine))
            Add(token);
    }

    /// <summary>
    /// True if the given flag token is present (either bare or as "flag=value").
    /// </summary>
    public bool HasFlag(string flag)
    {
        foreach (string token in Tokens)
        {
            if (token == flag || token.StartsWith($"{flag}="))
                return true;
        }
        return false;
    }

    /// <summary>
    /// Find the value supplied for an option flag, whether written as
    /// "flag value" or "flag=value".
    /// </summary>
    public bool TryGetOption(string flag, out string? value)
    {
        value = null;
        List<string> tokens = Tokens.ToList();
        for (int i = 0; i < tokens.Count; i++)
        {
            string token = tokens[i];
            if (token == flag)
            {
                value = i + 1 < tokens.Count ? tokens[i + 1] : null;
                return true;
            }
            if (token.StartsWith($"{flag}="))
            {
                value = token.Substring(flag.Length + 1);
                return true;
            }
        }
        return false;
    }

    private static IEnumerable<string> Tokenize(string s)
    {
        StringBuilder current = new StringBuilder();
        bool inQuotes = false;
        bool started = false;

        foreach (char c in s)
        {
            if (c is '"')
            {
                inQuotes = !inQuotes;
                started = true;
            }
            else if (char.IsWhiteSpace(c) && !inQuotes)
            {
                if (started)
                {
                    yield return current.ToString();
                    current.Clear();
                    started = false;
                }
            }
            else
            {
                current.Append(c);
                started = true;
            }
        }

        if (started)
            yield return current.ToString();
    }

    public override string ToString() => string.Join(" ", Tokens);
}
