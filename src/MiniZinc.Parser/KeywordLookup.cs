namespace MiniZinc.Parser;

internal sealed class KeywordLookup
{
    private static KeywordLookup? _table;
    public static KeywordLookup Table => _table ??= new KeywordLookup();

    private readonly Dictionary<TokenKind, string> _tokenToWord;
    public IReadOnlyDictionary<TokenKind, string> TokenToWord => _tokenToWord;

    private readonly Dictionary<string, TokenKind> _wordToToken;
    public IReadOnlyDictionary<string, TokenKind> WordToToken => _wordToToken;

    private KeywordLookup()
    {
        var names = Enum.GetNames<TokenKind>();
        var count = names.Length;
        _tokenToWord = new Dictionary<TokenKind, string>(count);
        _wordToToken = new Dictionary<string, TokenKind>(count);

        Add(TokenKind.KeywordAnnotation, "annotation");
        Add(TokenKind.KeywordAnn, "ann");
        Add(TokenKind.KeywordAny, "any");
        Add(TokenKind.KeywordArray, "array");
        Add(TokenKind.KeywordBool, "bool");
        Add(TokenKind.KeywordCase, "case");
        Add(TokenKind.KeywordConstraint, "constraint");
        Add(TokenKind.KeywordDefault, "default");
        Add(TokenKind.KeywordDiff, "diff");
        Add(TokenKind.KeywordDiv, "div");
        Add(TokenKind.KeywordElse, "else");
        Add(TokenKind.KeywordElseif, "elseif");
        Add(TokenKind.KeywordEndif, "endif");
        Add(TokenKind.KeywordEnum, "enum");
        Add(TokenKind.KeywordFalse, "false");
        Add(TokenKind.KeywordFloat, "float");
        Add(TokenKind.KeywordFunction, "function");
        Add(TokenKind.KeywordIf, "if");
        Add(TokenKind.KeywordIn, "in");
        Add(TokenKind.KeywordInclude, "include");
        Add(TokenKind.KeywordInt, "int");
        Add(TokenKind.KeywordIntersect, "intersect");
        Add(TokenKind.KeywordLet, "let");
        Add(TokenKind.KeywordList, "list");
        Add(TokenKind.KeywordMaximize, "maximize");
        Add(TokenKind.KeywordMinimize, "minimize");
        Add(TokenKind.KeywordMod, "mod");
        Add(TokenKind.KeywordNot, "not");
        Add(TokenKind.KeywordOf, "of");
        Add(TokenKind.KeywordOp, "op");
        Add(TokenKind.KeywordOpt, "opt");
        Add(TokenKind.KeywordOutput, "output");
        Add(TokenKind.KeywordPar, "par");
        Add(TokenKind.KeywordPredicate, "predicate");
        Add(TokenKind.KeywordRecord, "record");
        Add(TokenKind.KeywordSatisfy, "satisfy");
        Add(TokenKind.KeywordSet, "set");
        Add(TokenKind.KeywordSolve, "solve");
        Add(TokenKind.KeywordString, "string");
        Add(TokenKind.KeywordSubset, "subset");
        Add(TokenKind.KeywordSuperset, "superset");
        Add(TokenKind.KeywordSymdiff, "symdiff");
        Add(TokenKind.KeywordTest, "test");
        Add(TokenKind.KeywordThen, "then");
        Add(TokenKind.KeywordTrue, "true");
        Add(TokenKind.KeywordTuple, "tuple");
        Add(TokenKind.KeywordType, "type");
        Add(TokenKind.KeywordUnion, "union");
        Add(TokenKind.KeywordVar, "var");
        Add(TokenKind.KeywordWhere, "where");
        Add(TokenKind.KeywordXor, "xor");

        Add(TokenKind.DownWedge, "\\/");
        Add(TokenKind.UpWedge, "/\\");
        Add(TokenKind.LessThan, "<");
        Add(TokenKind.GreaterThan, ">");
        Add(TokenKind.LessThanEqual, "<=");
        Add(TokenKind.GreaterThanEqual, ">=");
        Add(TokenKind.Equal, "=");
        Add(TokenKind.DotDot, "..");
        Add(TokenKind.Plus, "+");
        Add(TokenKind.Minus, "-");
        Add(TokenKind.Star, "*");
        Add(TokenKind.Slash, "/");
        Add(TokenKind.PlusPlus, "++");
        Add(TokenKind.TildeEquals, "~=");
        Add(TokenKind.TildePlus, "~+");
        Add(TokenKind.TildeMinus, "~-");
        Add(TokenKind.TildeStar, "~*");
        Add(TokenKind.LeftBracket, "[");
        Add(TokenKind.RightBracket, "]");
        Add(TokenKind.LeftParen, "(");
        Add(TokenKind.RightParen, ")");
        Add(TokenKind.LeftBrace, "{");
        Add(TokenKind.RightBrace, "}");
        Add(TokenKind.Dot, ".");
        Add(TokenKind.Percent, "%");
        Add(TokenKind.Underscore, "_");
        Add(TokenKind.Tilde, "~");
        Add(TokenKind.BackSlash, "\\");
        Add(TokenKind.ForwardSlash, "/");
        Add(TokenKind.Colon, ":");
        Add(TokenKind.Delimiter, "");
        Add(TokenKind.Pipe, "|");
        Add(TokenKind.Empty, "<>");
    }

    private void Add(TokenKind kind, string word)
    {
        _tokenToWord[kind] = word;
        _wordToToken[word] = kind;
    }
}
