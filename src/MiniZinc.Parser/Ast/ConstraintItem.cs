namespace MiniZinc.Parser.Ast;

public sealed record ConstraintItem : Expr, ILetLocal
{
    public INode Expr { get; set; }
}

public interface ILetLocal : INode { }
