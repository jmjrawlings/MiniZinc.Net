#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace MiniZinc.Net;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Logging;
using static Char;

public enum TokenKind
{
    None,
    Error,
    Identifier,
    Polymorphic,
    IntLiteral,
    FloatLiteral,
    StringLiteral,
    LineComment,
    BlockComment,
    QuotedIdentifier,
    QuotedOperator,
    KeywordAnnotation,
    KeywordAnn,
    KeywordAny,
    KeywordArray,
    KeywordBool,
    KeywordCase,
    KeywordConstraint,
    KeywordDefault,
    KeywordDiff,
    KeywordDiv,
    KeywordElse,
    KeywordElseif,
    KeywordEndif,
    KeywordEnum,
    KeywordFalse,
    KeywordFloat,
    KeywordFunction,
    KeywordIf,
    KeywordIn,
    KeywordInclude,
    KeywordInt,
    KeywordIntersect,
    KeywordLet,
    KeywordList,
    KeywordMaximize,
    KeywordMinimize,
    KeywordMod,
    KeywordNot,
    KeywordOf,
    KeywordOp,
    KeywordOpt,
    KeywordOutput,
    KeywordPar,
    KeywordPredicate,
    KeywordRecord,
    KeywordSatisfy,
    KeywordSet,
    KeywordSolve,
    KeywordString,
    KeywordSubset,
    KeywordSuperset,
    KeywordSymdiff,
    KeywordTest,
    KeywordThen,
    KeywordTrue,
    KeywordTuple,
    KeywordType,
    KeywordUnion,
    KeywordVar,
    KeywordWhere,
    KeywordXor,
    DoubleArrow,
    LeftArrow,
    RightArrow,
    DownWedge,
    UpWedge,
    LessThan,
    GreaterThan,
    LessThanEqual,
    GreaterThanEqual,
    Equal,
    NotEqual,
    DotDot,
    Plus,
    Minus,
    Star,
    Slash,
    PlusPlus,
    TildeEquals,
    TildePlus,
    TildeMinus,
    TildeStar,
    LeftBracket,
    RightBracket,
    LeftParen,
    RightParen,
    LeftBrace,
    RightBrace,
    Dot,
    Percent,
    Underscore,
    Comma,
    Tilde,
    BackSlash,
    ForwardSlash,
    Colon,
    Delimiter,
    Pipe,
    Empty,
    EOF,

    // Errors
    ERROR_QUOTED_IDENT,
    ERROR_QUOTED_OPERATOR,
    ERROR_ESCAPED_STRING,
    ERROR_UNTERMINATED_STRING_LITERAL,
    ERROR_UNTERMINATED_STRING_EXPRESSION,
    ERROR_POLYMORPHIC_IDENTIFIER
}

public readonly struct Token
{
    public required TokenKind Kind { get; init; }
    public required uint Line { get; init; }
    public required uint Col { get; init; }
    public required uint Start { get; init; }
    public required uint Length { get; init; }
    public int? Int { get; init; }
    public string? String { get; init; }
    public double? Double { get; init; }
}

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

public sealed class Lexer : IDisposable, IEnumerable<Token>
{
    const char HASH = '#';
    const char FWD_SLASH = '/';
    const char BACK_SLASH = '\\';
    const char STAR = '*';
    const char DELIMITER = ';';
    const char EQUAL = '=';
    const char LEFT_CHEVRON = '<';
    const char RIGHT_CHEVRON = '>';
    const char UP_CHEVRON = '^';
    const char DOT = '.';
    const char PLUS = '+';
    const char DASH = '-';
    const char TILDE = '~';
    const char DOLLAR = '$';
    const char LEFT_BRACK = '[';
    const char RIGHT_BRACK = ']';
    const char LEFT_PAREN = '(';
    const char RIGHT_PAREN = ')';
    const char LEFT_BRACE = '{';
    const char RIGHT_BRACE = '}';
    const char PIPE = '|';
    const char PERCENT = '%';
    const char UNDERSCORE = '_';
    const char COMMA = ',';
    const char EXCLAMATION = '!';

