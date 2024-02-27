namespace MiniZinc.Parser.Ast;

public sealed record IndexAndValue(Expr index, Expr value) : Expr
{
    public Node Index => index;
    public Node Value => value;
}
