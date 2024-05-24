namespace MiniZinc.Parser.Syntax;

/// <summary>
/// A simple or complex type
/// </summary>
/// <mzn>var set of int</mzn>
/// <mzn>bool</mzn>
/// <mzn>array[X] of opt var float</mzn>
public record TypeSyntax(in Token Start) : SyntaxNode(Start)
{
    public TypeKind Kind { get; set; } = TypeKind.Any;

    /// True if the type is a variable (ie not a parameter)
    public bool Var { get; set; }

    /// True if the type is optional
    public bool Opt { get; set; }
}

/// <summary>
/// A concatenation of other types
/// </summary>
/// <mzn>record(a:int) ++ record(b: bool)</mzn>
public sealed record CompositeTypeSyntax(in Token Start) : TypeSyntax(Start)
{
    public List<TypeSyntax> Types { get; set; } = new();
}

public sealed record RecordTypeSyntax(in Token Start) : TypeSyntax(Start)
{
    public List<ParameterSyntax> Fields { get; set; } = new();
}

public sealed record TupleTypeSyntax(in Token Start) : TypeSyntax(Start)
{
    public List<TypeSyntax> Items { get; set; } = new();
}

public sealed record ExprType(in Token Start, SyntaxNode Expr) : TypeSyntax(Start) { }

public sealed record ArrayTypeSyntax(in Token Start) : TypeSyntax(Start)
{
    public required TypeSyntax Items { get; set; }

    public required List<SyntaxNode> Dimensions { get; set; }

    public int N => Dimensions.Count;
}

/// <summary>
///
/// </summary>
/// <mzn>list of var int</mzn>
public sealed record ListTypeSyntax(in Token Start) : TypeSyntax(Start)
{
    public required TypeSyntax Items { get; set; }
}

/// <mzn>set of  var int</mzn>
public sealed record SetTypeSyntax(in Token Start) : TypeSyntax(Start)
{
    public required TypeSyntax Items { get; init; }
}

/// <summary>
/// An identifier used as a type
/// </summary>
/// <mzn>type X = 1..3; var X: x;</mzn>
public sealed record NameTypeSyntax(in Token Start, IdentifierSyntax Name) : TypeSyntax(Start) { }
