namespace MiniZinc.Parser;

public enum ScopeKind
{
    Model,
    IncludeItem,
    FunctionItem,
    ConstraintItem,
    SolveItem,
    AliasItem,
    AnnotationItem,
    OutputItem,
    Expr,
    GenExpr,
    GenCall,
    IfElse,
    Let,
    TypeInst,
    TypeInstAndId,
    Enum,
    Array,
    Assign,
    Declare,
    Parameters,
    Arguments,
    ArrayDimensions,
    Variable
}
