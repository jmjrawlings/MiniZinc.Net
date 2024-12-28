namespace MiniZinc.Parser;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using static Char;
using static TokenKind;

sealed class Lexer : IEnumerator<Token>, IEnumerable<Token>
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
    const char TAB = '\t';
    const char SPACE = ' ';
    const char EOF = '\uffff';

    const string ERROR_QUOTED_IDENTIFIER = "Error quoted identifier";
    const string ERROR_UNEXPECTED_CHAR = "Error unexpected char";
    const string ERROR_BACKTICK_IDENTIFIER = "Error backtick identifier";
    const string ERROR_POLYMORPHIC_IDENTIFIER = "Error polymorphic identifier";
    const string ERROR_UNTERMINATED_STRING_LITERAL = "Error unterminated string literal";
    const string ERROR_ESCAPED_STRING = "Error escaped string";
    const string ERROR_INT_LITERAL = "Error int literal";
    const string ERROR_UNTERMINATED_BLOCK_COMMENT = "Unterminated block comment";

    private string _sourceText;
    private int _n;
    private int _line;
    private int _col;
    private int _index;
    private int _startPos;
    private int _startLine;
    private int _startCol;
    private int _length;
    private bool _fin;
    private char _char;
    private Token _token;
    private TokenKind _kind;
    private int _startString;

    private Lexer(string sourceText)
    {
        _line = 1;
        _startString = 0;
        _index = -1;
        _sourceText = sourceText;
        _n = sourceText.Length - 1;
        Step();
    }

    public bool MoveNext()
    {
        next:
        if (_fin)
            return false;

        _startPos = _index;
        _startLine = _line;
        _startCol = _col;
        _length = 1;
        switch (_char)
        {
            case PERCENT:
                LexLineComment();
                break;
            case FWD_SLASH:
                Step();
                if (Skip(STAR))
                    LexBlockComment();
                else if (!SkipReturn(BACK_SLASH, TOKEN_CONJUNCTION))
                    Return(TOKEN_DIVIDE);
                break;
            case BACK_SLASH:
                Step();
                if (!SkipReturn(FWD_SLASH, TOKEN_DISJUNCTION))
                    Return(TOKEN_BACKSLASH);
                break;
            case STAR:
                SkipReturn(TOKEN_TIMES);
                break;
            case DELIMITER:
                SkipReturn(TOKEN_EOL);
                break;
            case EQUAL:
                Step();
                Skip(EQUAL);
                Return(TOKEN_EQUAL);
                break;
            case LEFT_CHEVRON:
                Step();
                if (SkipReturn(RIGHT_CHEVRON, TOKEN_EMPTY))
                    break;
                if (SkipReturn(EQUAL, TOKEN_LESS_THAN_EQUAL))
                    break;
                if (Skip(DASH))
                {
                    if (SkipReturn(RIGHT_CHEVRON, TOKEN_BI_IMPLICATION))
                        break;
                    Return(TOKEN_REVERSE_IMPLICATION);
                    break;
                }

                Return(TOKEN_LESS_THAN);
                break;
            case RIGHT_CHEVRON:
                Step();
                if (SkipReturn(EQUAL, TOKEN_GREATER_THAN_EQUAL))
                    break;
                Return(TOKEN_GREATER_THAN);
                break;
            case UP_CHEVRON:
                SkipReturn(TOKEN_EXPONENT);
                break;
            case PIPE:
                SkipReturn(TOKEN_PIPE);
                break;
            case DOT:
                Step();
                if (SkipReturn(DOT, TOKEN_CLOSED_RANGE)) { }
                else if (IsDigit(_char))
                    LexTupleAccess();
                else
                    LexRecordAccess();

                break;
            case PLUS:
                Step();
                if (SkipReturn(PLUS, TOKEN_PLUS_PLUS))
                    break;
                Return(TOKEN_PLUS);
                break;
            case DASH:
                Step();
                if (SkipReturn(RIGHT_CHEVRON, TOKEN_FORWARD_IMPLICATION))
                    break;
                Return(TOKEN_MINUS);
                break;
            case TILDE:
                Step();
                switch (_char)
                {
                    case DASH:
                        SkipReturn(TOKEN_TILDE_MINUS);
                        break;
                    case PLUS:
                        SkipReturn(TOKEN_TILDE_PLUS);
                        break;
                    case STAR:
                        SkipReturn(TOKEN_TILDE_TIMES);
                        break;
                    case EQUAL:
                        SkipReturn(TOKEN_TILDE_EQUALS);
                        break;
                    default:
                        Return(TOKEN_TILDE);
                        break;
                }

                break;
            case OPEN_BRACKET:
                SkipReturn(TOKEN_OPEN_BRACKET);
                break;
            case CLOSE_BRACKET:
                SkipReturn(TOKEN_CLOSE_BRACKET);
                break;
            case OPEN_PAREN:
                SkipReturn(TOKEN_OPEN_PAREN);
                break;
            case CLOSE_PAREN:
                SkipReturn(TOKEN_CLOSE_PAREN);
                break;
            case OPEN_BRACE:
                SkipReturn(TOKEN_OPEN_BRACE);
                break;
            case CLOSE_BRACE:
                SkipReturn(TOKEN_CLOSE_BRACE);
                break;
            case SINGLE_QUOTE:
                LexQuotedIdentifier();
                break;
            case DOUBLE_QUOTE:
                LexStringLiteral();
                break;
            case BACKTICK:
                LexBacktickIdentifier();
                break;
            case DOLLAR:
                LexGenericIdentifier();
                break;
            case COLON:
                Step();
                if (SkipReturn(COLON, TOKEN_COLON_COLON))
                    break;
                Return(TOKEN_COLON);
                break;
            case UNDERSCORE:
                if (IsLetter(Peek))
                {
                    var ident = LexIdentifier();
                    StringToken(_kind, ident);
                }
                else
                {
                    SkipReturn(TOKEN_UNDERSCORE);
                }

                break;
            case COMMA:
                Step();
                Return(TOKEN_COMMA);
                break;
            case EXCLAMATION:
                Step();
                if (SkipReturn(EQUAL, TOKEN_NOT_EQUAL))
                    break;
                Error(ERROR_UNEXPECTED_CHAR);
                break;
            case TAB:
            case NEWLINE:
            case RETURN:
            case SPACE:
                do
                {
                    Step();
                } while (_char is TAB or NEWLINE or RETURN or SPACE);

                goto next;
            default:
                if (IsDigit(_char))
                {
                    LexNumber();
                }
                else
                {
                    string ident = LexIdentifier();
                    if (Keyword.Lookup.TryGetValue(ident, out _kind))
                        Return(_kind);
                    else
                        StringToken(TOKEN_IDENTIFIER, ident);
                }

                break;
        }

        return true;
    }

    private void LexRecordAccess()
    {
        string ident = LexIdentifier();
        StringToken(TOKEN_RECORD_ACCESS, ident);
    }

    private void LexTupleAccess()
    {
        BeginString();
        do
        {
            Step();
        } while (IsDigit(_char));

        var span = ReadChars();
        int item = int.Parse(span);
        IntToken(TOKEN_TUPLE_ACCESS, item);
    }

    private void LexQuotedIdentifier()
    {
        string ident = LexIdentifier();
        StringToken(_kind, ident);
    }

    /// <summary>
    /// Lex a generic identifier eg $T or $$T
    /// </summary>
    private void LexGenericIdentifier()
    {
        Step();
        var seq = Skip(DOLLAR);
        if (!IsLetter(_char))
        {
            Error(ERROR_POLYMORPHIC_IDENTIFIER);
            return;
        }

        BeginString();
        while (IsLetterOrDigit(_char))
            Step();

        var ident = ReadString();
        StringToken(seq ? TOKEN_IDENTIFIER_GENERIC_SEQUENCE : TOKEN_IDENTIFIER_GENERIC, ident);
    }

    /// <summary>
    /// Lex an identifier enclosed in backticks, used
    /// for turning normal functions into infix operators
    /// eg:
    /// ```
    /// var int: a;
    /// var int: b;
    /// constraint a `max` b = 2;`
    /// ```
    /// </summary>
    private void LexBacktickIdentifier()
    {
        Step();
        string ident = LexIdentifier();
        if (!Skip(BACKTICK))
            Error(ERROR_BACKTICK_IDENTIFIER);
        else if (_length <= 2)
            Error(ERROR_BACKTICK_IDENTIFIER);
        else if (!IsLetter(ident[0]))
            Error(ERROR_BACKTICK_IDENTIFIER);
        else
            StringToken(TOKEN_IDENTIFIER_INFIX, ident);
    }

    /// <summary>
    /// Lex a block comment
    /// </summary>
    /// <mzn>/* this is a block comment */</mzn>
    private void LexBlockComment()
    {
        BeginString();
        while (true)
        {
            if (Skip(STAR))
            {
                if (Skip(FWD_SLASH))
                    break;
            }
            else if (_char is EOF)
            {
                Error(ERROR_UNTERMINATED_BLOCK_COMMENT);
                return;
            }

            Step();
        }

        var comment = ReadString();
        StringToken(TOKEN_BLOCK_COMMENT, comment);
    }

    /// <summary>
    /// Set the current token to an Error token with
    /// the given message
    /// </summary>
    private void Error(string msg)
    {
        _token = new Token(_kind = ERROR, _startLine, _startCol, _startPos, _length - 1, s: msg);
    }

    /// <summary>
    /// Lex a line comment
    /// </summary>
    /// <mzn>var int: a % comment</mzn>
    private void LexLineComment()
    {
        Step();
        BeginString();
        line_comment:

        switch (_char)
        {
            case NEWLINE:
            case RETURN:
            case EOF:
                break;

            default:
                Step();
                goto line_comment;
        }

        var comment = ReadString();
        StringToken(TOKEN_LINE_COMMENT, comment);
    }

    /// <summary>
    /// Attempt to parse a valid identifer into the string buffer.
    /// This function will also check for reserved keywords.
    /// Upon exit:
    /// - `_string` will hold variable will be filled
    /// - `_kind` will be one of (IDENT / ERROR / KEYWORD)
    /// </summary>
    private string LexIdentifier()
    {
        BeginString();
        if (_char is SINGLE_QUOTE)
        {
            // Quoted identifer 'like this'
            quoted:
            Step();
            switch (_char)
            {
                case SINGLE_QUOTE when _length > 2:
                    break;
                case SINGLE_QUOTE:
                case BACK_SLASH:
                case RETURN:
                case NEWLINE:
                case EOF:
                    _kind = ERROR;
                    return string.Empty;
                default:
                    goto quoted;
            }

            Step();
        }
        else
        {
            while (true)
            {
                Step();
                if (IsLetter(_char))
                    continue;

                if (_char is UNDERSCORE || IsDigit(_char))
                    continue;

                break;
            }
        }

        _kind = TOKEN_IDENTIFIER;
        string str = ReadString();
        return str;
    }

    /// <summary>
    /// Lex a string literal
    /// </summary>
    private void LexStringLiteral()
    {
        bool inExpr = false;
        bool escaped = false;
        Step();
        BeginString();
        while (true)
        {
            switch (_char)
            {
                case DOUBLE_QUOTE when escaped:
                    escaped = false;
                    break;
                case DOUBLE_QUOTE when inExpr:
                    break;
                case DOUBLE_QUOTE:
                    string lit = ReadString();
                    StringToken(TOKEN_STRING_LITERAL, lit);
                    Step();
                    return;
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
                    Error(ERROR_UNTERMINATED_STRING_LITERAL);
                    return;
                default:
                    if (!escaped)
                        break;

                    if (_char is 'n' or 't' or SINGLE_QUOTE)
                    {
                        escaped = false;
                        break;
                    }

                    Error(ERROR_ESCAPED_STRING);
                    return;
            }

            Step();
        }
    }

    /// <summary>
    /// Lex a hexadecimal integer eg `0x1b7`
    /// </summary>
    private void LexHexInt()
    {
        Step(); // 0
        Step(); // x
        BeginString();
        do
        {
            Step();
        } while (IsLetterOrDigit(_char));

        var hex = ReadChars();
        if (int.TryParse(hex, NumberStyles.AllowHexSpecifier, null, out var i))
            IntToken(TOKEN_INT_LITERAL, i);
        else
            Error($"Could not parse \"{hex.ToString()}\" as an integer");
    }

    /// <summary>
    /// Lex an octal integer eg `0o777`
    /// </summary>
    private void LexOctalInt()
    {
        Step(); // 0
        Step(); // o
        BeginString();
        do
        {
            Step();
        } while (IsDigit(_char));

        string oct = ReadString();
        try
        {
            int i = Convert.ToInt32(oct, 8);
            IntToken(TOKEN_INT_LITERAL, i);
        }
        catch (Exception)
        {
            Error($"Could not parse \"{oct}\" as an integer");
        }
    }

    /// <summary>
    /// Lex a number, it can be either a standard integer,
    /// hexadecimal, octal, or float
    /// </summary>
    private void LexNumber()
    {
        if (_char is '0')
        {
            switch (Peek)
            {
                case 'o':
                    LexOctalInt();
                    return;

                case 'x':
                    LexHexInt();
                    return;
            }
        }

        BeginString();

        do
        {
            Step();
        } while (IsDigit(_char));

        // Peek needed because of range literals eg: `1..T`
        if (_char is DOT && IsDigit(Peek))
        {
            Step();
            Step();
            while (true)
            {
                if (Skip('e'))
                    Skip('-');
                else if (IsDigit(_char))
                    Step();
                else
                    break;
            }

            var span = ReadChars();
            if (decimal.TryParse(span, NumberStyles.Float, null, out var d))
                FloatToken(TOKEN_FLOAT_LITERAL, d);
            else
                Error($"Could not parse \"{span}\" as a float");
        }
        else if (Skip('e'))
        {
            Skip('-');
            do
            {
                Step();
            } while (IsDigit(_char));
            var span = ReadChars();
            if (decimal.TryParse(span, NumberStyles.Float, null, out var d))
                FloatToken(TOKEN_FLOAT_LITERAL, d);
            else
                Error($"Could not parse \"{span}\" as a float");
        }
        else
        {
            var span = ReadChars();
            if (int.TryParse(span, NumberStyles.None, null, out var i))
                IntToken(TOKEN_INT_LITERAL, i);
            else
                Error($"Could not parse \"{span}\" as an integer");
            return;
        }
    }

    void Step()
    {
        if (++_index <= _n)
        {
            _char = _sourceText[_index];
            _length++;
            if (_char is NEWLINE)
            {
                _line++;
                _col = 0;
            }
            else
            {
                _col++;
            }
        }
        else
        {
            _char = EOF;
            _fin = true;
        }
    }

    private char Peek => _index < _n ? _sourceText[_index + 1] : EOF;

    bool Skip(char c)
    {
        if (_char == c)
        {
            Step();
            return true;
        }

        return false;
    }

    private void Return(in TokenKind kind)
    {
        _token = new Token(_kind = kind, _startLine, _startCol, _startPos, _length - 1);
    }

    private void IntToken(in TokenKind kind, int i)
    {
        _token = new Token(_kind = kind, _startLine, _startCol, _startPos, _length - 1, i: i);
    }

    private void FloatToken(in TokenKind kind, decimal d)
    {
        _token = new Token(_kind = kind, _startLine, _startCol, _startPos, _length - 1, f: d);
    }

    private void StringToken(in TokenKind kind, string s)
    {
        _token = new Token(_kind = kind, _startLine, _startCol, _startPos, _length - 1, s: s);
    }

    private bool SkipReturn(in char c, in TokenKind kind)
    {
        if (_char != c)
            return false;

        Step();
        Return(kind);
        return true;
    }

    private void SkipReturn(in TokenKind kind)
    {
        Return(kind);
        Step();
    }

    void BeginString()
    {
        _startString = _index;
    }

    string ReadString() => _sourceText.Substring(_startString, _index - _startString);

#if NETSTANDARD2_0
    string ReadChars() => ReadString();
#else
    ReadOnlySpan<char> ReadChars() => _sourceText.AsSpan(_startString, _index - _startString);
#endif

    /// <summary>
    /// Lex the given string
    /// </summary>
    public static Token[] Lex(string s)
    {
        var lexer = new Lexer(s);
        var tokens = lexer.ToArray();
        return tokens;
    }

    public void Reset()
    {
        throw new NotImplementedException();
    }

    object IEnumerator.Current => _token;

    Token IEnumerator<Token>.Current => _token;

    public IEnumerator<Token> GetEnumerator() => this;

    IEnumerator IEnumerable.GetEnumerator() => this;

    public void Dispose() { }
}
