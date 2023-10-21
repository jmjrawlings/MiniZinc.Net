#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
namespace MiniZinc.Net;

using System;
using System.Collections.Generic;
using System.Text;

public enum Kind
{
    TNone,
    TError,
    TWord,
    TInt,
    TFloat,
    TString,
    TLineComment,
    TBlockComment,
    TQuoted,
    KAnnotation,
    KAnn,
    KAny,
    KArray,
    KBool,
    KCase,
    KConstraint,
    KDefault,
    KDiff,
    KDiv,
    KElse,
    KElseif,
    KEndif,
    KEnum,
    KFalse,
    KFloat,
    KFunction,
    KIf,
    KIn,
    KInclude,
    KInt,
    KIntersect,
    KLet,
    KList,
    KMaximize,
    KMinimize,
    KMod,
    KNot,
    KOf,
    KOp,
    KOpt,
    KOutput,
    KPar,
    KPredicate,
    KRecord,
    KSatisfy,
    KSet,
    KSolve,
    KString,
    KSubset,
    KSuperset,
    KSymdiff,
    KTest,
    KThen,
    KTrue,
    KTuple,
    KType,
    KUnion,
    KVar,
    KWhere,
    KXor,
    TDoubleArrow,
    TLeftArrow,
    TRightArrow,
    TDownWedge,
    TUpWedge,
    TLessThan,
    TGreaterThan,
    TLessThanEqual,
    TGreaterThanEqual,
    TEqual,
    TDotDot,
    TPlus,
    TMinus,
    TStar,
    TSlash,
    TPlusPlus,
    TTildeEquals,
    TTildePlus,
    TTildeMinus,
    TTildeStar,
    TLeftBracket,
    TRightBracket,
    TLeftParen,
    TRightParen,
    TLeftBrace,
    TRightBrace,
    TDot,
    TPercent,
    TUnderscore,
    TTilde,
    TBackSlash,
    TForwardSlash,
    TColon,
    TDelimiter,
    TEmpty
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
    const char PERCENT = '%';
    const char UNDERSCORE = '_';
    const char SINGLE_QUOTE = '\'';
    const char DOUBLE_QUOTE = '"';
    const char BACKTICK = '`';
    const char COLON = ':';
    const char NEWLINE = '\n';
    const char RETURN = '\r';
    const char NULL = '\x00';

    const string ERR_QUOTED_IDENT = "Invalid quoted identifier";
    const string ERR_ESCAPED_STRING = "String was not escaped properly";

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
    private readonly bool _debug;
    private readonly StreamReader _sr;
    private readonly StringBuilder _sb;

    public Lexer(StreamReader sr)
    {
        _startTime = DateTime.Now;
        _sr = sr;
        _sb = new StringBuilder();
    }

    private void Next()
    {
        _last = _char;
        _char = (char)_sr.Read();
        _index++;
    }

    private void Store()
    {
        _sb.Append(_char);
    }

    private char Peek()
    {
        var c = (char)_sr.Peek();
        return c;
    }

    private bool Skip(char c)
    {
        var p = Peek();
        if (p != c)
            return false;
        Next();
        return true;
    }

