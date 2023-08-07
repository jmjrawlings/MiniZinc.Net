(*

Parse.fs

Contains functions and values that enable the
parsing of a MiniZinc model/file into an
Abstract Syntax Tree (AST).

*)

namespace MiniZinc

open System
open System.IO
open System.Runtime.CompilerServices
open System.Runtime.InteropServices
open System.Text
open FParsec
open MiniZinc
open FParsec.Primitives

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
            ("annotation", Keyword.ANNOTATION)
            ("any", Keyword.ANY)
            ("array", Keyword.ARRAY)
            ("bool", Keyword.BOOL)
            ("case", Keyword.CASE)
            ("constraint", Keyword.CONSTRAINT)
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
            
            
module Operator =
    
    let list =
        [ ("+", Op.Add)
          ("-", Op.Subtract)
          ("*", Op.Multiply)
          ("/", Op.Divide)       
          ("^", Op.Exponent)
          ("~+", Op.TildeAdd)
          ("~-", Op.TildeSubtract)
          ("~*", Op.TildeMultiply)
          ("~/", Op.TildeDivide)
          ("div", Op.Div)
          ("mod", Op.Mod)
          ("~div", Op.TildeDiv) 
          ("<->", Op.Equivalent)
          ("->", Op.Implies)
          ("<-", Op.ImpliedBy)
          ("\/", Op.Or)
          ("/\\", Op.And)
          ("<=", Op.LessThanEqual)
          (">=", Op.GreaterThanEqual)
          ("==", Op.EqualEqual)
          ("<", Op.LessThan)
          (">", Op.GreaterThan)
          ("=", Op.Equal)
          ("!=", Op.NotEqual)
          ("~=", Op.TildeEqual)
          ("~!=", Op.TildeNotEqual)
          ("..", Op.DotDot)
          ("++", Op.PlusPlus)
          ("xor", Op.Xor)
          ("intersect", Op.Intersect)
          ("in", Op.In)
          ("subset", Op.Subset)
          ("superset", Op.Superset)
          ("union", Op.Union)
          ("diff", Op.Diff)
          ("symdiff", Op.SymDiff)
          ("default", Op.Default)
          ("not", Op.Not) ]
    
    let byName =
        Map.ofList list
    
    let byValue =
        list
        |> Seq.map (fun (k,v) -> (v,k))
        |> Map.ofSeq

    let byInt =
        list
        |> Seq.map (fun (k,v) -> (int v,k))
        |> Map.ofSeq
    
type ParserState() =
    let sb = StringBuilder()
    let mutable indent = 0

    member this.Indent
        with get() = indent
        and set(v : int) = indent <- v
            
    member this.write (msg: string) =
        sb.AppendLine msg
        
    member this.Message =
        sb.ToString()

type Parser<'t> =
    Parser<'t, ParserState>
    
