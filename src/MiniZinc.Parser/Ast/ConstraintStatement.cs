namespace MiniZinc.Parser.Ast;

public sealed record ConstraintStatement : Node, ILetLocal
{
    public required Expr Expr { get; set; }
}
