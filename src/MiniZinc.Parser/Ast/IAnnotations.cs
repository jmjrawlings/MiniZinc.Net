namespace MiniZinc.Parser.Ast;

public interface IAnnotations : INode
{
    public List<INode>? Annotations { get; set; }
}
