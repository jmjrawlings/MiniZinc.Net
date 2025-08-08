namespace MiniZinc.Parser;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using static Char;
using static TokenKind;

internal ref struct Lexer
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

    private readonly string _sourceText;
    private int _line;
    private int _col;
    private int _index;
    private int _outdex;
    private int _length;
    private int _startPos;
    private int _startLine;
    private int _startCol;
    private bool _fin;
    private bool _err;
    private char _char;
    private TokenKind _kind;
    private Token[] _tokens;
    private readonly int _n;

    private Lexer(string sourceText)
    {
        _line = 1;
        _index = -1;
        _fin = false;
        _err = false;
        _sourceText = sourceText;
        _n = sourceText.Length;
        _tokens = new Token[(_n + 1) / 2];
    }

    public void MoveNext()
    {
        while (_char is TAB or NEWLINE or RETURN or SPACE)
            Step();

        _startPos = _index;
        _startLine = _line;
        _startCol = _col;
        _length = 1;
        switch (_char)
        {
            case EOF:
                Ok(TOKEN_EOF);
                _fin = true;
                break;

            case PERCENT:
                LexLineComment();
                break;

            case FWD_SLASH:
                Step();
                if (Skip(STAR))
                    LexBlockComment();
                else if (!SkipReturn(BACK_SLASH, TOKEN_CONJUNCTION))
                    Ok(TOKEN_DIVIDE);
                break;

            case BACK_SLASH:
                Step();
                if (!SkipReturn(FWD_SLASH, TOKEN_DISJUNCTION))
                    Ok(TOKEN_BACKSLASH);
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
                Ok(TOKEN_EQUAL);
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
                    Ok(TOKEN_REVERSE_IMPLICATION);
                    break;
                }

                if (Skip(DOT))
                {
                    if (Skip(DOT))
                    {
                        if (Skip(LEFT_CHEVRON))
                            Ok(TOKEN_RANGE_EXCLUSIVE);
                        else
                            Ok(TOKEN_RANGE_LEFT_EXCLUSIVE);
                        break;
                    }

                    Error(ERROR_UNEXPECTED_CHAR);
                    break;
                }
                Ok(TOKEN_LESS_THAN);
                break;

            case RIGHT_CHEVRON:
                Step();
                if (SkipReturn(EQUAL, TOKEN_GREATER_THAN_EQUAL))
                    break;
                Ok(TOKEN_GREATER_THAN);
                break;

            case UP_CHEVRON:
                SkipReturn(TOKEN_EXPONENT);
                break;

            case PIPE:
                SkipReturn(TOKEN_PIPE);
                break;

            case DOT:
                Step();
                if (Skip(DOT))
                {
                    if (Skip(LEFT_CHEVRON))
                        Ok(TOKEN_RANGE_RIGHT_EXCLUSIVE);
                    else
                        Ok(TOKEN_RANGE_INCLUSIVE);
                }
                else if (IsDigit(_char))
                {
                    LexTupleAccess();
                }
                else
                {
                    LexRecordAccess();
                }
                break;

            case PLUS:
                Step();
                if (SkipReturn(PLUS, TOKEN_PLUS_PLUS))
                    break;
                Ok(TOKEN_PLUS);
                break;

            case DASH:
                Step();
                if (SkipReturn(RIGHT_CHEVRON, TOKEN_FORWARD_IMPLICATION))
                    break;
                Ok(TOKEN_MINUS);
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
                        Ok(TOKEN_TILDE);
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
                Ok(TOKEN_COLON);
                break;

            case UNDERSCORE:
                if (IsLetter(Peek()))
                {
                    var ident = LexIdentifier();
                    Ok(_kind, s: ident);
                }
                else
                {
                    SkipReturn(TOKEN_UNDERSCORE);
                }
                break;

            case COMMA:
                Step();
                Ok(TOKEN_COMMA);
                break;

            case EXCLAMATION:
                Step();
                if (SkipReturn(EQUAL, TOKEN_NOT_EQUAL))
                    break;
                Error(ERROR_UNEXPECTED_CHAR);
                break;

            default:
                if (IsDigit(_char))
                {
                    LexNumber();
                }
                else
                {
                    string ident = LexIdentifier();
                    if (Keyword.Lookup.TryGetValue(ident, out _kind))
                        Ok(_kind);
                    else
                        Ok(TOKEN_IDENTIFIER, s: ident);
                }

                break;
        }
    }

    private void LexRecordAccess()
    {
        string ident = LexIdentifier();
        Ok(TOKEN_RECORD_ACCESS, s: ident);
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
        Ok(TOKEN_TUPLE_ACCESS, i: item);
    }

    private void LexQuotedIdentifier()
    {
        string id = LexIdentifier();
        Ok(_kind, s: id);
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

        string id = ReadString();
        Ok(seq ? TOKEN_IDENTIFIER_GENERIC_SEQUENCE : TOKEN_IDENTIFIER_GENERIC, s: id);
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
        string id = LexIdentifier();
        if (!Skip(BACKTICK))
            Error(ERROR_BACKTICK_IDENTIFIER);
        else if (_length <= 2)
            Error(ERROR_BACKTICK_IDENTIFIER);
        else if (!IsLetter(id[0]))
            Error(ERROR_BACKTICK_IDENTIFIER);
        else
            Ok(TOKEN_IDENTIFIER_INFIX, s: id);
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
        Ok(TOKEN_BLOCK_COMMENT, s: comment);
    }

    /// <summary>
    /// Set the current token to an Error token with
    /// the given message
    /// </summary>
    private void Error(string msg)
    {
        var token = new Token(_kind = ERROR, _startLine, _startCol, _startPos, _length - 1, s: msg);
        if (_outdex >= _tokens.Length)
            Array.Resize(ref _tokens, _tokens.Length * 2);
        _tokens[_outdex++] = token;
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
        Ok(TOKEN_LINE_COMMENT, s: comment);
    }

    /// <summary>
    /// Attempt to parse a valid identifier into the string buffer.
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
            // Quoted identifier 'like this'
            quoted:
            Step();
            switch (_char)
            {
                case SINGLE_QUOTE when _length > 2:
                    break;
                case SINGLE_QUOTE:
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
                    Ok(TOKEN_STRING_LITERAL, s: lit);
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
            Ok(TOKEN_INT_LITERAL, i: i);
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
            Ok(TOKEN_INT_LITERAL, i: i);
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
            switch (Peek())
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
        if (_char is DOT && IsDigit(Peek()))
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
                Ok(TOKEN_FLOAT_LITERAL, f: d);
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
                Ok(TOKEN_FLOAT_LITERAL, f: d);
            else
                Error($"Could not parse \"{span}\" as a float");
        }
        else
        {
            var span = ReadChars();
            if (int.TryParse(span, NumberStyles.None, null, out var i))
                Ok(TOKEN_INT_LITERAL, i: i);
            else
                Error($"Could not parse \"{span}\" as an integer");
            return;
        }
    }

    void Step()
    {
        if (++_index < _n)
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
        }
    }

    private char Peek()
    {
        int i = _index + 1;
        if (i < _n)
            return _sourceText[i];
        else
            return EOF;
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

    private void Ok(in TokenKind kind, int i = default, string? s = null, decimal f = default)
    {
        if (_outdex >= _tokens.Length)
            Array.Resize(ref _tokens, _tokens.Length * 2);

        ref Token token = ref _tokens[_outdex++];
        token.Kind = kind;
        token.Line = _startLine;
        token.Start = _startPos;
        token.Col = _startCol;
        token.Length = _length - 1;
        token.FloatValue = f;
        token.IntValue = i;
        token.StringValue = s;
    }

    private bool SkipReturn(in char c, in TokenKind kind)
    {
        if (_char != c)
            return false;

        Step();
        Ok(kind);
        return true;
    }

    private void SkipReturn(in TokenKind kind)
    {
        Ok(kind);
        Step();
    }

    void BeginString()
    {
        _startPos = _index;
    }

    string ReadString() => _sourceText.Substring(_startPos, _index - _startPos);

#if NETSTANDARD2_0
    string ReadChars() => ReadString();
#else
    ReadOnlySpan<char> ReadChars() => _sourceText.AsSpan(_startPos, _index - _startPos);
#endif

    /// <summary>
    /// Lex the given string
    /// </summary>
    public static bool Lex(string s, out Token[] tokens)
    {
        var lexer = new Lexer(s);
        lexer.Step();
        while (!(lexer._fin || lexer._err))
            lexer.MoveNext();

        tokens = lexer._tokens[..(lexer._outdex)];
        if (lexer._err)
            return false;
        return true;
    }

    public void Reset()
    {
        throw new NotImplementedException();
    }

    public void Dispose() { }
}
