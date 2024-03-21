namespace MiniZinc.Parser.Ast;

public record TypeInst : IAnnotations, INamed
{
    public string Name { get; set; } = string.Empty;

    public TypeKind Kind { get; set; } = TypeKind.Any;

    public TypeFlags Flags { get; set; } = TypeFlags.None;

    public List<INode>? Annotations { get; set; }

    public bool IsKind(TypeFlags kind) => (Flags & kind) > 0;
}

public sealed record RecordTypeInst : TypeInst
{
    public List<Binding<TypeInst>> Fields { get; set; } = new();
}

public sealed record TupleTypeInst : TypeInst
{
    public List<TypeInst> Items { get; set; } = new();
}

public sealed record ExprTypeInst : TypeInst
{
    public INode Expr { get; set; }
}

public sealed record ArrayTypeInst : TypeInst
{
    public TypeInst Type { get; set; }

    public List<INode> Dimensions { get; set; }

    public int N => Dimensions.Count;
}
