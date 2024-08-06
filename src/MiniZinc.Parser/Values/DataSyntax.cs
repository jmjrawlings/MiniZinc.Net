namespace MiniZinc.Parser;

public abstract class DataSyntax
{
    public string Write(WriteOptions? options = null)
    {
        var writer = new Writer(options);
        writer.WriteValue(this);
        var mzn = writer.ToString();
        return mzn;
    }
}

public sealed class IntData(int value) : DataSyntax
{
    public int Value => value;

    public static implicit operator int(IntData expr) => expr.Value;

    public override string ToString() => Value.ToString();
}

public sealed class BoolData(bool value) : DataSyntax
{
    public bool Value => value;

    public static implicit operator bool(BoolData expr) => expr.Value;

    public override string ToString() => Value.ToString();
}

public sealed class FloatData(decimal value) : DataSyntax
{
    public decimal Value => value;

    public static implicit operator decimal(FloatData expr) => expr.Value;

    public override string ToString() => Value.ToString();
}

public sealed class StringData(string value) : DataSyntax
{
    public string Value => value;

    public static implicit operator string(StringData expr) => expr.Value;

    public override string ToString() => Value;
}

public sealed class EmptyData : DataSyntax { }
