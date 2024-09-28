namespace MiniZinc.Parser;

public sealed class BoolDatum(bool value) : Datum
{
    public bool Value => value;

    public override DatumKind Kind => DatumKind.Bool;

    public static implicit operator bool(BoolDatum expr) => expr.Value;

    public override bool Equals(object? obj)
    {
        if (obj is not bool other)
            return false;
        if (!value.Equals(other))
            return false;
        return true;
    }

    public override string ToString() => Value.ToString();
}
