namespace MiniZinc.Parser.Ast;

public sealed record ConstraintItem : Item, ILetLocal
{
    public IExpr Expr { get; set; }
}

public interface ILetLocal : IExpr { }
