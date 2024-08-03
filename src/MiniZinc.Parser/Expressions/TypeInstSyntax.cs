namespace MiniZinc.Parser.Syntax;

/// <summary>
/// A simple or complex type
/// </summary>
/// <mzn>var set of int</mzn>
/// <mzn>bool</mzn>
/// <mzn>array[X] of opt var float</mzn>
public class TypeSyntax : SyntaxNode
{
    /// <summary>
    /// A simple or complex type
    /// </summary>
    /// <mzn>var set of int</mzn>
    /// <mzn>bool</mzn>
    /// <mzn>array[X] of opt var float</mzn>
    public TypeSyntax(in Token start)
        : base(start) { }

    public TypeKind Kind { get; set; } = TypeKind.Any;

    /// True if the type is a variable (ie not a parameter)
    public Token Name { get; set; }

    /// True if the type is a variable (ie not a parameter)
    public bool Var { get; set; }

    /// True if the type is optional
    public bool Opt { get; set; }
}

/// <summary>
/// A concatenation of other types
/// </summary>
/// <mzn>class(a:int) ++ class(b: bool)</mzn>
public sealed class CompositeTypeSyntax : TypeSyntax
{
    public IReadOnlyList<TypeSyntax> Types { get; }

    public CompositeTypeSyntax(in Token Start, List<TypeSyntax> types)
        : base(Start)
    {
        Types = types;
        Kind = TypeKind.Composite;
    }
}

public sealed class RecordTypeSyntax : TypeSyntax
{
    public IReadOnlyList<(Token, TypeSyntax)> Fields { get; }

    public RecordTypeSyntax(in Token start, List<(Token, TypeSyntax)> fields)
        : base(start)
    {
        Fields = fields;
    }
}

public sealed class TupleTypeSyntax : TypeSyntax
{
    public List<TypeSyntax> Items { get; }

    public TupleTypeSyntax(in Token Start, List<TypeSyntax> items)
        : base(Start)
    {
        Items = items;
        Kind = TypeKind.Tuple;
    }
}

public sealed class ExprTypeSyntax : TypeSyntax
{
    public ExpressionSyntax Expr { get; }

    public ExprTypeSyntax(in Token Start, ExpressionSyntax Expr)
        : base(Start)
    {
        this.Expr = Expr;
    }
}

public sealed class ArrayTypeSyntax : TypeSyntax
{
    public ArrayTypeSyntax(in Token Start)
        : base(Start) { }

    public required TypeSyntax Items { get; set; }

    public required List<TypeSyntax> Dimensions { get; set; }

    public int N => Dimensions.Count;
}

/// <summary>
///
/// </summary>
/// <mzn>list of var int</mzn>
public sealed class ListTypeSyntax : TypeSyntax
{
    /// <summary>
    ///
    /// </summary>
    /// <mzn>list of var int</mzn>
    public ListTypeSyntax(in Token Start)
        : base(Start) { }

    public required TypeSyntax Items { get; set; }
}

/// <mzn>set of  var int</mzn>
public sealed class SetTypeSyntax : TypeSyntax
{
    /// <mzn>set of  var int</mzn>
    public SetTypeSyntax(in Token Start)
        : base(Start) { }

    public required TypeSyntax Items { get; init; }
}
