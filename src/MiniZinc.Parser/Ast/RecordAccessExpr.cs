namespace MiniZinc.Parser.Ast;

public readonly struct RecordAccessExpr(INode expr, string field) : INode
{
    public INode Expr => expr;
    public string Field => field;
}
