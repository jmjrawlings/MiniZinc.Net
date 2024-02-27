namespace MiniZinc.Parser.Ast;

public sealed record ArrayAccessExpr : Expr
{
    public Expr Array { get; }
    public List<Expr> Access { get; }

    public ArrayAccessExpr(Expr array, List<Expr> access)
    {
        Array = array;
        Access = access;
    }

    public override string ToString() => $"{Array.Write()}[{Access.Write()}]";
}
