namespace MiniZinc.Parser

open System.Collections.Generic
open MiniZinc
open FParsec
open System.Text

[<AutoOpen>]
module Keyword =
    
    type Keyword =
        | NONE = -1
        | ANNOTATION = 0
        | ANN = 1
        | ANY = 2
        | ARRAY = 3 
        | BOOL = 4 
        | CASE = 5
        | CONSTRAINT = 6
        | DEFAULT = 50
        | DIFF = 7
        | DIV = 8
        | ELSE = 9
        | ELSEIF = 10
        | ENDIF = 11
        | ENUM = 12
        | FALSE = 13
        | FLOAT = 14
        | FUNCTION = 15 
        | IF = 16
        | IN = 17
        | INCLUDE = 18 
        | INT = 19
        | INTERSECT = 20 
        | LET = 21
        | LIST = 22
        | MAXIMIZE = 23 
        | MINIMIZE = 24
        | MOD = 25
        | NOT = 26
        | OF = 27
        | OP = 28
        | OPT = 29
        | OUTPUT = 30
        | PAR = 31
        | PREDICATE = 32 
        | RECORD = 33
        | SATISFY = 34
        | SET = 35
        | SOLVE = 36
        | STRING = 37
        | SUBSET = 38
        | SUPERSET = 39
        | SYMDIFF = 40
        | TEST = 41
        | THEN = 42
        | TRUE = 43      
        | TUPLE = 44
        | TYPE = 45
        | UNION = 46       
        | VAR = 47
        | WHERE = 48
        | XOR = 49
        
    module Keyword =
        
        let list = [
            ("ann", Keyword.ANN)
            ("annotation", Keyword.ANNOTATION)
            ("any", Keyword.ANY)
            ("array", Keyword.ARRAY)
            ("bool", Keyword.BOOL)
            ("case", Keyword.CASE)
            ("constraint", Keyword.CONSTRAINT)
            ("default", Keyword.DEFAULT)
            ("diff", Keyword.DIFF)
            ("div", Keyword.DIV)
            ("else", Keyword.ELSE)
            ("elseif", Keyword.ELSEIF)
            ("endif", Keyword.ENDIF)
            ("enum", Keyword.ENUM)
            ("false", Keyword.FALSE)
            ("float", Keyword.FLOAT)
            ("function", Keyword.FUNCTION)
            ("if", Keyword.IF)
            ("in", Keyword.IN)
            ("include", Keyword.INCLUDE)
            ("int", Keyword.INT)
            ("intersect", Keyword.INTERSECT)
            ("let", Keyword.LET)
            ("list", Keyword.LIST)
            ("maximize", Keyword.MAXIMIZE)
            ("minimize", Keyword.MINIMIZE)
            ("mod", Keyword.MOD)
            ("not", Keyword.NOT)
            ("of", Keyword.OF)
            ("op", Keyword.OP)  
            ("opt", Keyword.OPT)
            ("output", Keyword.OUTPUT)
            ("par", Keyword.PAR)
            ("predicate", Keyword.PREDICATE)
            ("record", Keyword.RECORD)
            ("satisfy", Keyword.SATISFY)
            ("set", Keyword.SET)
            ("solve", Keyword.SOLVE)
            ("string", Keyword.STRING)
            ("subset", Keyword.SUBSET)
            ("superset", Keyword.SUPERSET)
            ("symdiff", Keyword.SYMDIFF)
            ("test", Keyword.TEST)
            ("then", Keyword.THEN)
            ("true", Keyword.TRUE)
            ("tuple", Keyword.TUPLE)
            ("type", Keyword.TYPE)
            ("union", Keyword.UNION)
            ("var", Keyword.VAR)
            ("where", Keyword.WHERE)
            ("xor", Keyword.XOR) ]
    
        let byName =
            Map.ofList list
        
        let byValue =
            list
            |> Seq.map (fun (k,v) -> (v,k))
            |> Map.ofSeq
            
        let lookup key =
            let mutable word = Keyword.NONE
            byName.TryGetValue(key, &word)
            word

type Context =
    | Expr = 1
    | TypeInst = 2
    | Let  = 3
    | Numeric = 4
    
type ParseOptions =
    { Debug: bool }
    static member Default =
        { Debug = false }
    
