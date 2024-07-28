namespace MiniZinc.Parser.Syntax;

public sealed class RecordAccessSyntax : ExpressionSyntax
{
    public readonly ExpressionSyntax Expr;
    public readonly Token Field;

    public RecordAccessSyntax(ExpressionSyntax expr, Token field)
        : base(expr.Start)
    {
        Expr = expr;
        Field = field;
    }
}
