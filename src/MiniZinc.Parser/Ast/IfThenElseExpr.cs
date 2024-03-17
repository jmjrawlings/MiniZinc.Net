namespace MiniZinc.Parser.Ast;

public sealed record IfThenElseExpr : IExpr
{
    public IExpr If;

    public IExpr Then;

    public List<(IExpr @elseif, IExpr @then)>? ElseIfs { get; set; } = new();

    public IExpr? Else { get; set; }
}
