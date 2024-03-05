namespace MiniZinc.Parser.Ast;

public readonly record struct IntLit(int Value) : IExpr
{
    public static implicit operator int(IntLit expr) => expr.Value;
}
