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
    public bool BoolValue => Kind is TokenKind.TRUE;
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
            TokenKind.ANNOTATION => "annotation",
            TokenKind.ANN => "ann",
            TokenKind.ANY => "any",
            TokenKind.ARRAY => "array",
            TokenKind.BOOL => "bool",
            TokenKind.CASE => "case",
            TokenKind.CONSTRAINT => "constraint",
            TokenKind.DEFAULT => "default",
            TokenKind.DIFF => "diff",
            TokenKind.DIV => "div",
            TokenKind.ELSE => "else",
            TokenKind.ELSEIF => "elseif",
            TokenKind.ENDIF => "endif",
            TokenKind.ENUM => "enum",
            TokenKind.FALSE => "false",
            TokenKind.FLOAT => "float",
            TokenKind.FUNCTION => "function",
            TokenKind.IF => "if",
            TokenKind.IN => "in",
            TokenKind.INCLUDE => "include",
            TokenKind.INT => "int",
            TokenKind.INTERSECT => "intersect",
            TokenKind.LET => "let",
            TokenKind.LIST => "list",
            TokenKind.MAXIMIZE => "maximize",
            TokenKind.MINIMIZE => "minimize",
            TokenKind.MOD => "mod",
            TokenKind.NOT => "not",
            TokenKind.OF => "of",
            TokenKind.OP => "op",
            TokenKind.OPT => "opt",
            TokenKind.OUTPUT => "output",
            TokenKind.PAR => "par",
            TokenKind.PREDICATE => "predicate",
            TokenKind.RECORD => "record",
            TokenKind.SATISFY => "satisfy",
            TokenKind.SET => "set",
            TokenKind.SOLVE => "solve",
            TokenKind.STRING => "string",
            TokenKind.SUBSET => "subset",
            TokenKind.SUPERSET => "superset",
            TokenKind.SYMDIFF => "symdiff",
            TokenKind.TEST => "test",
            TokenKind.THEN => "then",
            TokenKind.TRUE => "true",
            TokenKind.TUPLE => "tuple",
            TokenKind.TYPE => "type",
            TokenKind.UNION => "union",
            TokenKind.VAR => "var",
            TokenKind.WHERE => "where",
            TokenKind.XOR => "xor",
            TokenKind.DOWN_WEDGE => "\\/",
            TokenKind.UP_WEDGE => "/\\",
            TokenKind.LESS_THAN => "<",
            TokenKind.GREATER_THAN => ">",
            TokenKind.LESS_THAN_EQUAL => "<=",
            TokenKind.GREATER_THAN_EQUAL => ">=",
            TokenKind.EQUAL => "=",
            TokenKind.RANGE_INCLUSIVE => "..",
            TokenKind.RANGE_LEFT_INCLUSIVE => "..<",
            TokenKind.RANGE_RIGHT_INCLUSIVE => "<..",
            TokenKind.RANGE_EXCLUSIVE => "<..<",
            TokenKind.PLUS => "+",
            TokenKind.MINUS => "-",
            TokenKind.STAR => "*",
            TokenKind.SLASH => "/",
            TokenKind.PLUS_PLUS => "++",
            TokenKind.TILDE_EQUALS => "~=",
            TokenKind.TILDE_PLUS => "~+",
            TokenKind.TILDE_MINUS => "~-",
            TokenKind.TILDE_STAR => "~*",
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
            TokenKind.FWDSLASH => "/",
            TokenKind.COLON => ":",
            TokenKind.COLON_COLON => "::",
            TokenKind.EOL => ";",
            TokenKind.PIPE => "|",
            TokenKind.EMPTY => "<>",
            TokenKind.INT_LITERAL => IntValue.ToString(),
            TokenKind.FLOAT_LITERAL => DecimalValue.ToString(CultureInfo.InvariantCulture),
            TokenKind.STRING_LITERAL => $"\"{Data}\"",
            TokenKind.ANONENUM => "anon_enum",
            TokenKind.DOUBLE_ARROW => "<->",
            TokenKind.LEFT_ARROW => "<-",
            TokenKind.RIGHT_ARROW => "->",
            TokenKind.NOT_EQUAL => "!=",
            TokenKind.EXP => "^",
            TokenKind.COMMA => ",",
            TokenKind.GENERIC_SEQUENCE => $"$${Data}",
            TokenKind.GENERIC => $"${Data}",
            TokenKind.INFIX_IDENTIFIER => $"`{Data}`",
            _ => Data?.ToString() ?? string.Empty
        };
}
