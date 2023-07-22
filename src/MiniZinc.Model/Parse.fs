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
                
        // Between with whitespace
        static member betweenWs (pStart : Parser<_>, pEnd : Parser<_>) =
            between (pStart .>> ws) (ws >>. pEnd)
                
        // Between with whitespace                
        static member betweenWs (s: char, e:char) =
            P.betweenWs (pchar s, pchar e)
            
        // Between with whitespace
        static member betweenWs (s: string, e:string) =
            P.betweenWs (pstring s, pstring e)
        
        // Between with whitespace        
        static member betweenWs (s: string) =
            P.betweenWs(s,s)
        
        // Between with whitespace    
        static member betweenWs (c: char) =
            P.betweenWs(c, c)
            
        // Parse 0 or more 'p' between 'start' and 'end' with optional whitespace            
        static member betweenSep (
                pStart : Parser<_>,
                pEnd : Parser<_>,
                pDelim : Parser<_>,
                [<Optional; DefaultParameterValue(false)>] many : bool,
                [<Optional; DefaultParameterValue(false)>] allowTrailing : bool
            ) : Parser<'t> -> Parser<'t list> =
                P.sepBy(pDelim, many=many, allowTrailing=allowTrailing)
                >> P.betweenWs(pStart, pEnd)
            
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
            P.sepBy(pDelim, many=true, allowTrailing=allowTrailing)
           

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
            match P.ResolveInst ti.Type with
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
            | Type.Instanced _
            | Type.Ann ->
                ty, Inst.Par
            
            // Any var item means a var tuple
            | Type.Tuple x ->
                let mutable inst = Inst.Par
                                
                let fields =
                    x
                    |> List.map (fun item ->
                        match P.ResolveInst item with
                        | ti, Inst.Par ->
                            ti
                        | ti, _ ->
                            inst <- Inst.Var
                            ti
                        )
                    
                (Type.Tuple fields), inst
                    
            // Any var field means a var record
            | Type.Record x ->
                let mutable inst = Inst.Par
                                
                let resolved =
                    x
                    |> List.map (fun field ->
                        match P.ResolveInst field with
                        | ti, Inst.Par ->
                            ti
                        | ti, _ ->
                            inst <- Inst.Var
                            ti
                        )
                    
                (Type.Record resolved), inst
                
            // A var item means a var array
            | Type.Array arr ->
                let ty, inst = P.ResolveInst arr.Elements
                (Type.Array {arr with Elements = ty}), inst
        
            
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
        
    let s (str: string) =
        pstring str
        
    let c (chr : char) =
        pchar chr
        
    let sw (str: string) =
        pstring str >>. ws
        
    let cw (chr: char) =
        pchar chr >>. ws
        
    /// Parse the given keyword
    /// Care is taken here that the given string is
    /// not part of a large string
    /// eg `keyword "function"` would not match the
    /// string "function1" 
    let keyword (name: string) : Parser<string> =
        s name
        .>> notFollowedBy (
            satisfy (fun c ->
                Char.IsDigit c || Char.IsLetter c || c = '_')
            )
        |> attempt
        .>> ws
        
    let keywordL (name: string) : Parser<string> =
        keyword name <?!> name
    
    let line_comment : Parser<string> =
        c '%' >>.
        manyCharsTill (noneOf "\r\n") (skipNewline <|> eof)
        <?!> "line-comment"

    let block_comment : Parser<string> =
        regex "\/\*([\s\S]*?)\*\/"
        <?!> "block-comment"

    let comment : Parser<string> =
        line_comment
        <|> block_comment
        |>> (fun s -> s.Trim())
        
    let (=>) (key: string) value =
        keyword key >>% value
            
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
        choice
            [ "true" => true
            ; "false" => false ]
        
    // <float-literal>        
    let float_literal : Parser<float> =
        "[0-9]+\.[0-9]+"
        |> regex 
        |>> float
        
    // <string-literal>
    let string_literal : Parser<string> =
        manySatisfy (fun c -> c <> '"')
        |> between (c '"') (c '"')
    
    let builtin_num_un_ops =
        [ "+" => Op.Add
        ; "-" => Op.Subtract ]
        
    // <builtin-num-un-op>
    let builtin_num_un_op =
        builtin_num_un_ops
        |> choice
        |>> (int >> enum<NumericUnaryOp>)
        <?!> "builtin-num-un-op"
    
    let builtin_num_bin_ops =
         [ "+"    => Op.Add
         ; "-"    => Op.Subtract 
         ; "*"    => Op.Multiply
         ; "/"    => Op.Divide         
         ; "^"    => Op.Exponent
         ; "~+"   => Op.TildeAdd
         ; "~-"   => Op.TildeSubtract
         ; "~*"   => Op.TildeMultiply
         ; "~/"   => Op.TildeDivide
         ; "div"  => Op.Div
         ; "mod"  => Op.Mod
         ; "~div" => Op.TildeDiv ]
        
    // <builtin-num-bin-op>
    let builtin_num_bin_op =
         builtin_num_bin_ops
         |> choice
         |>> (int >> enum<NumericBinaryOp>)
         <?!> "num-bin-op"
            
    let builtin_bin_ops = 
        [ "<->"       => Op.Equivalent
        ; "->"        => Op.Implies
        ; "<-"        => Op.ImpliedBy
        ; "\/"        => Op.Or
        ; "/\\"       => Op.And
        ; "<="        => Op.LessThanEqual
        ; ">="        => Op.GreaterThanEqual
        ; "=="        => Op.EqualEqual
        ; "<"         => Op.LessThan
        ; ">"         => Op.GreaterThan
        ; "="         => Op.Equal
        ; "!="        => Op.NotEqual
        ; "~="        => Op.TildeEqual
        ; "~!="       => Op.TildeNotEqual
        ; ".."        => Op.DotDot
        ; "++"        => Op.PlusPlus
        ; "xor"       => Op.Xor
        ; "in"        => Op.In
        ; "subset"    => Op.Subset
        ; "superset"  => Op.Superset
        ; "union"     => Op.Union
        ; "diff"      => Op.Diff
        ; "symdiff"   => Op.SymDiff
        ; "intersect" => Op.Intersect
        ; "default"   => Op.Default ]
        @ builtin_num_bin_ops
        
    // <builtin-bin-op>            
    let builtin_bin_op : Parser<BinaryOp> =
        builtin_bin_ops
        |> choice
        |>> (int >> enum<BinaryOp>)
        
    let builtin_un_ops =
        builtin_num_un_ops
        @ ["not" => Op.Not]

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
        <?!> "un-op"

    let builtin_ops =
        builtin_bin_ops @ builtin_un_ops

    // <builtin-op>            
    let builtin_op : Parser<Op> =
        builtin_ops
        |> choice
    
    // <array1d-literal>
    let array1d_literal : Parser<Array1dExpr> =
        expr
        |> betweenSep(c '[', c ']', c ',', allowTrailing=true)
        <?!> "array1d-literal"
            
    // <set-literal>
    let set_literal : Parser<SetLiteral>=
        expr
        |> betweenSep(c '{', c '}', c ',') 
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
        many (annotation .>> ws)
   
    // <ti-expr-and-id>
    let ti_expr_and_id : Parser<TypeInst> =
        pipe(
            ti_expr,
            c ':',
            ident,
            fun ti _ id -> { ti with Name = id } 
        )
        
    let parameters : Parser<TypeInst list> =
        ti_expr_and_id
        |> betweenSep(c '(', c ')', c ',')
        <?!> "named-args"
        
    let tupled_args : Parser<Expr list> =
        expr
        |> betweenSep(c '(', c ')', c ',')
        <?!> "tupled-args"

    // <operation-item-tail>
    // eg: even(var int: x) = x mod 2 = 0;
    let operation_item_tail =
        tuple(
            parameters,
            annotations,
            opt (c '=' >>. ws >>. expr)
        )
        
    // <predicate-item>
    let predicate_item : Parser<FunctionType> =
        pipe(
            keyword "predicate",
            ident,
            operation_item_tail,
            fun _ id (pars, anns, body) ->
            { Name = id
            ; Parameters = pars
            ; Annotations = anns
            ; Returns =
                { TypeInst.Empty
                    with
                        Type = Type.Bool
                        Inst = Inst.Var  }
            ; Body = body })
        <?!> "predicate"

    // <test_item>
    let test_item : Parser<TestItem> =
        pipe(
            keyword "test",
            ident,
            operation_item_tail,
            fun _ id (pars, anns, body) ->
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
            keyword "function",
            ti_expr,
            c ':',
            ident,
            operation_item_tail,
            (fun _ ti _ id (pars, anns, body) ->
                { Name = id
                ; Returns = ti
                ; Annotations = anns 
                ; Parameters = pars
                ; Body = body })
        )
        <?!> "function"
    
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
    let enum_item : Parser<EnumType> =            
        pipe(
            keyword "enum",
            ident,
            annotations,
            (opt_or [] (cw '=' >>. enum_cases)),
            fun _ name anns cases ->
                { Name = name
                ; Annotations = anns
                ; Cases = cases })
    
    // <include-item>
    let include_item : Parser<IncludeItem> =
        keyword "include"
        >>. string_literal
        |>> IncludeItem.Create
    
    // <var-par>
    let inst : Parser<Inst> =
        choice
            [ "var" => Inst.Var
            ; "par" => Inst.Par
            ; "any" => Inst.Any
            ; preturn  Inst.Par ]
        
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
            inst,
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
                ; Inst = inst
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
            |> betweenSep(c '[', c ']', c ',')
            <?!> "array-dimensions"
        
        pipe(
            keyword "array",
            dimensions,
            keyword "of",
            base_ti_expr,
            fun _ dims _ ty ->
                let ty, inst =
                    { Dimensions = dims; Elements=ty}
                    |> Type.Array
                    |> P.ResolveInst
                    
                let ti =
                    { TypeInst.Empty with
                        Type = ty
                        Inst = inst
                        IsArray = true }
                ti)        
        <?!> "array-ti-expr"
    
    // <ti-expr>        
    ti_expr_ref.contents <-
        [ array_ti_expr
        ; base_ti_expr  ]
        |> choice
        <?!> "ti-expr"

    // <tuple-ti-expr-tail>
    let tuple_ti : Parser<TypeInst list> =
        keyword "tuple"
        >>. betweenSep(c '(', c ')', c ',', many=true) ti_expr
        <?!> "tuple-ti"
            
    // <record-ti-expr-tail>
    let record_ti : Parser<TypeInst list> =
        keyword "record"
        >>. betweenSep(c '(', c ')', c ',', many=true) ti_expr_and_id
        <?!> "record-ti"
    
    let instanced_type : Parser<string> =
        c '$' >>. ident
            
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
        ; instanced_type   |>> Type.Instanced
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
            <?!> "gen-vars"
            
        let gen_where =
            keyword "where"
            >>. expr
            <?!> "gen-where"
            
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
    let declare_item : Parser<Item> =
        pipe(
            ti_expr_and_id,
            opt_or [] (parameters),
            annotations,
            opt (cw '=' >>. expr),
            fun ti args anns expr ->
                let ti, inst = P.ResolveInst ti
                match args with
                // No arguments - a simple declaration
                | [] ->
                    Item.Declare
                        { ti with
                            Annotations = anns
                            Value=expr }
                // Arguments - must be a function                    
                | _ ->
                    Item.Function
                        { Name = ti.Name
                        ; Returns = { ti with Name = "" }
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
        |> betweenWs '''
        |> attempt
    
    let tuple_access_tail =
        cw '.' >>. puint8 |> attempt
        
    let record_access_tail =
        cw '.' >>. ident |> attempt
        
    let array_access_tail =
        expr
        |> betweenSep(c '[', c ']', c ',', many=true)
    
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
    let num_expr_atom_head=
        [ float_literal      |>> NumExpr.Float
          int_literal        |>> NumExpr.Int
          bracketed num_expr |>> NumExpr.Bracketed
          let_expr           |>> NumExpr.Let
          if_else_expr       |>> NumExpr.IfThenElse
          call_expr          |>> NumExpr.Call
          num_un_op          |>> NumExpr.UnaryOp
          ident              |>> NumExpr.Id
          ]
        |> choice
        <?!> "num-expr-head"
        
    // <num-expr-atom-tail>
    let rec num_expr_atom_tail (expr: NumExpr) : Parser<NumExpr> =
        let tail =
            [ tuple_access_tail |>> fun i -> NumExpr.TupleAccess(i, expr)
              record_access_tail |>> fun f -> NumExpr.RecordAccess(f, expr)
              array_access_tail |>> fun xs -> NumExpr.ArrayAccess(xs, expr) ]
            |> choice
        (tail >>== num_expr_atom_tail)
        <|>
        preturn expr
            
    // <num-expr-atom>        
    num_expr_atom_ref.contents <-
        num_expr_atom_head
        >>== num_expr_atom_tail
        <?!> "num-expr-atom"
        
    // <num-expr-binop-tail>
    let num_expr_binop_tail (head: NumExpr) =
        
        let binop =
            pipe(
                op builtin_num_bin_op,
                num_expr,
                fun operator tail ->
                    NumExpr.BinaryOp (head, operator, tail)
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
        |> betweenSep(c '(', c ')', c ',', many=true, allowTrailing=true)
        |> attempt
          
    // <record-literal>
    let record_literal : Parser<RecordExpr> =
        tuple(ident, cw ':' >>. expr)
        |> betweenSep(c '(', c ')', c ',', many=true, allowTrailing=true)
        |> attempt
        
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
          ident           |>> Expr.Ident
          ]
        |> choice
            
    let string_annotation : Parser<string> =
        sw "::" >>. string_literal
        
    let string_annotations : Parser<string list> =
        string_annotation
        .>> ws
        |> many
        
    // <annotations>
    annotation_ref.contents <-
        sw "::" 
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
            
        binop <|> preturn head
            
    // <expr>
    expr_ref.contents <-
        expr_atom
        >>== expr_binop_tail
            
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
    let assign_item : Parser<AssignExpr> =
        (ident .>> ws .>> cw '=')
        |> attempt
        .>>. expr
        
    // <type-inst-syn-item>
    let alias_item : Parser<TypeAlias> =
        pipe(
            keyword "type",
            ident,
            annotations,
            s "=",
            ti_expr,
            fun _ id anns _ ti ->
                { Name = id
                ; Annotations = anns
                ; TypeInst = ti })
        
    // <output-item>
    let output_item : Parser<Expr> =
        keyword "output"
        >>. expr
        
    // <annotation_item>
    let annotation_item : Parser<AnnotationType> =
        pipe(
            keyword "annotation",
            ident,
            opt_or [] parameters,
            fun _ name pars ->
                { Name = name
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
        ; declare_item    
        ; block_comment   |>> Item.Comment ]
        |> choice
                      
    let ast : Parser<Ast> =
        ws
        >>. many (item .>> ws .>> c ';' .>> ws)
        .>> eof
        
    let data : Parser<AssignExpr list> =
        ws
        >>. many (assign_item .>> ws .>> c ';')
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
                        
        match runParserOnString (Parsers.ws >>. parser) state "" input with
        
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