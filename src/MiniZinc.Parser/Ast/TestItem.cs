namespace MiniZinc.Parser.Ast;

public sealed record TestItem : INamed, IAnnotations
{
    public string Name { get; set; }
    public List<Variable> Params { get; set; }
    public List<IExpr>? Annotations { get; set; }
    public IExpr? Body { get; set; }
}
