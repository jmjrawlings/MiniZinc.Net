namespace MiniZinc.Parser.Ast;

public sealed record ConstraintStatement : Node, ILetLocal
{
    public Expr Expr { get; set; }
}
