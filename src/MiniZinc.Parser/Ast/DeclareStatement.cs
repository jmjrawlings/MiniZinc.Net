namespace MiniZinc.Parser.Ast;

/// <summary>
/// A variable
/// </summary>
public sealed record DeclareStatement : SyntaxNode, INamed, ILetLocal
{
    public string Name { get; set; } = string.Empty;

    public required TypeInst Type { get; set; }

    public SyntaxNode? Body { get; set; }

    public List<Binding<TypeInst>>? Parameters { get; set; }

    public bool IsFunction { get; set; }
}
