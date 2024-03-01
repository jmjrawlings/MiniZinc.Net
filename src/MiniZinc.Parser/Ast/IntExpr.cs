namespace MiniZinc.Parser.Ast;

public readonly record struct IntExpr(int Value) : IExpr
{
    public static implicit operator int(IntExpr expr) => expr.Value;
}
