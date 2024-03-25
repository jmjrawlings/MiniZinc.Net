namespace MiniZinc.Parser;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using static Char;

internal sealed class Lexer : IEnumerator<Token>, IEnumerable<Token>
{
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
    const char OPEN_BRACKET = '[';
    const char CLOSE_BRACKET = ']';
    const char OPEN_PAREN = '(';
    const char CLOSE_PAREN = ')';
    const char OPEN_BRACE = '{';
    const char CLOSE_BRACE = '}';
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
    const char RETURN = '\r';
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
    private bool _fin;
    private Token _token;
    private TokenKind _kind;
    private readonly StreamReader _reader;
    private readonly StringBuilder _sb;
    public readonly bool LexLineComments;
    public readonly bool LexBlockComments;

    private Lexer(StreamReader reader, LexOptions options)
    {
        LexLineComments = options.HasFlag(LexOptions.LexLineComments);
        LexBlockComments = options.HasFlag(LexOptions.LexBlockComments);
        _sb = new StringBuilder();
        _string = string.Empty;
        _line = 1;
        _reader = reader;
        Step();
    }

    public bool MoveNext()
    {
        next:
        if (_fin)
            return false;

        _startPos = _pos;
        _startLine = _line;
        _startCol = _col;
        _length = 1;
        switch (_char)
        {
            case PERCENT:
                Step();
                if (!LexLineComment())
                    goto next;
                break;
            case FWD_SLASH:
                Step();
                if (Skip(STAR))
                    if (!LexBlockComment())
                        goto next;
                if (!SkipReturn(BACK_SLASH, TokenKind.UP_WEDGE))
                    Token(TokenKind.FWDSLASH);
                break;
            case BACK_SLASH:
                Step();
                if (!SkipReturn(FWD_SLASH, TokenKind.DOWN_WEDGE))
                    Token(TokenKind.BACKSLASH);
                break;
            case STAR:
                Step();
                Token(TokenKind.STAR);
                break;
            case DELIMITER:
                Step();
                Token(TokenKind.EOL);
                break;
            case EQUAL:
                Step();
                Skip(EQUAL);
                Token(TokenKind.EQUAL);
                break;
            case LEFT_CHEVRON:
                Step();
                if (SkipReturn(RIGHT_CHEVRON, TokenKind.EMPTY))
                    break;
                if (SkipReturn(EQUAL, TokenKind.LESS_THAN_EQUAL))
                    break;
                if (Skip(DASH))
                {
                    if (SkipReturn(RIGHT_CHEVRON, TokenKind.DOUBLE_ARROW))
                        break;
                    Token(TokenKind.LEFT_ARROW);
                    break;
                }
                Token(TokenKind.LESS_THAN);
                break;
            case RIGHT_CHEVRON:
                Step();
                if (SkipReturn(EQUAL, TokenKind.GREATER_THAN_EQUAL))
                    break;
                Token(TokenKind.GREATER_THAN);
                break;
            case UP_CHEVRON:
                Step();
                Token(TokenKind.EXP);
                break;
            case PIPE:
                Step();
                Token(TokenKind.PIPE);
                break;
            case DOT:
                Step();
                if (SkipReturn(DOT, TokenKind.DOT_DOT))
                    break;
                Token(TokenKind.DOT);
                break;
            case PLUS:
                Step();
                if (SkipReturn(PLUS, TokenKind.PLUS_PLUS))
                    break;
                Token(TokenKind.PLUS);
                break;
            case DASH:
                Step();
                if (SkipReturn(RIGHT_CHEVRON, TokenKind.RIGHT_ARROW))
                    break;
                Token(TokenKind.MINUS);
                break;
            case TILDE:
                Step();
                switch (_char)
                {
                    case DASH:
                        Step();
                        Token(TokenKind.TILDE_MINUS);
                        break;
                    case PLUS:
                        Step();
                        Token(TokenKind.TILDE_PLUS);
                        break;
                    case STAR:
                        Step();
                        Token(TokenKind.TILDE_STAR);
                        break;
                    case EQUAL:
                        Step();
                        Token(TokenKind.TILDE_EQUALS);
                        break;
                    default:
                        Token(TokenKind.TILDE);
                        break;
                }
                break;
            case OPEN_BRACKET:
                Step();
                Token(TokenKind.OPEN_BRACKET);
                break;
            case CLOSE_BRACKET:
                Step();
                Token(TokenKind.CLOSE_BRACKET);
                break;
            case OPEN_PAREN:
                Step();
                Token(TokenKind.OPEN_PAREN);
                break;
            case CLOSE_PAREN:
                Step();
                Token(TokenKind.CLOSE_PAREN);
                break;
            case OPEN_BRACE:
                Step();
                Token(TokenKind.OPEN_BRACE);
                break;
            case CLOSE_BRACE:
                Step();
                Token(TokenKind.CLOSE_BRACE);
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
                Step();
                Skip(DOLLAR);
                LexPolymorphicIdentifier();
                break;
            case COLON:
                Step();
                if (SkipReturn(COLON, TokenKind.COLON_COLON))
                    break;
                Token(TokenKind.COLON);
                break;
            case UNDERSCORE when FollowedByLetter:
                LexIdentifier(checkKeyword: false);
                break;
            case UNDERSCORE:
                Step();
                Token(TokenKind.UNDERSCORE);
                break;
            case COMMA:
                Step();
                Token(TokenKind.COMMA);
                break;
            case EXCLAMATION:
                Step();
                if (SkipReturn(EQUAL, TokenKind.NOT_EQUAL))
                    break;
                Error(TokenKind.ERROR_UNEXPECTED_CHAR);
                break;
            case '\t':
            case '\n':
            case '\r':
            case ' ':
                Step();
                goto next;
            default:
                if (LexNumber())
                    break;

                LexIdentifier();
                break;
        }
        return true;
    }

