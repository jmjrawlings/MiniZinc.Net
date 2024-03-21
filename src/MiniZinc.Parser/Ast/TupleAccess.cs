namespace MiniZinc.Parser.Ast;

public readonly struct TupleAccess(INode expr, int field) : INode
{
    public INode Expr => expr;
    public int Field => field;
}
