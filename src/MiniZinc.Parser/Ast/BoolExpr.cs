namespace MiniZinc.Parser.Ast;

public readonly record struct BoolExpr(bool Value) : IExpr
{
    public static implicit operator bool(BoolExpr expr) => expr.Value;
}
