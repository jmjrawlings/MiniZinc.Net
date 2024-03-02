namespace MiniZinc.Parser;

public enum NodeKind
{
    Model,
    Include,
    Function,
    Constraint,
    Expr,
    GenExpr,
    GenCall,
    Solve,
    IfElse,
    Let,
    Type,
    TypeInst,
    Output,
    Enum,
    Array
}
