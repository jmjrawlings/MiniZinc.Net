namespace MiniZinc.Parser.Ast;

public sealed record UnaryExpr : Expr
{
    public UnaryExpr(Operator @operator, Expr operand)
    {
        Operator = @operator;
        Operand = operand;
    }

    public Operator Operator { get; }
    public Expr Operand { get; }
}
