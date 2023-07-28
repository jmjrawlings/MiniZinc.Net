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

        // Skip string and whitespace        
        static member skip(s: string) =
            skipString s  .>> ws
                
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

        static member pipe(a: Parser<'a>, b: Parser<'b>, f) =
            pipe2 (a .>> ws) b f
               
        static member pipe(a: Parser<'a>, b: Parser<'b>, c: Parser<'c>, f) =
            pipe3 (a .>> ws) (b .>> ws) c f
            
        static member pipe(a: Parser<'a>, b: Parser<'b>, c: Parser<'c>, d: Parser<'d>, f) =
            pipe4 (a .>> ws) (b .>> ws) (c .>> ws) d f
            
        static member pipe(a: Parser<'a>, b: Parser<'b>, c: Parser<'c>, d: Parser<'d>, e: Parser<'e>, f) =
            pipe5 (a .>> ws) (b .>> ws) (c .>> ws) (d .>> ws) e f
        
        static member tuple(a: Parser<'a>, b: Parser<'b>) =
            tuple2 (a .>> ws) b
               
        static member tuple(a: Parser<'a>, b: Parser<'b>, c: Parser<'c>) =
            tuple3 (a .>> ws) (b .>> ws) c
            
        static member commaSep p =
            sepEndBy (p .>> ws) (P.skip ',')
            
        static member commaSep1 p =
            sepEndBy1 (p .>> ws) (P.skip ',')
            
        static member betweenBrackets p =
            P.betweenWs('(', ')') p
            
    open type P
        
    let addToDebug (stream: CharStream<ParserState>) label event =
        let msgPadLen = 50
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
    let (<!>) (p: Parser<'t>) label : Parser<'t> =
        fun stream ->
            addToDebug stream label Enter
            let reply = p stream
            addToDebug stream label (Leave reply)
            reply
            
    let (<?!>) (p: Parser<'t>) (label : string) : Parser<'t> =
        p
        <?> label
        <!> label

    
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
            
        | Type.Array (i, ti) ->
            let ti = resolveTypeInst ti
            (Type.Array (i, ti)), ti.IsVar
            
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
        
    let s (str: string) =
        pstring str
        
    let c (chr : char) =
        pchar chr
        
    // let cw (chr: char) =
    //     pchar chr >>. ws
        
    /// Parse the given keyword
    /// Care is taken here that the given string is
    /// not part of a large string
    /// eg `keyword "function"` would not match the
    /// string "function1" 
    let keyword (name: string) =
        attempt(
            skipString name
            .>> notFollowedBy (
                satisfy (fun c ->
                    Char.IsDigit c || Char.IsLetter c || c = '_')
                )
        )
        .>> ws
        
    let keywordL (name: string) =
        keyword name <?!> name
        
    // Parse the keyword and return the value
    let (=>) (key: string) (value: 't) =
        keyword key
        >>% value
    
    // Parse the given operator and return the value
    // where value is an enum
    let (=!>) (operator: string) (value: 't) =
        pstring operator
        |> attempt
        >>% value
            
    let value_or_quoted_name (parser: Parser<'T>) : Parser<IdOr<'T>> =
        
        let value =
            parser |>> IdOr.Val
        
        let name =
            simple_ident
            |> betweenWs '`'
            |>> IdOr.Id
            
        name <|> value
    
      
    let name_or_quoted_value (p: Parser<'T>) : Parser<IdOr<'T>> =
        
        let name =
            ident
            |>> IdOr.Id
        
        let value =
            p
            |> betweenWs '''
            |> attempt
            |>> IdOr.Val
        
        value <|> name
    
    // <bool-literal>
    let bool_literal : Parser<bool> =
        choice
            [ "true" => true
            ; "false" => false ]
        
    let string_lit_contents : Parser<string> =
         
        let quoted_string =
            attempt (
                previousCharSatisfies (fun c -> c = '\\')
                >>. pchar '"'
            )
            <?!> "escaped-quote"
        
        manyChars 
            (satisfy (fun c -> c <> '"')
            <|>
            quoted_string)
                
    let number_lit : Parser<Expr> =
        let format =
           NumberLiteralOptions.AllowMinusSign
           ||| NumberLiteralOptions.AllowFraction
           ||| NumberLiteralOptions.AllowExponent

        numberLiteral format "number"
        |>> function
                | nl when nl.IsInteger ->
                    Expr.Int (int32 nl.String)
                | nl ->
                    Expr.Float (float nl.String)
                    
                        
            
    // <string-literal>
    let string_lit : Parser<string> =
            
        between
            (pchar '\"')
            (pchar '\"')
            string_lit_contents
            <?!> "string-lit"
    
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
        <?!> "builtin-num-un-op"
    
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
         <?!> "num-bin-op"
            
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
        <?!> "binop"
        
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
    let (expr : Parser<Expr>, expr_ref) =
        createParserForwardedToRef<Expr, ParserState>()
    
    // <expr-atom>        
    let (expr_atom: Parser<Expr>), expr_atom_ref =
        createParserForwardedToRef<Expr, ParserState>()

    // <num-expr>    
    let (num_expr: Parser<Expr>, num_expr_ref) =
        createParserForwardedToRef<Expr, ParserState>()
        
    // <num-expr-atom>
    let (num_expr_atom: Parser<Expr>, num_expr_atom_ref) =
        createParserForwardedToRef<Expr, ParserState>()
        
    // <num-expr-atom>
    let (annotation: Parser<Annotation>, annotation_ref) =
        createParserForwardedToRef<Annotation, ParserState>()
                
    let bracketed x =
        betweenWs('(', ')') x

    let op p =
        value_or_quoted_name p
        
    let un_op =
        op builtin_un_op
        .>> ws
        .>>. expr_atom
        <?!> "un-op"

    let builtin_ops =
        builtin_bin_ops @ builtin_un_ops

    // <builtin-op>            
    let builtin_op : Parser<Op> =
        builtin_ops
        |> choice
    
    // <array1d-literal>
    let array1d_literal =
        sepEndBy expr (skip ',')
        |> betweenWs ('[', ']')
        |>> Array.ofList
        <?!> "array1d-literal"
            
    // <set-literal>
    let set_literal : Parser<Expr list>=
        commaSep expr
        |> betweenWs('{', '}')
                
    // <array2d-literal>
    let array2d_literal : Parser<Expr[,]>=
                
        let row =
            commaSep1 expr
            
        let rowSep =
            attempt (
                skipChar '|'
                >>. notFollowedBy (skipChar ']')
            )
            >>. ws
            
        let rows =
            sepEndBy row rowSep
            
        let array =
            rows
            |> betweenWs ("[|", "|]")
            >>= (fun exprs ->
                try
                    preturn (array2D exprs)
                with
                | exn ->
                    fail exn.Message)
                
        array
    
    // eg: ```1..10```
    // eg: ```1 .. (a = 10)```
    // We make a special case here so it can be attempted
    // before the float parser
    let number_range : Parser<Expr> =
        
        let left = 
            pint32
            .>> ws
            .>> skipChar '.'
            .>> skipChar '.'
            |>> Expr.Int
            |> attempt
            
        let right =
            num_expr
            
        pipe(
            left,
            right,
            fun left right ->
                let op = IdOr.Val BinaryOp.DotDot
                Expr.BinaryOp(left, op, right)
                )
            
    // <annotations>
    // eg: `:: output :: x(2)`
    let annotations : Parser<Annotations> =
        many (annotation .>> ws)
    
    // <annotations>
    // eg: `:: output :: x(2)`
    let ann_capture : Parser<Ident> =
        keyword "ann"
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
        skipMany1 (pchar '=')
        >>. ws
        >>. expr
        
    // eg: `(int: a, var bool: b)`        
    let parameters : Parser<TypeInst list> =
        parameter_ti
        |> commaSep
        |> betweenWs('(', ')')
        <?!> "parameters"
        
    // eg: `(1, 2, "abc")`
    let tupled_args : Parser<Expr list> =
        expr
        |> commaSep
        |> betweenWs('(', ')')
        <?!> "tupled-args"
        
    // <predicate-item>
    // eg: `predicate isOk(int x) = x > 2`
    let predicate_item : Parser<FunctionType> =
        pipe(
            keyword "predicate"
            >>. ident,
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
                ; Body = body })
        <?!> "predicate"

    // <test_item>
    let test_item : Parser<TestItem> =
        pipe(
            keyword "test",
            ident,
            parameters,
            annotations,
            opt assign_tail,
            fun _ id pars anns body ->
                { Name = id
                ; Parameters = pars
                ; Annotations = anns
                ; Body = body }
                )
        <?!> "test"
        
    // <function-item>
    (*
    TODO - combine this with variable parsing because the optional function keyword is making it hard
    *)
    let function_item : Parser<FunctionType> =
        pipe(
            keyword "function"
            >>. named_ti,
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
                ; Body = body })
        )
        <?!> "function"
    
    // <enum-case-list>    
    let enum_cases : Parser<EnumCases list> =
                
        let enum_names : Parser<EnumCases> =
            ident
            |> commaSep
            |> betweenWs('{', '}')
            |>> EnumCases.Names

        let anon_enum : Parser<EnumCases> =
            choice [ s "_"; s "anon_enum"]
            >>. ws
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
    let enum_item : Parser<EnumType> =            
        pipe(
            keyword "enum",
            ident,
            annotations,
            (opt_or [] (skip '=' >>. enum_cases)),
            fun _ name anns cases ->
                { Name = name
                ; Annotations = anns
                ; Cases = cases })
    
    // <include-item>
    let include_item : Parser<IncludeItem> =
        keyword "include"
        >>. string_lit
        |>> IncludeItem.Create
    
    // <var-par>
    let isVar : Parser<bool> =
        choice
            [ "var" => true
            ; "par" => false
            ; preturn  false ]
        
    // <opt-ti>        
    let opt_ti =
        "opt" => true
        <|> preturn false
    
    // <set-ti>    
    let set_ti =
        keyword "set"
        >>. keyword "of"
        >>% true
        <|> preturn false
           
    // <base-ti-expr>
    let base_ti_expr : Parser<TypeInst> =
        pipe(
            isVar,
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
        
    // <array-ti-expr>        
    let array_ti_expr : Parser<TypeInst> =

        let dimension =
            ti_expr
            >>= fun ti ->
                match ti.Type with
                | Type.Ident id ->
                    preturn (ArrayDim.Id id)
                | Type.Int ->
                    preturn ArrayDim.Int
                | Type.Set x ->
                    preturn (ArrayDim.Set x)
                | other ->
                    fail $"Bad array dimension {other}"
        
        let dimensions =
            dimension
            |> commaSep
            |> betweenWs ('[', ']')
            <?!> "array-dimensions"
        
        pipe(
            keyword "array",
            dimensions,
            keyword "of",
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
                    arr <| Type.Array (i, ti)
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
        <?!> "array-ti-expr"
    
    // <ti-expr>        
    ti_expr_ref.contents <-
        [ array_ti_expr
        ; base_ti_expr  ]
        |> choice
        <?!> "ti-expr"

    // <tuple-ti-expr-tail>
    let tuple_ti : Parser<TypeInst list> =
        
        let fields =
            ti_expr
            |> commaSep1
            |> betweenWs ('(', ')')
        
        keyword "tuple" 
        >>. fields
        <?!> "tuple-ti"
            
    // <record-ti-expr-tail>
    let record_ti : Parser<TypeInst list> =
        
        let fields =
            named_ti
            |> commaSep1
            |> betweenWs('(', ')')
            
        keyword "record"
        >>. fields
        <?!> "record-ti"
    
    let instanced_type : Parser<string> =
        c '$' >>. ident
            
    // <base-ti-expr-tail>
    base_ti_expr_tail_ref.contents <-
        [ "int"          => Type.Int
        ; "bool"         => Type.Bool
        ; "string"       => Type.String
        ; "float"        => Type.Float
        ; "ann"          => Type.Ann
        ; "any"          => Type.Any    
        ; record_ti      |>> Type.Record
        ; tuple_ti       |>> Type.Tuple
        ; expr           |>> Type.Set
        ; instanced_type |>> Type.Generic
        ; ident          |>> Type.Ident ]
        |> choice
        <?!> "base-ti-tail"
    
    let id_or_op =
        name_or_quoted_value builtin_op
    
    // <call-expr>
    let call_expr : Parser<CallExpr> =
        attempt (id_or_op .>> ws .>> (followedBy (c '(')))
        .>> ws
        .>>. tupled_args
        <?!> "call-expr"
        
    let wildcard : Parser<WildCard> =
        skip '_'
        >>. notFollowedBy letter
        >>% WildCard.WildCard
    
    let absent : Parser<Absent> =
        attempt (skip "<>")
        >>% Absent
        
    // <comp-tail>
    let comp_tail : Parser<Generator list> =
        
        let gen_var =
            [ wildcard |>> IdOr.Val 
            ; ident    |>> IdOr.Id ]
            |> choice
            <?!> "gen-var"
            
        let gen_vars =
            gen_var
            |> commaSep1            
            <?!> "gen-vars"
            
        let gen_where =
            keyword "where"
            >>. expr
            <?!> "gen-where"
            
        let generator =    
            pipe(
                gen_vars,
                skip "in",
                expr,
                opt gen_where,
                (fun idents _ source filter ->
                    { Yields = idents
                    ; From = source
                    ; Where = filter })
            )
            
        generator
        |> commaSep1       
            
    // <gen-call-expr>
    let gen_call_expr =
        pipe(
            id_or_op,
            betweenWs('(', ')') comp_tail,
            betweenWs('(', ')') expr,
            fun name gens expr ->
                { Operation = name
                ; From = gens
                ; Yields = expr }
        )
        |> attempt
    
    // <array-comp>
    let array_comp : Parser<ArrayCompExpr> =
        tuple(expr, skip '|' >>. comp_tail) 
        |> betweenWs ('[', ']')
        |> attempt
        |>> (fun (expr, gens) -> 
             { Yields=expr
             ; From = gens })
        <?!> "array-comp"

    // <set-comp>
    let set_comp : Parser<SetCompExpr> =
        tuple(expr, skip '|' >>. comp_tail)
        |> betweenWs('{', '}')
        |> attempt
        |>> (fun (expr, gens) -> 
             { Yields = expr
             ; From = gens })
            
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
        <?!> "declare-item"

    // <constraint-item>
    let constraint_item : Parser<ConstraintExpr> =
        pipe(
            keyword "constraint",
            expr,
            annotations,
            fun _ expr anns ->
              { Expr = expr
              ; Annotations = anns }
        )
       
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
            constraint_item
            |>> Choice2Of2
            
        // Let constraint must go first            
        let_constraint <|> let_declare
    
    // <let-expr>
    let let_expr : Parser<LetExpr> =
                
        let item =
            let_item .>> ws .>> opt (anyOf ";,")
    
        let items =
            many1 (item .>> ws)
            |> betweenWs('{', '}')
        
        pipe(
            keyword "let",
            items,
            keyword "in",
            expr,
            (fun _a items _b body ->
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
                ))
        <?!> "let-expr"
        
    // <if-then-else-expr>
    let if_else_expr : Parser<IfThenElseExpr> =
                
        let case word =
            keyword word
            >>. expr
           .>> ws
            <?!> $"{word}-case"
            
        pipe(
            case "if",
            case "then",
            many (case "elseif" .>>. case "then"),
            opt (case "else"),
            s "endif",
            fun if_ then_ elseif_ else_ _ ->
                { If = if_
                ; Then = then_
                ; ElseIf = elseif_
                ; Else = else_ }
        )
    
    // <num-un-op>    
    let num_un_op =
        tuple(
            op builtin_num_un_op,
            num_expr_atom
        )
        
    let quoted_op : Parser<Op> =
        builtin_op
        |> between (c ''') (c ''')
        |> attempt
    
    let tuple_access_tail =
        attempt (skip '.' >>. puint8)
        <?!> "tuple-access"
        
    let record_access_tail =
        attempt (skip '.' >>. ident)
        <?!> "record-access"
        
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
    
    // <num-expr-atom-head>    
    let num_expr_atom_head : Parser<Expr> =
        [ number_range 
          number_lit 
          bracketed num_expr |>> Expr.Bracketed
          let_expr           |>> Expr.Let
          if_else_expr       |>> Expr.IfThenElse
          call_expr          |>> Expr.Call
          num_un_op          |>> Expr.UnaryOp
          ident              |>> Expr.Ident
          ]
        |> choice
        <?!> "num-expr-head"
                    
    // <num-expr-atom>        
    num_expr_atom_ref.contents <-
        num_expr_atom_head
        >>== expr_atom_tail
        <?!> "num-expr-atom"
        
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
    let tuple_literal : Parser<TupleExpr> =
        expr
        |> commaSep1
        |> betweenBrackets 
        |> attempt
          
    // <record-literal>
    let record_literal : Parser<RecordExpr> =
        tuple(ident, skip ':' >>. expr)
        |> commaSep1
        |> betweenBrackets 
        |> attempt
        
    // <expr-atom-head>    
    let expr_atom_head : Parser<Expr> =
        [
          wildcard        |>> Expr.WildCard
          absent          |>> Expr.Absent
          number_range
          number_lit
          bool_literal    |>> Expr.Bool
          string_lit      |>> Expr.String
          record_literal  |>> Expr.Record
          tuple_literal   |>> Expr.Tuple
          bracketed expr  |>> Expr.Bracketed
          let_expr        |>> Expr.Let
          if_else_expr    |>> Expr.IfThenElse
          gen_call_expr   |>> Expr.GenCall
          call_expr       |>> Expr.Call
          array_comp      |>> Expr.ArrayComp
          set_comp        |>> Expr.SetComp          
          array2d_literal |>> Expr.Array2D
          array1d_literal |>> Expr.Array1D
          set_literal     |>> Expr.Set
          un_op           |>> Expr.UnaryOp
          ident           |>> Expr.Ident
          ]
        |> choice
        <?!> "expr-atom-head"
            
    // <annotations>
    annotation_ref.contents <-
        skip "::"
        >>. expr_atom_head
        >>== expr_atom_tail
        <?!> "annotation"
    
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
        <?!> "expr"
            
    let solve_type : Parser<SolveMethod> =
        choice
          [ "satisfy" => SolveMethod.Satisfy
          ; "minimize" => SolveMethod.Minimize
          ; "maximize" => SolveMethod.Maximize ]
        
    // <solve-item>
    let solve_item : Parser<SolveItem> =
        pipe(
            keyword "solve",
            annotations,
            solve_type,
            opt expr,
            fun _ anns solveType obj ->
                match (solveType, obj) with
                | SolveMethod.Maximize, Some exp ->
                    SolveItem.Max (exp, anns)
                | SolveMethod.Minimize, Some exp ->
                    SolveItem.Min (exp, anns)
                | _ ->
                    SolveItem.Sat anns
                )
        
    // <assign-item>
    // eg ```a = 1;```
    // eg ```b == func(4);```
    let assign_item : Parser<AssignExpr> =
        attempt (ident .>> ws .>> followedBy (pchar '='))
        .>>. assign_tail
        <?!> "assign-item"
        
    // <type-inst-syn-item>
    let alias_item : Parser<TypeAlias> =
        pipe(
            keyword "type",
            ident,
            annotations,
            skip "=",
            ti_expr,
            fun _ id anns _ ti ->
                { Name = id
                ; Annotations = anns
                ; TypeInst = ti })
        
    // <output-item>
    let output_item : Parser<OutputExpr> =
        pipe(
            keyword "output",
            opt string_annotation,
            expr,
            fun _ ann expr ->
                { Expr = expr
                ; Annotation = ann })
        
    // <annotation_item>
    let annotation_item : Parser<AnnotationType> =
        pipe(
            keyword "annotation",
            ident,
            opt_or [] parameters,
            opt assign_tail,
            fun _ name pars body ->
                { Name = name
                ; Body = body
                ; Params = pars })
        <?!> "annotation-type"

    // <item>
    let item =
        [ enum_item       |>> Item.Enum
        ; constraint_item |>> Item.Constraint
        ; include_item    |>> Item.Include
        ; solve_item      |>> Item.Solve
        ; alias_item      |>> Item.Synonym
        ; output_item     |>> Item.Output
        ; predicate_item  |>> Item.Function
        ; function_item   |>> Item.Function
        ; test_item       |>> Item.Test
        ; annotation_item |>> Item.Annotation
        ; assign_item     |>> Item.Assign
        ; declare_item  ]
        |> choice
                      
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

    /// Parse and remove comments from the given model string
    // let parseComments (mzn: string) : string * List<Comment> =
    //     let comments = ResizeArray<string>()
    //     let statements = ResizeArray<string>()
    //             
    //     match parseWith Parsers.statements mzn with
    //     | Result.Ok xs ->
    //         for x in xs do
    //             match x with
    //             | Comment c ->
    //                 comments.Add c
    //             | Statement s ->
    //                 statements.Add s
    //     | Result.Error err ->
    //         failwith err.Message
    //         
    //     let comments = Seq.toList comments
    //     let statement = String.Join("\n", statements)
    //     statement, comments
    
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
            parseWith Parsers.data source
            
        result            
            
    /// Parse a '.dzn' model data file            
    let parseDataFile (filepath: string) : Result<AssignExpr list, ParseError> =
        let dzn = File.ReadAllText filepath
        let data = parseDataString dzn
        data