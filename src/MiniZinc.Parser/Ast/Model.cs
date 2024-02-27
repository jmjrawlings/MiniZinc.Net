namespace MiniZinc.Parser.Ast;

/// <summary>
/// A MiniZinc model
/// </summary>
public sealed class Model
{
    public readonly NameSpace<Node> NameSpace = new();
    public readonly List<IncludeStatement> Includes = new();
    public readonly List<ConstraintStatement> Constraints = new();
    public readonly List<OutputStatement> Outputs = new();
    public readonly List<SolveStatement> SolveItems = new();
}
