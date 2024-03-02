namespace MiniZinc.Parser;

public enum NodeKind
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
    Arguments
}
