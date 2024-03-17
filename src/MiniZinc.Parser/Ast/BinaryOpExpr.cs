namespace MiniZinc.Parser.Ast;

public readonly struct BinaryOpExpr : IExpr
{
    public BinaryOpExpr(IExpr left, Operator op, string? id, IExpr right)
    {
        Left = left;
        Op = op;
        Name = id;
        Right = right;
    }

    public IExpr Left { get; }
    public Operator? Op { get; }
    public string? Name { get; }
    public IExpr Right { get; }
}
