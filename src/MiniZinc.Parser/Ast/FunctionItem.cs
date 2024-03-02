namespace MiniZinc.Parser.Ast;

public sealed class FunctionItem : INamed, IAnnotations
{
    public string Name { get; set; }
    public Type ReturnType { get; set; }
    public List<IExpr>? Annotations { get; set; }
    public List<Type>? Parameters { get; set; }

    public string? Ann { get; set; }

    public IExpr? Body { get; set; }
}