module Parsers =
    
    open Keyword
    
    [<Struct>]
    type ParseDebugEvent<'a> =
        | Enter
        | Leave of Reply<'a>

    let ws = spaces
        
    let ws1 = spaces1
    
    let (>>==) a b =
        (a .>> ws) >>= b
        
    type private P () =
        
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
            skipString s  .>> ws
            
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

        static member pipe (a: Parser<'a>, b: Parser<'b>, f) =
            pipe2 (a .>> ws) b f
               
        static member pipe (a: Parser<'a>, b: Parser<'b>, c: Parser<'c>, f) =
            pipe3 (a .>> ws) (b .>> ws) c f
            
        static member pipe (a: Parser<'a>, b: Parser<'b>, c: Parser<'c>, d: Parser<'d>, f) =
            pipe4 (a .>> ws) (b .>> ws) (c .>> ws) d f
            
        static member pipe (a: Parser<'a>, b: Parser<'b>, c: Parser<'c>, d: Parser<'d>, e: Parser<'e>, f) =
            pipe5 (a .>> ws) (b .>> ws) (c .>> ws) (d .>> ws) e f
        
        static member tuple (a: Parser<'a>, b: Parser<'b>) =
            tuple2 (a .>> ws) b
               
        static member tuple (a: Parser<'a>, b: Parser<'b>, c: Parser<'c>) =
            tuple3 (a .>> ws) (b .>> ws) c
            
        static member commaSep (p: Parser<'t>) : Parser<'t list> =
            P.delimitedBy(',') p
            
        static member commaSep1 (p: Parser<'t>) : Parser<'t list> =
            P.delimitedBy1(',') p
        
        static member delimitedBy (d: Parser<'d>) : Parser<'t> -> Parser<'t list> =
            fun p ->
                sepEndBy (p .>> ws) (d >>. ws)
            
        static member delimitedBy (c:char) =
            P.delimitedBy (pchar c)
        
        static member delimitedBy1 (d: Parser<'d>) : Parser<'t> -> Parser<'t list> =
            fun p ->
                sepEndBy1 (p .>> ws) (d >>. ws)
        
        static member delimitedBy1(c:char) =
            P.delimitedBy1 (pchar c)
            
        static member betweenBrackets p =
            P.betweenWs('(', ')') p
            
    open type P
        
    let addToDebug (stream: CharStream<ParserState>) label event =
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

        let row = stream.Position.Line.ToString().PadRight(5)
        let col = stream.Position.Column.ToString().PadRight(5)
        let posStr = $"{row} |{col} | "
        let posIdentStr = posStr + indentStr

        // The %A for res.Result makes it go onto multiple lines - pad them out correctly
        let replaceStr = "\n" + "".PadRight(max posStr.Length 0) + "".PadRight(max indent 0, '\u2502').PadRight(max msgPadLen 0)
        let correctedStr = msg.Replace("\n", replaceStr)
        let fullStr = $"%s{posIdentStr} %s{correctedStr}"

        stream.UserState.write fullStr
        stream.UserState.Indent <- nextIndent

    // Add debug info to the given parser
    let (<!>) (p: Parser<'t>) (label: string) : Parser<'t> =
        let error = expected label
        fun stream ->
            let stateTag = stream.StateTag
            addToDebug stream label Enter
            let mutable reply = p stream
            if stateTag = stream.StateTag then
                reply.Error <- error
            addToDebug stream label (Leave reply)
            reply
    
    /// Determine the correct Inst for the given TypeInst
    /// </summary>
    /// <remarks>
    /// The purpose of this step is to correctly identify
    /// which TypeInsts are 'var' versus 'par' at every level.
    ///
    /// Consider the two examples:
    /// array[1..3] of record(var bool: a): x;
    /// array[1..3] of var record(bool: b): y;
    ///
    /// Both are decision variables however only the second example
    /// would be given a TypeInst with Inst == Var.
    ///
    /// This function returns the TypeInst with the correctly
    /// inferred Vars 
    /// </remarks>
    let rec resolveTypeInst (ti: TypeInst) =
        match resolveType ti.Type with
        // If the type is a Var it overrides the setting here
        | ty, true ->
            { ti with Type = ty; IsVar = true }
        // Otherwise use the existing value
        | ty, _ ->
            { ti with Type = ty; }
            
    and resolveType (ty: Type) =
        match ty with
        | Type.Int 
        | Type.Bool 
        | Type.String 
        | Type.Float 
        | Type.Ident _ 
        | Type.Set _
        | Type.Generic _
        | Type.Any
        | Type.Range _
        | Type.Ann ->
            ty, false
        
        // Any var field means a var tuple
        | Type.Tuple x ->
            let mutable isVar = false
            let fields =
                x
                |> List.map (fun field ->
                    match resolveTypeInst field with
                    | ti when ti.IsVar ->
                        isVar <- true
                        ti
                    | ti ->
                        ti
                    )
                
            (Type.Tuple fields), isVar
                
        // Any var field means a var record
        | Type.Record x ->
            let mutable isVar = false
            
            let resolved =
                x
                |> List.map (fun field ->
                    let ti = resolveTypeInst field
                    if ti.IsVar then
                        isVar <- true
                    ti)
                
            (Type.Record resolved), isVar
            
        | Type.Array1D (i, ti) ->
            let ti = resolveTypeInst ti
            (Type.Array1D (i, ti)), ti.IsVar
            
        | Type.Array2D (i, j, ti) ->
            let ti = resolveTypeInst ti
            (Type.Array2D (i, j, ti)), ti.IsVar
            
        | Type.Array3D (i, j, k, ti) ->
            let ti = resolveTypeInst ti
            (Type.Array3D (i, j, k, ti)), ti.IsVar
        
        | Type.Array4D (i, j, k, l, ti) ->
            let ti = resolveTypeInst ti
            (Type.Array4D (i, j, k, l, ti)), ti.IsVar
        
        | Type.Array5D (i, j, k, l, m, ti) ->
            let ti = resolveTypeInst ti
            (Type.Array5D (i, j, k, l, m, ti)), ti.IsVar
            
        | Type.Array6D (i, j, k, l, m, n, ti) ->
            let ti = resolveTypeInst ti
            (Type.Array6D (i, j, k, l, m, n, ti)), ti.IsVar

                        
    let opt_or backup p =
        (opt p) |>> Option.defaultValue backup
       
    let simple_ident : Parser<Ident> =
        let options = IdentifierOptions(isAsciiIdStart = fun c -> c = '_' || Char.IsLetter c )
        identifier options

    let quoted_ident : Parser<Ident> =
        between
            (skipChar '\'')
            (skipChar '\'')
            (many1Chars (noneOf "'\x0A\x0D\x00"))
            |>> (fun s -> $"\'{s}\'")
               
    let ident : Parser<Ident> =
        simple_ident <|> quoted_ident
    
    [<Struct>]
    // An identifier or a value of 'T
    type IdentOrWord =
        | Ident of id:string
        | Word of word:Keyword
        
    let ident_or_keyword : Parser<IdentOrWord> =        
        fun stream ->
            let reply = ident stream
            if reply.Status <> Ok then
                Reply(reply.Status, reply.Error)
            else
                let mutable word = Keyword.NONE
                if Keyword.byName.TryGetValue(reply.Result, &word)
                then Reply(Word word)
                else Reply(Ident reply.Result)
    
    let (=>) (kw: Keyword) b =
        let name = Keyword.byValue[kw]
        stringReturn name b
    
    let (==>) (kw: Keyword) b =
        skip1 kw >>. (preturn b)
    
    // Parse the given operator and return the value
    // where value is an enum
    let (=!>) (operator: string) (value: 't) =
         attempt (stringReturn operator value)
            
    let name_or_quoted_value (p: Parser<'T>) : Parser<IdOr<'T>> =
                        
        let name =
            ident
            |>> IdOr.Id
        
        let value =
            p
            |> between (skipChar ''') (skipChar ''') 
            |>> IdOr.Val
        
        value <|> name
            
    // <float-literal> or <int-literal>
    let number_lit : Parser<Expr> =

        let format =
           NumberLiteralOptions.AllowMinusSign
           ||| NumberLiteralOptions.AllowFraction
           ||| NumberLiteralOptions.AllowExponent
           
        fun stream ->
            let reply = numberLiteral format "number" stream
            let num = reply.Result
            // Parsing failed
            if reply.Status <> Ok then
                Reply(reply.Status, reply.Error)
            // Integer                
            elif num.IsInteger then
                Reply(Expr.Int (int32 num.String))
            // Float with a trailing dot (not allowed)
            elif num.String[num.String.Length - 1] = '.' then
                stream.Seek(stream.Index - 1L)
                let expr =
                    num.String.Substring(0, num.String.Length - 1)
                    |> int32
                    |> Expr.Int
                Reply(expr)
            else
                let expr = Expr.Float(float num.String)
                Reply(expr)
            
    // <string-literal>
    let string_lit : Parser<string> =
        let error = expected "string-literal"
        let invalidEscape = messageError "Invalid escape sequence"
        fun stream ->
            if stream.Read() <> '"' then
                 Reply(Error, error)
            else
                let buffer = StringBuilder()
                let rec loop () =
                    match stream.Peek() with
                    | '"' ->
                        stream.Skip()
                        Reply(buffer.ToString())
                    | '\\' ->
                        stream.Skip()
                        match stream.Peek() with
                        | 'n' ->
                            buffer.Append('\n')
                            stream.Skip()
                            loop ()
                        | 't' ->
                            buffer.Append('\t')
                            stream.Skip()
                            loop ()
                        | '"' ->
                            buffer.Append("\\\"")
                            stream.Skip()
                            loop ()
                        | '(' ->
                            buffer.Append("\\\"")
                            stream.Skip()
                            loop ()
                        | ''' ->
                            buffer.Append("\\\'")
                            stream.Skip()
                            loop ()
                        | _   ->
                            Reply(ReplyStatus.Error, invalidEscape)
                    | CharStream.EndOfStreamChar ->
                        Reply(Error, messageError "Unexpected end of input")
                        
                    | c ->    
                        buffer.Append(c)
                        stream.Skip()
                        loop ()

                loop ()
                    
            
    
    let string_annotation : Parser<string> =
        skip "::"
        >>. string_lit
        |>> sprintf "\"%s\""
        
    let builtin_num_un_ops =
        [ "+" =!> Op.Add
        ; "-" =!> Op.Subtract ]
        
    // <builtin-num-un-op>
    let builtin_num_un_op =
        builtin_num_un_ops
        |> choice
        |>> (int >> enum<UnaryOp>)
        <!> "builtin-num-un-op"
    
    let builtin_num_bin_ops =
         [ "+"    =!> Op.Add
         ; "-"    =!> Op.Subtract 
         ; "*"    =!> Op.Multiply
         ; "/"    =!> Op.Divide         
         ; "^"    =!> Op.Exponent
         ; "~+"   =!> Op.TildeAdd
         ; "~-"   =!> Op.TildeSubtract
         ; "~*"   =!> Op.TildeMultiply
         ; "~/"   =!> Op.TildeDivide
         ; "div"  =!> Op.Div
         ; "mod"  =!> Op.Mod
         ; "~div" =!> Op.TildeDiv ]
        
    // <builtin-num-bin-op>
    let builtin_num_bin_op =
         builtin_num_bin_ops
         |> choice
         |>> (int >> enum<BinaryOp>)
         <!> "num-bin-op"
            
    let builtin_bin_ops = 
        [ "<->"       =!> Op.Equivalent
        ; "->"        =!> Op.Implies
        ; "<-"        =!> Op.ImpliedBy
        ; "\/"        =!> Op.Or
        ; "/\\"       =!> Op.And
        ; "<="        =!> Op.LessThanEqual
        ; ">="        =!> Op.GreaterThanEqual
        ; "=="        =!> Op.EqualEqual
        ; "<"         =!> Op.LessThan
        ; ">"         =!> Op.GreaterThan
        ; "="         =!> Op.Equal
        ; "!="        =!> Op.NotEqual
        ; "~="        =!> Op.TildeEqual
        ; "~!="       =!> Op.TildeNotEqual
        ; ".."        =!> Op.DotDot
        ; "++"        =!> Op.PlusPlus
        ; "xor"       =!> Op.Xor
        ; "intersect" =!> Op.Intersect
        ; "in"        =!> Op.In
        ; "subset"    =!> Op.Subset
        ; "superset"  =!> Op.Superset
        ; "union"     =!> Op.Union
        ; "diff"      =!> Op.Diff
        ; "symdiff"   =!> Op.SymDiff
        ; "default"   =!> Op.Default ]
        @ builtin_num_bin_ops
        
    // <builtin-bin-op>            
    let builtin_bin_op : Parser<BinaryOp> =
        builtin_bin_ops
        |> choice
        |>> (int >> enum<BinaryOp>)
        <!> "binop"
        
    let builtin_un_ops =
        builtin_num_un_ops
        @ ["not" =!> Op.Not]

    // <builtin-un-op>           
    let builtin_un_op : Parser<UnaryOp> =
        builtin_un_ops
        |> choice
        |>> (int >> enum<UnaryOp>)

    // <ti-expr>
    let ti_expr, ti_expr_ref =
        createParserForwardedToRef<TypeInst, ParserState>()

    // <ti-expr>
    let base_ti_expr_tail, base_ti_expr_tail_ref =
        createParserForwardedToRef<Type, ParserState>()
        
    // <expr>
    let expr, expr_ref =
        createParserForwardedToRef<Expr, ParserState>()
    
    // <expr-atom>        
    let expr_atom, expr_atom_ref =
        createParserForwardedToRef<Expr, ParserState>()

    // <num-expr>    
    let num_expr, num_expr_ref =
        createParserForwardedToRef<Expr, ParserState>()
        
    // <num-expr-atom>
    let num_expr_atom, num_expr_atom_ref =
        createParserForwardedToRef<Expr, ParserState>()
        
    // <num-expr-atom>
    let annotation, annotation_ref =
        createParserForwardedToRef<Annotation, ParserState>()

    /// Parse the operator or its quoted name
    let op (parser: Parser<'T>) : Parser<IdOr<'T>> =
        
        let value =
            parser |>> IdOr.Val
        
        let name =
            simple_ident
            |> betweenWs '`'
            |>> IdOr.Id
            
        name <|> value
            
    // <annotations>
    // eg: `:: output :: x(2)`
    let annotations : Parser<Annotations> =
        many (annotation .>> ws)
    
    // <annotations>
    // eg: `:: output :: x(2)`
    let ann_capture : Parser<Ident> =
        skip "ann"
        >>. skip ':'
        >>. ident
    
    // A TypeInst with a name
    // <ti-expr-and-id>
    // eg: `var int: x`
    let named_ti : Parser<TypeInst> =
        pipe(
            ti_expr,
            skipChar ':',
            ident,
            fun ti _ id ->
                { ti with
                    Name = id } 
        )
        
    // A TypeInst with an optional name
    // eg: function(int, a:bool)                
    let parameter_ti : Parser<TypeInst> =
        pipe(
            ti_expr,
            opt_or "" (skip ':' >>. ident),
            annotations,
            fun ti id anns ->
                { ti with
                    Name = id
                    Annotations = anns }
        )
        
    let assign_tail : Parser<Expr> =
        skipChar '='
        >>. (opt <| skipChar '=')
        >>. ws
        >>. expr
       
    // eg: `(int: a, var bool: b)`        
    let parameters : Parser<TypeInst list> =
        parameter_ti
        |> commaSep
        |> betweenWs('(', ')')
        <!> "parameters"
        
    // eg: `(1, 2, "abc")`
    let tupled_args : Parser<Expr list> =
        expr
        |> commaSep
        |> betweenWs('(', ')')
        <!> "tupled-args"
        
    // <predicate-item>
    // eg: `predicate isOk(int x) = x > 2`
    let predicate_item : Parser<Item> =
        pipe(
            ident,
            parameters,
            opt_or "" ann_capture,
            annotations,
            opt assign_tail,
            fun id pars ann anns body ->
                { Name = id
                ; Parameters = pars
                ; Ann = ann
                ; Annotations = anns
                ; Returns =
                    { TypeInst.Empty
                        with
                            Type = Type.Bool
                            IsVar = true  }
                ; Body = body }
                |> Item.Function
                )
        <!> "predicate"

    // <test_item>
    let test_item : Parser<Item> =
        pipe(
            ident,
            parameters,
            annotations,
            opt assign_tail,
            fun id pars anns body ->
                { Name = id
                ; Parameters = pars
                ; Annotations = anns
                ; Body = body }
                |> Item.Test
                )
        <!> "test"
        
    // <function-item>
    (*
    TODO - combine this with variable parsing because the optional function keyword is making it hard
    *)
    let function_item : Parser<Item> =
        pipe(
            named_ti,
            parameters,
            opt_or "" ann_capture,
            annotations,
            opt assign_tail,
            (fun ti pars ann anns body ->
                { Name = ti.Name
                ; Returns = { ti with Name = ""}
                ; Ann = ann
                ; Annotations = anns 
                ; Parameters = pars
                ; Body = body }
                |> Item.Function
                )
        )
        <!> "function"
    
    // <enum-case-list>    
    let enum_cases : Parser<EnumCases list> =
                
        let enum_names : Parser<EnumCases> =
            ident
            |> commaSep
            |> betweenWs('{', '}')
            |>> EnumCases.Names

        let anon_enum : Parser<EnumCases> =
            skip "_"
            <|> skip "anon_enum"
            >>. betweenWs('(', ')') expr
            |>> EnumCases.Anon
            
        let enum_call : Parser<EnumCases> =
            ident
            .>>. betweenWs('(', ')') expr
            |>> EnumCases.Call
        
        let enum_case =
            [ enum_names; anon_enum; enum_call ]
            |> choice
            .>> ws
            
        sepBy enum_case (skip "++")
          
    // <enum-item>
    let enum_item : Parser<Item> =            
        pipe(
            ident,
            annotations,
            (opt_or [] (skip '=' >>. enum_cases)),
            fun name anns cases ->
                { Name = name
                ; Annotations = anns
                ; Cases = cases }
                |> Item.Enum)
    
    // <include-item>
    let include_item : Parser<Item> =
        string_lit
        |>> (IncludeItem.Create >> Item.Include)
    
    // <var-par>
    let is_var =
        choice
            [ Keyword.VAR ==> true
            ; Keyword.PAR ==> false
            ; preturn   false ]
        
    // <opt-ti>        
    let opt_ti =
        Keyword.OPT ==> true
        <|> preturn false
    
    // <set-ti>    
    let set_ti =
        skip1 Keyword.SET
        >>. skip Keyword.OF
        >>% true
        <|> preturn false
           
    // <base-ti-expr>
    let base_ti_expr : Parser<TypeInst> =
        pipe(
            is_var,
            set_ti,
            opt_ti,
            base_ti_expr_tail,
            fun inst set opt typ ->
                { Type = typ
                ; Name = ""
                ; IsOptional = opt
                ; IsSet = set
                ; IsArray = false
                ; Annotations = [] 
                ; IsVar = inst
                ; Value = None
                ; IsInstanced = false }
        )
        <!> "base-ti"
        
    // <array-ti-expr>        
    let array_ti_expr : Parser<TypeInst> =
                
        let array_dims =
            base_ti_expr_tail
            |> commaSep
            |> betweenWs ('[', ']')
            <!> "array-dims"
        
        pipe(
            skip Keyword.ARRAY,
            array_dims,
            skip Keyword.OF,
            base_ti_expr,
            fun _ dims _ ti ->
                (dims, resolveTypeInst ti))
        >>= (fun (dims, ti) ->
                let arr ty =
                    preturn
                        { TypeInst.Empty with
                            IsArray = true
                            IsVar = ti.IsVar
                            Type = ty }
                        
                match dims with
 
                | [i] ->
                    arr <| Type.Array1D (i, ti)
                | [i;j] ->
                    arr <| Type.Array2D (i, j, ti)
                | [i;j;k]->
                    arr <| Type.Array3D (i, j, k, ti)
                | [i;j;k;l]->
                    arr <| Type.Array4D (i, j, k, l, ti)
                | [i;j;k;l;m] ->
                    arr <| Type.Array5D (i, j, k, l, m, ti)
                | [i;j;k;l;m;n] ->
                    arr <| Type.Array6D (i, j, k, l, m, n, ti)
                | xs ->
                    fail $"Number of array dimension must be between 1 and 6 (got {xs.Length})."
        )        
        <!> "array-ti"
    
    // <ti-expr>        
    ti_expr_ref.contents <-
        [ array_ti_expr
        ; base_ti_expr  ]
        |> choice
        <!> "ti"

    // <tuple-ti-expr-tail>
    let tuple_ti : Parser<Type> =
        ti_expr
        |> commaSep1
        |> betweenWs ('(', ')')
        |>> Type.Tuple
        <!> "tuple-ti"
            
    // <record-ti-expr-tail>
    let record_ti : Parser<Type> =
        named_ti
        |> commaSep1
        |> betweenWs('(', ')')
        |>> Type.Record
        <!> "record-ti"
        
    // eg: ```1..10```
    // eg: ```1 .. (a = 10)```
    // We make a special case here so it can be attempted
    // before the float parser
    let range_ti : Parser<Type> =
        num_expr .>> ws
        .>> skip ".."
        .>>. num_expr
        |>> Type.Range
    
    let instanced_type : Parser<string> =
        skipChar '$' >>. ident
            
    // <base-ti-expr-tail>
    base_ti_expr_tail_ref.contents <-
                         
        let set_ti =
            expr
            |> commaSep
            |> betweenWs('{', '}')
            |>> Type.Set
            
        let generic =
            skipChar '$' >>. ident |>> Type.Generic
         
        fun stream ->
            let stateTag = stream.StateTag
            let reply = ident_or_keyword stream
            
            if reply.Status = Ok then
                
                match reply.Result with
                | Word Keyword.INT ->
                    Reply(Type.Int)
                | Word Keyword.BOOL ->
                    Reply(Type.Bool)
                | Word Keyword.FLOAT ->
                    Reply(Type.Float)
                | Word Keyword.STRING ->
                    Reply(Type.String)
                | Word Keyword.ANN ->
                  Reply(Type.Ann)
                | Word Keyword.ANY    ->
                    Reply(Type.Any)
                | Word Keyword.RECORD ->
                    record_ti stream
                | Word Keyword.TUPLE  ->
                    tuple_ti stream
                | Word other ->
                    Reply(ReplyStatus.Error, messageError "Unexected keyword {other}")
                | Ident id ->
                    // Ident is a part of tuple, range, or call
                    // eg: `x[0]`, `x .. 10`, `x(100)`
                    match stream.Peek() with
                    | '.' | '[' | '(' ->
                        stream.Seek(stream.Index - (int64)id.Length)
                        range_ti stream
                    // Ident is standalone
                    | _c ->
                        Reply(Type.Ident id)
                      
            elif reply.Status = Error && stateTag = stream.StateTag then
              match stream.Peek() with
              | '{' -> set_ti stream
              | '$' -> generic stream
              | _x  -> range_ti stream
                  
            else 
              Reply(reply.Status, reply.Error)
    
        //
        // |>> (fun (name, args) ->
        //     // Handle some special known cases of function application
        //     match name, args with
        //     | (Id "array1d"), [i; Expr.Array1DLit arr] ->
        //         Expr.Array1D (i, arr)
        //     | (Id "array2d"), [i; j; Expr.Array1DLit arr] ->
        //         Expr.Array2D (i, j, arr)
        //     | (Id "array3d"), [i; j; k; Expr.Array1DLit arr] ->
        //         Expr.Array3D (i, j, k, arr)
        //     | (Id "array4d"), [i; j; k; l; Expr.Array1DLit arr] ->
        //         Expr.Array4D (i, j, k, l, arr)
        //     | (Id "array5d"), [i; j; k; l; m; Expr.Array1DLit arr] ->
        //         Expr.Array5D (i, j, k, l, m, arr)
        //     | (Id "array6d"), [i; j; k;l;m;n; Expr.Array1DLit arr] ->
        //         Expr.Array6D (i, j, k, l, m, n, arr)
        //     | _, _ ->
        //         Expr.Call(name,args)
        // )
        // <!> "call-expr"
        
    let wildcard : Parser<WildCard> =
        skip '_'
        >>. notFollowedBy letter
        >>% WildCard.WildCard
    
    let absent : Parser<Absent> =
        attempt (skip "<>")
        >>% Absent
        
    /// Generators
    /// Used in generator calls and list or set comprehensions
    /// eg: ```sum(i in 1..3 where i < 2)(i)```
    let generators : Parser<Generator list> =
                
        let gen_var =
            [ wildcard |>> IdOr.Val 
            ; ident    |>> IdOr.Id ]
            |> choice
            <!> "gen-var"
            
        let gen_vars =
            gen_var
            |> commaSep1            
            <!> "gen-vars"
            
        let gen_where =
            skip "where "
            >>. expr
            <!> "gen-where"
            
        let generator =    
            pipe(
                gen_vars,
                skip Keyword.IN,
                expr,
                opt gen_where,
                (fun idents _ source filter ->
                    { Yields = idents
                    ; From = source
                    ; Where = filter })
            )
            <!> "generator"
            
        generator
        |> commaSep1
    
    // <declare-item>
    let declare_item : Parser<Item> =
        pipe(
            named_ti,
            opt parameters,
            annotations,
            opt assign_tail,
            fun ti args anns expr ->
                let ti = resolveTypeInst ti
                match args with
                // No arguments, not a function
                | None ->
                    Item.Declare
                        { ti with
                            Annotations = anns
                            Value=expr }
                // Arguments, must be a function                    
                | Some args ->
                    Item.Function
                        { Name = ti.Name
                        ; Returns = {ti with Name = ""}
                        ; Ann = ""
                        ; Annotations = anns
                        ; Parameters = args 
                        ; Body = expr}
        )
        <!> "declare-item"

    let constraint_tail : Parser<ConstraintExpr> =
        pipe(
            expr,
            annotations,
            fun expr anns ->
              { Expr = expr
              ; Annotations = anns }
        )
        
    // <constraint-item>
    let constraint_item =
        constraint_tail |>> Item.Constraint
    
       
    // <let-item>
    let let_item : Parser<LetLocal> =
            
        let let_declare =
            declare_item
            >>= function
              | Item.Declare x ->
                  preturn (Choice1Of2 x)
              | other ->
                  fail $"{other} not allowed in let locals"
                                    
        let let_constraint =
            skip Keyword.CONSTRAINT
            >>. constraint_tail
            |>> Choice2Of2
            
        // Let constraint must go first            
        let_constraint <|> let_declare
    
    // <let-expr>
    let let_expr : Parser<Expr> =
                
        let item =
            let_item .>> ws .>> opt (anyOf ";,")
    
        let items =
            many1 (item .>> ws)
            |> betweenWs('{', '}')
        
        pipe(
            items,
            skip "in",
            expr,
            (fun items _b body ->
                let declares, constraints =
                    items
                    |> List.fold (fun (ds, cs) item ->
                        match item with
                        | Choice1Of2 x -> (x::ds, cs)
                        | Choice2Of2 x -> (ds, x::cs)
                    ) ([], [])
                
                // let nameSpace : NameSpace =
                //     (NameSpace.empty, declares)
                //     ||> List.fold (fun ns decl -> ns.Add decl)
                //
                { Declares = declares
                ; Constraints = constraints
                ; Body=body }
                |> Expr.Let
                ))
        <!> "let-expr"
        
    // <if-then-else-expr>
    let if_else_expr : Parser<Expr> =
                        
        let case (word: Keyword) =
            skip word
            >>. expr
           .>> ws
            <!> $"{word}-case"
            
        pipe(
            expr,
            case Keyword.THEN,
            many (case Keyword.ELSEIF .>>. case Keyword.THEN),
            opt (case Keyword.ELSE),
            skip Keyword.ENDIF,
            fun if_ then_ elseif_ else_ _ ->
                { If = if_
                ; Then = then_
                ; ElseIf = elseif_
                ; Else = else_ }
                |> Expr.IfThenElse
        )
    
    // <num-un-op>    
    let num_un_op =
        tuple(
            op builtin_num_un_op,
            num_expr_atom
        )
        
    let tuple_access_tail =
        attempt (skip '.' >>. puint8)
        <!> "tuple-access"
        
    let record_access_tail =
        attempt (skip '.' >>. ident)
        <!> "record-access"
        
    let array_access_tail =
        expr
        |> commaSep1
        |> betweenWs ('[', ']')
    
    // <expr-atom-tail>
    let rec expr_atom_tail expr : Parser<Expr> =
        let tail =
            [ tuple_access_tail |>> fun i -> Expr.TupleAccess(i, expr)
              record_access_tail |>> fun f -> Expr.RecordAccess(f, expr)
              array_access_tail |>> fun xs -> Expr.ArrayAccess(xs, expr) ]
            |> choice
        (tail >>== expr_atom_tail)
        <|>
        preturn expr
    
    let brackets_or_tuple : Parser<Expr> =
        
        fun stream ->
            stream.Skip('(')
            stream.SkipWhitespace()
            let xs = ResizeArray<Expr>()
            let mutable fin = false
            let mutable comma = false
            let mutable exitOk = false
            let mutable reply = Reply(ReplyStatus.Error, expected "Bracketed expr or Tuple literal")
                        
            while not fin do
                // The end of the brackets was reached 
                if stream.Peek() = ')' then
                    stream.Skip()
                    fin <- true
                    exitOk <- true
                else
                    // Parse item failed 
                    reply <- expr stream
                    if reply.Status <> Ok then
                        fin <- true
                        comma <- false

                    // Parse item succeeded                                            
                    else
                        xs.Add (reply.Result)
                        stream.SkipWhitespace()
                        match stream.Peek() with
                        | ',' ->
                            stream.Skip()
                            stream.SkipWhitespace()
                            comma <- true
                        | _ ->
                            comma <- false
                            
            match exitOk, comma, xs.Count with
            | true, _, 0 ->
                reply
            | true, false, 1 ->
                reply.Error <- null
                reply.Status <- Ok
                reply.Result <- Expr.Bracketed xs[0]
                reply
            | true, _, _ ->
                reply.Error <- null
                reply.Status <- Ok
                reply.Result <- Expr.Tuple (Seq.toList xs)
                reply
            | false, _, _ ->
                reply
    
    // <num-expr-atom-head>    
    let num_expr_atom_head : Parser<Expr> =
            
        let gen_call_tail =
            tuple(
                betweenWs('(', ')') generators,
                betweenWs('(', ')') expr
            )
            
        let num_add =
            skip '+'
            >>. num_expr_atom
            |>> (fun expr -> Expr.UnaryOp (UnaryOp.Add, expr))
            
        let num_subtract =
            skip '-'
            >>. num_expr_atom
            |>> (fun expr -> Expr.UnaryOp (UnaryOp.Subtract, expr))
        
        let id =
            ident_or_keyword .>> ws <!> "id"
        
        fun stream ->
            let tag = stream.StateTag
            let reply = id stream 
            if reply.Status = Ok then
                match reply.Result with
                
                // <let-expr>
                | Word Keyword.LET ->
                    let_expr stream

                // <if-then-else-expr>
                | Word Keyword.IF ->
                    if_else_expr stream

                // <bool-lit>
                | Word Keyword.TRUE ->
                    Reply(Expr.Bool true)

                // <bool-lit>
                | Word Keyword.FALSE ->
                    Reply (Expr.Bool false)

                // <call-expr> or <gen-call-expr> 
                | Ident id when stream.Peek() = '(' ->
                    let mutable state = CharStreamState(stream)
                    let reply = gen_call_tail stream
                    // <gen-call-expr>
                    if reply.Status = Ok then
                        { Id = id
                        ; From = fst reply.Result
                        ; Yields = snd reply.Result }
                        |> Expr.GenCall
                        |> Reply
                    // <call-expr>                        
                    else
                        stream.BacktrackTo(&state)
                        match tupled_args stream with
                        | r when r.Status = Ok ->
                            Expr.Call (Id id, r.Result)
                            |> Reply
                        | r ->
                            Reply(r.Status, r.Error)
                            
                // <ident>                            
                | Ident id ->
                    Reply(Expr.Ident id)
                    
                | Word word ->
                    Reply(ReplyStatus.Error, unexpected $"Keyword {word}")
                        
            elif stream.StateTag = tag then
                match stream.Peek() with
                // <num-un-op>
                | '+' -> num_add stream
                // <num-un-op>
                | '-' -> num_subtract stream
                // <tuple-literal> or bracketed expr
                | '(' -> brackets_or_tuple stream
                // <float-literal> or <int-literal>
                | _c  -> number_lit stream
            
            else 
                Reply(reply.Status, reply.Error)
                    
    // <num-expr-atom>        
    num_expr_atom_ref.contents <-
        num_expr_atom_head
        >>== expr_atom_tail
        <!> "num-expr-atom"
        
    // <num-expr-binop-tail>
    let num_expr_binop_tail (head: Expr) =
        
        let binop =
            pipe(
                op builtin_num_bin_op,
                num_expr,
                fun operator tail ->
                    Expr.BinaryOp (head, operator, tail)
            )
            
        binop <|> preturn head
       
    // <num-expr>
    num_expr_ref.contents <-
        num_expr_atom
        >>== num_expr_binop_tail
            
    // <tuple-literal>
    // TODO - Required trailing comma if tuple is 1 element
    let tuple_lit : Parser<Expr> =
        expr
        |> commaSep1
        |> betweenBrackets 
        |> attempt
        |>> Expr.Tuple
          
    // <record-literal>
    let record_lit : Parser<Expr> =
        tuple(ident, skip ':' >>. expr)
        |> commaSep1
        |> betweenBrackets 
        |> attempt
        |>> Expr.Record
        
    // <expr-atom-head>    
    let expr_atom_head : Parser<Expr> =
                            
        let plus =
            charReturn '+' UnaryOp.Add
            .>> ws
            .>>. expr
            |>> Expr.UnaryOp
            
        let minus =
            charReturn '-' UnaryOp.Add
            .>> ws
            .>>. expr
            |>> Expr.UnaryOp
            
        let wildcard =
            wildcard |>> Expr.WildCard
            
        let absent =             
            absent |>> Expr.Absent
        
        let string_lit =
            string_lit |>> Expr.String
            
        let wildcard =
            charReturn '_' (Expr.WildCard WildCard)
            
        let absent =
            stringReturn "<>" (Expr.Absent Absent)
            
        let id =
            ident_or_keyword .>> ws
            
        let array2d_lit : Parser<Expr> =
            
            let row =
                commaSep1 expr <!> "array2d-row"
            
            let sep : Parser<unit> =
                fun stream ->
                    let next = stream.Peek2()
                    match (next.Char0, next.Char1) with
                    | '|', x when x <> ']' ->
                        stream.Skip()
                        stream.SkipWhitespace()
                        Reply(())
                    | _1, _2 ->
                        Reply(ReplyStatus.Error, expected "|")
            
            let rows = sepEndBy row sep

            let error = expected "2D array with uniform dimensions"

            fun stream ->
                let reply = rows stream
                if reply.Status = Ok then
                    stream.SkipWhitespace()
                    stream.Skip("|]")
                    try
                        let array = array2D reply.Result
                        Reply(Expr.Array2DLit array)
                    with
                    | exn ->
                        Reply(ReplyStatus.Error, error)
                else
                    Reply(reply.Status, reply.Error)
                    
        let array3d_lit : Parser<Expr> =
            
            let row =
                commaSep expr <!> "array3d-row"
            
            let sep : Parser<unit> =
                let error = expected "| not followed by another | or ,"
                fun stream ->
                    match stream.Peek() with
                    | '|' ->
                        let mutable state = CharStreamState(stream)
                        stream.Skip()
                        stream.SkipWhitespace()
                        match stream.Peek() with
                        | '|' | ',' ->
                            stream.BacktrackTo(&state)
                            Reply(ReplyStatus.Error, error)
                        | _ ->
                            Reply(())
                    | _ ->
                        Reply(ReplyStatus.Error, error)
            
            let rows = sepEndBy row sep
            
            let nested_matrix : Parser<Expr list list> =
                row
                |> delimitedBy sep
                |> betweenWs('|', '|')
                <!> "array3d-matrix"
                            
            nested_matrix
            |> commaSep1
            .>> ws
            .>> skipString "|]"
            >>= (fun exprs ->
                
                try
                    let I = exprs.Length
                    let J = if I > 0 then exprs[0].Length else 0
                    let K = if J > 0 then exprs[0].[0].Length else 0
                    let array = Array3D.zeroCreate I J K
                    for i,x in Seq.indexed exprs do
                        for j,y in Seq.indexed x do
                            for k, z in Seq.indexed y do
                                array[i,j,k] <- z
                    array
                    |> Expr.Array3DLit 
                    |> preturn                            
                with
                | exn ->
                    fail $"Bad array dimensions"
                )
            |> attempt
            <!> "array3d"                    

        let array1d_lit =
            commaSep1 expr .>> skipChar ']'
        
        let array_expr : Parser<Expr> =
                
            let array_comp_tail =
                generators .>> ws .>> skipChar ']'
            
            fun stream ->
                stream.Skip('[')
                stream.SkipWhitespace()
                
                match stream.Peek() with
                
                // Empty array
                | ']' ->
                    stream.Skip()
                    Reply(Expr.Array1DLit Array.empty)
                
                // 2D or 3D Array
                | '|' ->
                    stream.Skip()
                    stream.SkipWhitespace()
                    let next = stream.Peek2()
                    match next.Char0, next.Char1 with
                    | '|' , ']' ->
                        stream.Skip(2)
                        Reply(Expr.Array1DLit Array.empty)
                    | '|', _ ->
                        array3d_lit stream
                    |  _  ->
                        array2d_lit stream
                    
                // 1D literal or comprehension
                | _ ->
                    let reply = expr stream
                    if reply.Status = Ok then
                        stream.SkipWhitespace()
                        match stream.Read() with
                        // Comma separated means array literal
                        | ',' ->
                            stream.SkipWhitespace()
                            match array1d_lit stream with
                            | r when r.Status = Ok ->
                                reply.Result :: r.Result
                                |> Expr.Set
                                |> Reply
                            | r ->
                                Reply(r.Status, r.Error)
                                
                        // Pipe indicates a comprehension
                        | '|' ->
                            stream.SkipWhitespace()
                            match array_comp_tail stream with
                            | r when r.Status = Ok ->
                                { Yields = reply.Result
                                ; IsSet = false
                                ; From = r.Result }
                                |> Expr.ArrayComp
                                |> Reply
                            | r ->
                                Reply(r.Status, r.Error)
                            
                        // End of array literal
                        | ']' ->
                             [| reply.Result |]
                             |> Expr.Array1DLit
                             |> Reply

                        | c ->
                            Reply(ReplyStatus.Error, unexpected (string c))
                             
                    else
                        Reply(reply.Status, reply.Error)    
            
        let set_expr : Parser<Expr> =
            
            let set_lit_tail =
                commaSep1 expr .>> skipChar '}'
                
            let set_comp_tail =
                generators .>> ws .>> skipChar '}'
            
            fun stream ->
                stream.Skip('{')
                stream.SkipWhitespace()

                // Empty set literal
                if stream.Skip('}') then
                    Reply(Expr.Set [])

                // Could be set lit or set comp                    
                else
                    let reply = expr stream
                    if reply.Status = Ok then
                        stream.SkipWhitespace()
                        match stream.Peek() with
                        // Comma separated means set literal
                        | ',' ->
                            stream.Skip(1)
                            stream.SkipWhitespace()
                            match set_lit_tail stream with
                            | r when r.Status = Ok ->
                                reply.Result :: r.Result
                                |> Expr.Set
                                |> Reply
                            | r ->
                                Reply(r.Status, r.Error)
                                
                        // Pipe indicated set comprehension
                        | '|' ->
                            stream.Skip()
                            stream.SkipWhitespace()
                            match set_comp_tail stream with
                            | r when r.Status = Ok ->
                                { Yields = reply.Result
                                ; IsSet = true
                                ; From = r.Result }
                                |> Expr.SetComp
                                |> Reply
                            | r ->
                                Reply(r.Status, r.Error)
                            
                        // End of set literal
                        | '}' ->
                             stream.Skip(1)
                             Reply(Expr.Set[reply.Result])

                        | c ->
                            Reply(ReplyStatus.Error, unexpected (string c))
                             
                    else
                        Reply(reply.Status, reply.Error)
            

        let gen_call_tail =
            tuple(
                betweenWs('(', ')') generators,
                betweenWs('(', ')') expr
            )
                        
        fun stream ->
            let tag = stream.StateTag
            match id stream with
            | r when r.Status = Ok ->
                match r.Result with
                
                | Word Keyword.LET ->
                    let_expr stream
                    
                | Word Keyword.IF ->
                    if_else_expr stream
                    
                | Word Keyword.TRUE ->
                    Reply(Expr.Bool true)
                    
                | Word Keyword.FALSE ->
                    Reply(Expr.Bool false)
                   
                // Function-call or Generator-call                    
                | Ident id when stream.Peek() = '(' ->
                    let mutable state = CharStreamState(stream)
                    let reply = gen_call_tail stream
                    if reply.Status = Ok then
                        { Id = id
                        ; From = fst reply.Result
                        ; Yields = snd reply.Result }
                        |> Expr.GenCall
                        |> Reply
                    else
                        stream.BacktrackTo(&state)
                        match tupled_args stream with
                        | r when r.Status = Ok ->
                            Expr.Call (Id id, r.Result)
                            |> Reply
                        | r ->
                            Reply(r.Status, r.Error)
                            
                | Ident id ->
                    Reply(Expr.Ident id)

                | Word Keyword.NOT ->
                    stream.SkipWhitespace()
                    let reply = expr stream
                    if reply.Status <> Ok then
                        Reply(reply.Status, reply.Error)
                    else
                        Reply(Expr.UnaryOp (UnaryOp.Not, reply.Result))
                                    
                // TODO This is an error usually except when used as an annotation eg: `:: output`                     
                | Word word ->
                    Reply(Expr.Ident Keyword.byValue[word])
                    
                        
            | _ when stream.StateTag = tag ->
                match stream.Peek() with
                | '(' -> brackets_or_tuple stream
                | '+' -> plus stream
                | '-' -> minus stream
                | '{' -> set_expr stream
                | '[' -> array_expr stream
                | '_' -> wildcard stream 
                | '<' -> absent stream
                | '"' -> string_lit stream                
                | _other -> number_lit stream
                
            | c ->
                Reply(ReplyStatus.Error, unexpected (string c))
            
    // <annotations>
    annotation_ref.contents <-
                
        skip "::"
        >>. expr_atom_head
        >>== expr_atom_tail
        <!> "annotation"
    
    // <expr-atom>        
    expr_atom_ref.contents <-
        expr_atom_head
        >>== expr_atom_tail
            
    // <expr-binop-tail>
    let expr_binop_tail (head: Expr) =
        
        let binop =
            pipe(
                op builtin_bin_op,
                expr,
                fun operator tail ->
                    Expr.BinaryOp (head, operator, tail)
            )
            
        binop
        <|> preturn head
            
    // <expr>
    expr_ref.contents <-
        expr_atom
        >>== expr_binop_tail
        <!> "expr"
            
    let solve_type : Parser<SolveMethod> =
        choice
          [ Keyword.SATISFY => SolveMethod.Satisfy
          ; Keyword.MINIMIZE => SolveMethod.Minimize
          ; Keyword.MAXIMIZE => SolveMethod.Maximize ]
        
    // <solve-item>
    let solve_item : Parser<Item> =
        pipe(
            annotations,
            solve_type,
            opt expr,
            fun anns solveType obj ->
                match (solveType, obj) with
                | SolveMethod.Maximize, Some exp ->
                    SolveItem.Max (exp, anns)
                | SolveMethod.Minimize, Some exp ->
                    SolveItem.Min (exp, anns)
                | _ ->
                    SolveItem.Sat anns
                |> Item.Solve)
        
    // <assign-item>
    // eg ```a = 1;```
    // eg ```b == func(4);```
    let assign_item : Parser<AssignExpr> =
        attempt (ident .>> ws .>> followedBy (pchar '='))
        .>>. assign_tail
        <!> "assign-item"
        
    // <type-inst-syn-item>
    let alias_item : Parser<Item> =
        pipe(
            ident,
            annotations,
            skip '=',
            ti_expr,
            fun id anns _ ti ->
                { Name = id
                ; Annotations = anns
                ; TypeInst = ti }
                |> Item.Synonym
                )
        
    // <output-item>
    let output_item : Parser<Item> =
        pipe(
            opt string_annotation,
            expr,
            fun ann expr ->
                { Expr = expr
                ; Annotation = ann }
                |> Item.Output)
        
    // <annotation_item>
    let annotation_item : Parser<Item> =
        pipe(
            ident,
            opt_or [] parameters,
            opt assign_tail,
            fun name pars body ->
                { Name = name
                ; Body = body
                ; Params = pars }
                |> Item.Annotation)
        <!> "annotation-type"

    // <item>
    let item : Parser<Item> =
        let err = expected "item"
        let ident = ident .>> ws
        fun stream ->
            let mutable state = CharStreamState(stream)
            let tag = stream.StateTag
            let reply = ident stream
            
            // identifier was found
            if reply.Status = Ok then
              match Keyword.lookup reply.Result with
              | Keyword.ENUM ->
                  enum_item stream
              | Keyword.CONSTRAINT ->
                  constraint_item stream
              | Keyword.INCLUDE ->
                  include_item stream
              | Keyword.SOLVE ->
                  solve_item stream
              | Keyword.TYPE ->
                  alias_item stream
              | Keyword.OUTPUT ->
                  output_item stream
              | Keyword.PREDICATE ->
                  predicate_item stream
              | Keyword.FUNCTION ->
                  function_item stream
              | Keyword.TEST ->
                  test_item stream
              | Keyword.ANNOTATION ->
                  annotation_item stream
              | Keyword.NONE when stream.Peek() = '=' ->
                  let id = reply.Result
                  match assign_tail stream with
                  | r when r.Status = ReplyStatus.Ok ->
                      let item = Item.Assign (id, r.Result)
                      Reply(item)
                  | r ->
                      Reply(r.Status, r.Error)
              | _other ->
                  stream.BacktrackTo(&state)
                  declare_item stream

            // error within identifier string
            else 
              Reply(reply.Status, reply.Error)
                          
    let ast : Parser<Ast> =
        
        let sep =
            ws .>> skipChar ';' .>> ws
        
        ws
        >>. sepEndBy item sep
        .>> eof
        
    let data : Parser<AssignExpr list> =
        
        let sep =
            skip ';'
            
        ws
        >>. sepEndBy (assign_item .>> ws) (skip ';')
        .>> ws 
        .>> eof
        
    [<Struct>]        
    type Statement =
        | Comment of c:string
        | Statement of s:string
        
    let line_comment : Parser<string> =
        skip '%' >>. restOfLine true
        
    let block_comment : Parser<string> =
        let pStart = attempt <| skip "/*"
        let pEnd = attempt <| skipString "*/"
        pStart
        >>. manyCharsTill anyChar pEnd
        
    let non_comment : Parser<string> =
        let pEnd =
            (skipChar '%')
            <|> (attempt (skipString "/*"))
            <|> eof
        
        many1CharsTill anyChar (followedBy pEnd)
        
    let statement =
        [ line_comment  |>> Comment
        ; block_comment |>> Comment
        ; string_lit |>> Statement
        ; non_comment   |>> Statement ] 
        |> choice
        
    let statements =
        ws
        >>. many (statement .>> ws)
        .>> eof
        
        
open Parsers

[<AutoOpen>]       
module Parse =
        
    // Parse a string with the given parser
    let parseWith (parser: Parser<'t>) (input: string) : Result<'t, ParseError> =
        
        let state = ParserState()
                        
        match runParserOnString parser state "" input with
                
        | ParserResult.Success (value, _state, _pos) ->
            Result.Ok value
            
        | ParserResult.Failure (msg, err, state) ->
            
            let err =
                { Message = msg
                ; Line = err.Position.Line
                ; Column = err.Position.Column
                ; Index = err.Position.Index
                ; Trace = state.Message }
                
            Result.Error err
    
    [<Struct>]
    type PPX =
        | Mzn
        | LineComment
        | BlockOpenCheck
        | BlockCloseCheck
        | Block
        | StringLit
        | StringEscape
        
    // Parse and remove comments from the given model string
    let parseComments (mzn: string) : string * string =
        let comments = StringBuilder()
        let source = StringBuilder()
        let chars = mzn.AsSpan()
        let mutable state = PPX.Mzn
        let mutable char = 'a'
        
        for c in chars do
            match state, c with
            
            | PPX.LineComment, '\n' ->
                source.AppendLine()
                state <- PPX.Mzn
                
            | PPX.LineComment, _ ->
                comments.Append c
                state <- PPX.LineComment
                
            | PPX.Mzn, '%' ->
                state <- PPX.LineComment
                
            | PPX.Mzn, '/' ->
                state <- PPX.BlockOpenCheck
                
            | PPX.BlockOpenCheck, '*' ->
                state <- PPX.Block
                
            | PPX.BlockOpenCheck, _ ->
                source.Append '/'
                source.Append c
                state <- PPX.Mzn
                ()
            
            | PPX.Block, '*' ->
                state <- PPX.BlockCloseCheck
                
            | PPX.BlockCloseCheck, '/' ->
                state <- PPX.Mzn
            
            | PPX.BlockCloseCheck, _ ->
                comments.Append '*'
                state <- PPX.Block
               
            | PPX.Block, _ ->
                comments.Append c
                state <- PPX.Block
                
            | PPX.Mzn, '"' ->
                source.Append '"'
                state <- PPX.StringLit
                
            | PPX.StringLit, '\\' ->
                source.Append '\\'
                state <- PPX.StringEscape
                
            | PPX.StringEscape, _ ->
                source.Append c
                state <- PPX.StringLit
                
            | PPX.StringLit, '"' ->
                source.Append '"'
                state <- PPX.Mzn
                
            | PPX.StringLit, _ ->
                source.Append c
                ()
                
            | PPX.Mzn, '\n' when char = '\n' ->
                ()
                
            | PPX.Mzn, _ ->
                source.Append c
                ()
    
            char <- c                
                

        let source = source.ToString().Trim()
        let comments = comments.ToString()
        source, comments
         
    /// Parse '.dzn' style model data from a string        
    let parseDataString (dzn: string) : Result<AssignExpr list, ParseError> =
                            
        let source, comments =
            parseComments dzn
            
        let result =
            parseWith data source
            
        result            
            
    /// Parse a '.dzn' model data file            
    let parseDataFile (filepath: string) : Result<AssignExpr list, ParseError> =
        let dzn = File.ReadAllText filepath
        let data = parseDataString dzn
        data