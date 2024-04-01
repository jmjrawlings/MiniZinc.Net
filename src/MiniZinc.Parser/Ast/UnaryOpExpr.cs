namespace MiniZinc.Parser.Ast;

public readonly record struct UnaryOpExpr : IExpr
{
    public UnaryOpExpr(Operator @operator, IExpr operand)
    {
        Operator = @operator;
        Operand = operand;
    }

    public Operator Operator { get; }
    public IExpr Operand { get; }
}
