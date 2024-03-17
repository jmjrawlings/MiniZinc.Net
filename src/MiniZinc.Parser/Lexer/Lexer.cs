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
    private Token _token;
    private bool skipNext;
    private TokenKind _kind;
    private readonly StreamReader _reader;
    private readonly StringBuilder _sb;
    public readonly LexOptions Options;
    public readonly bool LexLineComments;
    public readonly bool LexBlockComments;

    private Lexer(StreamReader reader, LexOptions options)
    {
        _reader = reader;
        _sb = new StringBuilder();
        _string = string.Empty;
        _line = 1;
        Options = options;
        LexLineComments = options.HasFlag(LexOptions.LexLineComments);
        LexBlockComments = options.HasFlag(LexOptions.LexBlockComments);
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
                Token(TokenKind.FWDSLASH);
                break;
            case BACK_SLASH:
                Token(TokenKind.BACKSLASH);
                break;
            case STAR:
                Token(TokenKind.STAR);
                break;
            case DELIMITER:
                Token(TokenKind.EOL);
                break;
            case EQUAL:
                Skip(EQUAL);
                Token(TokenKind.EQUAL);
                break;
            case LEFT_CHEVRON:
                switch (Peek2())
                {
                    case (RIGHT_CHEVRON, _):
                        Read();
                        Token(TokenKind.EMPTY);
                        break;
                    case (DASH, RIGHT_CHEVRON):
                        Read();
                        Read();
                        Token(TokenKind.DOUBLE_ARROW);
                        break;
                    case (DASH, _):
                        Read();
                        Token(TokenKind.LEFT_ARROW);
                        break;
                    case (EQUAL, _):
                        Token(TokenKind.LESS_THAN_EQUAL);
                        break;
                    default:
                        Token(TokenKind.LESS_THAN);
                        break;
                }

                break;
            case RIGHT_CHEVRON:
                Token(Skip(EQUAL) ? TokenKind.GREATER_THAN_EQUAL : TokenKind.GREATER_THAN);
                break;
            case UP_CHEVRON:
                Token(TokenKind.EXP);
                break;
            case PIPE:
                Token(TokenKind.PIPE);
                break;
            case DOT:
                Token(Skip(DOT) ? TokenKind.DOT_DOT : TokenKind.DOT);
                break;
            case PLUS:
                Token(Skip(PLUS) ? TokenKind.PLUS_PLUS : TokenKind.PLUS);
                break;
            case DASH:
                Token(Skip(RIGHT_CHEVRON) ? TokenKind.RIGHT_ARROW : TokenKind.MINUS);
                break;
            case TILDE:
                Read();
                switch (_char)
                {
                    case DASH:
                        Token(TokenKind.TILDE_MINUS);
                        break;
                    case PLUS:
                        Token(TokenKind.TILDE_PLUS);
                        break;
                    case STAR:
                        Token(TokenKind.TILDE_STAR);
                        break;
                    case EQUAL:
                        Token(TokenKind.TILDE_EQUALS);
                        break;
                    default:
                        Token(TokenKind.TILDE);
                        skipNext = true;
                        break;
                }
                break;
            case LEFT_BRACK:
                Token(TokenKind.OPEN_BRACKET);
                break;
            case RIGHT_BRACK:
                Token(TokenKind.CLOSE_BRACKET);
                break;
            case LEFT_PAREN:
                Token(TokenKind.OPEN_PAREN);
                break;
            case RIGHT_PAREN:
                Token(TokenKind.CLOSE_PAREN);
                break;
            case LEFT_BRACE:
                Token(TokenKind.OPEN_BRACE);
                break;
            case RIGHT_BRACE:
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
                Skip(DOLLAR);
                LexPolymorphicIdentifier();
                break;
            case COLON:
                if (Skip(COLON))
                    Token(TokenKind.COLON_COLON);
                else
                    Token(TokenKind.COLON);
                break;
            case UNDERSCORE when FollowedByLetter:
                LexIdentifier(checkKeyword: false);
                break;
            case UNDERSCORE:
                Token(TokenKind.UNDERSCORE);
                break;
            case COMMA:
                Token(TokenKind.COMMA);
                break;
            case EXCLAMATION when Skip(EQUAL):
                Token(TokenKind.NOT_EQUAL);
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
        StringToken(TokenKind.POLYMORPHIC);
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
        StringToken(TokenKind.QUOTED_OP);
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
            s: _string
        );
    }

    private void Token(TokenKind kind)
    {
        _token = new Token(
            _kind = kind,
            _startLine,
            _startCol,
            _startPos,
            skipNext ? _length - 1 : _length
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

        StringToken(TokenKind.QUOTED_IDENT);
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

        StringToken(TokenKind.BLOCK_COMMENT);
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

        StringToken(TokenKind.LINE_COMMENT);
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
        if (checkKeyword && Keyword.Lookup.TryGetValue(_string!, out _kind))
            _string = null;
        else
            _kind = TokenKind.IDENT;

        _token = new Token(_kind, _startLine, _startCol, _startPos, _length - 1, s: _string);
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
                StringToken(TokenKind.STRING_LIT);
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
            _kind = TokenKind.INT_LIT,
            _line,
            _col,
            _startPos,
            _length - 1,
            i: int.Parse(_string!)
        );

        return;

        lex_float:
        Store();
        Read();
        if (IsDigit(_char))
            goto lex_float;

        ReadString();

        _token = new Token(
            _kind = TokenKind.FLOAT_LIT,
            _line,
            _col,
            _startPos,
            _length - 1,
            d: double.Parse(_string!)
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

    /// <summary>
    /// Lex the given string
    /// </summary>
    public static Lexer LexString(string s, LexOptions options = default)
    {
        var stream = new MemoryStream();
        var writer = new StreamWriter(stream);
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
    public static Lexer LexFile(
        string path,
        Encoding? encoding = null,
        LexOptions options = default
    )
    {
        var stream = new StreamReader(
            path,
            encoding ?? Encoding.UTF8,
            detectEncodingFromByteOrderMarks: true
        );
        var lexer = new Lexer(stream, options);
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
