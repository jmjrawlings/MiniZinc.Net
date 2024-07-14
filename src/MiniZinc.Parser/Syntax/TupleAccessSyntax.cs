namespace MiniZinc.Parser.Syntax;

public sealed class TupleAccessSyntax : ExpressionSyntax
{
    public readonly ExpressionSyntax Expr;

    public readonly Token Field;

    public int Index => Field.IntValue;

    public TupleAccessSyntax(ExpressionSyntax expr, Token field)
        : base(expr.Start)
    {
        Expr = expr;
        Field = field;
    }
}
