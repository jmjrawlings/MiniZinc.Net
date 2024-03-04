namespace MiniZinc.Parser.Ast;

public sealed class Model
{
    public readonly List<string> Includes = new();
    public readonly List<FunctionDeclare> Functions = new();
    public readonly List<AnnotationDef> Annotations = new();
    public readonly NameSpace<IExpr> NameSpace = new();
    public readonly List<ConstraintItem> Constraints = new();
    public readonly List<OutputItem> Outputs = new();
    public readonly List<SolveItem> SolveItems = new();
}
