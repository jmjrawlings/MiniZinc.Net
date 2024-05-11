namespace MiniZinc.Parser.Ast;

public record TypeSyntax(Token Start) : SyntaxNode(Start)
{
    public TypeKind Kind { get; set; } = TypeKind.Any;

    public bool Var { get; set; }

    public bool Opt { get; set; }
}

public sealed record ComplexTypeSyntax(Token Start) : TypeSyntax(Start)
{
    public List<TypeSyntax> Types { get; set; } = new();
}

public sealed record RecordTypeSyntax(Token Start) : TypeSyntax(Start)
{
    public List<ParameterSyntax> Fields { get; set; } = new();
}

public sealed record TupleTypeSyntax(Token Start) : TypeSyntax(Start)
{
    public List<TypeSyntax> Items { get; set; } = new();
}

public sealed record ExprType(Token Start, SyntaxNode Expr) : TypeSyntax(Start) { }

public sealed record ArrayTypeSyntax(Token Start) : TypeSyntax(Start)
{
    public required TypeSyntax Items { get; set; }

    public required List<SyntaxNode> Dimensions { get; set; }

    public int N => Dimensions.Count;
}

public sealed record ListTypeSyntax(Token Start) : TypeSyntax(Start)
{
    public required TypeSyntax Items { get; set; }
}

public sealed record SetTypeSyntax(Token Start) : TypeSyntax(Start)
{
    public required TypeSyntax Items { get; init; }
}

public sealed record NameTypeSyntax(Token Start, Token Name) : TypeSyntax(Start) { }
