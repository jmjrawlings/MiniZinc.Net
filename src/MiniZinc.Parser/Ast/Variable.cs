namespace MiniZinc.Parser.Ast;

/// <summary>
/// A variable
/// </summary>
public sealed record Variable : INamed, INode, IAnnotations, ILetLocal
{
    public string Name { get; set; }

    public TypeInst Type { get; set; }

    public INode Body { get; set; }

    public List<Binding<TypeInst>>? Parameters { get; set; }

    public bool IsFunction { get; set; }

    public List<INode>? Annotations { get; set; }
}
