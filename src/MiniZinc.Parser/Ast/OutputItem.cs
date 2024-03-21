namespace MiniZinc.Parser.Ast;

public sealed record OutputItem : IAnnotations
{
    public INode Expr { get; set; }
    public List<INode>? Annotations { get; set; }
}
