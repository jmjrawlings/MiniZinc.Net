namespace MiniZinc.Parser.Ast;

public sealed record LeftOpenRangeExpr : IExpr
{
    public IExpr Min { get; set; }
}
