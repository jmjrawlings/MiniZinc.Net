namespace MiniZinc.Parser.Ast;

public sealed record AnnotationDef : INamed
{
    public string Name { get; set; }

    public List<Variable>? Params { get; set; }

    public INode? Body { get; set; }
}
