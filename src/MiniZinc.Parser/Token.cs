namespace MiniZinc.Parser;

using System.Globalization;

public readonly struct Token
{
    public readonly TokenKind Kind;
    public readonly int Line;
    public readonly int Col;
    public readonly int Start;
    public readonly int Length;
    public readonly object? Data;
    public int IntValue => (int)Data!;
    public string StringValue => (string)Data!;
    public decimal DecimalValue => (decimal)Data!;
    public bool BoolValue => Kind is TokenKind.KEYWORD_TRUE;
    public int End => Start + Length;

    public Token(TokenKind kind, int line, int col, int start, int length, object? data = null)
    {
        Kind = kind;
        Line = line;
        Col = col;
        Start = start;
        Length = length;
        Data = data;
    }

    public override string ToString() =>
        Kind switch
        {
            TokenKind.KEYWORD_ANNOTATION => "annotation",
            TokenKind.KEYWORD_ANN => "ann",
            TokenKind.KEYWORD_ANY => "any",
            TokenKind.KEYWORD_ARRAY => "array",
            TokenKind.KEYWORD_BOOL => "bool",
            TokenKind.KEYWORD_CASE => "case",
            TokenKind.KEYWORD_CONSTRAINT => "constraint",
            TokenKind.KEYWORD_DEFAULT => "default",
            TokenKind.KEYWORD_DIFF => "diff",
            TokenKind.KEYWORD_DIV => "div",
            TokenKind.KEYWORD_ELSE => "else",
            TokenKind.KEYWORD_ELSEIF => "elseif",
            TokenKind.KEYWORD_ENDIF => "endif",
            TokenKind.KEYWORD_ENUM => "enum",
            TokenKind.KEYWORD_FALSE => "false",
            TokenKind.KEYWORD_FLOAT => "float",
            TokenKind.KEYWORD_FUNCTION => "function",
            TokenKind.KEYWORD_IF => "if",
            TokenKind.KEYWORD_IN => "in",
            TokenKind.KEYWORD_INCLUDE => "include",
            TokenKind.KEYWORD_INT => "int",
            TokenKind.KEYWORD_INTERSECT => "intersect",
            TokenKind.KEYWORD_LET => "let",
            TokenKind.KEYWORD_LIST => "list",
            TokenKind.KEYWORD_MAXIMIZE => "maximize",
            TokenKind.KEYWORD_MINIMIZE => "minimize",
            TokenKind.KEYWORD_MOD => "mod",
            TokenKind.KEYWORD_NOT => "not",
            TokenKind.KEYWORD_OF => "of",
            TokenKind.KEYWORD_OP => "op",
            TokenKind.KEYWORD_OPT => "opt",
            TokenKind.KEYWORD_OUTPUT => "output",
            TokenKind.KEYWORD_PAR => "par",
            TokenKind.KEYWORD_PREDICATE => "predicate",
            TokenKind.KEYWORD_RECORD => "record",
            TokenKind.KEYWORD_SATISFY => "satisfy",
            TokenKind.KEYWORD_SET => "set",
            TokenKind.KEYWORD_SOLVE => "solve",
            TokenKind.KEYWORD_STRING => "string",
            TokenKind.KEYWORD_SUBSET => "subset",
            TokenKind.KEYWORD_SUPERSET => "superset",
            TokenKind.KEYWORD_SYMDIFF => "symdiff",
            TokenKind.KEYWORD_TEST => "test",
            TokenKind.KEYWORD_THEN => "then",
            TokenKind.KEYWORD_TRUE => "true",
            TokenKind.KEYWORD_TUPLE => "tuple",
            TokenKind.KEYWORD_TYPE => "type",
            TokenKind.KEYWORD_UNION => "union",
            TokenKind.KEYWORD_VAR => "var",
            TokenKind.KEYWORD_WHERE => "where",
            TokenKind.KEYWORD_XOR => "xor",
            TokenKind.DISJUNCTION => "\\/",
            TokenKind.CONJUNCTION => "/\\",
            TokenKind.BI_IMPLICATION => "<->",
            TokenKind.REVERSE_IMPLICATION => "<-",
            TokenKind.FORWARD_IMPLICATION => "->",
            TokenKind.LESS_THAN => "<",
            TokenKind.GREATER_THAN => ">",
            TokenKind.LESS_THAN_EQUAL => "<=",
            TokenKind.GREATER_THAN_EQUAL => ">=",
            TokenKind.EQUAL => "==",
            TokenKind.CLOSED_RANGE => "..",
            TokenKind.RIGHT_OPEN_RANGE => "..<",
            TokenKind.LEFT_OPEN_RANGE => "<..",
            TokenKind.OPEN_RANGE => "<..<",
            TokenKind.PLUS_PLUS => "++",
            TokenKind.PLUS => "+",
            TokenKind.MINUS => "-",
            TokenKind.TIMES => "*",
            TokenKind.DIVIDE => "/",
            TokenKind.TILDE_EQUALS => "~=",
            TokenKind.TILDE_PLUS => "~+",
            TokenKind.TILDE_MINUS => "~-",
            TokenKind.TILDE_TIMES => "~*",
            TokenKind.OPEN_BRACKET => "[",
            TokenKind.CLOSE_BRACKET => "]",
            TokenKind.OPEN_PAREN => "(",
            TokenKind.CLOSE_PAREN => ")",
            TokenKind.OPEN_BRACE => "{",
            TokenKind.CLOSE_BRACE => "}",
            TokenKind.TUPLE_ACCESS => $".{IntValue}",
            TokenKind.RECORD_ACCESS => $".{Data}",
            TokenKind.PERCENT => "%",
            TokenKind.UNDERSCORE => "_",
            TokenKind.TILDE => "~",
            TokenKind.BACKSLASH => "\\",
            TokenKind.COLON => ":",
            TokenKind.COLON_COLON => "::",
            TokenKind.EOL => ";",
            TokenKind.PIPE => "|",
            TokenKind.EMPTY => "<>",
            TokenKind.INT_LITERAL => IntValue.ToString(),
            TokenKind.FLOAT_LITERAL => DecimalValue.ToString(CultureInfo.InvariantCulture),
            TokenKind.STRING_LITERAL => $"\"{Data}\"",
            TokenKind.KEYWORD_ANONENUM => "anon_enum",
            TokenKind.NOT_EQUAL => "!=",
            TokenKind.EXPONENT => "^",
            TokenKind.COMMA => ",",
            TokenKind.IDENTIFIER_GENERIC_SEQUENCE => $"$${Data}",
            TokenKind.IDENTIFIER_GENERIC => $"${Data}",
            TokenKind.IDENTIFIER_INFIX => $"`{Data}`",
            _ => Data?.ToString() ?? string.Empty
        };
}
