namespace MiniZinc.Parser;

internal readonly struct Token
{
    public readonly TokenKind Kind;
    public readonly uint Line;
    public readonly uint Col;
    public readonly uint Start;
    public readonly uint Length;
    public readonly int Int;
    public readonly string? String;
    public readonly double Double;

    public Token(
        TokenKind kind,
        uint line,
        uint col,
        uint start,
        uint length,
        int i = default,
        double d = default,
        string? s = default
    )
    {
        Kind = kind;
        Line = line;
        Col = col;
        Start = start;
        Length = length;
        Int = i;
        String = s;
        Double = d;
    }

    internal static string KindString(TokenKind kind) =>
        kind switch
        {
            TokenKind.KeywordAnnotation => "annotation",
            TokenKind.KeywordAnn => "ann",
            TokenKind.KeywordAny => "any",
            TokenKind.KeywordArray => "array",
            TokenKind.KeywordBool => "bool",
            TokenKind.KeywordCase => "case",
            TokenKind.KeywordConstraint => "constraint",
            TokenKind.KeywordDefault => "default",
            TokenKind.KeywordDiff => "diff",
            TokenKind.KeywordDiv => "div",
            TokenKind.KeywordElse => "else",
            TokenKind.KeywordElseif => "elseif",
            TokenKind.KeywordEndif => "endif",
            TokenKind.KeywordEnum => "enum",
            TokenKind.KeywordFalse => "false",
            TokenKind.KeywordFloat => "float",
            TokenKind.KeywordFunction => "function",
            TokenKind.KeywordIf => "if",
            TokenKind.KeywordIn => "in",
            TokenKind.KeywordInclude => "include",
            TokenKind.KeywordInt => "int",
            TokenKind.KeywordIntersect => "intersect",
            TokenKind.KeywordLet => "let",
            TokenKind.KeywordList => "list",
            TokenKind.KeywordMaximize => "maximize",
            TokenKind.KeywordMinimize => "minimize",
            TokenKind.KeywordMod => "mod",
            TokenKind.KeywordNot => "not",
            TokenKind.KeywordOf => "of",
            TokenKind.KeywordOp => "op",
            TokenKind.KeywordOpt => "opt",
            TokenKind.KeywordOutput => "output",
            TokenKind.KeywordPar => "par",
            TokenKind.KeywordPredicate => "predicate",
            TokenKind.KeywordRecord => "record",
            TokenKind.KeywordSatisfy => "satisfy",
            TokenKind.KeywordSet => "set",
            TokenKind.KeywordSolve => "solve",
            TokenKind.KeywordString => "string",
            TokenKind.KeywordSubset => "subset",
            TokenKind.KeywordSuperset => "superset",
            TokenKind.KeywordSymdiff => "symdiff",
            TokenKind.KeywordTest => "test",
            TokenKind.KeywordThen => "then",
            TokenKind.KeywordTrue => "true",
            TokenKind.KeywordTuple => "tuple",
            TokenKind.KeywordType => "type",
            TokenKind.KeywordUnion => "union",
            TokenKind.KeywordVar => "var",
            TokenKind.KeywordWhere => "where",
            TokenKind.KeywordXor => "xor",
            TokenKind.DownWedge => "\\/",
            TokenKind.UpWedge => "/\\",
            TokenKind.LessThan => "<",
            TokenKind.GreaterThan => ">",
            TokenKind.LessThanEqual => "<=",
            TokenKind.GreaterThanEqual => ">=",
            TokenKind.Equal => "=",
            TokenKind.DotDot => "..",
            TokenKind.Plus => "+",
            TokenKind.Minus => "-",
            TokenKind.Star => "*",
            TokenKind.Slash => "/",
            TokenKind.PlusPlus => "++",
            TokenKind.TildeEquals => "~=",
            TokenKind.TildePlus => "~+",
            TokenKind.TildeMinus => "~-",
            TokenKind.TildeStar => "~*",
            TokenKind.LeftBracket => "[",
            TokenKind.RightBracket => "]",
            TokenKind.LeftParen => "(",
            TokenKind.RightParen => ")",
            TokenKind.LeftBrace => "{",
            TokenKind.RightBrace => "}",
            TokenKind.Dot => ".",
            TokenKind.Percent => "%",
            TokenKind.Underscore => "_",
            TokenKind.Tilde => "~",
            TokenKind.BackSlash => "\\",
            TokenKind.ForwardSlash => "/",
            TokenKind.Colon => ":",
            TokenKind.Delimiter => "",
            TokenKind.Pipe => "|",
            TokenKind.Empty => "<>",
            _ => string.Empty
        };

    public override string ToString() =>
        $"{Kind} {String} | Line {Line}, Col {Col}, Start {Start}, End {Start + Length}, Len: {Length}";
}
