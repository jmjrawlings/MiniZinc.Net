namespace MiniZinc.Parser.Ast;

/// <summary>
/// Binary and unary operators
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
    ClosedRange,
    LeftOpenRange,
    RightOpenRange,
    OpenRange,
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
    Plus,
    Minus,
    Not,
    TildeNotEqual,
    TildeEqual,
    TildeAdd,
    TildeSubtract,
    TildeMultiply
}
