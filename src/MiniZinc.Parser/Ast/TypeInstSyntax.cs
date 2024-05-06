namespace MiniZinc.Parser.Ast;

public record TypeInstSyntax(Token Start) : SyntaxNode(Start)
{
    public Token Name { get; set; }

    public TypeKind Kind { get; set; } = TypeKind.Any;

    public bool Var { get; set; }
    public bool Opt { get; set; }
}

public sealed record ComplexTypeInstSyntax(Token Start) : TypeInstSyntax(Start)
{
    public List<TypeInstSyntax> Types { get; set; } = new();
}

public sealed record RecordTypeInstSyntax(Token Start) : TypeInstSyntax(Start)
{
    public List<(Token, TypeInstSyntax)> Fields { get; set; } = new();
}

public sealed record TupleTypeInstSyntax(Token Start) : TypeInstSyntax(Start)
{
    public List<TypeInstSyntax> Items { get; set; } = new();
}

public sealed record ExprTypeInst(Token Start, SyntaxNode Expr) : TypeInstSyntax(Start) { }

public sealed record ArrayTypeInstSyntax(Token Start) : TypeInstSyntax(Start)
{
    public required TypeInstSyntax Type { get; set; }

    public required List<SyntaxNode> Dimensions { get; set; }

    public int N => Dimensions.Count;
}

public sealed record SetTypeInstSyntax(Token Start) : TypeInstSyntax(Start)
{
    public required TypeInstSyntax Type { get; init; }
}

public sealed record TypeAliasSyntax(Token Start, TypeInstSyntax type) : SyntaxNode(Start) { }
