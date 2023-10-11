namespace MiniZinc

open System
open System.Text
open FParsec

module Lexer =
    
    let [<Literal>] HASH = '#'
    let [<Literal>] SLASH = '/'
    let [<Literal>] STAR = '*'
    
    type Parser<'t> = Parser<'t, unit>
      
    type TokenKind =
        | TError  = 0
        // Values
        | TWord  = 1
        | TInt   = 2
        | TFloat = 3
        | TString = 4
        | TLineComment = 5
        | TBlockComment = 6
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
        | TDoubleArrow      = 70// <->
        | TLeftArrow        = 71// <-
        | TRightArrow       = 72// -> 
        | TDownWedge        = 73// \/                
        | TUpWedge          = 74// /\
        | TLessThan         = 75// <
        | TGreaterThan      = 76// >
        | TLessThanEqual    = 77// <=
        | TGreaterThanEqual = 78// >=
        | TEqual            = 79// =
        | TDotDot           = 80// ..
        | TPlus             = 81// +
        | TMinus            = 82// -
        | TStar             = 83// *
        | TSlash            = 84// /
        | TTildeEquals      = 85// ~=
        | TTildePlus        = 86// ~+
        | TTildeMinus       = 87// ~-
        | TTildeStar        = 88// ~*
        // Symbol
        
        | SLeftBracket = 8
        | SRightBracket = 9
        | SLeftParen = 10
        | SRightParen = 11
    
    [<Struct>]
    type Token =
        { Kind    : TokenKind
        ; Line    : int
        ; Column  : int
        ; Index   : int64
        ; String  : string
        ; Int     : Nullable<int>
        ; Float   : Nullable<float> }
    
    let tString pos kind s =
        { Kind    = kind
        ; Line    = pos.Line
        ; Column  = pos.Column
        ; Index   = pos.Index
        ; String  = s
        ; Int     = Nullable()
        ; Float   = Nullable() }
        
    let tInt pos kind int
        { Kind    = kind
        ; Line    = pos.Line
        ; Column  = pos.Column
        ; Index   = pos.Index
        ; String  = s
        ; Int     = Nullable()
        ; Float   = Nullable()
        ; Keyword = Keyword.NONE }
    
        
    // Parse and remove comments from the given model string
    let parser : Parser<Token> =
        fun stream ->
            let mutable result = Reply(null)
            stream.SkipWhitespace()
            let pos = stream.Position
            let next = stream.Peek2()
            // Line comment
            if next.Char0 = HASH then
                stream.Skip()
                let mzn = stream.ReadRestOfLine()
                let token =
                    { Type = TokenType.LineComment
                    ; Mzn = mzn
                    ; Start = pos
                    ; End = stream.Position }
                reply.Result <- token
                reply.Status <- ReplyStatus.Ok
            // Block comment
            elif next.Char0 = SLASH && next.Char1 = STAR then
                stream.Skip(2)
                let mutable fin = false
                let sb = StringBuilder()
                while not fin do
                    match stream.Read() with
                    | STAR when stream.Peek() = SLASH ->
                        stream.Skip(2)
                        stream.SkipWhitespace()
                        fin <- true
                    | c ->
                        sb.Append c
                        fin <- stream.IsEndOfStream
                let token =
                    { Type = TokenType.BlockComment
                    ; Mzn = mzn
                    ; Start = pos
                    ; End = stream.Position }
                reply.Result <- token
                reply.Status <- ReplyStatus.Ok
            // Source code
            else
                                        
            
            let chars = 
            let mutable state = State.Mzn
            let mutable char = 'a'
            
            for c in mzn.AsSpan() do
                match state, c with
                
                | State.LineComment, '\n' ->
                    source.AppendLine()
                    state <- State.Mzn
                    
                | State.LineComment, _ ->
                    comments.Append c
                    state <- State.LineComment
                    
                | State.Mzn, '%' ->
                    state <- State.LineComment
                    
                | State.Mzn, '/' ->
                    state <- State.BlockOpenCheck
                    
                | State.BlockOpenCheck, '*' ->
                    state <- State.Block
                    
                | State.BlockOpenCheck, _ ->
                    source.Append '/'
                    source.Append c
                    state <- State.Mzn
                    ()
                
                | State.Block, '*' ->
                    state <- State.BlockCloseCheck
                    
                | State.BlockCloseCheck, '/' ->
                    state <- State.Mzn
                
                | State.BlockCloseCheck, _ ->
                    comments.Append '*'
                    state <- State.Block
                   
                | State.Block, _ ->
                    comments.Append c
                    state <- State.Block
                    
                | State.Mzn, '"' ->
                    source.Append '"'
                    state <- State.StringLit
                    
                | State.StringLit, '\\' ->
                    source.Append '\\'
                    state <- State.StringEscape
                    
                | State.StringEscape, _ ->
                    source.Append c
                    state <- State.StringLit
                    
                | State.StringLit, '"' ->
                    source.Append '"'
                    state <- State.Mzn
                    
                | State.StringLit, _ ->
                    source.Append c
                    ()
                    
                | State.Mzn, '\n' when char = '\n' ->
                    ()
                    
                | State.Mzn, _ ->
                    source.Append c
                    ()
        
                char <- c                
                    

            let source = source.ToString().Trim()
            let comments = comments.ToString()
            source, comments