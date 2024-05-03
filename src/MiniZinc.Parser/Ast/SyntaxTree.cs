namespace MiniZinc.Parser.Ast;

/// <summary>
/// A MiniZinc model
/// </summary>
public sealed class SyntaxTree
{
    public readonly NameSpace<SyntaxNode> NameSpace = new();
    public readonly List<SyntaxNode> Nodes = new();
    public readonly List<IncludeStatement> Includes = new();
    public readonly List<ConstraintStatement> Constraints = new();
    public readonly List<OutputStatement> Outputs = new();
    public readonly List<SolveStatement> SolveItems = new();
}
