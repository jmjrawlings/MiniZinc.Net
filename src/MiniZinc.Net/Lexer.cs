#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace MiniZinc.Net;

using System;
using System.Collections.Generic;
using System.Text;

public enum Kind
{
    None,
    Error,
    Word,
    Int,
    Float,
    String,
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
    Empty
}

public readonly struct Token
{
    public required Kind Kind { get; init; }
    public required int Line { get; init; }
    public required int Col { get; init; }
    public required int Start { get; init; }
    public required int Length { get; init; }
    public int? Int { get; init; }
    public string? String { get; init; }
    public float? Float { get; init; }
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
    const char MINUS = '-';
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
    const char NULL = '\x00';
    const char SPACE = ' ';
    const char EOF = '\0';

    const string ERR_QUOTED_IDENT = "Invalid quoted identifier";
    const string ERR_ESCAPED_STRING = "String was not escaped properly";
    const string ERR_UNTERMINATED_LITERAL =
        "Literal was not terminated properly at the end of the stream";

    private readonly DateTime _startTime;
    private int _line;
    private int _col;
    private int _index;
    private int _start;
    private int _offset;
    private Kind _kind;
    private string? _string;
    private string? _error;
    private char _char;
    private char _last;
    private char _peek;
    private int _int;
    private float _float;
    private readonly bool _debug;
    private readonly StreamReader _sr;
    private readonly StringBuilder _sb;

    public Lexer(StreamReader sr)
    {
        _startTime = DateTime.Now;
        _sr = sr;
        _sb = new StringBuilder();
    }

    private void Move()
    {
        _last = _char;
        _char = (char)_sr.Read();
        _index++;
    }

    private void Store()
    {
        _sb.Append(_char);
    }

    private void Peek()
    {
        _peek = (char)_sr.Peek();
    }

    private bool Skip(char c)
    {
        Peek();
        if (_peek != c)
            return false;
        Move();
        return true;
    }

    /// Skip the next char
    private void Skip()
    {
        Move();
    }

    /// Skip the next char
    private void Skip(int n)
    {
        switch (n)
        {
            case 0:
                return;
            case 1:
                Skip();
                break;
            case 2:
                Skip();
                Skip();
                break;
            case 3:
                for (int i = 0; i < n; n++)
                    Skip();
                break;
        }
    }

    public IEnumerable<Token> ReadTokens()
    {
        while (_char != EOF)
        {
            var token = ReadToken();
            yield return token;
            if (_error is null)
                break;
        }
    }

    public Token ReadToken()
    {
        start:
        _start = _index;
        _offset = 0;

        Move();
        switch (_char)
        {
            case EOF:
                break;
            case NEWLINE:
            case TAB:
            case RETURN:
            case SPACE:
                goto start;

            case HASH:
                ReadLineComment();
                break;
            case FWD_SLASH when Skip(STAR):
                ReadBlockComment();
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
                _kind = Kind.TildeEquals;
                break;
            case LEFT_CHEVRON:
                break;
            case RIGHT_CHEVRON:
                break;
            case UP_CHEVRON:
                break;
            case DOT:
                _kind = Skip(DOT) ? Kind.DotDot : Kind.Dot;
                break;
            case PLUS:
                _kind = Skip(PLUS) ? Kind.PlusPlus : Kind.Plus;
                break;
            case MINUS:
                _kind = Kind.Minus;
                break;
            case TILDE:
                Peek();
                _kind = _peek switch
                {
                    MINUS => Kind.TildeMinus,
                    PLUS => Kind.TildePlus,
                    STAR => Kind.TildeStar,
                    EQUAL => Kind.TildeEquals,
                    _ => Kind.Tilde
                };

                if (_kind != Kind.Tilde)
                    Skip();

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
            case SINGLE_QUOTE:
                ReadQuotedIdentifier();
                break;
            case DOUBLE_QUOTE:
                ReadStringLiteral();
                break;
            case BACKTICK:
                break;
            case COLON:
                break;
            case UNDERSCORE:
                Peek();
                if (char.IsWhiteSpace(_peek))
                    _kind = Kind.Underscore;
                else if (char.IsLetter(_peek))
                    ReadWord();
                else
                {
                    Error("Underscore xd");
                }
                break;
            default:
                if (char.IsDigit(_char))
                    ReadNumber();
                else if (char.IsLetter(_char))
                    ReadWordOrKeyword();
                break;
        }

        Kind kind = _kind;
        string? s = _string;
        if (_error is not null)
        {
            kind = Kind.Error;
            s = _error;
        }

        var token = new Token
        {
            Kind = kind,
            Line = _line,
            Col = _col,
            Start = _start,
            Length = _offset,
            String = s,
            Int = _int,
            Float = _float
        };

        return token;
    }

    private void ReadWordOrKeyword()
    {
        ReadWord();
    }

    private void ReadWord()
    {
        _kind = Kind.Word;
    }

    private void ReadNumber()
    {
        bool isFloat = false;
        Store();
        while (_char != EOF)
        {
            Peek();
            if (!char.IsDigit(_peek))
                break;

            Move();
            Store();
        }
        String();
        _kind = isFloat ? Kind.Float : Kind.Int;
        _int = int.Parse(_string!);
    }

    private void ReadStringLiteral()
    {
        _kind = Kind.String;
        bool ok = false;
        while (_char != EOF)
        {
            Move();
            if (_char is DOUBLE_QUOTE)
            {
                ok = true;
                break;
            }

            if (_char is BACK_SLASH)
            {
                Store();
                Move();
                Store();
                if (_char is BACK_SLASH or DOUBLE_QUOTE)
                    continue;
                Error(ERR_ESCAPED_STRING);
                break;
            }
            Store();
        }

        ErrorIf(!ok, ERR_UNTERMINATED_LITERAL);
        String();
    }

    private void ReadQuotedIdentifier()
    {
        bool ok = false;
        while (_char != EOF)
        {
            Move();
            if (_char is SINGLE_QUOTE)
            {
                ok = true;
                break;
            }

            if (_char is BACK_SLASH or RETURN or NEWLINE)
                break;

            Store();
        }

        ErrorIf(!ok, ERR_QUOTED_IDENT);
        String();
    }

    /// Record and error
    private void Error(string msg)
    {
        _error = msg;
    }

    /// Record an error if the given condition holds
    private void ErrorIf(bool cond, string msg)
    {
        if (cond)
            Error(msg);
    }

    /// Return the contents of the current string buffer
    private void String()
    {
        _string = _sb.ToString();
        _sb.Clear();
    }

    private void ReadBlockComment()
    {
        bool ok = false;
        while (_char != EOF)
        {
            Move();
            if (_char is STAR && Skip(FWD_SLASH))
            {
                ok = true;
                break;
            }
            Store();
        }

        _kind = Kind.BlockComment;
        String();
        ErrorIf(!ok, "Unclosed block comment");
    }

    private void ReadLineComment()
    {
        while (_char != EOF)
        {
            Move();
            Store();
            if (_char is NEWLINE)
                break;
        }

        _kind = Kind.LineComment;
        String();
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
//     match stream.Read() with
//     | DELIMITER ->
//     Token.TDelimiter
//     | LEFT_BRACK ->
//     Token.TLeftBracket
//     | RIGHT_BRACK ->
//     Token.TRightBracket
//     | LEFT_PAREN ->
//     Token.TLeftParen
//     | RIGHT_PAREN ->
//     Token.TRightParen
//     | LEFT_BRACE ->
//     Token.TLeftBrace
//     | RIGHT_BRACE ->
//     Token.TRightBrace
//     | COLON ->
//     Token.TColon
//     | DOT ->
//     if stream.Skip(DOT) then
//     Token.TDotDot
//     else
//     Token.TDot
//     | TILDE ->
//     match stream.Read() with
//     | EQUAL ->
//     Token.TTildeEquals
//     | PLUS ->
//     Token.TTildePlus
//     | MINUS ->
//     Token.TTildeMinus
//     | STAR ->
//     Token.TTildeStar
//     | _ ->
//     stream.Seek(token.Start)
//         Token.TTilde
//
//     | BACK_SLASH ->
//
//     Token.TBackSlash
//     // Line comment
//     | HASH ->
//     let string = stream.ReadRestOfLine(false)
//         lexed.Strings.Add string
//         Token.TLineComment
//
//     // Block comment
//     | FWD_SLASH when stream.Skip(STAR) ->
//     let mutable fin = false
//     // let sb = StringBuilder()
//     while not fin do
//     match stream.Read() with
//     | STAR when stream.Peek() = FWD_SLASH ->
//     stream.Skip(2)
//         stream.SkipWhitespace()
//
//     fin<- true
//     | c ->
//
//     // sb.Append c
//     fin<- stream.IsEndOfStream
//         Token.TBlockComment
//
//     | FWD_SLASH ->
//     Token.TForwardSlash
//     | LEFT_CHEVRON ->
//     if stream.Skip(MINUS) then
//     if stream.Skip(RIGHT_CHEVRON) then
//     Token.TDoubleArrow
//     else
//
//     Token.TLeftArrow
//         elif stream.Skip(EQUAL) then
//
//     Token.TLessThan
//         elif stream.Skip(RIGHT_CHEVRON) then
//     Token.TEmpty
//     else
//     Token.TLessThan
//     | MINUS ->
//     if stream.Skip(RIGHT_CHEVRON) then
//     Token.TRightArrow
//     else
//     Token.TMinus
//     | RIGHT_CHEVRON ->
//     if stream.Skip(EQUAL) then
//     Token.TGreaterThanEqual
//     else
//     Token.TGreaterThan
//     | EQUAL ->
//     stream.Skip(EQUAL)
//         Token.TEqual
//
//     | PLUS ->
//     Token.TPlus
//     | STAR ->
//     Token.TStar
//     | SINGLE_QUOTE ->
//     let mutable fin = false
//
//     let sb = StringBuilder()
//         while not fin do
//     match stream.Read() with
//     | BACK_SLASH when stream.Skip(SINGLE_QUOTE) ->
//     ignore<| sb.Append(SINGLE_QUOTE)
//     | SINGLE_QUOTE ->
//     fin<- true
//     | c ->
//     sb.Append(c)
//     if stream.IsEndOfStream then
//     error<- messageError $"Unterminated quoted string"
//     fin<- true
//
//     // token.String <- sb.ToString()
//     Token.TQuoted
//     // String literal
//     | DOUBLE_QUOTE ->
//     let mutable fin = false
//
//     let sb = StringBuilder()
//         while not fin do
//     match stream.Read() with
//     | BACK_SLASH when stream.Skip(DOUBLE_QUOTE) ->
//     ignore<| sb.Append(DOUBLE_QUOTE)
//     | DOUBLE_QUOTE ->
//     fin<- true
//     | c ->
//     sb.Append(c)
//     if stream.IsEndOfStream then
//     error<- messageError "Unterminated string literal"
//     fin<- true
//
//     // token.String <- sb.ToString()
//     Token.TString
//     // Number literal
//     | c when isDigit c ->
//     stream.Seek(token.Start)
//         let reply = pNumber stream
//     if reply.Status = Ok then
//
//     let result = reply.Result
//         if result.IsInteger then
//     let i = int result.String
//         lexed.Add i
//         Token.TInt
//
//     else
//     let f = float result.String
//         lexed.Add f
//         Token.TFloat
//
//     else
//     error<- reply.Error
//         Token.TError
//
//     // Word
//     | c ->
//     stream.Seek(token.Start)
//         let reply = pIdentifier stream
//     if reply.Status = Ok then
//
//     // token.String <- reply.Result
//     Token.TWord
//     else
//     error<- reply.Error
//         Token.TError
//
//     token.End<- stream.Index
//     if error = NoErrorMessages then
//
//     Reply(token)
//         else
//
//     Reply(ReplyStatus.Error, error)
//
//
//     let pTokens : Parser<ResizeArray<Lexeme>> =
//     fun stream ->
//     let stateTag = stream.StateTag
//     let mutable xs = ResizeArray<Lexeme>()
//         let mutable fin = false
//     let mutable reply = Reply(xs)
//         while (not fin) do
//     stream.SkipWhitespace()
//         let tokenReply = pToken stream
//     if tokenReply.Status = Ok then
//     if stateTag = stream.StateTag then
//     reply.Error<- messageError "infinite loop"
//     fin<- true
//     else
//     xs.Add tokenReply.Result
//     elif stateTag = stream.StateTag then
//         reply.Error<- messageError "infinite loop"
//     fin<- true
//     else
//     reply.Error<- tokenReply.Error
//         fin<- true
//
//     reply
//
//
//         let createResult startTime endTime parseResult : LexResult =
//
//     let error,
//         tokens =
//             match parseResult with
//     | Success(tokens, _, _) ->
//         "", tokens :> Lexeme seq
//     | Failure(msg, parserError, _) ->
//     msg, Seq.empty<Lexeme>
//         let result = createResult startTime endTime parseResult
//
//     result
//
//         let lexFile(encoding: Encoding) (file: string) =
//     let startTime = DateTimeOffset.Now
//     let lexed = LexResult.Empty
//     let parseResult = runParserOnFile pTokens lexed file encoding
//         let endTime = DateTimeOffset.Now
//         let result = createResult startTime endTime parseResult
//
//     result
//
//         let lexString(mzn: string) =
//     let startTime = DateTimeOffset.Now
//     let lexed = LexResult.Empty
//     let parseResult = runParserOnString pTokens lexed "" mzn
//         let endTime = DateTimeOffset.Now
//         let result = createResult startTime endTime parseResult
//
//     result
//
//         let lexStream(encoding: Encoding) (stream: IO.Stream) =
//     let startTime = DateTimeOffset.Now
//     let lexed = LexResult.Empty
//     let parseResult = runParserOnStream pTokens lexed "" stream encoding
//     let endTime = DateTimeOffset.Now
//     let result = createResult startTime endTime parseResult
//         result
//
// }
