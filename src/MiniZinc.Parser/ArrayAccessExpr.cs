namespace MiniZinc.Parser.Syntax;

public sealed class ArrayAccessExpr : Expr
{
    public readonly Expr Array;
    public readonly List<Expr> Access;

    public ArrayAccessExpr(Expr array, List<Expr> access)
        : base(array.Start)
    {
        Array = array;
        Access = access;
    }
}
