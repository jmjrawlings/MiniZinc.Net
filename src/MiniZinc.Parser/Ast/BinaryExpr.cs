namespace MiniZinc.Parser.Ast;

public sealed record BinaryExpr : Expr
{
    public BinaryExpr(Expr left, Operator op, Expr right)
    {
        Left = left;
        Op = op;
        Right = right;
    }

    public BinaryExpr(Expr left, string id, Expr right)
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
