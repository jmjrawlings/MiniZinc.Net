namespace MiniZinc.Net.Process;

using System.Text.RegularExpressions;

/// <summary>
/// A command line argument
/// </summary>
public readonly record struct Arg
{
    public readonly string? Flag;
    public readonly string? Value;
    public readonly bool Eq;
    public readonly string String;

    public Arg(string? flag, bool? eq, string? value)
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
}
