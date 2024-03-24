namespace MiniZinc.Parser.Ast;

public enum TypeKind
{
    Any,
    Int,
    Bool,
    String,
    Float,
    Name,
    Annotation,
    PolyMorphic,
    Generic,
    Expr,
    Tuple,
    Record,
    Array
}