    const char SINGLE_QUOTE = '\'';
    const char DOUBLE_QUOTE = '"';
    const char BACKTICK = '`';
    const char COLON = ':';
    const char NEWLINE = '\n';
    const char TAB = '\t';
    const char RETURN = '\r';
    const char SPACE = ' ';
    const char EOF = '\uffff';

    uint _line; // Start line of current token
    uint _col; // Start column of current token
    uint _pos; // Position of the stream

    // Start index of current token
    uint _start;

    // Length of the current token
    uint _length;

    // Total numbef of tokens processed
    uint _count;

    // String contents of current token
    string? _string;

    // Char at the current stream position
    char _char;

    // Int value if applicable
    int? _int;

    // Double value if applicable
    double? _double;

    // Return comments as tokens?
    public readonly bool IncludeComments;

    readonly StreamReader _sr;

    // Used for string tokens
    readonly StringBuilder _sb;
    readonly KeywordLookup _keywords;

    private Lexer(StreamReader sr, bool includeComment = false)
    {
        _sr = sr;
        _sb = new StringBuilder();
        _string = string.Empty;
        _keywords = KeywordLookup.Table;
        IncludeComments = includeComment;
    }

    /// Yields tokens until either the end of stream is reached
    /// or an error is encountered
    public IEnumerable<Token> Lex()
    {
        var move = true;
        var kind = TokenKind.None;

        // Lex the next token
        lex:
        if (move)
            Move();
        move = true;
        _start = _pos;
        _length = 0;
        switch (_char)
        {
            case TAB:
            case RETURN:
            case SPACE:
                goto lex;
            case NEWLINE:
                _line++;
                _col = 0;
                goto lex;
            case EOF:
                kind = TokenKind.EOF;
                goto ok;
            // Line comment
            case PERCENT:
                kind = TokenKind.LineComment;
                while (true)
                {
                    Move();
                    if (_char is EOF or NEWLINE)
                        break;

                    if (IncludeComments)
                        Store();
                }

                if (!IncludeComments)
                    goto lex;

                ReadString();
                break;
            // Block comment
            case FWD_SLASH when Skip(STAR):
                kind = TokenKind.BlockComment;
                block_comment:
                Move();
                switch (_char)
                {
                    case STAR when Skip(FWD_SLASH):
                        if (!IncludeComments)
                            goto lex;
                        ReadString();
                        goto ok;
                    case EOF:
                        kind = TokenKind.ERROR_UNTERMINATED_STRING_LITERAL;
                        goto error;
                    default:
                        if (IncludeComments)
                            Store();
                        goto block_comment;
                }

            case FWD_SLASH:
                kind = TokenKind.ForwardSlash;
                break;
            case BACK_SLASH:
                kind = TokenKind.BackSlash;
                break;
            case STAR:
                kind = TokenKind.Star;
                break;
            case DELIMITER:
                kind = TokenKind.Delimiter;
                goto ok;
            case EQUAL:
                Skip(EQUAL);
                kind = TokenKind.Equal;
                break;
            case LEFT_CHEVRON:
                switch (Peek2())
                {
                    case (RIGHT_CHEVRON, _):
                        Move();
                        kind = TokenKind.Empty;
                        break;
                    case (DASH, RIGHT_CHEVRON):
                        Move();
                        Move();
                        kind = TokenKind.DoubleArrow;
                        break;
                    case (DASH, _):
                        Move();
                        kind = TokenKind.LeftArrow;
                        break;
                    case (EQUAL, _):
                        kind = TokenKind.LessThanEqual;
                        break;
                    default:
                        kind = TokenKind.LessThan;
                        break;
                }

                break;
            case RIGHT_CHEVRON:
                kind = Skip(EQUAL) ? TokenKind.GreaterThanEqual : TokenKind.GreaterThan;
                break;
            case UP_CHEVRON:
                break;
            case PIPE:
                kind = TokenKind.Pipe;
                break;
            case DOT:
                kind = Skip(DOT) ? TokenKind.DotDot : TokenKind.Dot;
                break;
            case PLUS:
                kind = Skip(PLUS) ? TokenKind.PlusPlus : TokenKind.Plus;
                break;
            case DASH:
                kind = Skip(RIGHT_CHEVRON) ? TokenKind.RightArrow : TokenKind.Minus;
                break;
            case TILDE:
                switch (Peek)
                {
                    case DASH:
                        kind = TokenKind.TildeMinus;
                        Move();
                        break;
                    case PLUS:
                        kind = TokenKind.TildePlus;
                        Move();
                        break;
                    case STAR:
                        kind = TokenKind.TildeStar;
                        Move();
                        break;
                    case EQUAL:
                        kind = TokenKind.TildeEquals;
                        Move();
                        break;
                    default:
                        kind = TokenKind.Tilde;
                        break;
                }
                break;
            case LEFT_BRACK:
                kind = TokenKind.LeftBracket;
                break;
            case RIGHT_BRACK:
                kind = TokenKind.RightBracket;
                break;
            case LEFT_PAREN:
                kind = TokenKind.LeftParen;
                break;
            case RIGHT_PAREN:
                kind = TokenKind.RightParen;
                break;
            case LEFT_BRACE:
                kind = TokenKind.LeftBrace;
                break;
            case RIGHT_BRACE:
                kind = TokenKind.RightBrace;
                break;
            // Quoted identifier
            case SINGLE_QUOTE:
                kind = TokenKind.QuotedIdentifier;
                single_quote:
                Move();
                switch (_char)
                {
                    case SINGLE_QUOTE when _length > 2:
                        ReadString();
                        goto ok;
                    case SINGLE_QUOTE:
                    case BACK_SLASH:
                    case RETURN:
                    case NEWLINE:
                    case EOF:
                        kind = TokenKind.ERROR_QUOTED_IDENT;
                        goto error;
                    default:
                        Store();
                        goto single_quote;
                }
            // String literal
            case DOUBLE_QUOTE:
                kind = TokenKind.StringLiteral;
                bool inExpr = false;
                double_quote:
                Move();
                switch (_char)
                {
                    case DOUBLE_QUOTE when !inExpr:
                        ReadString();
                        goto ok;
                    case DOUBLE_QUOTE:
                        kind = TokenKind.ERROR_UNTERMINATED_STRING_EXPRESSION;
                        goto error;
                    case RIGHT_PAREN when inExpr:
                        inExpr = false;
                        goto double_quote;
                    case BACK_SLASH:
                        Store();
                        Move();
                        Store();

                        if (_char is BACK_SLASH or DOUBLE_QUOTE or SINGLE_QUOTE)
                            goto double_quote;

                        if (_char is LEFT_PAREN && !inExpr)
                        {
                            inExpr = true;
                            goto double_quote;
                        }
                        kind = TokenKind.ERROR_ESCAPED_STRING;
                        goto error;
                    case EOF:
                        kind = TokenKind.ERROR_UNTERMINATED_STRING_LITERAL;
                        goto error;
                    default:
                        Store();
                        goto double_quote;
                }
            // Quoted operator
            case BACKTICK:
                kind = TokenKind.QuotedOperator;

                quoted_operator:
                Move();
                if (_char is BACKTICK && _length > 2)
                    goto ok;

                if (IsAsciiLetterOrDigit(_char))
                {
                    Store();
                    goto quoted_operator;
                }

                kind = TokenKind.ERROR_QUOTED_OPERATOR;
                goto error;

            case DOLLAR:
                Move();
                if (!IsLetter(_char))
                {
                    kind = TokenKind.ERROR_POLYMORPHIC_IDENTIFIER;
                    goto error;
                }

                kind = TokenKind.Polymorphic;
                move = false;
                Move();
                while (IsAsciiLetterOrDigit(_char))
                {
                    Store();
                    Move();
                }
                ReadString();
                goto ok;

            case COLON:
                kind = TokenKind.Colon;
                goto ok;
            case UNDERSCORE:
                if (FollowedByLetter)
                    goto lex_identifier;

                kind = TokenKind.Underscore;
                goto ok;
            case COMMA:
                kind = TokenKind.Comma;
                goto ok;
            case EXCLAMATION:
                kind = Skip(EQUAL) ? TokenKind.NotEqual : TokenKind.Equal;
                goto ok;
            default:
                if (IsDigit(_char))
                    goto lex_number;

                if (IsLetter(_char))
                    goto lex_identifier;

                if (_sr.EndOfStream)
                    goto stop;

                ReadString();
                var msg =
                    $"Unexpected character \"{_char}\" found on line {_line}, col {_col}, position {_pos}";
                if (!string.IsNullOrEmpty(_string))
                    msg += $".  The contents of the string buffer were \"{_string}\"";
                throw new Exception(msg);
        }

        error:
        ok:
        var token = new Token
        {
            Kind = kind,
            Line = _line,
            Col = _col,
            Start = _start,
            Length = _length,
            String = _string,
            Int = _int,
            Double = _double
        };
        _count++;
        yield return token;
        if (kind is TokenKind.Error or TokenKind.EOF)
            goto stop;

        goto lex;

        lex_identifier:
        move = false;
        bool checkKeyword = true;
        while (true)
        {
            Store();
            Move();

            if (IsLetter(_char))
                continue;

            if (_char is UNDERSCORE || IsDigit(_char))
            {
                checkKeyword = false;
                continue;
            }

            break;
        }
        ReadString();
        if (checkKeyword)
            if (_keywords.WordToToken.TryGetValue(_string!, out kind))
                ;
            else
                kind = TokenKind.Identifier;
        else
            kind = TokenKind.Identifier;

        goto ok;

        lex_number:
        move = false;

        lex_int:
        Store();
        Move();
        if (IsDigit(_char))
            goto lex_int;
        if (_char is DOT && FollowedByDigit)
            goto lex_float;

        ReadString();
        kind = TokenKind.IntLiteral;
        _int = int.Parse(_string!);
        ClearString();
        goto ok;

        lex_float:
        Store();
        Move();
        if (IsDigit(_char))
            goto lex_float;

        ReadString();
        kind = TokenKind.FloatLiteral;
        _double = double.Parse(_string!);
        ClearString();
        goto ok;

        stop:
        Dispose();
    }

