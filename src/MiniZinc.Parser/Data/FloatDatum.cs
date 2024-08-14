namespace MiniZinc.Parser;

public sealed class FloatDatum(decimal value) : MiniZincDatum
{
    public decimal Value => value;

    public static implicit operator decimal(FloatDatum expr) => expr.Value;

    public override bool Equals(object? obj)
    {
        if (obj is not decimal other)
            return false;
        if (!value.Equals(other))
            return false;
        return true;
    }

    public override string ToString() => Value.ToString();
}