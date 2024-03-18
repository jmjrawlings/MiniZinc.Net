namespace MiniZinc.Parser.Ast;

/// <summary>
/// A variable
/// </summary>
public sealed record Variable : INamed, IExpr, IAnnotations
{
    public string Name { get; set; }

    public TypeInst Type { get; set; }

    public IExpr Value { get; set; }

    public List<IExpr>? Annotations { get; set; }
}
