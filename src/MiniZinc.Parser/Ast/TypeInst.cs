namespace MiniZinc.Parser.Ast;

public record TypeInst : SyntaxNode, INamed
{
    public string Name { get; set; } = string.Empty;

    public TypeKind Kind { get; set; } = TypeKind.Any;

    public bool Var { get; set; }
    public bool Opt { get; set; }
}

public sealed record ComplexTypeInst : TypeInst
{
    public List<TypeInst> Types { get; set; } = new();
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
    public Expr Expr { get; set; } = Expr.Null;
}

public sealed record ArrayTypeInst : TypeInst
{
    public required TypeInst Type { get; set; }

    public required List<SyntaxNode> Dimensions { get; set; }

    public int N => Dimensions.Count;
}

public sealed record SetTypeInst : TypeInst
{
    public required TypeInst Type { get; init; }
}
