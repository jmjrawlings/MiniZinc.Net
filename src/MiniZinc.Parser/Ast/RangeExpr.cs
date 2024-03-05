namespace MiniZinc.Parser.Ast;

public sealed class RangeExpr : IExpr
{
    public IExpr Lower { get; set; }
    public IExpr Upper { get; set; }
}
