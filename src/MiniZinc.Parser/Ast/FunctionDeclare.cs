namespace MiniZinc.Parser.Ast;

public sealed class FunctionDeclare : INamed, IAnnotations, IExpr
{
    public string Name { get; set; }
    public TypeInst ReturnType { get; set; }
    public List<IExpr>? Annotations { get; set; }
    public List<Binding<TypeInst>>? Parameters { get; set; }

    public string? Ann { get; set; }

    public IExpr? Body { get; set; }
}
