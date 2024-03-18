namespace MiniZinc.Parser.Ast;

/// <summary>
/// A MiniZinc model
/// </summary>
public sealed class Model
{
    public readonly List<string> Includes = new();
    public readonly NameSpace<IExpr> NameSpace = new();
    public readonly List<ConstraintItem> Constraints = new();
    public readonly List<OutputItem> Outputs = new();
    public readonly List<SolveItem> SolveItems = new();
}
