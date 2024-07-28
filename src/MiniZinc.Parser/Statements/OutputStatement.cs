namespace MiniZinc.Parser.Syntax;

public sealed class OutputStatement : StatementSyntax
{
    public readonly ExpressionSyntax Expr;

    public OutputStatement(in Token start, ExpressionSyntax expr)
        : base(start)
    {
        Expr = expr;
    }
}
