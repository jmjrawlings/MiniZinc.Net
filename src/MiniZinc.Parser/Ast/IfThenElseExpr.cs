namespace MiniZinc.Parser.Ast;

public sealed record IfThenElseExpr : INode
{
    public INode If { get; set; }
    public INode Then { get; set; }
    public List<(INode @elseif, INode @then)>? ElseIfs { get; set; } = new();

    public INode? Else { get; set; }
}
