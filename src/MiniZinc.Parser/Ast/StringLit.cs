namespace MiniZinc.Parser.Ast;

public readonly record struct StringLit(string Value) : IExpr
{
    public static implicit operator string(StringLit expr) => expr.Value;
}