    void Move()
    {
        _char = (char)_sr.Read();
        _pos++;
        _length++;
        _col++;
    }

    char Peek => (char)_sr.Peek();

    (char a, char b) Peek2()
    {
        var a = (char)_sr.Read();
        var b = (char)_sr.Peek();
        _sr.BaseStream.Seek(-1, SeekOrigin.Current);
        //var c = (char)_sr.Peek();
        return (a, b);
    }

    bool FollowedByDigit => IsDigit(Peek);

    bool FollowedByLetter => IsLetter(Peek);

    void Store()
    {
        _sb.Append(_char);
    }

    bool Skip(char c)
    {
        if (Peek != c)
            return false;
        Move();
        return true;
    }

    void Backtrack(long n)
    {
        _sr.BaseStream.Seek(-n, SeekOrigin.Current);
    }

    /// Read the contents of the current string buffer
    private void ReadString()
    {
        _string = _sb.ToString();
        _sb.Clear();
    }

    private void ClearString()
    {
        _string = null;
    }

    public IEnumerator<Token> GetEnumerator()
    {
        var enumerator = Lex();
        return enumerator.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Dispose()
    {
        _sr.Dispose();
    }

    public static IEnumerable<Token> LexString(string s, ILogger? logger = null)
    {
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write(s);
        writer.Flush();
        stream.Position = 0;
        var reader = new StreamReader(stream);
        var lexer = new Lexer(reader);
        return lexer.Lex();
    }

    public static IEnumerable<Token> LexFile(
        FileInfo fi,
        ILogger? logger = null,
        bool includeComments = false
    )
    {
        var stream = fi.OpenText();
        var lexer = new Lexer(stream, includeComments);
        return lexer;
    }

    public static IEnumerable<Token> LexFile(
        string path,
        ILogger? logger = null,
        bool includeComments = false
    )
    {
        var fi = new FileInfo(path);
        var stream = fi.OpenText();
        var lexer = new Lexer(stream, includeComments);
        return lexer;
    }
}
