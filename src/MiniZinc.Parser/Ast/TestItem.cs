namespace MiniZinc.Parser.Ast;

public sealed record TestItem : IItem, IAnnotations
{
    public string Name { get; set; }
    public List<DeclareExpr> Params { get; set; }
    public List<IExpr>? Annotations { get; set; }
    public IExpr? Body { get; set; }
}
