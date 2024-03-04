namespace MiniZinc.Parser.Ast;

using Annotation = IExpr;

public record TypeInst : IAnnotations, IExpr
{
    public TypeKind Kind { get; set; }

    public TypeFlags Flags { get; set; }

    public List<Annotation>? Annotations { get; set; }

    public bool IsKind(TypeFlags kind) => (Flags & kind) > 0;
}
