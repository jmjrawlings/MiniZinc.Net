namespace MiniZinc.Parser;

public sealed class StringDatum(string value) : MiniZincDatum
{
    public string Value => value;

    public static implicit operator string(StringDatum expr) => expr.Value;

    public override bool Equals(object? obj)
    {
        if (obj is not string other)
            return false;
        if (!value.Equals(other))
            return false;
        return true;
    }

    public override string ToString() => Value;
}