type ParserState(options: ParseOptions) =
    let sb = StringBuilder()
    let mutable indent = 0
    let mutable context = Unchecked.defaultof<Context>
            
    member this.Debug =
        options.Debug
            
    member this.Options =
        options
            
    member this.Indent
        with get() = indent
        and set x = indent <- x
    
    member this.UpdateContext x =
        let old = context
        context <- x
        old

    member this.Context
        with get() = context
        and set x = context <- x
               
    member this.Write (msg: string) =
        sb.AppendLine msg
        
    member this.DebugString =
        sb.ToString()

type Parser<'t> =
    Parser<'t, ParserState>
    
module ParseUtils =
        
    type CharStream with
    
        member this.SkipWs(x: char) =
            this.Skip(x)
            this.SkipWhitespace()
           
        member this.SkipWs(offset: int) =
            this.Skip(offset)
            this.SkipWhitespace()
    
        member this.SkipWs(x: string) =
            this.Skip(x)
            this.SkipWhitespace()
    
    [<Struct>]
    type ParseDebugEvent<'a> =
        | Enter
        | Leave of Reply<'a>

    let ws = spaces
        
    let ws1 = spaces1

    // Bind that allows whitespace    
    let (>>==) a b =
        (a .>> ws) >>= b
        
    let (>>>.) (p: Parser<'a,'u>) (q: Parser<'b,'u>) =
        fun stream ->
            let mutable reply1 = p stream
            if reply1.Status = Ok then
                stream.SkipWhitespace()
                if isNull reply1.Error then
                    // in separate branch because the JIT can produce better code for a tail call
                    q stream
                else
                    let stateTag1 = stream.StateTag
                    let mutable reply2 = q stream
                    if stateTag1 = stream.StateTag then
                        reply2.Error <- mergeErrors reply2.Error reply1.Error
                    reply2
            else
                Reply(reply1.Status, reply1.Error)

    let (.>>>) (p: Parser<'a,'u>) (q: Parser<'b,'u>) =
        fun stream ->
            let mutable reply1 = p stream
            if reply1.Status = Ok then
                stream.SkipWhitespace()
                let stateTag1 = stream.StateTag
                let reply2 = q stream
                let error = if isNull reply1.Error then reply2.Error
                            elif stateTag1 <> stream.StateTag then reply2.Error
                            else mergeErrors reply2.Error reply1.Error
                reply1.Error  <- error
                reply1.Status <- reply2.Status
            reply1        
            
        
    type P () =
        
        // Skip char and whitespace
        static member skip(c: char) =
            skipChar c .>> ws
            
        // Skip char and whitespace
        static member skip1(c: char) =
            skipChar c .>> ws1

        // Skip char and whitespace
        static member skip(k: Keyword) =
            let name = Keyword.byValue[k]
            skipString name >>. ws
        
        // Skip char and whitespace
        static member skip1(k: Keyword) =
            let name = Keyword.byValue[k]
            skipString name >>. ws1
        
        // Skip string and whitespace        
        static member skip(s: string) =
            skipString s .>> ws
            
        // Skip string and whitespace        
        static member skip1(s: string) =
            skipString s .>> ws1
                
        // Between with whitespace
        static member betweenWs (pStart : Parser<_>, pEnd : Parser<_>) =
            between (pStart .>> ws) (ws >>. pEnd)
                
        // Between chars with whitespace                
        static member betweenWs (s: char, e: char) =
            P.betweenWs (skipChar s, skipChar e)
            
        // Between strings with whitespace
        static member betweenWs (s: string, e: string) =
            P.betweenWs (skipString s, skipString e)
        
        // Between chars with whitespace        
        static member betweenWs (s: string) =
            P.betweenWs(s,s)
        
        // Between chars with whitespace    
        static member betweenWs (c: char) =
            P.betweenWs(c, c)
        
        /// pipe2 reimplemented to skip whitespace 
        static member pipe (a: Parser<_>, b: Parser<_>, f) =
            let optF = OptimizedClosures.FSharpFunc<_,_,_>.Adapt(f)
            fun stream ->
                let mutable reply = Reply()
                let reply1 = a stream
                let mutable error = reply1.Error
                if reply1.Status = Ok then
                    stream.SkipWhitespace()
                    let stateTag1 = stream.StateTag
                    let reply2 = b stream
                    error <- if stateTag1 <> stream.StateTag then reply2.Error
                             else mergeErrors error reply2.Error
                    if reply2.Status = Ok then
                         reply.Result <- optF.Invoke(reply1.Result, reply2.Result)
                         reply.Status <- Ok
                    else reply.Status <- reply2.Status
                else reply.Status <- reply1.Status
                reply.Error <- error
                reply
               
        /// pipe3 reimplemented to skip whitespace
        static member pipe (p1: Parser<_>, p2: Parser<_>, p3: Parser<_>, f) =
            let optF = OptimizedClosures.FSharpFunc<_,_,_,_>.Adapt(f)
            fun stream ->
                let mutable reply = Reply()
                let reply1 = p1 stream
                let mutable error = reply1.Error
                if reply1.Status = Ok then
                    stream.SkipWhitespace()
                    let stateTag1 = stream.StateTag
                    let reply2 = p2 stream
                    error <- if stateTag1 <> stream.StateTag then reply2.Error
                             else mergeErrors error reply2.Error
                    if reply2.Status = Ok then
                        stream.SkipWhitespace()
                        let stateTag2 = stream.StateTag
                        let reply3 = p3 stream
                        error <- if stateTag2 <> stream.StateTag then reply3.Error
                                 else mergeErrors error reply3.Error
                        if reply3.Status = Ok then
                             reply.Result <- optF.Invoke(reply1.Result, reply2.Result, reply3.Result)
                             reply.Status <- Ok
                        else reply.Status <- reply3.Status
                    else reply.Status <- reply2.Status
                else reply.Status <- reply1.Status
                reply.Error <- error
                reply
            
        /// pipe4 reimplemented to skip whitespace
        static member pipe (p1: Parser<_>, p2: Parser<_>, p3: Parser<_>, p4: Parser<_>, f) =
            let optF = OptimizedClosures.FSharpFunc<_,_,_,_,_>.Adapt(f)
            fun stream ->
                let mutable reply = Reply()
                let reply1 = p1 stream
                let mutable error = reply1.Error
                if reply1.Status = Ok then
                    stream.SkipWhitespace()
                    let stateTag1 = stream.StateTag
                    let reply2 = p2 stream
                    error <- if stateTag1 <> stream.StateTag then reply2.Error
                             else mergeErrors error reply2.Error
                    if reply2.Status = Ok then
                        stream.SkipWhitespace()
                        let stateTag2 = stream.StateTag
                        let reply3 = p3 stream
                        error <- if stateTag2 <> stream.StateTag then reply3.Error
                                 else mergeErrors error reply3.Error
                        if reply3.Status = Ok then
                            stream.SkipWhitespace()
                            let stateTag3 = stream.StateTag
                            let reply4 = p4 stream
                            error <- if stateTag3 <> stream.StateTag then reply4.Error
                                     else mergeErrors error reply4.Error
                            if reply4.Status = Ok then
                                 reply.Result <- optF.Invoke(reply1.Result, reply2.Result, reply3.Result, reply4.Result)
                                 reply.Status <- Ok
                            else reply.Status <- reply4.Status
                        else reply.Status <- reply3.Status
                    else reply.Status <- reply2.Status
                else reply.Status <- reply1.Status
                reply.Error <- error
                reply
            
        /// pipe5 reimplemented to skip whitespace
        static member pipe (p1: Parser<_>, p2: Parser<_>, p3: Parser<_>, p4: Parser<_>, p5: Parser<_>, f) =
            let optF = OptimizedClosures.FSharpFunc<_,_,_,_,_,_>.Adapt(f)
            fun stream ->
                let mutable reply = Reply()
                let reply1 = p1 stream
                let mutable error = reply1.Error
                if reply1.Status = Ok then
                    stream.SkipWhitespace()
                    let stateTag1 = stream.StateTag
                    let reply2 = p2 stream
                    error <- if stateTag1 <> stream.StateTag then reply2.Error
                             else mergeErrors error reply2.Error
                    if reply2.Status = Ok then
                        stream.SkipWhitespace()
                        let stateTag2 = stream.StateTag
                        let reply3 = p3 stream
                        error <- if stateTag2 <> stream.StateTag then reply3.Error
                                 else mergeErrors error reply3.Error
                        if reply3.Status = Ok then
                            stream.SkipWhitespace()
                            let stateTag3 = stream.StateTag
                            let reply4 = p4 stream
                            error <- if stateTag3 <> stream.StateTag then reply4.Error
                                     else mergeErrors error reply4.Error
                            if reply4.Status = Ok then
                                stream.SkipWhitespace()
                                let stateTag4 = stream.StateTag
                                let reply5 = p5 stream
                                error <- if stateTag4 <> stream.StateTag then reply5.Error
                                         else mergeErrors error reply5.Error
                                if reply5.Status = Ok then
                                     reply.Result <- optF.Invoke(reply1.Result, reply2.Result, reply3.Result, reply4.Result, reply5.Result)
                                     reply.Status <- Ok
                                else reply.Status <- reply5.Status     
                            else reply.Status <- reply4.Status
                        else reply.Status <- reply3.Status
                    else reply.Status <- reply2.Status
                else reply.Status <- reply1.Status
                reply.Error <- error
                reply
        
        /// 2Tuple that allows whitespace between elements
        static member tuple (p1: Parser<_>, p2: Parser<_>) =
            P.pipe(p1, p2, fun a b -> a,b)
        
        /// 3Tuple that allows whitespace between elements
        static member tuple (p1: Parser<'a>, p2: Parser<'b>, p3: Parser<'c>) =
            P.pipe(p1, p2, p3, fun a b c -> a,b,c)
       
        /// 2Tuple (struct) that allows whitespace between elements 
        static member structTuple (a: Parser<_>, b: Parser<_>) =
            P.pipe(a, b, fun a b -> struct(a,b))
        
        /// 3Tuple (struct) that allows whitespace between elements
        static member structTuple (p1: Parser<'a>, p2: Parser<'b>, p3: Parser<'c>) =
            P.pipe(p1, p2, p3, fun a b c -> struct(a,b,c))
               
        static member commaSep (p: Parser<_>) : Parser<_> =
            P.delimitedBy(',') p
            
        static member commaSep1 (p: Parser<_>) : Parser<_> =
            P.delimitedBy1(',') p
        
        static member delimitedBy (d: Parser<_>) : Parser<_> -> Parser<_> =
            fun p ->
                sepEndBy (p .>> ws) (d >>. ws)
            
        static member delimitedBy (c:char) =
            P.delimitedBy (skipChar c)
        
        static member delimitedBy1 (d: Parser<_>) : Parser<_> -> Parser<_> =
            fun p ->
                sepEndBy1 (p .>> ws) (d >>. ws)
        
        static member delimitedBy1(c:char) =
            P.delimitedBy1 (skipChar c)
            
        static member delimitedBy1(s:string) =
            P.delimitedBy1 (skipString s)
            
        static member betweenBrackets (p: Parser<_>) =
            P.betweenWs('(', ')') p
            
    open type P
        
    let addToDebug (stream: CharStream<ParserState>) label  event =
        let msgPadLen = 40
        let startIndent = stream.UserState.Indent
        let msg, indent, nextIndent = 
            match event with
            | Enter ->
                $"{label}", startIndent, startIndent+1
            | Leave res ->
                let str = $"{label} ({res.Status})"
                let pad = max (msgPadLen - startIndent - 1) 0
                let resStr = $"%s{str.PadRight(pad)} {res.Result}"
                resStr, startIndent-1, startIndent-1
      
        let indentStr =
            let pad = max indent 0
            if indent = 0 then ""
            else "-".PadRight(pad, '-')

        let row = stream.Position.Line.ToString().PadRight(3)
        let col = stream.Position.Column.ToString().PadRight(3)
        let posStr = $"{row}|{col}| "
        let posIdentStr = posStr + indentStr
        
        // The %A for res.Result makes it go onto multiple lines - pad them out correctly
        let replaceStr = "\n" + "".PadRight(max posStr.Length 0) + "".PadRight(max indent 0, '\u2502').PadRight(max msgPadLen 0)
        let correctedStr = msg.Replace("\n", replaceStr)
        let fullStr = $"%s{posIdentStr} %s{correctedStr}"

        stream.UserState.Write fullStr
        stream.UserState.Indent <- nextIndent

    // Add debug info to the given parser
    let (<!>) (p: Parser<'t>) (label: string) : Parser<'t> =
        let error = expected label
        fun stream ->
            if stream.UserState.Debug then
                let stateTag = stream.StateTag
                addToDebug stream label Enter
                let mutable reply = p stream
                if stateTag = stream.StateTag then
                    reply.Error <- error
                addToDebug stream label (Leave reply)
                reply
            else
                p stream
                