namespace MiniZinc.Parser;

public sealed class IntDatum(int value) : MiniZincDatum
{
    public int Value => value;

    public static implicit operator int(IntDatum expr) => expr.Value;

    public override bool Equals(object? obj)
    {
        if (obj is not int other)
            return false;
        if (!value.Equals(other))
            return false;
        return true;
    }

    public override string ToString() => Value.ToString();
}