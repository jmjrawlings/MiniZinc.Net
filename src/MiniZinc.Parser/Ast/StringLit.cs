namespace MiniZinc.Parser.Ast;

public sealed record StringLit(string s) : Expr
{
    public static implicit operator string(StringLit expr) => expr.s;
}

public sealed record Identifier(string s) : Expr
{
    public static implicit operator string(Identifier expr) => expr.s;

    public static Identifier Anonymous = new Identifier("_");

    public override string ToString() => s;
}