    /// Skip the next char
    private void Skip()
    {
        _sr.Read();
        _index++;
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

    public IEnumerable<Token> ReadToEnd()
    {
        while (true)
        {
            var token = Read();
            if (!token.HasValue)
                break;

            yield return token.Value;
            if (_error is null)
                break;
        }
    }

    public Token? Read()
    {
        _start = _index;
        _offset = 0;

        if (!CharsRemain)
            return null;

        Next();
        switch (_char)
        {
            case HASH:
                ReadLineComment();
                break;
            case FWD_SLASH when Skip(STAR):
                ReadBlockComment();
                break;
            case BACK_SLASH:
                _kind = Kind.TBackSlash;
                break;
            case STAR:
                _kind = Kind.TStar;
                break;
            case DELIMITER:
                _kind = Kind.TDelimiter;
                break;
            case EQUAL:
                Skip(EQUAL);
                _kind = Kind.TTildeEquals;
                break;
            case LEFT_CHEVRON:
                break;
            case RIGHT_CHEVRON:
                break;
            case UP_CHEVRON:
                break;
            case DOT:
                _kind = Skip(DOT) ? Kind.TDotDot : Kind.TDot;
                break;
            case PLUS:
                _kind = Skip(PLUS) ? Kind.TPlusPlus : Kind.TPlus;
                break;
            case MINUS:
                _kind = Kind.TMinus;
                break;
            case TILDE:
                _kind = Peek() switch
                {
                    MINUS => Kind.TTildeMinus,
                    PLUS => Kind.TTildePlus,
                    STAR => Kind.TTildeStar,
                    EQUAL => Kind.TTildeEquals,
                    _ => Kind.TTilde
                };

                if (_kind != Kind.TTilde)
                    Skip();

                break;
            case LEFT_BRACK:
                _kind = Kind.TLeftBracket;
                break;
            case RIGHT_BRACK:
                _kind = Kind.TRightBracket;
                break;
            case LEFT_PAREN:
                _kind = Kind.TLeftParen;
                break;
            case RIGHT_PAREN:
                _kind = Kind.TRightParen;
                break;
            case LEFT_BRACE:
                _kind = Kind.TLeftBrace;
                break;
            case RIGHT_BRACE:
                _kind = Kind.TRightBrace;
                break;
            case PERCENT:
                _kind = Kind.TPercent;
                break;
            case UNDERSCORE:
                _kind = Kind.TUnderscore;
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
            default:
                if (char.IsDigit(_char))
                    ReadNumber();
                break;
        }

        Kind kind = _kind;
        string? s = _string;
        if (_error is not null)
        {
            kind = Kind.TError;
            s = _error;
        }

        var token = new Token
        {
            Kind = kind,
            Line = _line,
            Col = _col,
            Start = _start,
            Length = _offset,
            String = s
        };

        return token;
    }

    private void ReadNumber() { }

    private void ReadStringLiteral()
    {
        bool ok = false;
        bool fin = false;
        while (CharsRemain && !fin)
        {
            Next();
            if (_char is DOUBLE_QUOTE)
            {
                ok = true;
                fin = true;
            }
            else if (_char is BACK_SLASH)
            {
                Store();
                Next();
                Store();
                switch (_char)
                {
                    case BACK_SLASH:
                    case DOUBLE_QUOTE:
                        break;
                    default:
                        _error = ERR_ESCAPED_STRING;
                        fin = true;
                        break;
                }
            }
            else
            {
                Store();
            }
        }
        _kind = Kind.TString;
        _string = String();
    }

    private void ReadQuotedIdentifier()
    {
        bool ok = false;
        bool fin = false;
        while (CharsRemain && !fin)
        {
            Next();
            switch (_char)
            {
                case SINGLE_QUOTE:
                    fin = true;
                    ok = true;
                    break;
                case BACK_SLASH:
                case RETURN:
                case NEWLINE:
                    fin = true;
                    break;
                default:
                    Store();
                    break;
            }
        }

        if (!ok)
            Error(ERR_QUOTED_IDENT);

        _kind = Kind.TQuoted;
        _string = String();
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
    private string String()
    {
        var s = _sb.ToString();
        _sb.Clear();
        return s;
    }

    private bool CharsRemain => !_sr.EndOfStream;

    private void ReadBlockComment()
    {
        bool ok = false;
        while (CharsRemain)
        {
            Next();
            if (_char is STAR && Skip(FWD_SLASH))
            {
                ok = true;
                break;
            }
            Store();
        }

        _kind = Kind.TBlockComment;
        _string = String();
        ErrorIf(!ok, "Unclosed block comment");
    }

    private void ReadLineComment()
    {
        while (CharsRemain)
        {
            Next();
            Store();
            if (_char is NEWLINE)
                break;
        }

        _kind = Kind.TLineComment;
        _string = String();
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
