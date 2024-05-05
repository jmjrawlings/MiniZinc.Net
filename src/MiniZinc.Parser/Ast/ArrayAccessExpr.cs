namespace MiniZinc.Parser.Ast;

public sealed record ArrayAccessExpr(SyntaxNode Array, List<SyntaxNode> Access)
    : SyntaxNode(Array.Start)
{
    public override string ToString() => $"{Array.Write()}[{Access.Write()}]";
}
