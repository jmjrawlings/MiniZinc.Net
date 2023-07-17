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
type private ParseUtils () =
    
    // parse 
    static member p (x: char): Parser<char> =
        pchar x
        
    // parse 
    static member p (x: string) : Parser<string> =
        pstring x
    
    // parse spaces
    static member ps (x: Parser<'t>) : Parser<'t> =
        x .>> spaces
    
    // parse spaces    
    static member ps (x: string) : Parser<string> =
        pstring x .>> spaces
        
    // parse spaces
    static member ps (x: char) : Parser<char> =
        pchar x .>> spaces
    
    // parse spaces1
    static member ps1 (x: Parser<'t>) : Parser<'t> =
        x .>> spaces1
    
    // parse spaces1    
    static member ps1 (x: string) : Parser<string> =
        pstring x .>> spaces1

    // parse spaces1                        
    static member ps1 (x: char) : Parser<char> =
        pchar x .>> spaces1            
    
    // space parse space    
    static member sps (x: Parser<'t>) : Parser<'t> =
        between spaces spaces x
    
    // space parse space
    static member sps (x: string) : Parser<string> =
        ParseUtils.sps (pstring x)
        
    // space parse space            
    static member sps (c: char) : Parser<char> =
        ParseUtils.sps (pchar c)
        
    // space1 parse space1
    static member sps1 (x: Parser<'t>) : Parser<'t> =
        between spaces1 spaces1 x

    // space1 parse space1        
    static member sps1 (x: string) : Parser<string> =
        ParseUtils.sps1 (pstring x)
        
    // space1 parse space1
    static member sps1 (c: char) : Parser<char> =
        ParseUtils.sps1 (pchar c)
    
    // space parse     
    static member sp (x: Parser<'t>) : Parser<'t> =
        spaces >>. x

    // space parse     
    static member sp (c: char) =
        ParseUtils.sp (pchar c)

    // space parse     
    static member sp (s: string) =
        ParseUtils.sp (pstring s)
        
    // space1 parse
    static member sp1 (x: Parser<'t>) : Parser<'t> =
        spaces1 >>. x
        
    // space1 parse
    static member sp1 (c: char) =
        ParseUtils.sp1 (pchar c)
        
    // space1 parse            
    static member sp1 (s: string) =
        ParseUtils.sp1 (pstring s)

    // Parse between 'start' and 'end' with optional whitespace 
    static member between (
            pStart : Parser<_>,
            pEnd : Parser<_>,
            [<Optional; DefaultParameterValue(true)>] ws : bool
        ) =
            let pStart', pEnd' =
                match ws with
                | true -> (pStart .>> spaces), (spaces >>. pEnd)
                | false -> pStart, pEnd
            between pStart' pEnd'
            
    static member between(a: string, b: string, [<Optional; DefaultParameterValue(true)>] ws : bool) =
        ParseUtils.between(pstring a, pstring b, ws=ws)
        
    static member between(a: char, b: char, [<Optional; DefaultParameterValue(true)>] ws : bool) =
        ParseUtils.between(pchar a, pchar b, ws=ws)

    // Parse 0 or more 'p' between 'start' and 'end' with optional whitespace            
    static member between (
            pStart : Parser<_>,
            pEnd : Parser<_>,
            pDelim : Parser<_>,
            [<Optional; DefaultParameterValue(false)>] many : bool,
            [<Optional; DefaultParameterValue(false)>] allowTrailing : bool
        ) =
            ParseUtils.sepBy(pDelim, many=many, allowTrailing=allowTrailing)
            >> ParseUtils.between(pStart, pEnd, ws=true)
        
    // Parse 'p' separated by 'delim'.  Whitespace is consumed  
    static member sepBy (
            pDelim : Parser<_>,
            [<Optional; DefaultParameterValue(false)>] many : bool,
            [<Optional; DefaultParameterValue(false)>] allowTrailing : bool
        ) : Parser<'t> -> Parser<'t list> =
        
        fun p ->
            
            let p' =
                p .>> spaces
                
            let pDelim' =
                pDelim .>> spaces
                
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
        | Type.Id _ 
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


    
module Parsers =
    
    [<Struct>]
    type ParseDebugEvent<'a> =
        | Enter
        | Leave of Reply<'a>
        
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
       
    let simple_id : Parser<Id> = 
        regex "_?[A-Za-z][A-Za-z0-9_]*"

    let quoted_id : Parser<Id> =
        regex "'[^'\x0A\x0D\x00]+'"
        
    // <ident>
    let id : Parser<Id> =
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
    
    let line_comment : Parser<string> =
        p '%' >>.
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
            
    let value_or_quoted_name (p: Parser<'T>) : Parser<IdOr<'T>> =
        
        let value =
            p |>> IdOr.Val
        
        let name =
            simple_id
            |> between('`', '`')
            |> attempt
            |>> IdOr.Id
            
        name <|> value
    
      
    let name_or_quoted_value (p: Parser<'T>) : Parser<IdOr<'T>> =
        
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
    let bool_literal : Parser<bool> =
        lookup(
            "true" => true,
            "false" => false
            )
        
    // <float-literal>        
    let float_literal : Parser<float> =
        regex "[0-9]+\.[0-9]+"
        |>> float
        
    // <string-literal>
    let string_literal : Parser<string> =
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
    let expr, expr_ref =
        createParserForwardedToRef<Expr, ParserState>()
    
    // <expr-atom>        
    let expr_atom, expr_atom_ref =
        createParserForwardedToRef<Expr, ParserState>()

    // <num-expr>    
    let num_expr, num_expr_ref =
        createParserForwardedToRef<NumExpr, ParserState>()
        
    // <num-expr-atom>
    let num_expr_atom, num_expr_atom_ref =
        createParserForwardedToRef<NumExpr, ParserState>()
        
    // <num-expr-atom>
    let annotation, annotation_ref =
        createParserForwardedToRef<Annotation, ParserState>()
                
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
    let builtin_op : Parser<Op> =
        builtin_ops
        |> choice
        |>> enum<Op>
        
    // 0 .. 10
    let range_expr : Parser<RangeExpr> =
        attempt (
            num_expr
            .>> sps ".."
            .>>. num_expr
        )
        <?!> "range-expr"
    
    // <array1d-literal>
    let array1d_literal : Parser<Array1dExpr> =
        expr
        |> between(p '[', p ']', p ',', allowTrailing=true)
        <?!> "array1d-literal"
            
    // <set-literal>
    let set_literal : Parser<SetLiteral>=
        between(p '{', p '}', p ',') expr
        |> attempt
        |>> (fun exprs -> {Elements = exprs})
                
    // <array2d-literal>
    let array2d_literal : Parser<Array2dExpr> =
        
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
        
    // <annotations>        
    let annotations : Parser<Annotations> =
        many (annotation .>> spaces)        
   
    // <ti-expr-and-id>
    let ti_expr_and_id : Parser<NamedTypeInst> =
        pipe3
            (ti_expr .>> sps ':')
            (id .>> spaces)
            annotations
            (fun ti name anns ->
                { TypeInst = ti
                ; Name=name
                ; Annotations=anns })
        
    let parameters : Parser<Parameters> =
        ti_expr_and_id
        |> between(p '(', p ')', p ',')
        
    let arguments : Parser<Expr list> =
        expr
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
    let predicate_item : Parser<FunctionItem> =
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
    let test_item : Parser<TestItem> =
        kw1 "test"
        >>. operation_item_tail
        |>> (fun (id, pars, anns, body) ->
            { Name = id
            ; Parameters = pars
            ; Annotations = anns
            ; Body = body }
            )
        
    // <function-item>
    let function_item : Parser<FunctionItem> =
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
    
    // <enum-case-list>    
    let enum_cases : Parser<EnumCases list> =
                
        let names =
            id
            |> between(p '{', p '}', p ',')
            |>> EnumCases.Names
    
        let anon =
            choice [ p "_"; p "anon_enum"]
            >>. spaces
            >>. between('(', ')') expr
            |>> EnumCases.Anon
            
        let call =
            (ps id)
            .>>. between('(', ')') expr
            |>> EnumCases.Call
            
        [ names; anon; call ]
        |> choice
        |> sepBy(p "++")
          
    // <enum-item>
    let enum_item : Parser<EnumItem> =
            
        let enum_name =
            kw1 "enum" >>. id .>> spaces
            
        pipe3
            enum_name
            annotations
            (opt_or [] (ps '=' >>. enum_cases))
            (fun name anns cases ->
                { Name = name
                ; Annotations = anns
                ; Cases = cases })
    
    // <include-item>
    let include_item : Parser<IncludeItem> =
        kw1 "include"
        >>. string_literal
        |>> IncludeItem.Create
    
    // <var-par>
    let var_par : Parser<Inst> =
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
    let base_ti_expr : Parser<TypeInst> =
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
    let array_ti_expr : Parser<TypeInst> =

        let dimension =
            ti_expr
            >>= fun ti ->
                match ti.Type with
                | Type.Id id ->
                    preturn (ArrayDim.Id id)
                | Type.Int ->
                    preturn ArrayDim.Int
                | Type.Set x ->
                    preturn (ArrayDim.Set x)
                | other ->
                    fail $"Bad array dimension {other}"
        
        let dimensions =
            dimension
            |> between(p '[', p ']', p ',')
            <?!> "array-dimensions"
        
        ps  "array"
        >>.  dimensions
        .>>  sps1 "of"
        .>>. base_ti_expr
        |>> (fun (dims, ty) ->
            
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
        kw "tuple"
        >>. between(p '(', p ')', p ',', many=true) ti_expr
        |>> (fun fields -> {Fields=fields})
        <?!> "tuple-ti"
            
    // <record-ti-expr-tail>
    let record_ti : Parser<RecordType> =
        kw "record"
        >>. between(p '(', p ')', p ',', many=true) ti_expr_and_id
        |>> (fun fields -> {Fields=fields})
        <?!> "record-ti"
            
    // <base-ti-expr-tail>
    base_ti_expr_tail_ref.contents <-
        [ kw "bool"   >>% Type.Bool
        ; kw "int"    >>% Type.Int
        ; kw "string" >>% Type.String
        ; kw "float"  >>% Type.Float
        ; kw "ann"    >>% Type.Ann
        ; record_ti   |>> Type.Record
        ; tuple_ti    |>> Type.Tuple
        ; expr        |>> Type.Set 
        ; id          |>> Type.Id ]
        |> choice
        <?!> "base-ti-tail"
    
    let id_or_op =
        name_or_quoted_value builtin_op
    
    // <call-expr>
    let call_expr : Parser<CallExpr> =
        
        let operation =
            ps id_or_op
            
        let args =
            between(p '(', p ')', p ',') expr
            <?!> "call-args"
        
        pipe2
            operation
            args
            (fun name args ->
                { Function=name; Args=args })
        |> attempt
        <?!> "call-expr"
        
    let wildcard : Parser<WildCard> =
        p '_'
        >>. notFollowedBy letter
        >>% WildCard.WildCard
    
    let absent : Parser<Absent> =
        attempt (p "<>")
        >>% Absent
        
    // <comp-tail>
    let comp_tail : Parser<Generator list> =
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
    let array_comp : Parser<ArrayCompExpr> =
        (expr .>> sps '|' .>>. comp_tail)
        |> between('[', ']')
        |> attempt
        |>> (fun (expr, gens) -> 
             { Yields=expr
             ; From = gens })
        <?!> "array-comp"

    // <set-comp>
    let set_comp : Parser<SetCompExpr> =
        (expr .>> sps '|' .>>. comp_tail)
        |> between('{', '}')
        |> attempt
        |>> (fun (expr, gens) -> 
             { Yields=expr
             ; From = gens })
        <?!> "set-comp"
            
    // <declare-item>
    let var_decl_item : Parser<DeclareItem> =
        pipe2
            (ps ti_expr_and_id)
            (opt (ps '=' >>. expr))
            (fun nti expr ->
                let ti, inst = ParseUtils.ResolveInst nti.TypeInst
                { Name = nti.Name
                ; TypeInst = ti
                ; Annotations = nti.Annotations
                ; Expr = expr })

    // <constraint-item>
    let constraint_item : Parser<ConstraintItem> =
        pipe3
          (kw "constraint")
          (ps expr)
          annotations
          (fun _ expr anns ->
              { Expr = expr
              ; Annotations = anns })
            
    // <annotation-item>
    let annotation_item : Parser<AnnotationItem>=
        pipe3
          (kw1 "annotation")
          (ps id)
          (opt parameters)
          (fun _ id ->
            function
            | None -> AnnotationItem.Name id
            | Some xs -> AnnotationItem.Call (id, xs))
          
        
    // <let-item>
    let let_item : Parser<LetLocal> =
        choice
         [constraint_item |>> Choice2Of2
          var_decl_item   |>> Choice1Of2 ]
    
    // <let-expr>
    let let_expr : Parser<LetExpr> =
        
        let item =
            let_item
            .>> spaces
            .>> (opt (anyOf ";,"))
            .>> spaces
    
        let items =
            item
            |> many1
            |> between('{', '}')
        
        (kw "let")
        >>. items
        .>> (sps "in")
        .>>. expr
        |>> (fun (items, body) ->
            
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
            ; Body=body })
        <?!> "let-expr"
        
    // <if-then-else-expr>
    let if_else_expr : Parser<IfThenElseExpr> =
                
        let case keyword =
            kw keyword
            >>. expr
            .>> spaces
            <?!> $"{keyword}-case"
            
        pipe5
            (case "if")
            (case "then")
            (many (case "elseif" .>>. case "then"))
            (opt (case "else"))
            (p "endif")
            (fun if_ then_ elseif_ else_ _ ->
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
    let array_access : Parser<ArrayAccess> =
        expr
        |> between(p '[', p ']', p ',', many=true)
        |>> ArrayAccess.Access
        <?!> "array-access"
        
    // <expr-atom-tail>        
    let expr_atom_tail =
        many (array_access .>> spaces)

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
          id                 |>> NumExpr.Id
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
                    NumExpr.ArrayAccess (head, access)
                
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
                    NumExpr.BinaryOp (head, op, right)
            )
            
    // <tuple-literal>
    // TODO - Required trailing comma if tuple is 1 element
    let tuple_literal : Parser<TupleExpr> =
        expr
        |> between(p '(', p ')', p ',', many=true, allowTrailing=true)
        |> attempt
        <?!> "tuple-lit"
          
    // <record-literal>
    let record_literal : Parser<RecordExpr> =
        (id  .>> sps ':' .>>. expr)
        |> between(p '(', p ')', p ',', many=true, allowTrailing=true)
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
          id              |>> Expr.Id
          ]
        |> choice
            
    // <annotations>
    annotation_ref.contents <-
        pipe2
            ( p "::" >>. spaces >>. id .>> spaces)
            (opt_or [] arguments)
            (fun name args ->
                { Name = name; Args = args })
    
    // <expr-atom>        
    expr_atom_ref.contents <-
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
            
    let solve_type : Parser<SolveMethod> =
        lookup(
          "satisfy" => SolveMethod.Satisfy,
          "minimize" => SolveMethod.Minimize,
          "maximize" => SolveMethod.Maximize
        )
        
    // <solve-item>
    let solve_item : Parser<SolveItem> =
        pipe4
            (kw1 "solve")
            (ps annotations)
            (ps solve_type)
            (opt expr)
            (fun _ anns solveType obj ->
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
        tuple2
            (attempt (id .>> sps '='))
            expr
        
    // <type-inst-syn-item>
    let alias_item : Parser<TypeAlias> =
        pipe5
            (kw1 "type")
            (ps id)
            (ps annotations)
            (ps "=")
            ti_expr
            (fun _ id anns _ ti ->
                { Name = id
                ; Annotations = anns
                ; TypeInst = ti })
        
    // <output-item>
    let output_item : Parser<OutputItem> =
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
        ; predicate_item  |>> Item.Function
        ; function_item   |>> Item.Function
        ; test_item       |>> Item.Test
        ; annotation_item |>> Item.Annotation        
        ; assign_item     |>> Item.Assign
        ; var_decl_item   |>> Item.Declare
        ; block_comment   |>> Item.Comment ]
        |> choice
                      
    let ast : Parser<Ast> =
        spaces
        >>. sepEndBy1 item (sps ';')
        .>> eof
        
    let data : Parser<AssignItem list> =
        spaces
        >>. sepEndBy1 assign_item (sps ';')
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