namespace MiniZinc.Parser.Ast;

/// <summary>
/// Built in binary and unary operators
/// </summary>
public enum Operator
{
    Equivalent,
    Implies,
    ImpliedBy,
    Or,
    Xor,
    And,
    LessThan,
    GreaterThan,
    LessThanEqual,
    GreaterThanEqual,
    Equal,
    NotEqual,
    In,
    Subset,
    Superset,
    Union,
    Diff,
    SymDiff,
    Range,
    Add,
    Subtract,
    Multiply,
    Div,
    Mod,
    Divide,
    Intersect,
    Exponent,
    Default,
    Concat,
    Positive,
    Negative,
    Not,
    TildeNotEqual,
    TildeEqual,
    TildeAdd,
    TildeSubtract,
    TildeMultiply
}
