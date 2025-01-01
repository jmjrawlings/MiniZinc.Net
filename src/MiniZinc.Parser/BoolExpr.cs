namespace MiniZinc.Parser;

using Syntax;

public sealed class BoolExpr : Expr
{
    public bool Value { get; }

    public BoolExpr(in Token start, bool value)
        : base(start)
    {
        Value = value;
    }

    public static implicit operator bool(in BoolExpr literalSyntax) => literalSyntax.Value;

    public override string ToString() => Value.ToString();
}
