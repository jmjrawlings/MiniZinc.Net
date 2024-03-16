namespace MiniZinc.Parser.Ast;

using Annotation = IExpr;

public record TypeInst : IAnnotations, IExpr, INamed
{
    public string Name { get; set; }

    public TypeKind Kind { get; set; }

    public TypeFlags Flags { get; set; }

    public List<Annotation>? Annotations { get; set; }

    public bool IsKind(TypeFlags kind) => (Flags & kind) > 0;
}

public sealed record NamedType : TypeInst, INamed
{
    public string Name { get; set; }
}

public sealed record RecordTypeInst : TypeInst
{
    public List<Binding<TypeInst>> Fields { get; set; } = new();
}

public sealed record TupleTypeInst : TypeInst
{
    public List<TypeInst> Items { get; set; } = new();
}

public sealed record ExprType : TypeInst
{
    public IExpr Expr { get; set; }
}

public sealed record ArrayType : TypeInst
{
    public TypeInst Type { get; set; }

    public List<IExpr> Dimensions { get; set; }

    public int N => Dimensions.Count;
}
