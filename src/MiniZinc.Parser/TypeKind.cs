namespace MiniZinc.Parser;

public enum TypeKind : byte
{
    TYPE_ANY,
    TYPE_INT,
    TYPE_BOOL,
    TYPE_STRING,
    TYPE_FLOAT,
    TYPE_IDENT,
    TYPE_ANN,
    TYPE_ANNOTATION,
    TYPE_EXPR,
    TYPE_TUPLE,
    TYPE_RECORD,
    TYPE_ARRAY,
    TYPE_LIST,
    TYPE_COMPOSITE,
    TYPE_SET,
    TYPE_ENUM
}

public enum DeclareKind : byte
{
    DECLARE_VALUE,
    DECLARE_FUNCTION,
    DECLARE_TEST,
    DECLARE_PREDICATE,
    DECLARE_ANNOTATION,
    DECLARE_ENUM,
    DECLARE_TYPE
}
