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
    TTilde,
    TBackSlash,
    TForwardSlash,
    TColon,
    TDelimiter,
    TEmpty
}

public readonly struct Token
{
    public Kind Kind { get; init; }
    public int Line { get; init; }
    public int Col { get; init; }
    public int Start { get; init; }
    public int Length { get; init; }
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
    const char EOF = '\uffff';

    public DateTime StartTime { get; private set; }
    public DateTime EndTime { get; private set; }
    public TimeSpan Duration { get; private set; }

    private int _line;
    private int _col;
    private int _index;
    private Kind _kind;
    private int _start;
    private readonly StreamReader _stream;

    public Lexer(StreamReader stream)
    {
        StartTime = DateTime.Now;
        _stream = stream;
    }

    public Token Next()
    {
        _start = _index;
        var c = (char)_stream.Read();
        switch (c)
        {
            case HASH:
                break;
            case FWD_SLASH:
                break;
            case BACK_SLASH:
                break;
            case STAR:
                break;
            case DELIMITER:
                break;
            case EQUAL:
                break;
            case LEFT_CHEVRON:
                break;
            case RIGHT_CHEVRON:
                break;
            case UP_CHEVRON:
                break;
            case DOT:
                break;
            case PLUS:
                break;
            case MINUS:
                break;
            case TILDE:
                break;
            case LEFT_BRACK:
                break;
            case RIGHT_BRACK:
                break;
            case LEFT_PAREN:
                break;
            case RIGHT_PAREN:
                break;
            case LEFT_BRACE:
                break;
            case RIGHT_BRACE:
                break;
            case PERCENT:
                break;
            case UNDERSCORE:
                break;
            case SINGLE_QUOTE:
                break;
            case DOUBLE_QUOTE:
                break;
            case BACKTICK:
                break;
            case COLON:
                break;
            case EOF:
                break;
        }

        var token = new Token
        {
            Kind = _kind,
            Line = _line,
            Col = _col,
            Start = _start,
            Length = _index - _start
        };
        return token;
    }

    public void Dispose()
    {
        _stream.Dispose();
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
