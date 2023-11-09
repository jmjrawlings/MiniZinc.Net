using System.Diagnostics;
using System.Runtime.InteropServices.JavaScript;

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

    // Nodes
    Identifier,
    Polymorphic,
    IntLiteral,
    FloatLiteral,
    StringLiteral,
    LineComment,
    BlockComment,
    QuotedIdentifier,
    QuotedOperator,

    // Keywords
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

    // Binary Ops
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
    ERROR,
    ERROR_QUOTED_IDENT,
    ERROR_QUOTED_OPERATOR,
    ERROR_ESCAPED_STRING,
    ERROR_UNTERMINATED_STRING_LITERAL,
    ERROR_UNTERMINATED_BLOCK_COMMENT,
    ERROR_UNTERMINATED_STRING_EXPRESSION,
    ERROR_POLYMORPHIC_IDENTIFIER
}

public readonly struct Token
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
        int i,
        double d,
        string? s
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

public sealed class Lexer : IEnumerator<Token>, IEnumerable<Token>
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

    // Start line of current token
    uint _line;

    // Start column of current token
    uint _col;

    // Position of the stream
    uint _pos;

    // Start index of current token
    uint _start;

    // Length of the current token
    uint _length;

    // String contents of current token
    private string? _string;

    // Char at the current stream position
    private char _char;

    // Current token
    private Token _token;

    private bool skipNext;

    private TokenKind _kind;

    private readonly StreamReader _sr;
    private readonly StringBuilder _sb;
    private readonly KeywordLookup _keywords;

    // Return comments as tokens?
    public readonly bool IncludeComments;

    private Lexer(StreamReader sr, bool includeComment = false)
    {
        _sr = sr;
        _sb = new StringBuilder();
        _string = string.Empty;
        _keywords = KeywordLookup.Table;
        IncludeComments = includeComment;
    }

    public bool MoveNext()
    {
        if (!skipNext)
            Move();
        skipNext = false;
        _start = _pos;
        _length = 0;
        _string = null;
        switch (_char)
        {
            case EOF:
                _kind = TokenKind.EOF;
                var x = _sr.EndOfStream;
                break;
            case TAB:
            case RETURN:
            case SPACE:
                return MoveNext();
            case NEWLINE:
                _line++;
                _col = 0;
                return MoveNext();
            case PERCENT:
                return LexLineComment();
            case FWD_SLASH when Skip(STAR):
                return LexBlockComment();
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
                break;
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
            case SINGLE_QUOTE:
                return LexQuotedIdentifier();
            case DOUBLE_QUOTE:
                return LexStringLiteral();
            case BACKTICK:
                return LexQuotedOperator();
            case DOLLAR:
                return LexPolymorphicIdentifier();
            case COLON:
                _kind = TokenKind.Colon;
                break;
            case UNDERSCORE:
                if (FollowedByLetter)
                    return LexIdentifier(checkKeyword: false);
                _kind = TokenKind.Underscore;
                break;
            case COMMA:
                _kind = TokenKind.Comma;
                break;
            case EXCLAMATION:
                _kind = Skip(EQUAL) ? TokenKind.NotEqual : TokenKind.Equal;
                break;
            default:
                if (IsDigit(_char))
                    return LexNumber();

                if (IsLetter(_char))
                    return LexIdentifier();

                if (_sr.EndOfStream)
                    return false;

                ReadString();
                var msg =
                    $"Unexpected character \"{_char}\" found on line {_line}, col {_col}, position {_pos}";
                if (!string.IsNullOrEmpty(_string))
                    msg += $".  The contents of the string buffer were \"{_string}\"";
                throw new Exception(msg);
        }

        _token = new Token(_kind, _line, _col, _start, _length, 0, 0.0, _string);
        if (_kind is TokenKind.EOF)
            return false;

        return true;
    }

    private bool LexPolymorphicIdentifier()
    {
        Move();
        if (!IsLetter(_char))
            return Error(TokenKind.ERROR_POLYMORPHIC_IDENTIFIER);

        skipNext = true;
        Move();
        while (IsAsciiLetterOrDigit(_char))
        {
            Store();
            Move();
        }
        StringToken(TokenKind.Polymorphic);
        return true;
    }

    private bool LexQuotedOperator()
    {
        loop:
        Move();
        if (_char is BACKTICK && _length > 2)
            ;
        else if (IsAsciiLetterOrDigit(_char))
        {
            Store();
            goto loop;
        }
        else
        {
            return Error(TokenKind.ERROR_QUOTED_OPERATOR);
        }
        StringToken(TokenKind.QuotedOperator);
        return true;
    }

    private void StringToken(TokenKind kind)
    {
        ReadString();
        _token = new Token(
            _kind = kind,
            _line,
            _col,
            _start,
            skipNext ? _length - 1 : _length,
            0,
            0.0,
            _string
        );
    }

    private bool LexQuotedIdentifier()
    {
        quoted_identifier:
        Move();
        switch (_char)
        {
            case SINGLE_QUOTE when _length > 2:
                ReadString();
                break;
            case SINGLE_QUOTE:
            case BACK_SLASH:
            case RETURN:
            case NEWLINE:
            case EOF:
                return Error(TokenKind.ERROR_QUOTED_IDENT);
            default:
                Store();
                goto quoted_identifier;
        }

        StringToken(TokenKind.QuotedIdentifier);
        return true;
    }

    private bool LexBlockComment()
    {
        block_comment:
        Move();
        switch (_char)
        {
            case STAR when Skip(FWD_SLASH):
                if (!IncludeComments)
                    return true;
                ReadString();
                break;
            case EOF:
                return Error(TokenKind.ERROR_UNTERMINATED_BLOCK_COMMENT);
            default:
                if (IncludeComments)
                    Store();
                goto block_comment;
        }

        if (!IncludeComments)
            return MoveNext();

        StringToken(TokenKind.BlockComment);
        return true;
    }

    private bool Error(TokenKind kind)
    {
        StringToken(kind);
        return false;
    }

    private bool LexLineComment()
    {
        skipNext = true;
        line_comment:
        switch (_char)
        {
            case NEWLINE:
            case EOF:
                break;

            default:
                if (IncludeComments)
                    Store();
                goto line_comment;
        }

        if (!IncludeComments)
            return MoveNext();

        StringToken(TokenKind.LineComment);
        return true;
    }

    bool LexIdentifier(bool checkKeyword = true)
    {
        skipNext = true;

        identifier:
        Store();
        Move();

        if (IsLetter(_char))
            goto identifier;

        if (_char is UNDERSCORE || IsDigit(_char))
        {
            checkKeyword = false;
            goto identifier;
        }

        ReadString();
        _kind = TokenKind.Identifier;
        if (checkKeyword)
            _keywords.WordToToken.TryGetValue(_string!, out _kind);

        _token = new Token(_kind, _line, _col, _start, _length - 1, 0, 0.0, _string);
        return true;
    }

    private bool LexStringLiteral()
    {
        bool inExpr = false;
        Debug.Write("\n\n============================\n\n");
        Debug.Write(_char);

        double_quote:
        Move();
        Debug.Write(_char);
        switch (_char)
        {
            case DOUBLE_QUOTE when !inExpr:
                break;
            case DOUBLE_QUOTE:
                return Error(TokenKind.ERROR_UNTERMINATED_STRING_EXPRESSION);
            case RIGHT_PAREN when inExpr:
                inExpr = false;
                goto double_quote;
            case BACK_SLASH:
                Store();
                Move();
                Store();
                switch (_char)
                {
                    // Properly escaped chars
                    case BACK_SLASH:
                    case DOUBLE_QUOTE:
                    case SINGLE_QUOTE:
                    case 'n':
                    case 't':
                        goto double_quote;

                    case LEFT_PAREN when !inExpr:
                        inExpr = true;
                        goto double_quote;
                }
                return Error(TokenKind.ERROR_ESCAPED_STRING);
            case EOF:
                return Error(TokenKind.ERROR_UNTERMINATED_STRING_LITERAL);
            default:
                Store();
                goto double_quote;
        }
        StringToken(TokenKind.StringLiteral);
        return true;
    }

    bool LexNumber()
    {
        skipNext = true;

        lex_int:
        Store();
        Move();
        if (IsDigit(_char))
            goto lex_int;
        if (_char is DOT && FollowedByDigit)
            goto lex_float;

        ReadString();
        _kind = TokenKind.IntLiteral;
        _token = new Token(_kind, _line, _col, _start, _length, int.Parse(_string!), 0.0, null);
        return true;

        lex_float:
        Store();
        Move();
        if (IsDigit(_char))
            goto lex_float;

        ReadString();
        _kind = TokenKind.FloatLiteral;
        _token = new Token(_kind, _line, _col, _start, _length, 0, double.Parse(_string!), null);
        return true;
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
        return lexer;
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

    public void Reset()
    {
        throw new NotImplementedException();
    }

    object IEnumerator.Current => _token;

    Token IEnumerator<Token>.Current => _token;

    public IEnumerator<Token> GetEnumerator() => this;

    IEnumerator IEnumerable.GetEnumerator() => this;
}
