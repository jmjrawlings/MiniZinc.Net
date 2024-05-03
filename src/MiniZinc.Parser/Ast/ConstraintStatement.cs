namespace MiniZinc.Parser.Ast;

public sealed record ConstraintStatement : SyntaxNode, ILetLocal
{
    public required Expr Expr { get; set; }
}
