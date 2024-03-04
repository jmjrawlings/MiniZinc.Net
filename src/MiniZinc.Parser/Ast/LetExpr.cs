namespace MiniZinc.Parser.Ast;

public sealed record LetExpr : IExpr
{
    public List<TypeInst> Declares { get; set; } = new();

    public List<ConstraintItem> Constraints { get; set; } = new();

    public IExpr Body { get; set; }
}
