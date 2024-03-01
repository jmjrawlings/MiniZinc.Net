namespace MiniZinc.Parser.Ast;

public readonly record struct StringExpr(string Value) : IExpr
{
    public static implicit operator string(StringExpr expr) => expr.Value;
}
