namespace MiniZinc.Parser.Ast;

public readonly struct RecordAccess(INode expr, string field) : INode
{
    public INode Expr => expr;
    public string Field => field;
}
