namespace MiniZinc.Parser.Ast;

public sealed record AnnotationDef : INamed
{
    public string Name { get; set; }

    public List<DeclareExpr>? Params { get; set; }

    public IExpr? Body { get; set; }
}
