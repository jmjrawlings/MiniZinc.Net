﻿(*

Parse.fs

Contains functions and values that enable the
parsing of a MiniZinc model/file into an
Abstract Syntax Tree (AST).

*)

namespace MiniZinc

open System
open System.Runtime.InteropServices
open System.Text
open FParsec
open MiniZinc

type ParseError =
    { Message: string
    ; Line : int64
    ; Column : int64
    ; Index : int64
    ; Trace: string }


module Parsers =

    type UserState() =
        let sb = StringBuilder()
        let mutable indent = 0

        member this.Indent
            with get() = indent
            and set(v : int) = indent <- v
                
        member this.write (msg: string) =
            sb.AppendLine msg
            
        member this.Message =
            sb.ToString()

    type P<'t> = Parser<'t, UserState>
    
    [<Struct>]
    type DebugEvent<'a> =
        | Enter
        | Leave of Reply<'a>
    
    let addToDebug (stream: CharStream<UserState>) label event =
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
    let (<!>) (p: P<'t>) label : P<'t> =
        fun stream ->
            addToDebug stream label Enter
            let reply = p stream
            addToDebug stream label (Leave reply)
            reply
            
    (*
    Adding debug information to the parser is very
    slow so we only enable it for DEBUG
    *)
    // let (<?!>) (p: P<'t>) label : P<'t> =
    //     p <?> label <!> label
    // #else
    let (<?!>) (p: P<'t>) label : P<'t> =
        p <?> label
                    
    let opt_or backup p =
        (opt p) |>> Option.defaultValue backup

    /// <summary>
    /// Overloaded methods that clean up parsing code a bit
    /// </summary>
    /// <remarks>
    /// The shorthand used here is:
    /// - p: parse
    /// - s: space
    /// - 1: at least 1
    ///
    /// And so `sp1` would be "at least one space followed by p" etc
    ///
    /// All of the overload just make calling easier instead of
    /// having to wrap chars and strings in `pchar` and `pstring`
    /// respectively
    /// </remarks>
    type P () =
        
        // parse 
        static member p (x: char): P<char> =
            pchar x
            
        // parse 
        static member p (x: string) : P<string> =
            pstring x
        
        // parse spaces
        static member ps (x: P<'t>) : P<'t> =
            x .>> spaces
        
        // parse spaces    
        static member ps (x: string) : P<string> =
            pstring x .>> spaces
            
        // parse spaces
        static member ps (x: char) : P<char> =
            pchar x .>> spaces
        
        // parse spaces1
        static member ps1 (x: P<'t>) : P<'t> =
            x .>> spaces1
        
        // parse spaces1    
        static member ps1 (x: string) : P<string> =
            pstring x .>> spaces1

        // parse spaces1                        
        static member ps1 (x: char) : P<char> =
            pchar x .>> spaces1            
        
        // space parse space    
        static member sps (x: P<'t>) : P<'t> =
            between spaces spaces x
        
        // space parse space
        static member sps (x: string) : P<string> =
            P.sps (pstring x)
            
        // space parse space            
        static member sps (c: char) : P<char> =
            P.sps (pchar c)
            
        // space1 parse space1
        static member sps1 (x: P<'t>) : P<'t> =
            between spaces1 spaces1 x

        // space1 parse space1        
        static member sps1 (x: string) : P<string> =
            P.sps1 (pstring x)
            
        // space1 parse space1
        static member sps1 (c: char) : P<char> =
            P.sps1 (pchar c)
        
        // space parse     
        static member sp (x: P<'t>) : P<'t> =
            spaces >>. x

        // space parse     
        static member sp (c: char) =
            P.sp (pchar c)

        // space parse     
        static member sp (s: string) =
            P.sp (pstring s)
            
        // space1 parse
        static member sp1 (x: P<'t>) : P<'t> =
            spaces1 >>. x
            
        // space1 parse
        static member sp1 (c: char) =
            P.sp1 (pchar c)
            
        // space1 parse            
        static member sp1 (s: string) =
            P.sp1 (pstring s)

        // Parse between 'start' and 'end' with optional whitespace 
        static member between (
                pStart : P<_>,
                pEnd : P<_>,
                [<Optional; DefaultParameterValue(true)>] ws : bool
            ) =
                let pStart', pEnd' =
                    match ws with
                    | true -> (pStart .>> spaces), (spaces >>. pEnd)
                    | false -> pStart, pEnd
                between pStart' pEnd'
                
        static member between(a: string, b: string, [<Optional; DefaultParameterValue(true)>] ws : bool) =
            P.between(pstring a, pstring b, ws=ws)
            
        static member between(a: char, b: char, [<Optional; DefaultParameterValue(true)>] ws : bool) =
            P.between(pchar a, pchar b, ws=ws)

        // Parse 0 or more 'p' between 'start' and 'end' with optional whitespace            
        static member between (
                pStart : P<_>,
                pEnd : P<_>,
                pDelim : P<_>,
                [<Optional; DefaultParameterValue(false)>] many : bool,
                [<Optional; DefaultParameterValue(false)>] allowTrailing : bool
            ) =
                P.sepBy(pDelim, many=many, allowTrailing=allowTrailing)
                >> P.between(pStart, pEnd, ws=true)

        static member between1 (
                pStart : P<_>,
                pEnd : P<_>,
                pDelim : P<_>,
                [<Optional; DefaultParameterValue(false)>] allowTrailing : bool
            ) =
                P.between(pStart, pEnd, pDelim, many=true, allowTrailing=allowTrailing)

                    
        // Parse 'p' separated by 'delim'.  Whitespace is consumed  
        static member sepBy (
                pDelim : P<_>,
                [<Optional; DefaultParameterValue(false)>] many : bool,
                [<Optional; DefaultParameterValue(false)>] allowTrailing : bool
            ) =
            
            fun p ->
                
                let p' =
                    p .>> spaces
                    
                let pDelim' =
                    pDelim .>> spaces
                    
                let items : P<'t list> =    
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
                pDelim : P<_>,
                [<Optional; DefaultParameterValue(false)>] allowTrailing : bool
            ) =
            P.sepBy(pDelim, many=true, allowTrailing=allowTrailing)
        
        static member lookup([<ParamArray>] parsers: P<'t>[]) =
            choice parsers
            
        // Parse an Operator            
        static member pOp<'T when 'T:enum<int>>(symbol: string, value: 'T) : P<int> =
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
    
        /// <summary>
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
            | Type.Id _ 
            | Type.Literal _ 
            | Type.Range _
            | Type.Variable _ ->
                ty, Inst.Par
            
            // Any var item means a var tuple
            | Type.Tuple (TupleType.TupleType items) ->
                let mutable inst = Inst.Par
                                
                let resolved =
                    items
                    |> List.map (fun item ->
                        match P.ResolveInst item with
                        | ty, Inst.Var ->
                            inst <- Inst.Var
                            ty
                        | ty, _ -> ty
                        )
                    
                (Type.Tuple (TupleType.TupleType resolved)), inst
                    
            // Any var field means a var record
            | Type.Record (RecordType.RecordType fields) ->
                let mutable inst = Inst.Par
                                
                let resolved =
                    fields
                    |> List.map (fun (name, field) ->
                        match P.ResolveInst field with
                        | ty, Inst.Var ->
                            inst <- Inst.Var
                            name, ty
                        | ty, _ ->
                            name, ty
                        )
                    
                (Type.Record (RecordType.RecordType resolved)), inst
                
            // A var item means a var tuple
            | Type.List (ListType.ListType itemType) ->
                let ty, inst = P.ResolveInst itemType
                (Type.List (ListType.ListType itemType)), inst
                
            // A var item means a var array
            | Type.Array (ArrayType.ArrayType (dims, itemType)) ->
                let ty, inst = P.ResolveInst itemType
                (Type.Array (ArrayType.ArrayType (dims, ty))), inst

    open type P
       
    let simple_id : P<Id> = 
        regex "_?[A-Za-z][A-Za-z0-9_]*"

    let quoted_id : P<Id> =
        regex "'[^'\x0A\x0D\x00]+'"
        
    // <ident>
    let id : P<Id> =
        regex "_?[A-Za-z][A-Za-z0-9_]*|'[^'\x0A\x0D\x00]+'"
        <?!> "identifier"

    // Parse a keyword, ensures its not a part of a larger string
    let kw (name: string) =
        attempt (
            p name
            .>> notFollowedBy letter
            .>> spaces
        )
        
    // Parse a keyword, ensures its not a part of a larger string
    let kw1 (name: string) =
        attempt (
            p name
            .>> notFollowedBy letter
            >>. spaces1
        )
    
    let line_comment : P<string> =
        p '%' >>.
        manyCharsTill (noneOf "\r\n") (skipNewline <|> eof)

    let block_comment : P<string> =
        regex "\/\*([\s\S]*?)\*\/"

    let comment : P<string> =
        line_comment
        <|> block_comment
        |>> (fun s -> s.Trim())
        
    let (=>) (key: string) value =
        pstring key >>% value
        
    let (=!>)(key: string) (value: 'T) =
        pOp(key, value)
            
    let value_or_quoted_name (p: P<'T>) : P<IdOr<'T>> =
        
        let value =
            p |>> IdOr.Val
        
        let name =
            simple_id
            |> between('`', '`')
            |> attempt
            |>> IdOr.Id
            
        name <|> value
    
      
    let name_or_quoted_value (p: P<'T>) : P<IdOr<'T>> =
        
        let name =
            id |>> IdOr.Id
        
        let value =
            p
            |> between(''', ''')
            |> attempt
            |>> IdOr.Val
        
        value <|> name
        
    // <int-literal>
    let int_literal =
        many1Satisfy Char.IsDigit
        |>> int
    
    // <bool-literal>    
    let bool_literal : P<bool> =
        lookup(
            "true" => true,
            "false" => false
            )
        
    // <float-literal>        
    let float_literal : P<float> =
        regex "[0-9]+\.[0-9]+"
        |>> float
        
    // <string-literal>
    let string_literal : P<string> =
        manySatisfy (fun c -> c <> '"')
        |> between('"', '"')
    
    let builtin_num_un_ops =
        [ "+" =!> NumericBinaryOp.Add
        ; "-" =!> NumericBinaryOp.Subtract ]
        
    // <builtin-num-un-op>
    let builtin_num_un_op =
        builtin_num_un_ops
        |> choice
        |>> enum<NumericUnaryOp>
    
    let builtin_num_bin_ops =
         [ "+" =!> BinaryOp.Add
         ; "-" =!> BinaryOp.Subtract 
         ; "*" =!> BinaryOp.Multiply
         ; "/" =!> BinaryOp.Divide         
         ; "^" =!> BinaryOp.Exponent
         ; "~+" =!> BinaryOp.TildeAdd
         ; "~-" =!> BinaryOp.TildeSubtract
         ; "~*" =!> BinaryOp.TildeMultiply
         ; "~/" =!> BinaryOp.TildeDivide
         ; "div" =!> BinaryOp.Div
         ; "mod" =!> BinaryOp.Mod
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
    let builtin_bin_op : P<BinaryOp> =
        builtin_bin_ops
        |> choice
        |>> enum<BinaryOp>
        
    let builtin_un_ops =
        builtin_num_un_ops
        @ ["not" =!> UnaryOp.Not]

    // <builtin-un-op>           
    let builtin_un_op : P<UnaryOp> =
        builtin_un_ops
        |> choice
        |>> enum<UnaryOp>

    // <ti-expr>
    let ti_expr, ti_expr_ref =
        createParserForwardedToRef<TypeInst, UserState>()

    // <ti-expr>
    let base_ti_expr_tail, base_ti_expr_tail_ref =
        createParserForwardedToRef<Type, UserState>()
        
    // <expr>
    let expr, expr_ref =
        createParserForwardedToRef<Expr, UserState>()
    
    // <expr-atom>        
    let expr_atom, expr_atom_ref =
        createParserForwardedToRef<Expr, UserState>()

    // <num-expr>    
    let num_expr, num_expr_ref =
        createParserForwardedToRef<NumericExpr, UserState>()
        
    // <num-expr-atom>
    let num_expr_atom, num_expr_atom_ref =
        createParserForwardedToRef<NumericExpr, UserState>()
        
    // <num-expr-atom>
    let annotations, annotations_ref =
        createParserForwardedToRef<Annotations, UserState>()
                
    let bracketed x =
        between('(', ')' , ws=true) x

    let op p =
        value_or_quoted_name p
        
    let un_op =
        op builtin_un_op
        .>> spaces
        .>>. expr_atom

    let builtin_ops =
        builtin_bin_ops @ builtin_un_ops

    // <builtin-op>            
    let builtin_op : P<Op> =
        builtin_ops
        |> choice
        |>> enum<Op>
        
    // 0 .. 10
    let range_expr : P<Range> =
        attempt (
            num_expr
            .>> sps ".."
            .>>. num_expr
        )
    
    // <array1d-literal>
    let array1d_literal =
        expr
        |> between(p '[', p ']', p ',', allowTrailing=true)
        |>> Array1dExpr.Array1d
        <?!> "array1d-literal"
            
    // <set-literal>
    let set_literal : P<SetLiteral>=
        between(p '{', p '}', p ',') expr
        |> attempt
        |>> SetLiteral.SetLiteral
        
    // <set-expr>
    let set_expr : P<SetLiteral>=
        set_literal
                
    // <array2d-literal>
    let array2d_literal =
        
        let row =
            expr
            |> sepBy1(p ',', allowTrailing=true)
            
        let rows =
            let sep =
                attempt (p '|' >>. notFollowedBy (p ']'))
            row
            |> sepBy(sep, allowTrailing=true)
        
        let array =
            rows
            |> between(p "[|", p "|]")
                
        array
        |>> Array2dExpr.Array2d
   
    // <ti-expr-and-id>
    let ti_expr_and_id : P<Id * TypeInst> =
        ti_expr
        .>> sps ':'
        .>>. id
        |>> (fun (expr, name) -> (name, expr))
    
    let parameters : P<Parameters> =
        ti_expr_and_id
        |> between(p '(', p ')', p ',')
    
    // <operation-item-tail>
    // eg: even(var int: x) = x mod 2 = 0;
    let operation_item_tail =
        tuple4
            (ps id)
            (ps parameters)
            (ps annotations)
            (opt (ps "=" >>. expr))
        
    // <predicate-item>
    let predicate_item : P<FunctionItem> =
        kw1 "predicate"
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
    let test_item : P<TestItem> =
        kw1 "test"
        >>. operation_item_tail
        |>> (fun (id, pars, anns, body) ->
            { Name = id
            ; Parameters = pars
            ; Annotations = anns
            ; Body = body }
            )
        
    // <function-item>
    let function_item : P<FunctionItem> =
        kw1 "function"
        >>. ti_expr
        .>> sps ':'
        .>>. operation_item_tail
        |>> (fun (ti, (id, pars, anns, body)) ->
            { Name = id
            ; Returns = ti
            ; Annotations = anns 
            ; Parameters = pars
            ; Body = body })
    
    // <enum-case>
    // TODO: complex variants
    let enum_case : P<string> =
        id
          
    // <enum-item>
    // TODO: complex constructors
    let enum_item : P<EnumItem> =
        let members =
            enum_case
            |> between(p '{', p '}', p ',')
            
        pipe3
            (kw1 "enum" >>. id .>> sps '=')
            (ps annotations)
            (opt_or [] members)
            (fun name anns cases ->
                { Name = name
                ; Annotations = anns
                ; Cases = List.map EnumCase.Name cases
                })
    
    // <include-item>
    let include_item : P<IncludeItem> =
        kw1 "include"
        >>. string_literal
        |>> IncludeItem.Include
    
    // <var-par>
    let var_par : P<Inst> =
        lookup(
            "var" => Inst.Var,
            "par" => Inst.Par
        )
        .>> spaces1
        |> opt_or Inst.Par
        
    // <opt-ti>        
    let opt_ti =
        ps1 "opt"
        >>% true
        |> opt_or false
    
    // <set-ti>    
    let set_ti =
        ps1 "set" >>. ps1 "of"
        >>% true
        |> opt_or false
   
    // <base-ti-expr>
    let base_ti_expr : P<TypeInst> =
        pipe4
            var_par
            set_ti
            opt_ti
            base_ti_expr_tail
            (fun inst set opt typ ->
                { Type = typ
                ; IsOptional = opt
                ; IsSet = set
                ; IsArray = false 
                ; Inst = inst }
            )    
        
    // <array-ti-expr>        
    let array_ti_expr : P<TypeInst> =

        let dimensions =
            ti_expr
            |> between(p '[', p ']', p ',')
            <?!> "array-dimensions"
        
        ps  "array"
        >>.  dimensions
        .>>  sps1 "of"
        .>>. base_ti_expr
        |>> (fun (dims, ty) ->
            
            let ty, inst =
                (dims, ty)
                |> ArrayType.ArrayType
                |> Type.Array
                |> P.ResolveInst
                
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
    let tuple_ti : P<TupleType> =
        kw "tuple"
        >>. between1(p '(', p ')', p ',') ti_expr
        |>> TupleType.TupleType
        <?!> "tuple-ti"
            
    // <record-ti-expr-tail>
    let record_ti =
        kw "record"
        >>. between1(p '(', p ')', p ',') ti_expr_and_id
        |>> RecordType.RecordType
        <?!> "record-ti"
            
    // <base-ti-expr-tail>
    base_ti_expr_tail_ref.contents <-
        [ kw "bool"   >>% Type.Bool
        ; kw "int"    >>% Type.Int
        ; kw "string" >>% Type.String
        ; kw "float"  >>% Type.Float
        ; record_ti   |>> Type.Record
        ; tuple_ti    |>> Type.Tuple
        ; range_expr  |>> Type.Range 
        ; id          |>> Type.Id
        ; set_literal |>> Type.Literal ]
        |> choice
        <?!> "base-ti-tail"
    
    let id_or_op =
        name_or_quoted_value builtin_op
    
    // <call-expr>
    let call_expr : P<CallExpr> =
        
        let operation =
            ps id_or_op
            
        let args =
            between1(p '(', p ')', p ',') expr
            <?!> "call-args"
        
        pipe2
            operation
            args
            (fun name args ->
                { Function=name; Args=args })
        |> attempt
        <?!> "call-expr"
        
    let wildcard : P<WildCard> =
        p '_'
        >>. notFollowedBy letter
        >>% WildCard.WildCard
        
        
    // <comp-tail>
    let comp_tail : P<Generator list> =
        let var =
            (wildcard |>> IdOr.Val)
            <|>
            (id |>> IdOr.Id)
            <?!> "gen-var"
        let vars =
            var
            |> sepBy(p ",", many=true)
        let where =
            kw1 "where" >>. expr
        let generator =    
            pipe3
                (vars .>> sps "in")
                (ps expr)
                (opt where)
                (fun idents source filter ->
                    { Yields = idents
                    ; From = source
                    ; Where = filter })
            <?!> "generator"
            
        generator
        |> sepBy(p ",", many=true)       
            
    // <gen-call-expr>
    let gen_call_expr =
        pipe3
            (id_or_op .>> spaces)
            (ps (between('(', ')') comp_tail))             
            (ps (between('(', ')') expr))
            (fun name gens expr ->
                { Operation = name
                ; From = gens
                ; Yields = expr })
        |> attempt
        <?!> "gen-call"
    
    // <array-comp>
    let array_comp : P<ArrayCompExpr> =
        (expr .>> sps '|' .>>. comp_tail)
        |> between('[', ']')
        |> attempt
        |>> (fun (expr, gens) -> 
             { Yields=expr
             ; From = gens })
        <?!> "array-comp"

    // <set-comp>
    let set_comp : P<SetCompExpr> =
        (expr .>> sps '|' .>>. comp_tail)
        |> between('{', '}')
        |> attempt
        |>> (fun (expr, gens) -> 
             { Yields=expr
             ; From = gens })
        <?!> "set-comp"
            
    // <declare-item>
    let var_decl_item : P<DeclareItem> =
        pipe3
            (ps ti_expr_and_id)
            (ps annotations)
            (opt (ps '=' >>. expr))
            (fun (id, ti) anns expr ->
                let ti, inst = P.ResolveInst ti
                { Name = id
                ; Type = ti
                ; Annotations = anns
                ; Expr = expr })

    // <constraint-item>
    let constraint_item : P<ConstraintItem> =
        kw "constraint"
        >>. expr
        |>> ConstraintItem.Constraint
        
    // <annotation-item>
    let annotation_item =
        kw1 "annotation"
        >>. call_expr
        
    // <let-item>
    let let_item : P<LetLocal> =
        (var_decl_item |>> Choice1Of2)
        <|>
        (constraint_item |>> Choice2Of2)
    
    // <let-expr>
    let let_expr : P<LetExpr> =        
        kw "let"
        >>. between(p '{', p '}', anyOf ":,") let_item
        .>> sps "in"
        .>>. expr
        |>> (fun (items, body) ->
            let declares, constraints =
                items
                |> List.fold (fun (ds, cs) item ->
                    match item with
                    | Choice1Of2 x -> (x::ds, cs)
                    | Choice2Of2 x -> (ds, x::cs)
                ) ([], [])
           
            { Declares = declares
            ; Constraints = constraints
            ; Body=body })
        <?!> "let-expr"
        
    // <if-then-else-expr>
    let if_else_expr : P<IfThenElseExpr> =
        
        let if_case = 
            kw "if"
            >>. expr
            .>> spaces
            <?!> "if-case"
            
        let then_case =
            kw "then"
            >>. expr
            .>> spaces
            
        let elseif_case =
            kw1 "elseif"
            >>. (ps expr)
            .>> (kw1 "then")
            .>>. (ps expr)
            
        let else_case =
            kw "else"
            >>. (ps expr)
            .>> p "endif"
                        
        pipe4
            if_case
            then_case
            (many elseif_case)
            else_case
            (fun if_ then_ elseif_ else_ ->
                { If = if_
                ; Then = then_
                ; ElseIf = elseif_
                ; Else = else_ })    
    
    // <num-un-op>    
    let num_un_op =
        op builtin_num_un_op
        .>> spaces
        .>>. num_expr_atom
        
    let quoted_op =
        builtin_op
        |> between(''', ''')
        |> attempt
    
    // <array-acces-tail>
    let array_access : P<ArrayAccess> =
        expr
        |> between(p '[', p ']', p ',', many=true)
        |>> ArrayAccess.Access
        <?!> "array-access"
        
    // <expr-atom-tail>        
    let expr_atom_tail =
        many (array_access .>> spaces)

    // <num-expr-atom-head>    
    let num_expr_atom_head=
        [ float_literal      |>> NumericExpr.Float
          int_literal        |>> NumericExpr.Int
          bracketed num_expr |>> NumericExpr.Bracketed
          let_expr           |>> NumericExpr.Let
          if_else_expr       |>> NumericExpr.IfThenElse
          call_expr          |>> NumericExpr.Call
          num_un_op          |>> NumericExpr.UnaryOp
          quoted_op          |>> NumericExpr.Op 
          id                 |>> NumericExpr.Id
          ]
        |> choice
        
    // <num-expr-atom>        
    num_expr_atom_ref.contents <-
        pipe2
            num_expr_atom_head
            (sp expr_atom_tail)
            (fun head access ->
                match access with
                | [] ->
                    head
                | _ ->
                    NumericExpr.ArrayAccess (head, access)
                
            )
        <?!> "num-expr-atom"
        
    // <num-expr-binop-tail>
    let num_expr_binop_tail =        
        sps (op builtin_num_bin_op)
        .>>. num_expr

    // <num-expr>
    num_expr_ref.contents <-
        pipe2
            num_expr_atom
            (opt <| attempt num_expr_binop_tail)
            (fun head tail ->
                match tail with
                | None ->
                    head
                | Some (op, right) ->
                    NumericExpr.BinaryOp (head, op, right)
            )
        
    // <expr-atom-head>    
    let expr_atom_head=
        [ float_literal   |>> Expr.Float
          int_literal     |>> Expr.Int
          bool_literal    |>> Expr.Bool
          string_literal  |>> Expr.String
          wildcard        |>> Expr.WildCard
          bracketed expr  |>> Expr.Bracketed
          let_expr        |>> Expr.Let
          if_else_expr    |>> Expr.IfThenElse
          gen_call_expr   |>> Expr.GenCall
          call_expr       |>> Expr.Call
          array_comp      |>> Expr.ArrayComp
          set_comp        |>> Expr.SetComp
          array2d_literal |>> Expr.Array2d
          array1d_literal |>> Expr.Array1d
          set_expr        |>> Expr.Set
          un_op           |>> Expr.UnaryOp
          quoted_op       |>> Expr.Op
          id              |>> Expr.Id
          ]
        |> choice

    let expr_atom_impl =
        pipe2
            expr_atom_head
            expr_atom_tail
            (fun head tail ->
                match tail with
                | [] ->
                    head
                | access ->
                    (head, access)
                    |> IndexExpr.Index 
                    |> Expr.Indexed
            )
    
    // <annotation>
    let annotation : P<Annotation> =
        ps "::"
        >>. expr_atom_impl
            
    // <annotations>
    annotations_ref.contents <-
        many annotation
    
    // <expr-atom>        
    expr_atom_ref.contents <-
        expr_atom_impl
            
    // <expr-binop-tail>
    let expr_binop_tail =
        sps (op builtin_bin_op) .>>. expr
            
    // <expr>
    expr_ref.contents <-
        pipe2
            expr_atom
            (opt <| attempt expr_binop_tail)
            (fun head tail ->
                match tail with
                | None ->
                    head
                | Some (op, right) ->
                    Expr.BinaryOp (head, op, right)
            )
            
    let solve_type : P<SolveType> =
        lookup(
          "satisfy" => SolveType.Satisfy,
          "minimize" => SolveType.Minimize,
          "maximize" => SolveType.Maximize
        )
        
    // <solve-item>
    let solve_item : P<SolveMethod> =
        pipe3
            (kw1 "solve" >>. annotations)
            (sps solve_type)
            (opt expr)
            (fun anns solveType obj ->
                match (solveType, obj) with
                | SolveType.Maximize, Some exp ->
                    SolveMethod.Max (exp, anns)
                | SolveType.Minimize, Some exp ->
                    SolveMethod.Min (exp, anns)
                | _ ->
                    SolveMethod.Sat anns
                )            
        
    // <assign-item>
    let assign_item : P<AssignItem> =
        tuple2
            (attempt (id .>> sps '='))
            expr
        
    // <type-inst-syn-item>
    let alias_item : P<SynonymItem> =
        pipe3
            (kw1 "type" >>. id .>> spaces)
            (ps annotations .>> ps "=")
            ti_expr
            (fun id anns ti ->
                { Name = id
                ; Annotations = anns
                ; TypeInst = ti })
        
    // <output-item>
    let output_item : P<OutputItem> =
        kw1 "output"
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
        ; predicate_item  |>> Item.Predicate
        ; function_item   |>> Item.Function
        ; test_item       |>> Item.Test
        ; annotation_item |>> Item.Annotation        
        ; assign_item     |>> Item.Assign
        ; var_decl_item   |>> Item.Declare
        ; block_comment   |>> Item.Comment ]
        |> choice
                      
    let model : P<Ast> =
        spaces
        >>. sepEndBy1 item (sps ';')
        .>> eof
    
                
module Parse =

    open System.Text.RegularExpressions
    
    /// <summary>
    /// Strip comments from the given minizinc model
    /// </summary>
    let stripComments (mzn: string) : string * List<Comment> =
        
        let comments = ResizeArray<string>()
        let line_comment = "%(.*)$"
        let block_comment = "\/\*([\s\S]*?)\*\/"
        let pattern = $"{line_comment}|{block_comment}"
        let evaluator =
            MatchEvaluator(fun m ->
                
                let comment =
                    match m.Groups[1], m.Groups[2] with
                    | m, _ when m.Success ->
                        m.Value
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
        
    // Parse the given string with the given parser
    let string (parser: Parsers.P<'t>) (input: string) : Result<'t, ParseError> =
        
        let state = Parsers.UserState()
        
        match runParserOnString parser state "" input with
        
        | Success (value, _state, _pos) ->
            Result.Ok value
            
        | Failure (msg, err, state) ->
            
            let err =
                { Message = msg
                ; Line = err.Position.Line
                ; Column = err.Position.Column
                ; Index = err.Position.Index
                ; Trace = state.Message }
                
            Result.Error err
            
    // Parse the given string with the given parser
    let model (input: string) : Result<Ast, ParseError> =
        let result = string Parsers.model input
        result                
            
    // Parse the given file with the given encoding
    let file (encoding: Encoding) (path: string) : Result<Ast, ParseError> =
                
        let state  = Parsers.UserState()
        
        match runParserOnFile Parsers.model state path encoding with
        
        | Success (value, _state, _pos) ->
            Result.Ok value
            
        | Failure (msg, err, state) ->
            
            let err =
                { Message = msg
                ; Line = err.Position.Line
                ; Column = err.Position.Column
                ; Index = err.Position.Index
                ; Trace = state.Message }
                
            Result.Error err