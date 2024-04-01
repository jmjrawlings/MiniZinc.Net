using System.Runtime.InteropServices.JavaScript;

namespace MiniZinc.Parser.Ast;

public readonly record struct ArrayAccessExpr : IExpr
{
    public IExpr Array { get; }
    public List<IExpr> Access { get; }

    public ArrayAccessExpr(IExpr array, List<IExpr> access)
    {
        Array = array;
        Access = access;
    }

    public override string ToString() => $"{Array.Write()}[{Access.Write()}]";
}
