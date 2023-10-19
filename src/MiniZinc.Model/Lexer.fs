namespace MiniZinc

open System
open System.Collections.Generic
open System.Text
open FParsec

module rec Lexer =
        
    let [<Literal>] HASH = '#'
    let [<Literal>] FWD_SLASH = '/'
    let [<Literal>] BACK_SLASH = '\\'
    let [<Literal>] STAR = '*'
    let [<Literal>] DELIMITER = ';'
    let [<Literal>] EQUAL = '='
    let [<Literal>] LEFT_CHEVRON = '<'
    let [<Literal>] RIGHT_CHEVRON = '>'
    let [<Literal>] UP_CHEVRON = '^'
    let [<Literal>] DOT = '.'
    let [<Literal>] PLUS = '+'
    let [<Literal>] MINUS = '-'
    let [<Literal>] TILDE = '~'
    let [<Literal>] LEFT_BRACK= '[' 
    let [<Literal>] RIGHT_BRACK = ']'
    let [<Literal>] LEFT_PAREN = '('
    let [<Literal>] RIGHT_PAREN = ')'
    let [<Literal>] LEFT_BRACE = '{'
    let [<Literal>] RIGHT_BRACE = '}'
    let [<Literal>] PERCENT = '%'
    let [<Literal>] UNDERSCORE = '_'
    let [<Literal>] SINGLE_QUOTE = '''
    let [<Literal>] DOUBLE_QUOTE = '"'
    let [<Literal>] BACKTICK = '`'
    let [<Literal>] COLON = ':'
    let [<Literal>] EOF = '\uffff'
                                
    type Parser<'t> = Parser<'t, unit>
            
    type LexResult =
        { StartTime : DateTime
        ; EndTime   : DateTime
        ; Duration  : TimeSpan
        ; Error     : string
        ; Tokens    : IEnumerable<Token> }
          
    type TokenKind =
        | TError  = 0
        // Values
        | TWord  = 1
        | TInt   = 2
        | TFloat = 3
        | TString = 4
        | TLineComment = 5
        | TBlockComment = 6
        | TQuoted = 7
        // Keywords
        | KAnnotation  = 10 
        | KAnn         = 11
        | KAny         = 12
        | KArray       = 13 
        | KBool        = 14
        | KCase        = 15 
        | KConstraint  = 16 
        | KDefault     = 17
        | KDiff        = 18
        | KDiv         = 19
        | KElse        = 20
        | KElseif      = 21
        | KEndif       = 22
        | KEnum        = 23
        | KFalse       = 24
        | KFloat       = 25
        | KFunction    = 26
        | KIf          = 27
        | KIn          = 28
        | KInclude     = 29 
        | KInt         = 30
        | KIntersect   = 31 
        | KLet         = 32
        | KList        = 33
        | KMaximize    = 34
        | KMinimize    = 35
        | KMod         = 36
        | KNot         = 37
        | KOf          = 38
        | KOp          = 39
        | KOpt         = 40
        | KOutput      = 41
        | KPar         = 42
        | KPredicate   = 43 
        | KRecord      = 44
        | KSatisfy     = 45
        | KSet         = 46
        | KSolve       = 47
        | KString      = 48
        | KSubset      = 49
        | KSuperset    = 50
        | KSymdiff     = 51
        | KTest        = 52
        | KThen        = 53
        | KTrue        = 54     
        | KTuple       = 55
        | KType        = 56
        | KUnion       = 57       
        | KVar         = 58
        | KWhere       = 59
        | KXor         = 60
        // Operators
        | TDoubleArrow      = 70 // <->
        | TLeftArrow        = 71 // <-
        | TRightArrow       = 72 // -> 
        | TDownWedge        = 73 // \/                
        | TUpWedge          = 74 // /\
        | TLessThan         = 75 // <
        | TGreaterThan      = 76 // >
        | TLessThanEqual    = 77 // <=
        | TGreaterThanEqual = 78 // >=
        | TEqual            = 79 // =
        | TDotDot           = 80 // ..
        | TPlus             = 81 // +
        | TMinus            = 82 // -
        | TStar             = 83 // *
        | TSlash            = 84 // /
        | TTildeEquals      = 85 // ~=
        | TTildePlus        = 86 // ~+
        | TTildeMinus       = 87 // ~-
        | TTildeStar        = 88 // ~*
        // Symbol
        | TLeftBracket     = 100 // [ 
        | TRightBracket    = 101 // ]
        | TLeftParen       = 102 // (
        | TRightParen      = 103 // )
        | TLeftBrace       = 102 // {
        | TRightBrace      = 103 // }
        | TDot             = 104 // .
        | TTilde           = 105 // ~
        | TBackSlash       = 106 // \                
        | TForwardSlash    = 107 // /
        | TColon           = 108 // :
        | TDelimiter       = 109 // ;
        
    [<Struct>]
    type Token =
        { mutable Kind    : TokenKind
        ; mutable Line    : int64
        ; mutable Column  : int64
        ; mutable Index   : int64
        ; mutable Length  : int64
        ; mutable String  : string
        ; mutable Int     : int
        ; mutable Float   : float }
        
    let pInteger : Parser<int> =
        pint32
        
    let pFloat : Parser<float> =
        pfloat
        
    let pIdentifier : Parser<Ident> =
        let options = IdentifierOptions(isAsciiIdStart = fun c -> c = '_' || isLetter c)
        identifier options
        
    let pNumber : Parser<NumberLiteral> =
        let format =
           NumberLiteralOptions.AllowFraction
           ||| NumberLiteralOptions.AllowExponent
           ||| NumberLiteralOptions.AllowHexadecimal
           ||| NumberLiteralOptions.AllowOctal
        numberLiteral format "number"
    
    // Parse and remove comments from the given model string
    let pToken : Parser<Token> =
        fun stream ->
            let mutable token = Unchecked.defaultof<Token>
            let mutable error = NoErrorMessages
            let pos = stream.Position
            token.Line <- pos.Line
            token.Column <- pos.Column
            token.Index <- pos.Index
            token.Kind <- 
                match stream.Read() with
                | DELIMITER ->
                    TokenKind.TDelimiter
                | LEFT_BRACK ->
                    TokenKind.TLeftBracket  
                | RIGHT_BRACK ->
                    TokenKind.TRightBracket 
                | LEFT_PAREN ->
                    TokenKind.TLeftParen 
                | RIGHT_PAREN ->
                    TokenKind.TRightParen 
                | LEFT_BRACE ->
                    TokenKind.TLeftBrace 
                | RIGHT_BRACE ->
                    TokenKind.TRightBrace
                | COLON ->
                    TokenKind.TColon
                | DOT ->
                    if stream.Skip(DOT) then
                        TokenKind.TDotDot
                    else
                        TokenKind.TDot
                | TILDE ->
                    match stream.Read() with
                    | EQUAL ->
                        TokenKind.TTildeEquals
                    | PLUS ->
                        TokenKind.TTildePlus
                    | MINUS ->
                        TokenKind.TTildeMinus
                    | STAR ->
                        TokenKind.TTildeStar
                    | _   ->
                        stream.Seek(token.Index)
                        TokenKind.TTilde
                | BACK_SLASH ->
                    TokenKind.TBackSlash
                // Line comment
                | HASH ->
                    token.String <- stream.ReadRestOfLine(false)
                    TokenKind.TLineComment
                // Block comment
                | FWD_SLASH when stream.Skip(STAR) ->
                    let mutable fin = false
                    let sb = StringBuilder()
                    while not fin do
                        match stream.Read() with
                        | STAR when stream.Peek() = FWD_SLASH ->
                            stream.Skip(2)
                            stream.SkipWhitespace()
                            fin <- true
                        | c ->
                            sb.Append c
                            fin <- stream.IsEndOfStream
                    token.String <- sb.ToString()
                    TokenKind.TBlockComment                    
                | FWD_SLASH ->
                    TokenKind.TForwardSlash
                | LEFT_CHEVRON ->
                    if stream.Skip(MINUS) then
                        if stream.Skip(RIGHT_CHEVRON) then
                            TokenKind.TDoubleArrow
                        else
                            TokenKind.TLeftArrow
                    elif stream.Skip(EQUAL) then
                        TokenKind.TLessThan
                    else
                        TokenKind.TLessThan
                | MINUS ->
                    if stream.Skip(RIGHT_CHEVRON) then
                        TokenKind.TRightArrow
                    else
                        TokenKind.TMinus
                | RIGHT_CHEVRON ->
                    if stream.Skip(EQUAL) then
                        TokenKind.TGreaterThanEqual
                    else
                        TokenKind.TGreaterThan
                | EQUAL ->
                    stream.Skip(EQUAL)
                    TokenKind.TEqual
                | PLUS ->
                    TokenKind.TPlus
                | STAR ->
                    TokenKind.TStar
                | SINGLE_QUOTE ->
                    let mutable fin = false
                    let sb = StringBuilder()
                    while not fin do
                        match stream.Read() with
                        | BACK_SLASH when stream.Skip(SINGLE_QUOTE) ->
                            ignore <| sb.Append(SINGLE_QUOTE)
                        | SINGLE_QUOTE ->
                            fin <- true
                        | c ->
                            sb.Append(c)
                            if stream.IsEndOfStream then
                                error <- messageError $"Unterminated quoted string"
                                fin <- true
                    token.String <- sb.ToString()
                    TokenKind.TQuoted
                // String literal                    
                | DOUBLE_QUOTE ->
                    let mutable fin = false
                    let sb = StringBuilder()
                    while not fin do
                        match stream.Read() with
                        | BACK_SLASH when stream.Skip(DOUBLE_QUOTE) ->
                            ignore <| sb.Append(DOUBLE_QUOTE)
                        | DOUBLE_QUOTE ->
                            fin <- true
                        | c ->
                            sb.Append(c)
                            if stream.IsEndOfStream then
                                error <- messageError "Unterminated string literal"
                                fin <- true
                    token.String <- sb.ToString()
                    TokenKind.TString
                // Number literal                    
                | c when isDigit c ->
                    stream.Seek(token.Index)
                    let reply = pNumber stream
                    if reply.Status = Ok then
                        let result = reply.Result
                        if result.IsInteger then
                            token.Int <- (int result.String)
                            TokenKind.TInt
                        else
                            token.Float <- (float result.String)
                            TokenKind.TFloat
                    else
                        error <- reply.Error
                        TokenKind.TError
                // Word
                | c ->
                    stream.Seek(token.Index)
                    let reply = pIdentifier stream
                    if reply.Status = Ok then
                        token.String <- reply.Result
                        TokenKind.TWord
                    else
                        error <- reply.Error
                        TokenKind.TError

            token.Length = stream.Index - token.Index
            if error = NoErrorMessages then
                Reply(token)
            else
                Reply(ReplyStatus.Error, error)
                
            
    let pTokens : Parser<ResizeArray<Token>> =
        fun stream ->
            let stateTag = stream.StateTag
            let mutable xs = ResizeArray<Token>()
            let mutable fin = false
            let mutable reply = Reply(xs)
            while fin do
                stream.SkipWhitespace()
                let tokenReply = pToken stream
                if tokenReply.Status = Ok then
                    if stateTag = stream.StateTag then
                        reply.Error <- messageError "infinite loop"
                        fin <- true
                    else
                        xs.Add tokenReply.Result                    
                elif stateTag = stream.StateTag then
                    reply.Error <- messageError "infinite loop"
                    fin <- true
                else
                    reply.Error <- tokenReply.Error
                    fin <- true
            reply                    
                    
        
    let createResult startTime endTime parseResult : LexResult =
        let error, tokens =
            match parseResult with
            | Success(tokens, _, _) ->
                 "", tokens :> Token seq
            | Failure(msg, parserError, _) ->
                msg, Seq.empty<Token>
        let result = createResult startTime endTime parseResult
        result        
                
    let lexFile (encoding: Encoding) (file: string) =
        let startTime = DateTimeOffset.Now
        let parseResult = runParserOnFile pTokens () file encoding
        let endTime = DateTimeOffset.Now
        let result = createResult startTime endTime parseResult
        result                
    
    let lexString (mzn: string) =
        let startTime = DateTimeOffset.Now
        let parseResult = runParserOnString pTokens () "" mzn
        let endTime = DateTimeOffset.Now
        let result = createResult startTime endTime parseResult
        result
    
    let lexStream (encoding: Encoding) (stream: IO.Stream) =
        let startTime = DateTimeOffset.Now
        let parseResult = runParserOnStream pTokens () "" stream encoding
        let endTime = DateTimeOffset.Now
        let result = createResult startTime endTime parseResult
        result