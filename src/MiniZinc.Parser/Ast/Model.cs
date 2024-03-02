namespace MiniZinc.Parser.Ast;

public sealed class Model
{
    public readonly List<IncludeStatement> Includes = new();
    public readonly List<FunctionDef> Functions = new();
    public readonly List<AnnotationDef> Annotations = new();
    public readonly NameSpace<IExpr> NameSpace = new();
    public SolveItem? SolveItem { get; set; }
}
