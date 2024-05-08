namespace MiniZinc.Parser.Ast;

/// <summary>
/// A MiniZinc model
/// </summary>
public sealed class SyntaxTree
{
    public readonly List<EnumDeclarationSyntax> Enums = new();
    public readonly List<TypeAliasSyntax> Aliases = new();
    public readonly List<SyntaxNode> Nodes = new();
    public readonly List<IncludeSyntax> Includes = new();
    public readonly List<ConstraintSyntax> Constraints = new();
    public readonly List<OutputSyntax> Outputs = new();
    public readonly List<SolveSyntax> SolveItems = new();
    public readonly List<AnnotationDeclarationSyntax> Annotations = new();
}
