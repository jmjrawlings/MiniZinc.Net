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
                                
    type Parser<'t> = Parser<'t, LexResult>
          
    type Token =
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
        | TEmpty           = 110 // <>
        
        
    [<Struct>]
    type Lexeme =
        { mutable Kind    : Token
        ; mutable Line    : int64
        ; mutable Column  : int64
        ; mutable Start   : int64
        ; mutable End     : int64 }

    let inline nullOf<'t> =
        Unchecked.defaultof<'t>
        
    type LexResult =          
        { Source    : string
        ; StartTime : DateTime
        ; EndTime   : DateTime
        ; Duration  : TimeSpan
        ; mutable Lexemes  : ResizeArray<Lexeme>
        ; mutable Strings : ResizeArray<string>
        ; mutable Ints    : ResizeArray<int>
        ; mutable Floats  : ResizeArray<float>
        ; mutable Error   : string }
        
        static member Empty =
            { Source    = null
            ; StartTime = DateTime.Now 
            ; EndTime   = DateTime.Now   
            ; Duration  = TimeSpan.Zero  
            ; Lexemes    = ResizeArray()   
            ; Strings   = nullOf   
            ; Ints      = nullOf       
            ; Floats    = nullOf    
            ; Error     = nullOf }
        
        member this.Add(x : string) =
            if isNull this.Strings then
                this.Strings <- ResizeArray()
            this.Strings.Add x
            
        member this.Add(x : int) =
            if isNull this.Ints then
                this.Ints <- ResizeArray()
            this.Ints.Add x
                    
        member this.Add(x : float) =
            if isNull this.Floats then
                this.Floats <- ResizeArray()
            this.Floats.Add x
        
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
    let pToken : Parser<Lexeme> =
        fun stream ->
            Console.WriteLine($"{stream.Index}")
            let mutable token = Unchecked.defaultof<Lexeme>
            let mutable error = NoErrorMessages
            let lexed = stream.UserState
            let pos = stream.Position
            token.Line <- pos.Line
            token.Column <- pos.Column
            token.Start <- pos.Index
            token.Kind <- 
                match stream.Read() with
                | DELIMITER ->
                    Token.TDelimiter
                | LEFT_BRACK ->
                    Token.TLeftBracket  
                | RIGHT_BRACK ->
                    Token.TRightBracket 
                | LEFT_PAREN ->
                    Token.TLeftParen 
                | RIGHT_PAREN ->
                    Token.TRightParen 
                | LEFT_BRACE ->
                    Token.TLeftBrace 
                | RIGHT_BRACE ->
                    Token.TRightBrace
                | COLON ->
                    Token.TColon
                | DOT ->
                    if stream.Skip(DOT) then
                        Token.TDotDot
                    else
                        Token.TDot
                | TILDE ->
                    match stream.Read() with
                    | EQUAL ->
                        Token.TTildeEquals
                    | PLUS ->
                        Token.TTildePlus
                    | MINUS ->
                        Token.TTildeMinus
                    | STAR ->
                        Token.TTildeStar
                    | _   ->
                        stream.Seek(token.Start)
                        Token.TTilde
                | BACK_SLASH ->
                    Token.TBackSlash
                // Line comment
                | HASH ->
                    let string = stream.ReadRestOfLine(false)
                    lexed.Strings.Add string
                    Token.TLineComment
                // Block comment
                | FWD_SLASH when stream.Skip(STAR) ->
                    let mutable fin = false
                    // let sb = StringBuilder()
                    while not fin do
                        match stream.Read() with
                        | STAR when stream.Peek() = FWD_SLASH ->
                            stream.Skip(2)
                            stream.SkipWhitespace()
                            fin <- true
                        | c ->
                            // sb.Append c
                            fin <- stream.IsEndOfStream
                    Token.TBlockComment                    
                | FWD_SLASH ->
                    Token.TForwardSlash
                | LEFT_CHEVRON ->
                    if stream.Skip(MINUS) then
                        if stream.Skip(RIGHT_CHEVRON) then
                            Token.TDoubleArrow
                        else
                            Token.TLeftArrow
                    elif stream.Skip(EQUAL) then
                        Token.TLessThan
                    elif stream.Skip(RIGHT_CHEVRON) then
                        Token.TEmpty                        
                    else
                        Token.TLessThan
                | MINUS ->
                    if stream.Skip(RIGHT_CHEVRON) then
                        Token.TRightArrow
                    else
                        Token.TMinus
                | RIGHT_CHEVRON ->
                    if stream.Skip(EQUAL) then
                        Token.TGreaterThanEqual
                    else
                        Token.TGreaterThan
                | EQUAL ->
                    stream.Skip(EQUAL)
                    Token.TEqual
                | PLUS ->
                    Token.TPlus
                | STAR ->
                    Token.TStar
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
                    // token.String <- sb.ToString()
                    Token.TQuoted
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
                    // token.String <- sb.ToString()
                    Token.TString
                // Number literal                    
                | c when isDigit c ->
                    stream.Seek(token.Start)
                    let reply = pNumber stream
                    if reply.Status = Ok then
                        let result = reply.Result
                        if result.IsInteger then
                            let i = int result.String
                            lexed.Add i
                            Token.TInt
                        else
                            let f = float result.String
                            lexed.Add f
                            Token.TFloat
                    else
                        error <- reply.Error
                        Token.TError
                // Word
                | c ->
                    stream.Seek(token.Start)
                    let reply = pIdentifier stream
                    if reply.Status = Ok then
                        // token.String <- reply.Result
                        Token.TWord
                    else
                        error <- reply.Error
                        Token.TError

            token.End <- stream.Index
            if error = NoErrorMessages then
                Reply(token)
            else
                Reply(ReplyStatus.Error, error)
                
            
    let pTokens : Parser<ResizeArray<Lexeme>> =
        fun stream ->
            let stateTag = stream.StateTag
            let mutable xs = ResizeArray<Lexeme>()
            let mutable fin = false
            let mutable reply = Reply(xs)
            while (not fin) do
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
                 "", tokens :> Lexeme seq
            | Failure(msg, parserError, _) ->
                msg, Seq.empty<Lexeme>
        let result = createResult startTime endTime parseResult
        result        
                
    let lexFile (encoding: Encoding) (file: string) =
        let startTime = DateTimeOffset.Now
        let lexed = LexResult.Empty
        let parseResult = runParserOnFile pTokens lexed file encoding
        let endTime = DateTimeOffset.Now
        let result = createResult startTime endTime parseResult
        result                
    
    let lexString (mzn: string) =
        let startTime = DateTimeOffset.Now
        let lexed = LexResult.Empty
        let parseResult = runParserOnString pTokens lexed "" mzn
        let endTime = DateTimeOffset.Now
        let result = createResult startTime endTime parseResult
        result
    
    let lexStream (encoding: Encoding) (stream: IO.Stream) =
        let startTime = DateTimeOffset.Now
        let lexed = LexResult.Empty
        let parseResult = runParserOnStream pTokens lexed "" stream encoding
        let endTime = DateTimeOffset.Now
        let result = createResult startTime endTime parseResult
        result