namespace MiniZinc.Parser.Ast;

public sealed record AnnotationDef : INamed
{
    public string Name { get; set; } = string.Empty;

    public List<VariableDeclarationSyntax>? Params { get; set; }

    public SyntaxNode? Body { get; set; }
}
