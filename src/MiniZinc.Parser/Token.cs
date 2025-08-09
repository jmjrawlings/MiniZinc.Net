namespace MiniZinc.Parser;

using System.Globalization;
using static TokenKind;

public readonly struct Token : IEquatable<Token>
{
    public readonly TokenKind Kind;
    public readonly int Line;
    public readonly int Col;
    public readonly int Start;
    public readonly int Length;
    public readonly int IntValue;
    public readonly string? StringValue;
    public readonly decimal FloatValue;
    public int End => Start + Length;

    public Token(
        TokenKind kind,
        int line,
        int col,
        int start,
        int length,
        int i = 0,
        string? s = null,
        decimal f = 0
    )
    {
        Kind = kind;
        Line = line;
        Col = col;
        Start = start;
        Length = length;
        StringValue = s;
        FloatValue = f;
        IntValue = i;
    }

    public bool Equals(Token other)
    {
        if (Kind != other.Kind)
            return false;

        if (IntValue != other.IntValue)
            return false;

        if (FloatValue != other.FloatValue)
            return false;

        if (!(StringValue?.Equals(other.StringValue) ?? true))
            return false;

        return true;
    }

    public override bool Equals(object? obj)
    {
        return obj is Token other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(
            (int)Kind,
            Line,
            Col,
            Start,
            Length,
            IntValue,
            FloatValue,
            StringValue
        );
    }

    public static bool operator ==(Token left, Token right)
    {
        return left.Equals(right);
    }

    public static bool operator !=(Token left, Token right)
    {
        return !left.Equals(right);
    }

    public override string ToString() =>
        Kind switch
        {
            KEYWORD_ANNOTATION => "annotation",
            KEYWORD_ANN => "ann",
            KEYWORD_ANY => "any",
            KEYWORD_ARRAY => "array",
            KEYWORD_BOOL => "bool",
            KEYWORD_CASE => "case",
            KEYWORD_CONSTRAINT => "constraint",
            KEYWORD_DEFAULT => "default",
            KEYWORD_DIFF => "diff",
            KEYWORD_DIV => "div",
            KEYWORD_ELSE => "else",
            KEYWORD_ELSEIF => "elseif",
            KEYWORD_ENDIF => "endif",
            KEYWORD_ENUM => "enum",
            KEYWORD_FALSE => "false",
            KEYWORD_FLOAT => "float",
            KEYWORD_FUNCTION => "function",
            KEYWORD_IF => "if",
            KEYWORD_IN => "in",
            KEYWORD_INCLUDE => "include",
            KEYWORD_INT => "int",
            KEYWORD_INTERSECT => "intersect",
            KEYWORD_LET => "let",
            KEYWORD_LIST => "list",
            KEYWORD_MAXIMIZE => "maximize",
            KEYWORD_MINIMIZE => "minimize",
            KEYWORD_MOD => "mod",
            KEYWORD_NOT => "not",
            KEYWORD_OF => "of",
            KEYWORD_OP => "op",
            KEYWORD_OPT => "opt",
            KEYWORD_OUTPUT => "output",
            KEYWORD_PAR => "par",
            KEYWORD_PREDICATE => "predicate",
            KEYWORD_RECORD => "record",
            KEYWORD_SATISFY => "satisfy",
            KEYWORD_SET => "set",
            KEYWORD_SOLVE => "solve",
            KEYWORD_STRING => "string",
            KEYWORD_SUBSET => "subset",
            KEYWORD_SUPERSET => "superset",
            KEYWORD_SYMDIFF => "symdiff",
            KEYWORD_TEST => "test",
            KEYWORD_THEN => "then",
            KEYWORD_TRUE => "true",
            KEYWORD_TUPLE => "tuple",
            KEYWORD_TYPE => "type",
            KEYWORD_UNION => "union",
            KEYWORD_VAR => "var",
            KEYWORD_WHERE => "where",
            KEYWORD_XOR => "xor",
            TOKEN_DISJUNCTION => "\\/",
            TOKEN_CONJUNCTION => "/\\",
            TOKEN_BI_IMPLICATION => "<->",
            TOKEN_REVERSE_IMPLICATION => "<-",
            TOKEN_FORWARD_IMPLICATION => "->",
            TOKEN_LESS_THAN => "<",
            TOKEN_GREATER_THAN => ">",
            TOKEN_LESS_THAN_EQUAL => "<=",
            TOKEN_GREATER_THAN_EQUAL => ">=",
            TOKEN_EQUAL => "==",
            TOKEN_RANGE_INCLUSIVE => "..",
            TOKEN_RANGE_RIGHT_EXCLUSIVE => "..<",
            TOKEN_RANGE_LEFT_EXCLUSIVE => "<..",
            TOKEN_RANGE_EXCLUSIVE => "<..<",
            TOKEN_PLUS_PLUS => "++",
            TOKEN_PLUS => "+",
            TOKEN_MINUS => "-",
            TOKEN_TIMES => "*",
            TOKEN_DIVIDE => "/",
            TOKEN_TILDE_EQUALS => "~=",
            TOKEN_TILDE_PLUS => "~+",
            TOKEN_TILDE_MINUS => "~-",
            TOKEN_TILDE_TIMES => "~*",
            TOKEN_OPEN_BRACKET => "[",
            TOKEN_CLOSE_BRACKET => "]",
            TOKEN_OPEN_PAREN => "(",
            TOKEN_CLOSE_PAREN => ")",
            TOKEN_OPEN_BRACE => "{",
            TOKEN_CLOSE_BRACE => "}",
            TOKEN_TUPLE_ACCESS => $".{IntValue}",
            TOKEN_RECORD_ACCESS => $".{StringValue}",
            TOKEN_PERCENT => "%",
            TOKEN_UNDERSCORE => "_",
            TOKEN_TILDE => "~",
            TOKEN_BACKSLASH => "\\",
            TOKEN_COLON => ":",
            TOKEN_COLON_COLON => "::",
            TOKEN_EOL => ";",
            TOKEN_PIPE => "|",
            TOKEN_EMPTY => "<>",
            TOKEN_INT_LITERAL => IntValue.ToString(),
            TOKEN_FLOAT_LITERAL => FloatValue.ToString(CultureInfo.InvariantCulture),
            TOKEN_STRING_LITERAL => $"\"{StringValue}\"",
            KEYWORD_ANONENUM => "anon_enum",
            TOKEN_NOT_EQUAL => "!=",
            TOKEN_EXPONENT => "^",
            TOKEN_COMMA => ",",
            TOKEN_IDENTIFIER_GENERIC_SEQUENCE => $"$${StringValue}",
            TOKEN_IDENTIFIER_GENERIC => $"${StringValue}",
            TOKEN_IDENTIFIER_INFIX => $"`{StringValue}`",
            _ => StringValue ?? string.Empty
        };
}
