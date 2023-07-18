(*

Parse.fs

Contains functions and values that enable the
parsing of a MiniZinc model/file into an
Abstract Syntax Tree (AST).

*)

namespace MiniZinc

open System
open System.IO
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
        
    type private ParseUtils () =
        
        // Between with whitespace
        static member betweenWs (pStart : Parser<_>, pEnd : Parser<_>) =
            fun parser ->
                between (pStart .>> ws) (ws >>. pEnd) parser
                
        // Between with whitespace                
        static member betweenWs (s: char, e:char) =
            ParseUtils.betweenWs (pchar s, pchar e)
            
        // Between with whitespace
        static member betweenWs (s: string, e:string) =
            ParseUtils.betweenWs (pstring s, pstring e)
        
        // Between with whitespace        
        static member betweenWs (s: string) =
            ParseUtils.betweenWs(s,s)
        
        // Between with whitespace    
        static member betweenWs (c: char) =
            ParseUtils.betweenWs(c)
            
        // Parse 0 or more 'p' between 'start' and 'end' with optional whitespace            
        static member betweenSep (
                pStart : Parser<_>,
                pEnd : Parser<_>,
                pDelim : Parser<_>,
                [<Optional; DefaultParameterValue(false)>] many : bool,
                [<Optional; DefaultParameterValue(false)>] allowTrailing : bool
            ) : Parser<'t> -> Parser<'t list> =
                ParseUtils.sepBy(pDelim, many=many, allowTrailing=allowTrailing)
                >> ParseUtils.betweenWs(pStart, pEnd)
            
        // Parse 'p' separated by 'delim'.  Whitespace is consumed  
        static member sepBy (
                pDelim : Parser<_>,
                [<Optional; DefaultParameterValue(false)>] many : bool,
                [<Optional; DefaultParameterValue(false)>] allowTrailing : bool
            ) : Parser<'t> -> Parser<'t list> =
            
            fun p ->
                
                let p' =
                    p.>> ws
                    
                let pDelim' =
                    pDelim.>> ws
                    
                let items : Parser<'t list> =    
                    match many, allowTrailing with
                    | false, false ->
                        sepBy p' pDelim'
                    | false, true ->
                        sepEndBy p' pDelim'
                    | true, false ->
                        sepBy1 p' pDelim'
                    | true, true ->
                        sepEndBy1 p' pDelim'
                        
                items                    

        static member sepBy1 (
                pDelim : Parser<_>,
                [<Optional; DefaultParameterValue(false)>] allowTrailing : bool
            ) =
            ParseUtils.sepBy(pDelim, many=true, allowTrailing=allowTrailing)
        
        static member lookup([<ParamArray>] parsers: Parser<'t>[]) =
            choice parsers
            
        // Parse an Operator            
        static member pOp<'T when 'T:enum<int>>(symbol: string, value: 'T) : Parser<int> =
            let first = symbol.Chars 0
            let p =
                match Char.IsLetter first with
                // Words (eg: diff) handled via keyword
                | true ->
                    attempt (
                       pstring symbol
                       .>> notFollowedBy letter
                    )
                | false ->
                    pstring symbol
            let v = LanguagePrimitives.EnumToValue value                    
            p >>% v            

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
            
        static member tuple(a: Parser<'a>, b: Parser<'b>, c: Parser<'c>, d: Parser<'d>) =
            tuple4 (a .>> ws) (b .>> ws) (c .>> ws) d
            
        static member tuple(a: Parser<'a>, b: Parser<'b>, c: Parser<'c>, d: Parser<'d>, e: Parser<'e>) =
            tuple5 (a .>> ws) (b .>> ws) (c .>> ws) (d .>> ws) e

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
        /// This function returns the typeinst with the correctly
        /// inferred Inst for every TypeInst and its children.
        /// </remarks>
        static member ResolveInst (ti: TypeInst) =
            match ParseUtils.ResolveInst ti.Type with
            // If the type is a Var it overrides the setting here
            | ty, Inst.Var ->
                { ti with Type = ty; Inst = Inst.Var }, Inst.Var
            // Otherwise use the existing value
            | ty, _ ->
                { ti with Type = ty; }, ti.Inst
                
            
        static member ResolveInst (ty: Type) =
            match ty with
            | Type.Int 
            | Type.Bool 
            | Type.String 
            | Type.Float 
            | Type.Ident _ 
            | Type.Set _
            | Type.Ann ->
                ty, Inst.Par
            
            // Any var item means a var tuple
            | Type.Tuple x ->
                let mutable inst = Inst.Par
                                
                let fields =
                    x.Fields
                    |> List.map (fun item ->
                        match ParseUtils.ResolveInst item with
                        | ty, Inst.Var ->
                            inst <- Inst.Var
                            ty
                        | ty, _ -> ty
                        )
                    
                (Type.Tuple {Fields = fields}), inst
                    
            // Any var field means a var record
            | Type.Record x ->
                let mutable inst = Inst.Par
                                
                let resolved =
                    x.Fields
                    |> List.map (fun field ->
                        match ParseUtils.ResolveInst field.TypeInst with
                        | ti, Inst.Var ->
                            inst <- Inst.Var
                            { field with TypeInst = ti }
                        | ti, _ ->
                            { field with TypeInst = ti }
                        )
                    
                (Type.Record {x with Fields=resolved}), inst
                
            // A var item means a var array
            | Type.Array arr ->
                let ty, inst = ParseUtils.ResolveInst arr.Elements
                (Type.Array {arr with Elements = ty}), inst
        
            
    open type ParseUtils
        
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
            
    let (<?!>) (p: Parser<'t>) label : Parser<'t> =
        p
        <?> label
        <!> label
                    
    let opt_or backup p =
        (opt p) |>> Option.defaultValue backup
       
    let simple_id : Parser<Ident> = 
        regex "_?[A-Za-z][A-Za-z0-9_]*"

    let quoted_id : Parser<Ident> =
        regex "'[^'\x0A\x0D\x00]+'"
        
    // <ident>
    let ident : Parser<Ident> =
        regex "_?[A-Za-z][A-Za-z0-9_]*|'[^'\x0A\x0D\x00]+'"
        <?!> "identifier"
        
    let s (string: string) =
        pstring string
        
    let c (char : char) =
        pchar char
        
    let sw s =
        pstring s >>. ws
            
    let wsw s =
        ws >>. pstring s >>. ws
        
    let cw c =
        pchar c >>. ws

    let wcw c =
        ws >>. pchar c >>. ws        
        
    // Parse a keyword, ensures its not a part of a larger string
    let keyword (name: string) : Parser<string> =
        s name
        >>= (fun id ->
             [ notFollowedBy digit
             ; notFollowedBy letter
             ; fail $"Expected {name}"]
             |> choice
             >>. preturn id)
        |> attempt
        
    // Parse a keyword, ensures its not a part of a larger string
    let keyword1 (name: string) =
        (s name >>. ws1)
        |> attempt
    
    let line_comment : Parser<string> =
        c '%' >>.
        manyCharsTill (noneOf "\r\n") (skipNewline <|> eof)

    let block_comment : Parser<string> =
        regex "\/\*([\s\S]*?)\*\/"

    let comment : Parser<string> =
        line_comment
        <|> block_comment
        |>> (fun s -> s.Trim())
        
    let (=>) (key: string) value =
        pstring key >>% value
        
    let (=!>)(key: string) (value: 'T) =
        pOp(key, value)
            
    let value_or_quoted_name (parser: Parser<'T>) : Parser<IdOr<'T>> =
        
        let value =
            parser |>> IdOr.Val
        
        let name =
            simple_id
            |> betweenWs '`'
            |> attempt
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
        
    // <int-literal>
    let int_literal =
        many1Satisfy Char.IsDigit
        |>> int
    
    // <bool-literal>    
    let bool_literal : Parser<bool> =
        lookup(
            "true" => true,
            "false" => false
            )
        
    // <float-literal>        
    let float_literal : Parser<float> =
        """[0-9]+"."[0-9]+|[0-9]+"."[0-9]+[Ee][-+]?[0-9]+|[0-9]+[Ee][-+]?[0-9]+|0[xX]([0-9a-fA-F]*"."[0-9a-fA-F]+|[0-9a-fA-F]+".")([pP][+-]?[0-9]+)|(0[xX][0-9a-fA-F]+[pP][+-]?[0-9]+)"""
        |> regex 
        |>> float
        
    // <string-literal>
    let string_literal : Parser<string> =
        manySatisfy (fun c -> c <> '"')
        |> between (c '"') (c '"')
    
    let builtin_num_un_ops =
        [ "+" =!> NumericBinaryOp.Add
        ; "-" =!> NumericBinaryOp.Subtract ]
        
    // <builtin-num-un-op>
    let builtin_num_un_op =
        builtin_num_un_ops
        |> choice
        |>> enum<NumericUnaryOp>
    
    let builtin_num_bin_ops =
         [ "+"    =!> BinaryOp.Add
         ; "-"    =!> BinaryOp.Subtract 
         ; "*"    =!> BinaryOp.Multiply
         ; "/"    =!> BinaryOp.Divide         
         ; "^"    =!> BinaryOp.Exponent
         ; "~+"   =!> BinaryOp.TildeAdd
         ; "~-"   =!> BinaryOp.TildeSubtract
         ; "~*"   =!> BinaryOp.TildeMultiply
         ; "~/"   =!> BinaryOp.TildeDivide
         ; "div"  =!> BinaryOp.Div
         ; "mod"  =!> BinaryOp.Mod
         ; "~div" =!> BinaryOp.TildeDiv ]
        
    // <builtin-num-bin-op>
    let builtin_num_bin_op =
         builtin_num_bin_ops
         |> choice
         |>> enum<NumericBinaryOp>
            
    let builtin_bin_ops = 
        [ "<->"       =!> BinaryOp.Equivalent
        ; "->"        =!> BinaryOp.Implies
        ; "<-"        =!> BinaryOp.ImpliedBy
        ; "\/"        =!> BinaryOp.Or
        ; "/\\"       =!> BinaryOp.And
        ; "<="        =!> BinaryOp.LessThanEqual
        ; ">="        =!> BinaryOp.GreaterThanEqual
        ; "=="        =!> BinaryOp.EqualEqual
        ; "<"         =!> BinaryOp.LessThan
        ; ">"         =!> BinaryOp.GreaterThan
        ; "="         =!> BinaryOp.Equal
        ; "!="        =!> BinaryOp.NotEqual
        ; "~="        =!> BinaryOp.TildeEqual
        ; "~!="       =!> BinaryOp.TildeNotEqual
        ; ".."        =!> BinaryOp.DotDot
        ; "++"        =!> BinaryOp.PlusPlus
        ; "xor"       =!> BinaryOp.Xor
        ; "in"        =!> BinaryOp.In
        ; "subset"    =!> BinaryOp.Subset
        ; "superset"  =!> BinaryOp.Superset
        ; "union"     =!> BinaryOp.Union
        ; "diff"      =!> BinaryOp.Diff
        ; "symdiff"   =!> BinaryOp.SymDiff
        ; "intersect" =!> BinaryOp.Intersect
        ; "default"   =!> BinaryOp.Default ]
        @ builtin_num_bin_ops
        
    // <builtin-bin-op>            
    let builtin_bin_op : Parser<BinaryOp> =
        builtin_bin_ops
        |> choice
        |>> enum<BinaryOp>
        
    let builtin_un_ops =
        builtin_num_un_ops
        @ ["not" =!> UnaryOp.Not]

    // <builtin-un-op>           
    let builtin_un_op : Parser<UnaryOp> =
        builtin_un_ops
        |> choice
        |>> enum<UnaryOp>

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
    let (num_expr: Parser<NumExpr>, num_expr_ref) =
        createParserForwardedToRef<NumExpr, ParserState>()
        
    // <num-expr-atom>
    let (num_expr_atom: Parser<NumExpr>, num_expr_atom_ref) =
        createParserForwardedToRef<NumExpr, ParserState>()
        
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

    let builtin_ops =
        builtin_bin_ops @ builtin_un_ops

    // <builtin-op>            
    let builtin_op : Parser<Op> =
        builtin_ops
        |> choice
        |>> enum<Op>
        
    // 0 .. 10
    let range_expr : Parser<RangeExpr> =
        pipe(
            num_expr,
            s "..",
            num_expr,
            fun a _ b -> RangeExpr(a,b)
        )
        |> attempt 
        <?!> "range-expr"
    
    // <array1d-literal>
    let array1d_literal : Parser<Array1dExpr> =
        expr
        |> betweenSep(c '[', c ']', c ',', allowTrailing=true)
        <?!> "array1d-literal"
            
    // <set-literal>
    let set_literal : Parser<SetLiteral>=
        betweenSep(c '{', c '}', c ',') expr
        |> attempt
        |>> (fun exprs -> {Elements = exprs})
                
    // <array2d-literal>
    let array2d_literal : Parser<Array2dExpr> =
        
        let row =
            expr
            |> sepBy1(c ',', allowTrailing=true)
            
        let rows =
            let sep =
                attempt (c '|' >>. notFollowedBy (c ']'))
            row
            |> sepBy(sep, allowTrailing=true)
        
        let array =
            rows
            |> betweenWs ("[|", "|]")
                
        array
        
    // <annotations>        
    let annotations : Parser<Annotations> =
        many (annotation.>> ws)        
   
    // <ti-expr-and-id>
    let ti_expr_and_id : Parser<NamedTypeInst> =
        pipe(
            ti_expr,
            c ':',
            ident,
            annotations,
            fun ti _ name anns ->
                { TypeInst = ti
                ; Name=name
                ; Annotations=anns }
        )
        
    let parameters : Parser<Parameters> =
        ti_expr_and_id
        |> betweenSep(c '(', c ')', c ',')
        
    let arguments : Parser<Expr list> =
        expr
        |> betweenSep(c '(', c ')', c ',')

    // <operation-item-tail>
    // eg: even(var int: x) = x mod 2 = 0;
    let operation_item_tail =
        tuple(
            ident,
            parameters,
            annotations,
            opt (c '=' >>. ws >>. expr)
        )
        
    // <predicate-item>
    let predicate_item : Parser<FunctionItem> =
        keyword1 "predicate"
        >>. operation_item_tail
        |>> (fun (id, pars, anns, body) ->
            { Name = id
            ; Parameters = pars
            ; Annotations = anns
            ; Returns =
                { Type = Type.Bool
                ; Inst = Inst.Var
                ; IsSet = false 
                ; IsOptional = false
                ; IsArray = false }
            ; Body = body } )

    // <test_item>
    let test_item : Parser<TestItem> =
        keyword1 "test"
        >>. operation_item_tail
        |>> (fun (id, pars, anns, body) ->
            { Name = id
            ; Parameters = pars
            ; Annotations = anns
            ; Body = body }
            )
        
    // <function-item>
    let function_item : Parser<FunctionItem> =
        pipe(
            keyword1 "function",
            ti_expr,
            c ':',
            operation_item_tail,
            (fun _ ti _ (id, pars, anns, body) ->
                { Name = id
                ; Returns = ti
                ; Annotations = anns 
                ; Parameters = pars
                ; Body = body })
        )
    
    // <enum-case-list>    
    let enum_cases : Parser<EnumCases list> =
                
        let enum_names : Parser<EnumCases> =
            ident
            |> betweenSep(c '{', c '}', c ',')
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
      
        [ enum_names; anon_enum; enum_call ]
        |> choice
        |> sepBy(s "++")
          
    // <enum-item>
    let enum_item : Parser<EnumItem> =
            
        let enum_name =
            keyword1 "enum" >>. ident .>> ws
            
        pipe(
            enum_name,
            annotations,
            (opt_or [] (cw '=' >>. enum_cases)),
            fun name anns cases ->
                { Name = name
                ; Annotations = anns
                ; Cases = cases })
    
    // <include-item>
    let include_item : Parser<IncludeItem> =
        keyword1 "include"
        >>. string_literal
        |>> IncludeItem.Create
    
    // <var-par>
    let var_par : Parser<Inst> =
        lookup(
            "var " => Inst.Var,
            "par " => Inst.Par
        )
       .>> ws
        |> opt_or Inst.Par
        
    // <opt-ti>        
    let opt_ti =
        keyword1 "opt"
        >>% true
        |> opt_or false
    
    // <set-ti>    
    let set_ti =
        keyword1 "set" >>. keyword1 "of"
        >>% true
        |> opt_or false
   
    // <base-ti-expr>
    let base_ti_expr : Parser<TypeInst> =
        pipe(
            var_par,
            set_ti,
            opt_ti,
            base_ti_expr_tail,
            (fun inst set opt typ ->
                { Type = typ
                ; IsOptional = opt
                ; IsSet = set
                ; IsArray = false 
                ; Inst = inst }
            )
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
            |> betweenSep(c '[', c ']', c ',')
            <?!> "array-dimensions"
        
        pipe(
            keyword "array",
            dimensions,
            s "of",
            base_ti_expr,
            fun _ dims _ ty ->
                let ty, inst =
                    { Dimensions = dims; Elements=ty}
                    |> Type.Array
                    |> ParseUtils.ResolveInst
                    
                { Type = ty
                ; Inst = inst
                ; IsSet = false
                ; IsArray = true 
                ; IsOptional = false })
        <?!> "array-ti-expr"
    
    // <ti-expr>        
    ti_expr_ref.contents <-
        [ array_ti_expr
        ; base_ti_expr  ]
        |> choice
        <?!> "ti-expr"

    // <tuple-ti-expr-tail>
    let tuple_ti : Parser<TupleType> =
        keyword "tuple"
        >>. betweenSep(c '(', c ')', c ',', many=true) ti_expr
        |>> (fun fields -> {Fields=fields})
        <?!> "tuple-ti"
            
    // <record-ti-expr-tail>
    let record_ti : Parser<RecordType> =
        keyword "record"
        >>. betweenSep(c '(', c ')', c ',', many=true) ti_expr_and_id
        |>> (fun fields -> {Fields=fields})
        <?!> "record-ti"
            
    // <base-ti-expr-tail>
    base_ti_expr_tail_ref.contents <-
        [ keyword "bool"   >>% Type.Bool
        ; keyword "int"    >>% Type.Int
        ; keyword "string" >>% Type.String
        ; keyword "float"  >>% Type.Float
        ; keyword "ann"    >>% Type.Ann
        ; record_ti        |>> Type.Record
        ; tuple_ti         |>> Type.Tuple
        ; expr             |>> Type.Set 
        ; ident            |>> Type.Ident ]
        |> choice
        <?!> "base-ti-tail"
    
    let id_or_op =
        name_or_quoted_value builtin_op
    
    // <call-expr>
    let call_expr : Parser<CallExpr> =
        tuple(
            id_or_op,
            betweenSep(c '(', c ')', c ',') expr)
        |> attempt
        <?!> "call-expr"
        
    let wildcard : Parser<WildCard> =
        c '_'
        >>. notFollowedBy letter
        >>% WildCard.WildCard
    
    let absent : Parser<Absent> =
        attempt (s "<>")
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
            |> sepBy(c ',', many=true)
            
        let gen_where =
            keyword1 "where"
            >>. expr
            
        let generator =    
            pipe(
                gen_vars,
                s "in",
                expr,
                opt gen_where,
                (fun idents _ source filter ->
                    { Yields = idents
                    ; From = source
                    ; Where = filter })
            )
            <?!> "generator"
            
        generator
        |> sepBy(c ',', many=true)       
            
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
        <?!> "gen-call"
    
    // <array-comp>
    let array_comp : Parser<ArrayCompExpr> =
        tuple(expr, cw '|' >>. comp_tail) 
        |> betweenWs ('[', ']')
        |> attempt
        |>> (fun (expr, gens) -> 
             { Yields=expr
             ; From = gens })
        <?!> "array-comp"

    // <set-comp>
    let set_comp : Parser<SetCompExpr> =
        tuple(expr, cw '|' >>. comp_tail)
        |> betweenWs('{', '}')
        |> attempt
        |>> (fun (expr, gens) -> 
             { Yields=expr
             ; From = gens })
        <?!> "set-comp"
            
    // <declare-item>
    let var_decl_item : Parser<DeclareItem> =
        pipe(
            ti_expr_and_id,
            opt (cw '=' >>. expr),
            fun nti expr ->
                let ti, inst = ParseUtils.ResolveInst nti.TypeInst
                { Name = nti.Name
                ; TypeInst = ti
                ; Annotations = nti.Annotations
                ; Expr = expr }
        )

    // <constraint-item>
    let constraint_item : Parser<ConstraintItem> =
        pipe(
            keyword "constraint",
            expr,
            annotations,
            fun _ expr anns ->
              { Expr = expr
              ; Annotations = anns }
        )
            
    // <annotation-item>
    let annotation_item : Parser<AnnotationItem>=
        pipe(
          keyword1 "annotation",
          ident,
          opt parameters,
          fun _ id ->
            function
            | None ->
                AnnotationItem.Name id
            | Some xs ->
                AnnotationItem.Call (id, xs)
        )
        
    // <let-item>
    let let_item : Parser<LetLocal> =
        choice
         [constraint_item |>> Choice2Of2
          var_decl_item   |>> Choice1Of2 ]
    
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
            s "in",
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
                
        let case kw =
            keyword kw
            >>. expr
           .>> ws
            <?!> $"{keyword}-case"
            
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
        |> betweenWs '''
        |> attempt
        
    // <expr-atom-tail>        
    let expr_atom_tail if_tuple if_record if_array head =
                    
        let item_access_tail =
            
            let tuple_access_tail =
                puint8
                |>> (fun item -> if_tuple(item, head)) 
                
            let record_access_tail =
                ident
                |>> (fun field -> if_record(field, head))
            
            cw '.'
            >>. (tuple_access_tail <|> record_access_tail)

        let array_access_tail =
            expr
            |> betweenSep(c '[', c ']', c ',', many=true)
            |>> (fun access -> if_array (access, head))
                    
        item_access_tail
        <|> array_access_tail
        <|> preturn head
        <?!> "atom-tail"

    // <num-expr-atom-head>    
    let num_expr_atom_head=
        [ float_literal      |>> NumExpr.Float
          int_literal        |>> NumExpr.Int
          bracketed num_expr |>> NumExpr.Bracketed
          let_expr           |>> NumExpr.Let
          if_else_expr       |>> NumExpr.IfThenElse
          call_expr          |>> NumExpr.Call
          num_un_op          |>> NumExpr.UnaryOp
          quoted_op          |>> NumExpr.Op 
          ident              |>> NumExpr.Id
          ]
        |> choice
        
    // <num-expr-atom>        
    num_expr_atom_ref.contents <-
        num_expr_atom_head
       .>> ws
        >>= (expr_atom_tail
                NumExpr.TupleAccess
                NumExpr.RecordAccess
                NumExpr.ArrayAccess)
        <?!> "num-expr-atom"
        
    // <num-expr-binop-tail>
    let num_expr_binop_tail =        
        (op builtin_num_bin_op)
       .>> ws
        .>>. num_expr
        |> attempt
        |> opt
       
    // <num-expr>
    num_expr_ref.contents <-
        pipe(
            num_expr_atom,
            num_expr_binop_tail,
            fun head tail ->
                match tail with
                | None ->
                    head
                | Some (op, right) ->
                    NumExpr.BinaryOp (head, op, right)
            )
            
    // <tuple-literal>
    // TODO - Required trailing comma if tuple is 1 element
    let tuple_literal : Parser<TupleExpr> =
        expr
        |> betweenSep(c '(', c ')', c ',', many=true, allowTrailing=true)
        |> attempt
        <?!> "tuple-lit"
          
    // <record-literal>
    let record_literal : Parser<RecordExpr> =
        tuple(ident, cw ':' >>. expr)
        |> betweenSep(c '(', c ')', c ',', many=true, allowTrailing=true)
        |> attempt
        <?!> "record-lit"
        
    // <expr-atom-head>    
    let expr_atom_head=
        [
          float_literal   |>> Expr.Float
          int_literal     |>> Expr.Int
          bool_literal    |>> Expr.Bool
          string_literal  |>> Expr.String
          wildcard        |>> Expr.WildCard
          absent          |>> Expr.Absent
          record_literal  |>> Expr.Record
          tuple_literal   |>> Expr.Tuple
          bracketed expr  |>> Expr.Bracketed
          let_expr        |>> Expr.Let
          if_else_expr    |>> Expr.IfThenElse
          gen_call_expr   |>> Expr.GenCall
          call_expr       |>> Expr.Call
          array_comp      |>> Expr.ArrayComp
          set_comp        |>> Expr.SetComp          
          array2d_literal |>> Expr.Array2d
          array1d_literal |>> Expr.Array1d
          set_literal     |>> Expr.Set
          un_op           |>> Expr.UnaryOp
          quoted_op       |>> Expr.Op
          ident           |>> Expr.Ident
          ]
        |> choice
            
    // <annotations>
    annotation_ref.contents <-
        pipe(
            s "::",
            ident,
            opt_or [] arguments,
            fun _ name args ->
                { Name = name; Args = args })
    
    // <expr-atom>        
    expr_atom_ref.contents <-
        expr_atom_head
       .>> ws
        >>= expr_atom_tail
                Expr.TupleAccess
                Expr.RecordAccess
                Expr.ArrayAccess
            
    // <expr-binop-tail>
    let expr_binop_tail =
        (op builtin_bin_op) .>> ws .>>. expr
            
    // <expr>
    expr_ref.contents <-
        pipe(
            expr_atom,
            expr_binop_tail |> attempt |> opt,
            fun head tail ->
                match tail with
                | None ->
                    head
                | Some (op, right) ->
                    Expr.BinaryOp (head, op, right)
            )
            
    let solve_type : Parser<SolveMethod> =
        lookup(
          "satisfy" => SolveMethod.Satisfy,
          "minimize" => SolveMethod.Minimize,
          "maximize" => SolveMethod.Maximize
        )
        
    // <solve-item>
    let solve_item : Parser<SolveItem> =
        pipe(
            keyword1 "solve",
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
    let assign_item : Parser<AssignItem> =
        (ident .>> ws .>> cw '=')
        |> attempt
        .>>. expr
        
    // <type-inst-syn-item>
    let alias_item : Parser<TypeAlias> =
        pipe(
            keyword1 "type",
            ident,
            annotations,
            s "=",
            ti_expr,
            fun _ id anns _ ti ->
                { Name = id
                ; Annotations = anns
                ; TypeInst = ti })
        
    // <output-item>
    let output_item : Parser<OutputItem> =
        keyword1 "output"
        >>. expr
        |>> (fun expr -> {Expr = expr})

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
        ; var_decl_item   |>> Item.Declare
        ; block_comment   |>> Item.Comment ]
        |> choice
                      
    let ast : Parser<Ast> =
        ws
        >>. sepEndBy1 item (wcw ';')
        .>> eof
        
    let data : Parser<AssignItem list> =
        ws
        >>. sepEndBy1 assign_item (wcw ';')
        .>> eof

[<AutoOpen>]       
module Parse =
    
    open System.Text.RegularExpressions
    
    /// Parse and remove comments from the given model string
    let parseComments (mzn: string) : string * List<Comment> =
        let comments = ResizeArray<string>()
        let line_comment = "%(.*)$"
        let block_comment = "\/\*([\s\S]*?)\*\/"
        let pattern = $"{line_comment}|{block_comment}"
        let evaluator =
            MatchEvaluator(fun m ->
                
                let comment =
                    match m.Groups[1], m.Groups[2] with
                    // Line comment
                    | m, _ when m.Success ->
                        m.Value
                    // Block comment
                    | _, m when m.Success ->
                        m.Value
                    | _ ->
                        ""
                if not (String.IsNullOrWhiteSpace comment) then
                    comments.Add comment
                        
                ""
            )
        
        let output =
            Regex.Replace(mzn, pattern, evaluator, RegexOptions.Multiline)

        let output =
            output.Trim()
        
        let comments = Seq.toList comments
        output, comments
        
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
    
    /// Parse '.dzn' style model data from a string        
    let parseDataString (dzn: string) : Result<AssignItem list, ParseError> =
                            
        let source, comments =
            parseComments dzn
            
        let result =
            parseWith Parsers.data source
            
        result            
            
    /// Parse a '.dzn' model data file            
    let parseDataFile (filepath: string) : Result<AssignItem list, ParseError> =
        let dzn = File.ReadAllText filepath
        let data = parseDataString dzn
        data