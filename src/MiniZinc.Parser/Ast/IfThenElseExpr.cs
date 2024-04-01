namespace MiniZinc.Parser.Ast;

public sealed record IfThenElseExpr : IExpr
{
    public IExpr If { get; set; }
    public IExpr Then { get; set; }
    public List<(IExpr @elseif, IExpr @then)>? ElseIfs { get; set; } = new();

    public IExpr? Else { get; set; }
}
