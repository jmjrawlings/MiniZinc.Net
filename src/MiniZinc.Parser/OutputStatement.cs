namespace MiniZinc.Parser.Syntax;

public sealed class OutputStatement : Statement
{
    public readonly Expr Expr;

    public OutputStatement(in Token start, Expr expr)
        : base(start)
    {
        Expr = expr;
    }
}
