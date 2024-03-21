namespace MiniZinc.Parser.Ast;

public readonly struct BinaryOpExpr : INode
{
    public BinaryOpExpr(INode left, Operator op, string? id, INode right)
    {
        Left = left;
        Op = op;
        Name = id;
        Right = right;
    }

    public INode Left { get; }
    public Operator? Op { get; }
    public string? Name { get; }
    public INode Right { get; }
}
