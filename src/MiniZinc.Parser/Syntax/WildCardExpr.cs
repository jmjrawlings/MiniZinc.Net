namespace MiniZinc.Parser.Syntax;

public sealed record WildCardExpr(in Token Start) : SyntaxNode(Start)
{
    public override string ToString() => "_";
}
