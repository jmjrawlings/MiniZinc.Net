namespace MiniZinc.Parser.Ast;

public readonly record struct StringLit(string s) : IExpr
{
    public static implicit operator string(StringLit expr) => expr.s;
}

public readonly record struct Identifier(string s) : IExpr
{
    public static implicit operator string(Identifier expr) => expr.s;

    public static Identifier Anonymous = new Identifier("_");

    public override string ToString() => s;
}
