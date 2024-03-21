namespace MiniZinc.Parser.Ast;

public readonly record struct UnaryOpExpr : INode
{
    public UnaryOpExpr(Operator @operator, INode operand)
    {
        Operator = @operator;
        Operand = operand;
    }

    public Operator Operator { get; }
    public INode Operand { get; }
}
