namespace MiniZinc.Command;

/// <summary>
/// A single command line argument: a bare flag (<c>--all-solutions</c>), a
/// positional value (a model file path), or a flag paired with a value
/// (<c>--solver gecode</c> or <c>--solver=gecode</c>).
/// </summary>
public readonly struct Arg
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
    /// How the flag and value combine
    /// </summary>
    public readonly ArgType ArgType;

    /// <summary>
    /// Create an Arg from a flag, a value, or both. When both are given,
    /// <paramref name="eq"/> controls whether they render as "flag=value"
    /// (true) or as the two tokens "flag" "value" (false).
    /// </summary>
    public Arg(string? flag, string? value, bool eq = false)
    {
        switch (flag, value)
        {
            case (null, null):
                throw new ArgumentException("One of 'flag' or 'value' must be provided");
            case (not null, null):
                ArgType = ArgType.FlagOnly;
                break;
            case (null, not null):
                ArgType = ArgType.ValueOnly;
                break;
            default:
                ArgType = eq ? ArgType.FlagOptionEqual : ArgType.FlagOptionSpace;
                break;
        }
        Flag = flag;
        Value = value;
    }

    /// <summary>
    /// The process-level tokens (argv entries) this argument expands to.
    /// One token, except a space-separated option which yields two.
    /// </summary>
    public IEnumerable<string> Tokens
    {
        get
        {
            switch (ArgType)
            {
                case ArgType.FlagOnly:
                    yield return Flag!;
                    break;
                case ArgType.ValueOnly:
                    yield return Value!;
                    break;
                case ArgType.FlagOptionSpace:
                    yield return Flag!;
                    yield return Value!;
                    break;
                case ArgType.FlagOptionEqual:
                    yield return $"{Flag}={Value}";
                    break;
            }
        }
    }

    ///
    public override string ToString()
    {
        switch (ArgType)
        {
            case ArgType.FlagOnly:
                return Flag!;
            case ArgType.ValueOnly:
                return Value!;
            case ArgType.FlagOptionEqual:
                return $"{Flag}={Value}";
            default:
                return $"{Flag} {Value}";
        }
    }
}
