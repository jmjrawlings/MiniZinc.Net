namespace MiniZinc.Parser;

using Annotation = IExpr;
using ArrayDim = Type;

public enum TypeKind
{
    Any,
    Int,
    Bool,
    String,
    Float,
    Annotation,
    Generic,
    Ident,
    Expr,
    Tuple,
    Record,
    Array
}

[Flags]
public enum TypeInstKind
{
    None = 1 << 0,
    Par = 1 << 1,
    Var = 1 << 2,
    Set = 1 << 3,
    Opt = 1 << 4,
    Array = 1 << 5
}

public record Type
{
    public TypeKind Kind { get; set; }
}

public sealed record NamedType : Type
{
    public string Name { get; set; }
    public bool IsGeneric { get; set; }
}

public sealed record RecordType : Type
{
    public List<TypeInst> Fields { get; set; } = new();
}

public sealed record TupleType : Type
{
    public List<TypeInst> Items { get; set; } = new();
}

public record ArrayType : Type
{
    public TypeInst ValueType { get; set; }
}

public record Array1DType : ArrayType
{
    public ArrayDim I { get; set; }
}

public record Array2DType : Array1DType
{
    public ArrayDim J { get; set; }
}

public record Array3DType : Array2DType
{
    public ArrayDim K { get; set; }
}

public record Array4DType : Array3DType
{
    public ArrayDim L { get; set; }
}

public record Array5DType : Array4DType
{
    public ArrayDim M { get; set; }
}

public record Array6DType : Array5DType
{
    public ArrayDim M { get; set; }
}

public sealed record TypeInst : IBinding
{
    public string Name { get; set; }
    public Type Type { get; set; }
    public List<Annotation>? Annotations { get; set; }
    public TypeInstKind Kind { get; set; }
    public List<Annotation>? PostFix { get; set; }
    public IExpr? Value { get; set; }

    public bool IsKind(TypeInstKind kind) => (Kind & kind) > 0;

    public bool IsCollection => IsKind(TypeInstKind.Set | Kind);
    public bool IsSingleton => !IsCollection;
}
