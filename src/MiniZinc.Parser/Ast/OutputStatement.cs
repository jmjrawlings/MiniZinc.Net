namespace MiniZinc.Parser.Ast;

public sealed record OutputStatement : SyntaxNode
{
    public Expr Expr { get; set; } = Expr.Null;
}
