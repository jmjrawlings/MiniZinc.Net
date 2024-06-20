namespace MiniZinc.Parser.Syntax;

public sealed record TupleAccessSyntax(SyntaxNode Expr, Token Field) : SyntaxNode(Expr.Start)
{
    public int Index => Field.IntValue;
}
