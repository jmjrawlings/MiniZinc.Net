namespace MiniZinc.Parser.Ast;

public sealed record WildCardExpr(Token Start) : SyntaxNode(Start)
{
    public override string ToString() => "_";
}
