namespace MiniZinc.Parser.Ast;

public sealed record IfThenElseExpr : IExpr
{
    public IExpr If { get; set; }

    public IExpr Then { get; set; }

    public List<ValueTuple<IExpr, IExpr>>? ElseIfs { get; set; }

    public IExpr? Else { get; set; }
}
