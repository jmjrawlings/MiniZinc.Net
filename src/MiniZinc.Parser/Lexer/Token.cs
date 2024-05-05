﻿namespace MiniZinc.Parser;

public readonly struct Token(
    TokenKind kind,
    uint line,
    uint col,
    uint start,
    uint length,
    object? o = null
)
{
    public readonly TokenKind Kind = kind;
    public readonly uint Line = line;
    public readonly uint Col = col;
    public readonly uint Position = start;
    public readonly uint Length = length;
    public object? Data => o;

    public int IntValue => (int)o!;
    public string StringValue => (string)o!;
    public double DoubleValue => (double)o!;

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
            TokenKind.DOT_DOT => "..",
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
            TokenKind.DOT => ".",
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
            TokenKind.INT_LIT => IntValue.ToString(),
            TokenKind.FLOAT_LIT => DoubleValue.ToString("F2"),
            TokenKind.STRING_LIT => $"\"{StringValue}\"",
            TokenKind.ANONENUM => "anon_enum",
            TokenKind.DOUBLE_ARROW => "<->",
            TokenKind.LEFT_ARROW => "<-",
            TokenKind.RIGHT_ARROW => "->",
            TokenKind.NOT_EQUAL => "!=",
            TokenKind.EXP => "^",
            TokenKind.COMMA => ",",
            TokenKind.GENERIC_SEQ => $"$${StringValue}",
            TokenKind.GENERIC => $"${StringValue}",
            _ => o?.ToString() ?? string.Empty
        };
}
