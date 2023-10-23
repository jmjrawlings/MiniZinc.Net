#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace MiniZinc.Net;

using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

public enum Kind
{
    None,
    Error,
    Word,
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
    Tilde,
    BackSlash,
    ForwardSlash,
    Colon,
    Delimiter,
    Pipe,
    Empty,
    EndOfStream
}

public readonly struct Token
{
    public required Kind Kind { get; init; }
    public required uint Line { get; init; }
    public required uint Col { get; init; }
    public required uint Start { get; init; }
    public required uint Length { get; init; }
    public int Int { get; init; }
    public string? String { get; init; }
    public double Double { get; init; }
}

public class Lexer : IDisposable
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
    const char SINGLE_QUOTE = '\'';
    const char DOUBLE_QUOTE = '"';
    const char BACKTICK = '`';
    const char COLON = ':';
    const char NEWLINE = '\n';
    const char TAB = '\t';
    const char RETURN = '\r';
    const char SPACE = ' ';
    const char EOF = '\0';

    const string ERR_FLOAT_LIT = "Invalid float literal";
    const string ERR_QUOTED_IDENT = "Invalid quoted identifier";
    const string ERR_ESCAPED_STRING = "String was not escaped properly";
    const string ERR_UNTERMINATED_LITERAL =
        "Literal was not terminated properly at the end of the stream";

    private uint _line; // Start line of current token
    private uint _col; // Start column of current token
    private uint _index; // Position of the stream
    private uint _start; // Start index of current token
    private uint _length; // Length of the current token
    private Kind _kind; // Kind of current token
    private string? _string; // String contents of current token
    private char _peek; // Value of a peek if used
    private char _char; // Char at the current stream position
    private char _prev; // Previous parsed char;
    private int _int;
    private double _double;
    private readonly bool _debug;
    private readonly StreamReader _sr;
    private readonly StringBuilder _sb;
    
    Lexer(StreamReader sr)
    {
        _sr = sr;
        _sb = new StringBuilder();
    }

    void MoveAndPeek()
    {
        Move();
        Peek();
    }

    void Move()
    {
        _prev = _char;
        _char = (char)_sr.Read();
        _index++;
        _length++;
    }

    void Peek()
    {
        _peek = (char)_sr.Peek();
    }

    void Store()
    {
        Console.Write(_char);
        _sb.Append(_char);
    }
    
    bool Skip(char c)
    {
        Peek();
        if (_peek != c)
            return false;
        Move();
        return true;
    }

    void Error(string msg)
    {
        _kind = Kind.Error;
        _string = msg;
    }
    
    public IEnumerable<Token> LexTokens()
    {
        Console.WriteLine(_char);
        // Move the stream forward
        move:
            Move();
            
        // Start the next token            
        next:
            _start = _index;
            _length = 0;
            
        // Handle the current token
        loop:
            switch (_char)
            {
                case NEWLINE:
                case TAB:
                case RETURN:
                case SPACE:
                    goto move;
                case EOF:
                    _kind = Kind.EndOfStream;
                    goto stop;
                // Line comment
                case HASH:
                    _kind = Kind.LineComment;
                    while (true)
                    {
                        Move();
                        if (_char is EOF or NEWLINE)
                            break;
                        Store();
                    }
                    ReadString();
                    break;
                // Block comment
                case FWD_SLASH when Skip(STAR):
                    _kind = Kind.BlockComment;
                    while (true)
                    {
                        Move();
                        switch (_char)
                        {
                            case STAR when Skip(FWD_SLASH):
                                goto ok;
                            case EOF:
                                Error(ERR_UNTERMINATED_LITERAL);
                                goto error;
                            default:
                                Store();
                                break;
                        }
                    }
                case FWD_SLASH:
                    _kind = Kind.ForwardSlash;
                    break;
                case BACK_SLASH:
                    _kind = Kind.BackSlash;
                    break;
                case STAR:
                    _kind = Kind.Star;
                    break;
                case DELIMITER:
                    _kind = Kind.Delimiter;
                    break;
                case EQUAL:
                    Skip(EQUAL);
                    _kind = Kind.Equal;
                    break;
                case LEFT_CHEVRON:
                    Peek();
                    if (_peek is RIGHT_CHEVRON)
                    {
                        Move();
                        _kind = Kind.Empty;
                        goto ok;
                    }
                    
                    if (_peek is DASH)
                    {
                        MoveAndPeek();
                        if (_peek is RIGHT_CHEVRON)
                        {
                            Move();
                            _kind = Kind.DoubleArrow;
                            goto ok;
                        }
                        _kind = Kind.LeftArrow;
                        goto ok;
                    }
                    
                    if (_peek is EQUAL)
                    {
                        _kind = Kind.LessThanEqual;
                        goto ok;
                    }
                    _kind = Kind.LessThan;
                    break;
                case RIGHT_CHEVRON:
                    _kind = Skip(EQUAL) ? Kind.GreaterThanEqual : Kind.GreaterThan;
                    break;
                case UP_CHEVRON:
                    break;
                case PIPE:
                    _kind = Kind.Pipe;
                    break;
                case DOT:
                    _kind = Skip(DOT) ? Kind.DotDot : Kind.Dot;
                    break;
                case PLUS:
                    _kind = Skip(PLUS) ? Kind.PlusPlus : Kind.Plus;
                    break;
                case DASH:
                    _kind = Skip(RIGHT_CHEVRON) ? Kind.RightArrow : Kind.Minus;
                    break;
                case TILDE:
                    Peek();
                    switch (_peek)
                    {
                        case DASH:
                            _kind = Kind.TildeMinus;
                            Move();
                            break;
                        case PLUS:
                            _kind = Kind.TildePlus;
                            Move();
                            break;
                        case STAR:
                            _kind = Kind.TildeStar;
                            Move();
                            break;
                        case EQUAL:
                            _kind = Kind.TildeEquals;
                            Move();
                            break;
                        default:
                            _kind = Kind.Tilde;
                            break;
                    }
                    break;
                case LEFT_BRACK:
                    _kind = Kind.LeftBracket;
                    break;
                case RIGHT_BRACK:
                    _kind = Kind.RightBracket;
                    break;
                case LEFT_PAREN:
                    _kind = Kind.LeftParen;
                    break;
                case RIGHT_PAREN:
                    _kind = Kind.RightParen;
                    break;
                case LEFT_BRACE:
                    _kind = Kind.LeftBrace;
                    break;
                case RIGHT_BRACE:
                    _kind = Kind.RightBrace;
                    break;
                case PERCENT:
                    _kind = Kind.Percent;
                    break;
                // Quoted identifier
                case SINGLE_QUOTE:
                    _kind = Kind.QuotedIdentifier;
                    while (true)
                    {
                        Move();
                        switch (_char)
                        {
                            case SINGLE_QUOTE when (_prev is SINGLE_QUOTE):
                                Error(ERR_QUOTED_IDENT);
                                goto error;
                            case SINGLE_QUOTE:
                                goto ok;
                            case BACK_SLASH:
                            case RETURN:
                            case NEWLINE:
                            case EOF:
                                Error(ERR_QUOTED_IDENT);
                                goto error;
                            default:
                                Store();
                                break;
                        }
                    }
                // String literal
                case DOUBLE_QUOTE:
                    _kind = Kind.StringLiteral;
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
                                if (_char is BACK_SLASH or DOUBLE_QUOTE)
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
                        goto ok;
                    }
                case COLON:
                    _kind = Kind.Colon;
                    break;
                case UNDERSCORE:
                    Peek();
                    if (char.IsWhiteSpace(_peek))
                        _kind = Kind.Underscore;
                    else if (char.IsLetter(_peek))
                        LexWord();
                    else
                        Error("Underscore xd");
                    break;
                default:
                    if (char.IsDigit(_char))
                        goto number;
                    
                    if (char.IsLetter(_char))
                    {
                        LexWordOrKeyword();
                        goto ok;
                    }
                    
                    if (_sr.EndOfStream)
                        goto stop;
                    throw new Exception($"Unhanlded {_char}");
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
                
                yield return token;
                if (_kind is Kind.Error)
                    goto stop;
                
                goto move;
                
            // {
            //     if (isFloat)
            //     {
            //         Error(ERR_FLOAT_LIT);
            //         goto error;
            //     }
            //
            //     Move();
            //     Store();
            //     Peek();
            //     if (!char.IsDigit(_peek))
            //     {
            //         Error(ERR_FLOAT_LIT);
            //         goto error;
            //     }
            //
            //     Move();
            //     Store();
            //     isFloat = true;
            //     goto start;
            // }
            //
            // if (!char.IsDigit(_peek))
            //     goto ok;
            //
            // Move();
            // Store();
            // goto start;

        // error:
        // Error(ERR_FLOAT_LIT);
        //
        // ok:
        // ReadString();
        // if (isFloat)
        // {
        //     _double = double.Parse(_string.AsSpan());
        //     _kind = Kind.Float;
        // }
        // else
        // {
        //     _int = int.Parse(_string.AsSpan());
        //     _kind = Kind.Int;
        // }
        
        
        stop:
            yield break;
        
        number:
            while (char.IsDigit(_char))
            {
                Store();
                Move();
            } 
            Peek();
            if (_peek is DOT)
            {
                Move();
                Peek();
            }
    }
    
    private void LexWordOrKeyword()
    {
        LexWord();
    }

    private void LexWord()
    {
        _kind = Kind.Word;
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

    public static Lexer FromString(string s)
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

    public static Lexer FromFile(FileInfo fi)
    {
        var stream = fi.OpenText();
        var lexer = new Lexer(stream);
        return lexer;
    }

    public static Lexer FromFile(string path)
    {
        FileInfo fi = new(path);
        var lexer = FromFile(fi);
        return lexer;
    }
}
