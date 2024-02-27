namespace MiniZinc.Parser.Ast;

public sealed record BinaryOpExpr : Expr
{
    public BinaryOpExpr(Expr left, Operator op, Expr right)
    {
        Left = left;
        Op = op;
        Right = right;
    }

    public BinaryOpExpr(Expr left, string id, Expr right)
    {
        Left = left;
        Name = id;
        Right = right;
    }

    public Expr Left { get; }
    public Operator? Op { get; }
    public string? Name { get; }
    public Expr Right { get; }
}
