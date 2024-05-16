﻿namespace MiniZinc.Parser;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using static Char;

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

    private string _source;
    private uint _sourceLength;
    private uint _line;
    private uint _col;
    private uint _index;
    private uint _startPos;
    private uint _startLine;
    private uint _startCol;
    private uint _length;
    private bool _fin;
    private string _string;
    private char _char;
    private Token _token;
    private TokenKind _kind;
    private readonly StringBuilder _sb;
    
    private Lexer(string source)
    {
        _sb = new StringBuilder();
        _string = string.Empty;
        _line = 1;
        _source = source;
        _sourceLength = (uint)source.Length;
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
                else if (!SkipReturn(BACK_SLASH, TokenKind.UP_WEDGE))
                    Return(TokenKind.FWDSLASH);
                break;
            case BACK_SLASH:
                Step();
                if (!SkipReturn(FWD_SLASH, TokenKind.DOWN_WEDGE))
                    Return(TokenKind.BACKSLASH);
                break;
            case STAR:
                SkipReturn(TokenKind.STAR);
                break;
            case DELIMITER:
                SkipReturn(TokenKind.EOL);
                break;
            case EQUAL:
                Step();
                Skip(EQUAL);
                Return(TokenKind.EQUAL);
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
                    Return(TokenKind.LEFT_ARROW);
                    break;
                }
                Return(TokenKind.LESS_THAN);
                break;
            case RIGHT_CHEVRON:
                Step();
                if (SkipReturn(EQUAL, TokenKind.GREATER_THAN_EQUAL))
                    break;
                Return(TokenKind.GREATER_THAN);
                break;
            case UP_CHEVRON:
                SkipReturn(TokenKind.EXP);
                break;
            case PIPE:
                SkipReturn(TokenKind.PIPE);
                break;
            case DOT:
                Step();
                // ..
                if (SkipReturn(DOT, TokenKind.DOT_DOT))
                {
                }
                // Tuple access
                else if (IsDigit(_char))
                {
                    do
                    {
                        Store();
                        Step();
                    } while (IsDigit(_char));
                    ReadString();
                    Return(TokenKind.TUPLE_ACCESS, int.Parse(_string));
                }
                // Record access
                else
                {
                    LexIdentifier();
                    Return(TokenKind.RECORD_ACCESS, _string);
                }
                break;
            case PLUS:
                Step();
                if (SkipReturn(PLUS, TokenKind.PLUS_PLUS))
                    break;
                Return(TokenKind.PLUS);
                break;
            case DASH:
                Step();
                if (SkipReturn(RIGHT_CHEVRON, TokenKind.RIGHT_ARROW))
                    break;
                Return(TokenKind.MINUS);
                break;
            case TILDE:
                Step();
                switch (_char)
                {
                    case DASH:
                        SkipReturn(TokenKind.TILDE_MINUS);
                        break;
                    case PLUS:
                        SkipReturn(TokenKind.TILDE_PLUS);
                        break;
                    case STAR:
                        SkipReturn(TokenKind.TILDE_STAR);
                        break;
                    case EQUAL:
                        SkipReturn(TokenKind.TILDE_EQUALS);
                        break;
                    default:
                        Return(TokenKind.TILDE);
                        break;
                }
                break;
            case OPEN_BRACKET:
                SkipReturn(TokenKind.OPEN_BRACKET);
                break;
            case CLOSE_BRACKET:
                SkipReturn(TokenKind.CLOSE_BRACKET);
                break;
            case OPEN_PAREN:
                SkipReturn(TokenKind.OPEN_PAREN);
                break;
            case CLOSE_PAREN:
                SkipReturn(TokenKind.CLOSE_PAREN);
                break;
            case OPEN_BRACE:
                SkipReturn(TokenKind.OPEN_BRACE);
                break;
            case CLOSE_BRACE:
                SkipReturn(TokenKind.CLOSE_BRACE);
                break;
            case SINGLE_QUOTE:
                LexIdentifier();
                Return(_kind, _string);
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
                if (SkipReturn(COLON, TokenKind.COLON_COLON))
                    break;
                Return(TokenKind.COLON);
                break;
            case UNDERSCORE:
                if (IsLetter(Peek))
                {
                    LexIdentifier();
                    Return(_kind);
                }
                else
                {
                    SkipReturn(TokenKind.UNDERSCORE);
                }
                break;
            case COMMA:
                Step();
                Return(TokenKind.COMMA);
                break;
            case EXCLAMATION:
                Step();
                if (SkipReturn(EQUAL, TokenKind.NOT_EQUAL))
                    break;
                Error(ERROR_UNEXPECTED_CHAR);
                break;
            case TAB:
            case NEWLINE:
            case RETURN:
            case SPACE:
                do { Step(); } while (_char is TAB or NEWLINE or RETURN or SPACE);
                goto next;
            default:
                if (IsDigit(_char))
                {
                    LexNumber();
                }
                else
                {
                    LexIdentifier();
                    if (Keyword.Lookup.TryGetValue(_string, out _kind))
                        Return(_kind);
                    else
                        Return(TokenKind.IDENTIFIER, _string);
                }
                break;
        }
        return true;
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
        
        while (IsAsciiLetterOrDigit(_char))
        {
            Store();
            Step();
        }
        ReadString();
        Return(seq ? TokenKind.GENERIC_SEQUENCE : TokenKind.GENERIC, _string);
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
        LexIdentifier();
        if (!Skip(BACKTICK))
            Error(ERROR_BACKTICK_IDENTIFIER);
        else if (_length <= 2)
            Error(ERROR_BACKTICK_IDENTIFIER);
        else if (!IsLetter(_string[0]))
            Error(ERROR_BACKTICK_IDENTIFIER);
        else
            Return(TokenKind.INFIX_IDENTIFIER, _string);
    }
    
    private void Return(in TokenKind kind, object? data = null)
    {
        _token = new Token(_kind = kind, _startLine, _startCol, _startPos, _length - 1, data);
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

    private void LexBlockComment()
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
                Error(ERROR_UNTERMINATED_BLOCK_COMMENT);
                return;
            }

            Store();
            Step();
        }
        
        ReadString();
        Return(TokenKind.BLOCK_COMMENT,_string);
    }


    private void Error(string msg)
    {
        _token = new Token(
            _kind = TokenKind.ERROR,
            _startLine,
            _startCol,
            _startPos,
            _length - 1,
            msg
        );
    }
    
    private void LexLineComment()
    {
        Step();
        line_comment:
        
        switch (_char)
        {
            case NEWLINE:
            case RETURN:
            case EOF:
                break;

            default:
                Step(); 
                Store();
                goto line_comment;
        }
        ReadString();
        Return(TokenKind.LINE_COMMENT,_string);
    }
    
    /// <summary>
    /// Attempt to parse a valid identifer into the string buffer.
    /// This function will also check for reserved keywords.
    /// Upon exit:
    /// - `_string` will hold variable will be filled
    /// - `_kind` will be one of (IDENT / ERROR / KEYWORD)
    /// </summary>
    private void LexIdentifier()
    {   
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
                    _kind = TokenKind.ERROR;
                    return;
                default:
                    Store();
                    goto quoted;
            }
            Step();
            ReadString();
            _kind = TokenKind.IDENTIFIER;
            return;
        }
        
        while (true)
        {
            Store();
            Step();
            if (IsLetter(_char))
                continue;

            if (_char is UNDERSCORE || IsDigit(_char))
                continue;
            
            break;
        }
        
        ReadString();
        _kind = TokenKind.IDENTIFIER;
    }

    private void LexStringLiteral()
    {
        bool inExpr = false;
        bool escaped = false;
        string_literal:
        Step();
        
        switch (_char)
        {
            case DOUBLE_QUOTE when escaped:
                escaped = false;
                break;

            case DOUBLE_QUOTE when inExpr:
                break;

            case DOUBLE_QUOTE:
                Step();
                ReadString();
                Return(TokenKind.STRING_LITERAL, _string);
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

#if DEBUG
                var s = _sb.ToString();
#endif

                Error(ERROR_ESCAPED_STRING);
                return;
        }

        Store();
        goto string_literal;
    }

    private void LexHexInt()
    {
        do
        {
            Store();
            Step();
        } while (IsAsciiLetterOrDigit(_char));
        ReadString();
        if (int.TryParse(_string, NumberStyles.AllowHexSpecifier, null, out var i))
            Return(TokenKind.INT_LITERAL, i);
        else
            Error($"Could not parse \"{_string}\" as an integer");
    }

    private void LexOctalInt()
    {
        do
        {
            Store();
            Step();
        } while (IsDigit(_char));
        ReadString();
        try
        {
            int i = Convert.ToInt32(_string, 8);
            Return(TokenKind.INT_LITERAL, i);
        }
        catch (Exception)
        {
            Error($"Could not parse \"{_string}\" as an integer");
        }
    }
    
    private void LexNumber()
    {
        if (_char is '0')
        {
            switch (Peek)
            {
                case 'o':
                    Step();
                    Step();
                    LexOctalInt();
                    return;

                case 'x':
                    Step();
                    Step();
                    LexHexInt();
                    return;
            }
        }

        do
        {
            Store();
            Step();
        } while (IsAsciiLetterOrDigit(_char));
        if (_char is DOT && IsDigit(Peek))
        {
            Store();
            Step();
            goto lex_float;
        }
        
        ReadString();
        if (int.TryParse(_string, NumberStyles.None, null, out var i))
            Return(TokenKind.INT_LITERAL, i);
        else
            Error($"Could not parse \"{_string}\" as an integer");
        return;

        lex_float:
        do
        {
            Store();
            Step();
        } while (IsDigit(_char) || _char is 'e');
        ReadString();
        
        if (double.TryParse(_string, null, out var d))
            Return(TokenKind.FLOAT_LITERAL, d);
        else
            Error($"Could not parse \"{_string}\" as a float");
    }

    void Step()
    {
        if (_index < _sourceLength)
        {
            _char = _source[(int)_index++];
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

    private char Peek => _index < _sourceLength ? _source[(int)_index] : EOF;
    
    /// <summary>
    /// Store the current character in the string builder
    /// </summary>
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
    /// TODO - use spans for string data?
    private void ReadString()
    {
        _string = _sb.ToString();
        _sb.Clear();
    }
    
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

    public void Dispose()
    {
    }
}
