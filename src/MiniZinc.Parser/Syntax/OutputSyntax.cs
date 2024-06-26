namespace MiniZinc.Parser.Syntax;

public sealed class OutputSyntax : StatementSyntax
{
    public readonly ExpressionSyntax Expr;

    public OutputSyntax(in Token start, ExpressionSyntax expr)
        : base(start)
    {
        Expr = expr;
    }
}
