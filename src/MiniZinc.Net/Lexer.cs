#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace MiniZinc.Net;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.Extensions.Logging;

public enum TokenKind
{
    None,
    Error,
    Identifier,
    IntLiteral,
    FloatLiteral,
    StringLiteral,
    LineComment,
    BlockComment,
    QuotedIdentifier,
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
    EOF
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

    const string ERR_QUOTED_IDENT = "Invalid quoted identifier";
    const string ERR_ESCAPED_STRING = "String was not escaped properly";
    const string ERR_UNTERMINATED_LITERAL =
        "Literal was not terminated properly at the end of the stream";

    uint _line; // Start line of current token
    uint _col; // Start column of current token
    uint _pos; // Position of the stream
    uint _start; // Start index of current token
    uint _length; // Length of the current token
    uint _count; // Total numbef of tokens processed
    TokenKind _kind; // Kind of current token
    string? _string; // String contents of current token
    char _char; // Char at the current stream position
    int? _int;
    double? _double;
    readonly StreamReader _sr;
    readonly StringBuilder _sb;
    readonly ILogger? _logger;
    readonly KeywordLookup _lookup;
    public readonly bool IncludeComments;

    private Lexer(StreamReader sr, ILogger? logger = null, bool includeComment = false)
    {
        _sr = sr;
        _sb = new StringBuilder();
        _logger = logger;
        _string = string.Empty;
        _lookup = KeywordLookup.Table;
        IncludeComments = includeComment;
    }

    void Move()
    {
        _char = (char)_sr.Read();
        _pos++;
        _length++;
        _col++;
        _logger?.LogInformation("{index} {char}", _pos, _char);
    }

    char Peek => (char)_sr.Peek();

    (char a, char b) Peek2()
    {
        var a = (char)_sr.Read();
        var b = (char)_sr.Peek();
        _sr.BaseStream.Seek(-1, SeekOrigin.Current);
        var c = (char)_sr.Peek();
        return (a, b);
    }

    bool FollowedByDigit => char.IsDigit(Peek);

    bool FollowedByLetter => char.IsLetter(Peek);

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

    void Error(string msg)
    {
        _logger?.LogError("{name}", msg);
        _kind = TokenKind.Error;
        _string = msg;
    }

    /// <summary>
    /// Yields tokens until either the end of stream is reached
    /// or an error is encountered
    /// </summary>
    public IEnumerable<Token> Lex()
    {
        bool _move = true;

        // Lex the next token
        lex:
        if (_move)
            Move();
        _move = true;
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
                _kind = TokenKind.EOF;
                goto ok;
            // Line comment
            case PERCENT:
                _kind = TokenKind.LineComment;
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
                _kind = TokenKind.BlockComment;
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
                        Error(ERR_UNTERMINATED_LITERAL);
                        goto error;
                    default:
                        if (IncludeComments)
                            Store();
                        goto block_comment;
                }

            case FWD_SLASH:
                _kind = TokenKind.ForwardSlash;
                break;
            case BACK_SLASH:
                _kind = TokenKind.BackSlash;
                break;
            case STAR:
                _kind = TokenKind.Star;
                break;
            case DELIMITER:
                _kind = TokenKind.Delimiter;
                goto ok;
            case EQUAL:
                Skip(EQUAL);
                _kind = TokenKind.Equal;
                break;
            case LEFT_CHEVRON:
                switch (Peek2())
                {
                    case (RIGHT_CHEVRON, _):
                        Move();
                        _kind = TokenKind.Empty;
                        break;
                    case (DASH, RIGHT_CHEVRON):
                        Move();
                        Move();
                        _kind = TokenKind.DoubleArrow;
                        break;
                    case (DASH, _):
                        Move();
                        _kind = TokenKind.LeftArrow;
                        break;
                    case (EQUAL, _):
                        _kind = TokenKind.LessThanEqual;
                        break;
                    default:
                        _kind = TokenKind.LessThan;
                        break;
                }

