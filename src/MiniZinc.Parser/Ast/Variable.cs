namespace MiniZinc.Parser.Ast;

/// <summary>
/// A variable
/// </summary>
public sealed record Variable : INamed, IExpr, IAnnotations
{
    public string Name { get; set; }

    public TypeInst Type { get; set; }

    public IExpr Body { get; set; }

    public List<Binding<TypeInst>>? Parameters { get; set; }

    public bool IsFunction { get; set; }

    public List<IExpr>? Annotations { get; set; }
}
