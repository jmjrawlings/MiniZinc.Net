namespace MiniZinc.Parser.Ast;

public sealed class Model
{
    public readonly List<IncludeStatement> Includes;
    public readonly List<FunctionDef> Functions;
    public readonly List<AnnotationDef> Annotations;
    public SolveItem? SolveItem { get; set; }
    public readonly NameSpace<IExpr> NameSpace;
}
