namespace MiniZinc.Parser.Syntax;

/// <summary>
/// A simple or complex type  
/// </summary>
/// <mzn>var set of int</mzn>
/// <mzn>bool</mzn>
/// <mzn>array[X] of opt var float</mzn>
public record TypeSyntax(Token Start) : SyntaxNode(Start)
{
    public TypeKind Kind { get; set; } = TypeKind.Any;

    public bool Var { get; set; }

    public bool Opt { get; set; }
}

/// <summary>
/// A concatenation of other types
/// </summary>
/// <param name="Start"></param>
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

/// <summary>
/// 
/// </summary>
/// <mzn>list of var int</mzn>
public sealed record ListTypeSyntax(Token Start) : TypeSyntax(Start)
{
    public required TypeSyntax Items { get; set; }
}

/// <mzn>set of  var int</mzn>
public sealed record SetTypeSyntax(Token Start) : TypeSyntax(Start)
{
    public required TypeSyntax Items { get; init; }
}

/// <summary>
/// An identifier used as a type
/// </summary>
/// <mzn>type X = 1..3; var X: x;</mzn>
public sealed record NameTypeSyntax(Token Start, IdentifierSyntax Name) : TypeSyntax(Start) { }
