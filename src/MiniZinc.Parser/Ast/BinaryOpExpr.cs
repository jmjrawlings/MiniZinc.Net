namespace MiniZinc.Parser.Ast;

public sealed record BinaryOpExpr : IExpr
{
    public IExpr Left { get; set; }
    public Operator Op { get; set; }
    public IExpr Right { get; set; }
}
