namespace MiniZinc.Parser.Ast;

public sealed record IndexAndValue(Expr index, Expr value) : Expr
{
    public SyntaxNode Index => index;
    public SyntaxNode Value => value;
}
