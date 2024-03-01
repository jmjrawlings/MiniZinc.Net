namespace MiniZinc.Parser.Ast;

using Annotation = IExpr;

public record Type : IAnnotations
{
    public TypeKind Kind { get; set; }
    public TypeInst Inst { get; set; }
    public List<Annotation>? Annotations { get; set; }

    public bool IsKind(TypeInst kind) => (Inst & kind) > 0;

    public bool IsCollection => IsKind(TypeInst.Set | Inst);

    public bool IsSingleton => !IsCollection;
}
