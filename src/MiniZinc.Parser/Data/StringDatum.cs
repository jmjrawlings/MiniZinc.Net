namespace MiniZinc.Parser;

public sealed class StringDatum(string value) : Datum
{
    public string Value => value;

    public override DatumKind Kind => DatumKind.String;

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