                break;
            case RIGHT_CHEVRON:
                _kind = Skip(EQUAL) ? TokenKind.GreaterThanEqual : TokenKind.GreaterThan;
                break;
            case UP_CHEVRON:
                break;
            case PIPE:
                _kind = TokenKind.Pipe;
                break;
            case DOT:
                _kind = Skip(DOT) ? TokenKind.DotDot : TokenKind.Dot;
                break;
            case PLUS:
                _kind = Skip(PLUS) ? TokenKind.PlusPlus : TokenKind.Plus;
                break;
            case DASH:
                _kind = Skip(RIGHT_CHEVRON) ? TokenKind.RightArrow : TokenKind.Minus;
                break;
            case TILDE:
                switch (Peek)
                {
                    case DASH:
                        _kind = TokenKind.TildeMinus;
                        Move();
                        break;
                    case PLUS:
                        _kind = TokenKind.TildePlus;
                        Move();
                        break;
                    case STAR:
                        _kind = TokenKind.TildeStar;
                        Move();
                        break;
                    case EQUAL:
                        _kind = TokenKind.TildeEquals;
                        Move();
                        break;
                    default:
                        _kind = TokenKind.Tilde;
                        break;
                }
                break;
            case LEFT_BRACK:
                _kind = TokenKind.LeftBracket;
                break;
            case RIGHT_BRACK:
                _kind = TokenKind.RightBracket;
                break;
            case LEFT_PAREN:
                _kind = TokenKind.LeftParen;
                break;
            case RIGHT_PAREN:
                _kind = TokenKind.RightParen;
                break;
            case LEFT_BRACE:
                _kind = TokenKind.LeftBrace;
                break;
            case RIGHT_BRACE:
                _kind = TokenKind.RightBrace;
                break;
            // Quoted identifier
            case SINGLE_QUOTE:
                _kind = TokenKind.QuotedIdentifier;
                loop:
                Move();
                switch (_char)
                {
                    case SINGLE_QUOTE when _length > 2:
                        goto ok;
                    case SINGLE_QUOTE:
                    case BACK_SLASH:
                    case RETURN:
                    case NEWLINE:
                    case EOF:
                        Error(ERR_QUOTED_IDENT);
                        goto error;
                    default:
                        Store();
                        goto loop;
                }
            // String literal
            case DOUBLE_QUOTE:
                _kind = TokenKind.StringLiteral;
                while (true)
                {
                    Move();
                    switch (_char)
                    {
                        case DOUBLE_QUOTE:
                            ReadString();
                            goto ok;
                        case BACK_SLASH:
                            Store();
                            Move();
                            Store();
                            if (_char is BACK_SLASH or DOUBLE_QUOTE or SINGLE_QUOTE)
                                continue;
                            Error(ERR_ESCAPED_STRING);
                            goto error;
                        case EOF:
                            Error(ERR_UNTERMINATED_LITERAL);
                            goto error;
                        default:
                            Store();
                            break;
                    }
                }
            case COLON:
                _kind = TokenKind.Colon;
                break;
            case UNDERSCORE:
                if (FollowedByLetter)
                    goto lex_identifier;

                _kind = TokenKind.Underscore;
                break;
            case COMMA:
                _kind = TokenKind.Comma;
                break;
            case EXCLAMATION:
                _kind = Skip(EQUAL) ? TokenKind.NotEqual : TokenKind.Equal;
                break;
            default:
                if (char.IsDigit(_char))
                    goto lex_number;

                if (char.IsLetter(_char))
                    goto lex_identifier;

                if (_sr.EndOfStream)
                    goto stop;

                throw new Exception($"Unhandled {_char}");
        }

        error:
        ok:
        var token = new Token
        {
            Kind = _kind,
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
        if (_kind is TokenKind.Error or TokenKind.EOF)
            goto stop;

        goto lex;

        lex_identifier:
        _move = false;
        bool checkKeyword = true;
        while (true)
        {
            Store();
            Move();

            if (char.IsLetter(_char))
                continue;

            if (_char is UNDERSCORE || char.IsDigit(_char))
            {
                checkKeyword = false;
                continue;
            }

            break;
        }
        ReadString();
        if (checkKeyword)
            if (_lookup.WordToToken.TryGetValue(_string!, out _kind))
                ;
            else
                _kind = TokenKind.Identifier;
        else
            _kind = TokenKind.Identifier;

        goto ok;

        lex_number:
        _move = false;

        lex_int:
        Store();
        Move();
        if (char.IsDigit(_char))
            goto lex_int;
        if (_char is DOT && FollowedByDigit)
            goto lex_float;

        ReadString();
        _kind = TokenKind.IntLiteral;
        _int = int.Parse(_string!);
        ClearString();
        goto ok;

        lex_float:
        Store();
        Move();
        if (char.IsDigit(_char))
            goto lex_float;

        ReadString();
        _kind = TokenKind.FloatLiteral;
        _double = double.Parse(_string!);
        ClearString();
        goto ok;

        stop:
        Dispose();
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
        _logger?.LogInformation("Disposing lexer");
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
        var lexer = new Lexer(reader, logger);
        return lexer.Lex();
    }

    public static IEnumerable<Token> LexFile(
        FileInfo fi,
        ILogger? logger = null,
        bool includeComments = false
    )
    {
        var stream = fi.OpenText();
        var lexer = new Lexer(stream, logger, includeComments);
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
        var lexer = new Lexer(stream, logger, includeComments);
        return lexer;
    }
}
