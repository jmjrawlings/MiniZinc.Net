namespace MiniZinc.Parser.Syntax;

/// <summary>
/// A simple or complex type
/// </summary>
/// <mzn>var set of int</mzn>
/// <mzn>bool</mzn>
/// <mzn>array[X] of opt var float</mzn>
public class TypeSyntax : StatementSyntax
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
    /// <summary>
    /// A concatenation of other types
    /// </summary>
    /// <mzn>class(a:int) ++ class(b: bool)</mzn>
    public CompositeTypeSyntax(in Token Start)
        : base(Start) { }

    public List<TypeSyntax> Types { get; set; } = new();
}

public sealed class RecordTypeSyntax : TypeSyntax
{
    public RecordTypeSyntax(in Token Start)
        : base(Start) { }

    public List<ParameterSyntax> Fields { get; set; } = new();
}

public sealed class TupleTypeSyntax : TypeSyntax
{
    public TupleTypeSyntax(in Token Start)
        : base(Start) { }

    public List<TypeSyntax> Items { get; set; } = new();
}

public sealed class ExprType : TypeSyntax
{
    public ExprType(in Token Start, SyntaxNode Expr)
        : base(Start)
    {
        this.Expr = Expr;
    }

    public SyntaxNode Expr { get; init; }
}

public sealed class ArrayTypeSyntax : TypeSyntax
{
    public ArrayTypeSyntax(in Token Start)
        : base(Start) { }

    public required TypeSyntax Items { get; set; }

    public required List<SyntaxNode> Dimensions { get; set; }

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

/// <summary>
/// An identifier used as a type
/// </summary>
/// <mzn>type X = 1..3; var X: x;</mzn>
public sealed class IdentifierTypeSyntax : TypeSyntax
{
    /// <summary>
    /// An identifier used as a type
    /// </summary>
    /// <mzn>type X = 1..3; var X: x;</mzn>
    public IdentifierTypeSyntax(in Token start, IdentifierSyntax identifier)
        : base(start)
    {
        Identifier = identifier;
    }

    public readonly IdentifierSyntax Identifier;
    public string Name => Identifier.Name;
}
