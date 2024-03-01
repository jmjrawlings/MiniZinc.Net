namespace MiniZinc.Parser.Ast;

public sealed class FunctionDef : INamed, IAnnotations
{
    public string Name { get; set; }

    public Type ReturnTypeExpr { get; set; }

    public List<IExpr>? Annotations { get; set; }

    public List<DeclareExpr> Parameters { get; set; }

    public string Ann { get; set; }

    public IExpr? Body { get; set; }
}