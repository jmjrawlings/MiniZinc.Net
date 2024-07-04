namespace MiniZinc.Parser;

public enum TypeKind : byte
{
    Any,
    Int,
    Bool,
    String,
    Float,
    Name,
    Ann,
    Annotation,
    Generic,
    GenericSeq,
    Expr,
    Tuple,
    Record,
    Array,
    List,
    Composite,
    Set,
    Enum
}

public enum DeclareKind : byte
{
    Value,
    Function,
    Test,
    Predicate,
    Annotation,
    Enum,
    TypeAlias
}
