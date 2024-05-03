namespace MiniZinc.Parser.Ast;

public sealed record UnaryOpExpr : Expr
{
    public UnaryOpExpr(Operator @operator, Expr operand)
    {
        Operator = @operator;
        Operand = operand;
    }

    public Operator Operator { get; }
    public Expr Operand { get; }
}
