namespace MiniZinc.Parser.Ast;

public sealed record RightOpenRangeExpr : IExpr
{
    public IExpr Max { get; set; }
}