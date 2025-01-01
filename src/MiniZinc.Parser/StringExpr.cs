namespace MiniZinc.Parser;

using Syntax;

public sealed class StringExpr : Expr
{
    public string Value { get; }

    public StringExpr(in Token start)
        : base(start)
    {
        if (start.StringValue is not { } str)
            throw new ArgumentException($"Null string value");

        Value = str;
    }

    public StringExpr(in Token start, string value)
        : base(start)
    {
        Value = value;
    }

    public static implicit operator string(StringExpr expr) => expr.Value;
}
