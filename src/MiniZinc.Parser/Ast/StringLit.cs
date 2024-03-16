namespace MiniZinc.Parser.Ast;

public readonly record struct StringLit(string s) : IExpr
{
    public static implicit operator string(StringLit expr) => expr.s;
}

public readonly record struct Identifer(string s) : IExpr
{
    public static implicit operator string(Identifer expr) => expr.s;
}
