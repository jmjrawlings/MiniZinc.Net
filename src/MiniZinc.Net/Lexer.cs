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
    ERROR_UNEXPECTED_CHAR,
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

    public override string ToString() =>
        $"{Kind} {String} | Line {Line}, Col {Col}, Start {Start}, End {Start + Length}, Len: {Length}";
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

    private uint _line;
    private uint _col;
    private uint _pos;
    private uint _startPos;
    private uint _startLine;
    private uint _startCol;
    private uint _length;
    private string? _string;
    private char _char;
    private Token _token;
    private bool skipNext;
    private TokenKind _kind;
    private readonly StreamReader _reader;
    private readonly StringBuilder _sb;
    private readonly KeywordLookup Keywords;
    public readonly bool LexLineComments;
    public readonly bool LexBlockComments;

    private Lexer(StreamReader reader, bool lexLineComment = false, bool lexBlockComments = false)
    {
        _reader = reader;
        _sb = new StringBuilder();
        _string = string.Empty;
        _line = 1;
        Keywords = KeywordLookup.Table;
        LexLineComments = lexLineComment;
        LexBlockComments = lexBlockComments;
    }

    public bool MoveNext()
    {
        next:

        if (_kind >= TokenKind.EOF)
            return false;

        if (!skipNext)
            Read();

        skipNext = false;
        if (IsWhiteSpace(_char))
            goto next;

        _startPos = _pos;
        _startLine = _line;
        _startCol = _col;
        _length = 1;
        switch (_char)
        {
            case EOF:
                Token(TokenKind.EOF);
                break;
            case PERCENT:
                if (!LexLineComment())
                    goto next;
                break;
            case FWD_SLASH when Skip(STAR):
                if (!LexBlockComment())
                    goto next;
                break;
            case FWD_SLASH:
                Token(TokenKind.ForwardSlash);
                break;
            case BACK_SLASH:
                Token(TokenKind.BackSlash);
                break;
            case STAR:
                Token(TokenKind.Star);
                break;
            case DELIMITER:
                Token(TokenKind.Delimiter);
                break;
            case EQUAL:
                Skip(EQUAL);
                Token(TokenKind.Equal);
                break;
            case LEFT_CHEVRON:
                switch (Peek2())
                {
                    case (RIGHT_CHEVRON, _):
                        Read();
                        Token(TokenKind.Empty);
                        break;
                    case (DASH, RIGHT_CHEVRON):
                        Read();
                        Read();
                        Token(TokenKind.DoubleArrow);
                        break;
                    case (DASH, _):
                        Read();
                        Token(TokenKind.LeftArrow);
                        break;
                    case (EQUAL, _):
                        Token(TokenKind.LessThanEqual);
                        break;
                    default:
                        Token(TokenKind.LessThan);
                        break;
                }

                break;
            case RIGHT_CHEVRON:
                Token(Skip(EQUAL) ? TokenKind.GreaterThanEqual : TokenKind.GreaterThan);
                break;
            case UP_CHEVRON:
                break;
            case PIPE:
                Token(TokenKind.Pipe);
                break;
            case DOT:
                Token(Skip(DOT) ? TokenKind.DotDot : TokenKind.Dot);
                break;
            case PLUS:
                Token(Skip(PLUS) ? TokenKind.PlusPlus : TokenKind.Plus);
                break;
            case DASH:
                Token(Skip(RIGHT_CHEVRON) ? TokenKind.RightArrow : TokenKind.Minus);
                break;
            case TILDE:
                Read();
                switch (_char)
                {
                    case DASH:
                        Token(TokenKind.TildeMinus);
                        break;
                    case PLUS:
                        Token(TokenKind.TildePlus);
                        break;
                    case STAR:
                        Token(TokenKind.TildeStar);
                        break;
                    case EQUAL:
                        Token(TokenKind.TildeEquals);
                        break;
                    default:
                        Token(TokenKind.Tilde);
                        skipNext = true;
                        break;
                }
                break;
            case LEFT_BRACK:
                Token(TokenKind.LeftBracket);
                break;
            case RIGHT_BRACK:
                Token(TokenKind.RightBracket);
                break;
            case LEFT_PAREN:
                Token(TokenKind.LeftParen);
                break;
            case RIGHT_PAREN:
                Token(TokenKind.RightParen);
                break;
            case LEFT_BRACE:
                Token(TokenKind.LeftBrace);
                break;
            case RIGHT_BRACE:
                Token(TokenKind.RightBrace);
                break;
            case SINGLE_QUOTE:
                LexQuotedIdentifier();
                break;
            case DOUBLE_QUOTE:
                LexStringLiteral();
                break;
            case BACKTICK:
                LexQuotedOperator();
                break;
            case DOLLAR:
                Skip(DOLLAR);
                LexPolymorphicIdentifier();
                break;
            case COLON:
                Token(TokenKind.Colon);
                break;
            case UNDERSCORE when FollowedByLetter:
                LexIdentifier(checkKeyword: false);
                break;
            case UNDERSCORE:
                Token(TokenKind.Underscore);
                break;
            case COMMA:
                Token(TokenKind.Comma);
                break;
            case EXCLAMATION when Skip(EQUAL):
                Token(TokenKind.NotEqual);
                break;
            case EXCLAMATION:
                Error(TokenKind.ERROR_UNEXPECTED_CHAR);
                break;
            default:
                if (IsDigit(_char))
                {
                    LexNumber();
                    break;
                }

                if (IsLetter(_char))
                {
                    LexIdentifier();
                    break;
                }

                ReadString();
                var msg =
                    $"Unexpected character \"{_char}\" found on line {_line}, col {_col}, position {_pos}";
                if (!string.IsNullOrEmpty(_string))
                    msg += $".  The contents of the string buffer were \"{_string}\"";
                throw new Exception(msg);
        }
        return true;
    }

    private void LexPolymorphicIdentifier()
    {
        Read();
        if (!IsLetter(_char))
        {
            Error(TokenKind.ERROR_POLYMORPHIC_IDENTIFIER);
            return;
        }

        skipNext = true;
        Read();
        while (IsAsciiLetterOrDigit(_char))
        {
            Store();
            Read();
        }
        StringToken(TokenKind.Polymorphic);
    }

    private void LexQuotedOperator()
    {
        loop:
        Read();
        if (_char is BACKTICK && _length > 2) { }
        else if (IsAsciiLetterOrDigit(_char))
        {
            Store();
            goto loop;
        }
        else
        {
            Error(TokenKind.ERROR_QUOTED_OPERATOR);
            return;
        }
        StringToken(TokenKind.QuotedOperator);
    }

    private void StringToken(TokenKind kind)
    {
        ReadString();
        _token = new Token(
            _kind = kind,
            _startLine,
            _startCol,
            _startPos,
            skipNext ? _length - 1 : _length,
            0,
            0.0,
            _string
        );
    }

    private void Token(TokenKind kind)
    {
        _token = new Token(
            _kind = kind,
            _startLine,
            _startCol,
            _startPos,
            skipNext ? _length - 1 : _length,
            0,
            0.0,
            null
        );
    }

    private void LexQuotedIdentifier()
    {
        quoted_identifier:
        Read();
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
                Error(TokenKind.ERROR_QUOTED_IDENT);
                return;
            default:
                Store();
                goto quoted_identifier;
        }

        StringToken(TokenKind.QuotedIdentifier);
    }

    private bool LexBlockComment()
    {
        block_comment:
        Read();
        switch (_char)
        {
            case STAR when Skip(FWD_SLASH):
                if (!LexBlockComments)
                    return false;
                break;
            case EOF:
                Error(TokenKind.ERROR_UNTERMINATED_BLOCK_COMMENT);
                return true;
            default:
                if (LexBlockComments)
                    Store();
                goto block_comment;
        }

        if (!LexBlockComments)
            return false;

        StringToken(TokenKind.BlockComment);
        return true;
    }

    private void Error(TokenKind kind)
    {
        StringToken(kind);
    }

    private bool LexLineComment()
    {
        line_comment:
        Read();
        switch (_char)
        {
            case NEWLINE:
            case EOF:
                break;

            default:
                if (LexLineComments)
                    Store();
                goto line_comment;
        }

        if (!LexLineComments)
            return false;

        StringToken(TokenKind.LineComment);
        return true;
    }

    void LexIdentifier(bool checkKeyword = true)
    {
        skipNext = true;

        identifier:
        Store();
        Read();

        if (IsLetter(_char))
            goto identifier;

        if (_char is UNDERSCORE || IsDigit(_char))
        {
            checkKeyword = false;
            goto identifier;
        }

        ReadString();
        if (checkKeyword && Keywords.WordToToken.TryGetValue(_string!, out _kind))
            _string = null;
        else
            _kind = TokenKind.Identifier;

        _token = new Token(_kind, _startLine, _startCol, _startPos, _length - 1, 0, 0.0, _string);
    }

    private void LexStringLiteral()
    {
        bool inExpr = false;
        bool escaped = false;
        Console.WriteLine("STRING");
        string_literal:
        Read();
        Console.Write(_char);
        switch (_char)
        {
            case DOUBLE_QUOTE when escaped:
                break;
            case DOUBLE_QUOTE:
                StringToken(TokenKind.StringLiteral);
                return;
            case RIGHT_PAREN when inExpr:
                inExpr = false;
                break;
            case LEFT_PAREN when escaped && !inExpr:
                inExpr = true;
                escaped = false;
                break;
            case BACK_SLASH:
                escaped = !escaped;
                break;
            case EOF:
                Error(TokenKind.ERROR_UNTERMINATED_STRING_LITERAL);
                return;
            default:
                if (!escaped)
                    break;

                if (_char is 'n' or 't' or SINGLE_QUOTE)
                {
                    escaped = false;
                    break;
                }

                Error(TokenKind.ERROR_ESCAPED_STRING);
                return;
        }

        Store();
        goto string_literal;
    }

    private void LexNumber()
    {
        skipNext = true;

        lex_int:
        Store();
        Read();
        if (IsDigit(_char))
            goto lex_int;
        if (_char is DOT && FollowedByDigit)
            goto lex_float;

        ReadString();

        _token = new Token(
            _kind = TokenKind.IntLiteral,
            _line,
            _col,
            _startPos,
            _length - 1,
            int.Parse(_string!),
            0.0,
            _string = null
        );

        return;

        lex_float:
        Store();
        Read();
        if (IsDigit(_char))
            goto lex_float;

        ReadString();

        _token = new Token(
            _kind = TokenKind.FloatLiteral,
            _line,
            _col,
            _startPos,
            _length - 1,
            0,
            double.Parse(_string!),
            _string = null
        );
    }

    void Read()
    {
        _char = (char)_reader.Read();
        _pos++;
        _length++;
        _col++;
        if (_char is NEWLINE)
        {
            _line++;
            _col = 0;
        }
    }

    char Peek => (char)_reader.Peek();

    (char a, char b) Peek2()
    {
        var a = (char)_reader.Read();
        var b = (char)_reader.Peek();
        _reader.BaseStream.Seek(-1, SeekOrigin.Current);
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
        Read();
        return true;
    }

    /// Read the contents of the current string buffer
    private void ReadString()
    {
        _string = _sb.ToString();
        _sb.Clear();
    }

    public void Dispose()
    {
        _reader.Dispose();
    }

    public static Lexer LexString(
        string s,
        bool lexLineComments = false,
        bool lexBlockComments = false
    )
    {
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
        writer.Write(s);
        writer.Flush();
        stream.Position = 0;
        var reader = new StreamReader(stream);
        var lexer = new Lexer(reader, lexLineComments, lexBlockComments);
        return lexer;
    }

    public static Lexer LexFile(
        FileInfo fi,
        ILogger? logger = null,
        bool lexLineComments = false,
        bool lexBlockComments = false
    )
    {
        var stream = new StreamReader(
            fi.FullName,
            Encoding.UTF8,
            detectEncodingFromByteOrderMarks: true
        );
        var lexer = new Lexer(stream, lexLineComments, lexBlockComments);
        return lexer;
    }

    public static Lexer LexFile(
        string path,
        ILogger? logger = null,
        bool lexLineComments = false,
        bool lexBlockComments = false
    ) => LexFile(new FileInfo(path), logger, lexLineComments, lexBlockComments);

    public static IEnumerable<Token> LexStream(
        StreamReader stream,
        bool lexLineComments = false,
        bool lexBlockComments = false
    ) => new Lexer(stream, lexLineComments, lexBlockComments);

    public void Reset()
    {
        throw new NotImplementedException();
    }

    object IEnumerator.Current => _token;

    Token IEnumerator<Token>.Current => _token;

    public IEnumerator<Token> GetEnumerator() => this;

    IEnumerator IEnumerable.GetEnumerator() => this;
}
