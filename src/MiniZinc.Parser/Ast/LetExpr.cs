namespace MiniZinc.Parser.Ast;

public sealed record LetExpr : IExpr
{
    public NameSpace<IExpr>? NameSpace { get; set; }

    public List<ConstraintItem>? Constraints { get; set; }

    public IExpr Body { get; set; }
}
