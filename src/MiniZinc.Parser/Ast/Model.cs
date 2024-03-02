namespace MiniZinc.Parser.Ast;

public sealed class Model
{
    public readonly List<IncludeStatement> Includes = new();
    public readonly List<FunctionItem> Functions = new();
    public readonly List<AnnotationDef> Annotations = new();
    public readonly NameSpace<IExpr> NameSpace = new();
    public readonly List<AssignExpr> Assignments = new();
    public readonly List<ConstraintItem> Constraints = new();
    public readonly List<OutputItem> Outputs = new();
    public readonly List<SolveItem> SolveItems = new();
}
