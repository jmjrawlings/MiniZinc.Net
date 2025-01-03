namespace MiniZinc.Parser;

using static TypeKind;

/// <summary>
/// A simple or complex type
/// </summary>
/// <mzn>var set of int</mzn>
/// <mzn>bool</mzn>
/// <mzn>array[X] of opt var float</mzn>
public abstract class TypeSyntax : MiniZincSyntax
{
    /// <summary>
    /// A simple or complex type
    /// </summary>
    /// <mzn>var set of int</mzn>
    /// <mzn>bool</mzn>
    /// <mzn>array[X] of opt var float</mzn>
    protected TypeSyntax(in Token start)
        : base(start) { }

    public TypeKind Kind { get; set; } = TYPE_ANY;

    /// TODO
    public Token Name { get; set; }

    /// True if the type is a variable (ie not a parameter)
    public bool IsVar { get; set; }

    /// True if the type is optional
    public bool IsOpt { get; set; }
}

/// <summary>
/// A simple/base type
/// </summary>
/// <mzn>class(a:int) ++ class(b: bool)</mzn>
public sealed class BaseTypeSyntax : TypeSyntax
{
    public BaseTypeSyntax(in Token Start, in TypeKind kind)
        : base(Start)
    {
        Kind = kind;
    }
}

/// <summary>
/// A concatenation of other types
/// </summary>
/// <mzn>class(a:int) ++ class(b: bool)</mzn>
public sealed class CompositeTypeSyntax : TypeSyntax
{
    public IReadOnlyList<TypeSyntax> Types { get; }

    public CompositeTypeSyntax(in Token Start, IReadOnlyList<TypeSyntax> types)
        : base(Start)
    {
        Types = types;
        Kind = TYPE_COMPOSITE;
    }
}

public sealed class RecordTypeSyntax : TypeSyntax
{
    public IReadOnlyList<ParameterSyntax> Fields { get; }

    public RecordTypeSyntax(in Token start, List<ParameterSyntax> fields)
        : base(start)
    {
        Fields = fields;
        Kind = TYPE_RECORD;
    }
}

public sealed class TupleTypeSyntax : TypeSyntax
{
    public List<TypeSyntax> Items { get; }

    public TupleTypeSyntax(in Token Start, List<TypeSyntax> items)
        : base(Start)
    {
        Items = items;
        Kind = TYPE_TUPLE;
    }
}

public sealed class ExprTypeSyntax : TypeSyntax
{
    public MiniZincExpr Expr { get; }

    public ExprTypeSyntax(in Token Start, MiniZincExpr expr)
        : base(Start)
    {
        Expr = expr;
        Kind = TYPE_EXPR;
    }
}

public sealed class ArrayTypeSyntax : TypeSyntax
{
    public ArrayTypeSyntax(in Token Start, List<TypeSyntax> dimensions, TypeSyntax items)
        : base(Start)
    {
        Dimensions = dimensions;
        Items = items;
    }

    public TypeSyntax Items { get; }

    public IReadOnlyList<TypeSyntax> Dimensions { get; }

    public int N => Dimensions.Count;
}

/// <summary>
///
/// </summary>
/// <mzn>list of var int</mzn>
public sealed class ListTypeSyntax : TypeSyntax
{
    public TypeSyntax Items { get; }

    /// <summary>
    ///
    /// </summary>
    /// <mzn>list of var int</mzn>
    public ListTypeSyntax(in Token Start, TypeSyntax type)
        : base(Start)
    {
        Items = type;
        Kind = TYPE_LIST;
    }
}

/// <mzn>set of  var int</mzn>
public sealed class SetTypeSyntax : TypeSyntax
{
    public TypeSyntax Items { get; }

    /// <mzn>set of  var int</mzn>
    public SetTypeSyntax(in Token start, TypeSyntax items)
        : base(start)
    {
        Items = items;
        Kind = TYPE_SET;
    }
}
