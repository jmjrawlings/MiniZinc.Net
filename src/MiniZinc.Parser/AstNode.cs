namespace MiniZinc.Parser;

public enum AstNode
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