    private void LexPolymorphicIdentifier()
    {
        Step();
        if (!IsLetter(_char))
        {
            Error(TokenKind.ERROR_POLYMORPHIC_IDENTIFIER);
            return;
        }

        while (IsAsciiLetterOrDigit(_char))
        {
            Store();
            Step();
        }

        StringToken(TokenKind.POLYMORPHIC);
    }

    private void LexQuotedOperator()
    {
        loop:
        Step();
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
        StringToken(TokenKind.QUOTED_OP);
    }

    private void StringToken(TokenKind kind)
    {
        ReadString();
        _token = new Token(_kind = kind, _startLine, _startCol, _startPos, _length - 1, s: _string);
        _length = 1;
    }

    private void Token(TokenKind kind)
    {
        _token = new Token(_kind = kind, _startLine, _startCol, _startPos, _length - 1);
        _length = 1;
    }

    private bool SkipReturn(char c, TokenKind kind)
    {
        if (_char != c)
            return false;

        Step();
        Token(kind);
        return true;
    }

    private bool SkipReturn(TokenKind kind)
    {
        Step();
        Token(kind);
        return true;
    }

    private void LexQuotedIdentifier()
    {
        quoted_identifier:
        Step();
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

        StringToken(TokenKind.QUOTED_IDENT);
    }

    private bool LexBlockComment()
    {
        while (true)
        {
            if (Skip(STAR))
            {
                if (Skip(FWD_SLASH))
                    break;
            }
            else if (_char is EOF)
            {
                return Error(TokenKind.ERROR_UNTERMINATED_BLOCK_COMMENT);
            }

            if (LexBlockComments)
                Store();
            Step();
        }

        if (!LexBlockComments)
            return false;

        StringToken(TokenKind.BLOCK_COMMENT);
        return true;
    }

    private bool Error(TokenKind kind)
    {
        StringToken(kind);
        return false;
    }

    private bool LexLineComment()
    {
        line_comment:
        Step();

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

        StringToken(TokenKind.LINE_COMMENT);
        return true;
    }

    void LexIdentifier(bool checkKeyword = true)
    {
        while (true)
        {
            Store();
            Step();
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
        if (checkKeyword && Keyword.Lookup.TryGetValue(_string!, out _kind))
            _string = null;
        else
            _kind = TokenKind.IDENT;

        _token = new Token(_kind, _startLine, _startCol, _startPos, _length - 1, s: _string);
        _length = 1;
    }

    private bool LexStringLiteral()
    {
        if (_char is not DOUBLE_QUOTE)
            return false;

#if DEBUG
        var sb = new StringBuilder();
        sb.Append(_char);
#endif

        bool inExpr = false;
        bool escaped = false;
        string_literal:
        Step();

#if DEBUG
        sb.Append(_char);
#endif

        switch (_char)
        {
            case DOUBLE_QUOTE when escaped:
                escaped = false;
                break;
            case DOUBLE_QUOTE:
                Step();
                StringToken(TokenKind.STRING_LIT);
                return true;
            case CLOSE_PAREN when inExpr:
                inExpr = false;
                break;
            case OPEN_PAREN when escaped && !inExpr:
                inExpr = true;
                escaped = false;
                break;
            case BACK_SLASH:
                escaped = !escaped;
                break;
            case EOF:
                return Error(TokenKind.ERROR_UNTERMINATED_STRING_LITERAL);
            default:
                if (!escaped)
                    break;

                if (_char is 'n' or 't' or SINGLE_QUOTE)
                {
                    escaped = false;
                    break;
                }

#if DEBUG
                var s = _sb.ToString();
#endif

                return Error(TokenKind.ERROR_ESCAPED_STRING);
        }

        Store();
        goto string_literal;
    }

    private bool LexNumber()
    {
        if (!IsDigit(_char))
            return false;
        do
        {
            Store();
            Step();
        } while (IsDigit(_char));

        if (_char is DOT && FollowedByDigit)
        {
            Store();
            Step();
            goto lex_float;
        }

        ReadString();
        _token = new Token(
            _kind = TokenKind.INT_LIT,
            _startLine,
            _startCol,
            _startPos,
            _length - 1,
            i: int.Parse(_string!)
        );
        _length = 1;
        return true;

        lex_float:
        while (IsDigit(_char))
        {
            Store();
            Step();
        }
        ReadString();

        _token = new Token(
            _kind = TokenKind.FLOAT_LIT,
            _startLine,
            _startCol,
            _startPos,
            _length - 1,
            d: double.Parse(_string!)
        );
        _length = 1;
        return true;
    }

    void Step()
    {
        if (_fin)
            return;

        _char = (char)_reader.Read();
        if (_char is EOF)
        {
            _fin = true;
            return;
        }

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
        if (_char == c)
        {
            Step();
            return true;
        }
        return false;
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

    /// <summary>
    /// Lex the given string
    /// </summary>
    public static Lexer LexString(string s, LexOptions options = default)
    {
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream, Encoding.UTF8);
        writer.Write(s);
        writer.Flush();
        stream.Position = 0;
        var reader = new StreamReader(stream);
        var lexer = new Lexer(reader, options);
        return lexer;
    }

    /// <summary>
    /// Lex the given file
    /// </summary>
    public static Lexer LexFile(string path, LexOptions options = default)
    {
        Encoding encoding;
        using (var reader = new StreamReader(path, Encoding.UTF8, true))
        {
            reader.Peek(); // you need this!
            encoding = reader.CurrentEncoding;
        }

        Console.WriteLine("Encoding for {0} is {1}", path, encoding);
        var s = File.ReadAllText(path, encoding);
        var lexer = LexString(s, options);
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
