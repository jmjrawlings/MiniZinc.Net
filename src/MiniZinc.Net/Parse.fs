namespace MiniZinc

open System
open System.IO
open System.Runtime.InteropServices
open FParsec

module ParseUtils =
    type DebugInfo = { Message: string; Indent: int }
    type UserState = { mutable Debug: DebugInfo }
    type Error = { Message: string; Trace: string; }
    type P<'t> = Parser<'t, UserState>
    [<Struct>] type DebugEvent<'a> = Enter | Leave of Reply<'a>
    
    let todo<'t> () : P<'t> =
        preturn Unchecked.defaultof<'t>
    
    let addToDebug (stream:CharStream<UserState>) label event =
        let msgPadLen = 50
        let startIndent = stream.UserState.Debug.Indent
        let str, indent, nextIndent = 
            match event with
            | Enter ->
                $"Entering %s{label}", startIndent, startIndent+1
            | Leave res ->
                let str = $"Leaving  %s{label} (%A{res.Status})"
                let pad = max (msgPadLen - startIndent - 1) 0
                let resStr = $"%s{str.PadRight(pad)} {res.Result}"
                resStr, startIndent-1, startIndent-1

        let indentStr =
            let pad = max indent 0
            if indent = 0 then ""
            else "\u251C".PadRight(pad, '\u251C')

        let posStr = $"%A{stream.Position}: ".PadRight(20)
        let posIdentStr = posStr + indentStr

        // The %A for res.Result makes it go onto multiple lines - pad them out correctly
        let replaceStr = "\n" + "".PadRight(max posStr.Length 0) + "".PadRight(max indent 0, '\u2502').PadRight(max msgPadLen 0)
        let correctedStr = str.Replace("\n", replaceStr)
        let fullStr = $"%s{posIdentStr} %s{correctedStr}\n"

        stream.UserState.Debug <- {
            Message = stream.UserState.Debug.Message + fullStr
            Indent = nextIndent
        }

    let (<!>) (p: P<'t>) label : P<'t> =
        fun stream ->
            addToDebug stream label Enter
            let reply = p stream
            addToDebug stream label (Leave reply)
            reply

    #if DEBUG
    let (<?!>) (p: P<'t>) label : P<'t> =
        p <?> label <!> label
    #else
    let (<?!>) (p: P<'t>) label : P<'t> =
        p <?> label <!> label
    #endif        
        
    let chr x =
        pchar x
        
    let str x =
        pstring x
        
    let opt_or backup p =
        (opt p) |>> Option.defaultValue backup
        
    let (=>) key value =
        pstring key >>% value

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
        
        // Parse
        static member p (x: char): P<char> =
            pchar x
            
        static member p (x: string) : P<string> =
            pstring x
        
        static member ps x =
            x .>> spaces
            
        static member ps (x: string) : P<string> =
            pstring x .>> spaces
            
        static member ps (x: char) : P<char> =
            pchar x .>> spaces
        
        static member ps1 x =
            x .>> spaces1
            
        static member ps1 (x: string) : P<string> =
            pstring x .>> spaces1
            
        static member ps1 (x: char) : P<char> =
            pchar x .>> spaces1            
            
        static member sps x =
            between spaces spaces x
        
        static member sps (x: string) : P<string> =
            P.sps (pstring x)
            
        static member sps (c: char) : P<char> =
            P.sps (pchar c)
            
        static member sps1 x =
            between spaces1 spaces1 x
        
        static member sps1 (x: string) : P<string> =
            P.sps1 (pstring x)
            
        static member sps1 (c: char) : P<char> =
            P.sps1 (pchar c)
            
        static member sp x =
            spaces >>. x
            
        static member sp (c: char) =
            P.sp (pchar c)
            
        static member sp (s: string) =
            P.sp (pstring s)
            
        static member sp1 x =
            spaces1 >>. x
            
        static member sp1 (c: char) =
            P.sp1 (pchar c)
            
        static member sp1 (s: string) =
            P.sp1 (pstring s)

        static member between (a: P<'a>, b: P<'a>, [<Optional; DefaultParameterValue(false)>] ws : bool) =
            let left, right =
                match ws with
                | true -> (a .>> spaces), (spaces >>. b)
                | false -> a,b
            between left right
            
        static member between (a: char, b: char, [<Optional; DefaultParameterValue(false)>] ws : bool) =
            P.between(pchar a, pchar b, ws)

        static member between (a: string, b: string, [<Optional; DefaultParameterValue(false)>] ws : bool) =
            P.between (pstring a, pstring b, ws)
            
        static member betweens (a: char, b: char) =
            P.between(a, b, ws=true)

        static member betweens (a: string, b: string) =
            P.between (a, b, ws=true)
            
        static member between (a: P<'a>, b: P<'a>, c: P<'b>, [<Optional; DefaultParameterValue(false)>] ws : bool, [<Optional; DefaultParameterValue(false)>] many : bool) =
            fun p ->
                let delim =
                    match ws with
                    | true -> P.sps c
                    | false -> c
                let inner =
                    match many with
                    | false -> sepBy p delim
                    | true -> sepBy1 p delim
                P.between(a, b, ws) inner
                
        static member between (a: char, b: char, c: char, [<Optional; DefaultParameterValue(false)>] many : bool) =
            P.between(pchar a, pchar b, pchar c, ws=true, many=many)
            
        static member between (a:string, b:string, c:string, [<Optional; DefaultParameterValue(false)>] many : bool) =
            P.between(pstring a, pstring b, pstring c, ws=true, many=many)
            
        static member between1 (a:string, b:string, c:string) =
            P.between(a, b, c, many=true)
            
        static member between1 (a:char, b:char, c:char) =
            P.between(a, b, c, many=true)
           
        static member lookup([<ParamArray>] parsers: P<'t>[]) =
            choice parsers
    

open ParseUtils
open type ParseUtils.P

module AST =
    
    let [<Literal>] DOUBLE_QUOTE = '"'
    let [<Literal>] SINGLE_QUOTE = '''
    let [<Literal>] BACKTICK = '`'

    type BaseType =    
        | Int
        | Bool
        | String
        | Float
    
    type BinaryOp = string
        
    type UnaryOp = string
    
    type Op = string
                
    type SolveMethod =
        | Satisfy = 0
        | Minimize = 1
        | Maximize = 2
        
    type Annotation = string

    type Annotations = Annotation list        
   
    type Expr =
        | Wildcard
        | Int       of int
        | Float     of float
        | Bool      of bool
        | String    of string
        | Id        of string
        | Op        of string
        | Bracketed of Expr
        | Set       of SetExpr
        | SetComp  
        | Array1d   of Expr list
        | Array1dIndex
        | Array2d   of Expr list list
        | Array2dIndex
        | ArrayComp
        | ArrayCompIndex
        | Tuple         of TupleExpr
        | Record        of RecordExpr
        | UnaryOp       of (UnaryOp OrId) * Expr
        | BinaryOp      of Expr * (BinaryOp OrId) * Expr
        | Annotation
        | IfThenElse    of IfThenElseExpr
        | Let           of LetExpr
        | Call          of CallExpr
        | Indexed       of expr:Expr * index: ArrayAccess list

    and OrId<'T> = Choice<'T, string>
    
    and ArrayAccess = Expr list
    
    and CallExpr =
        { Name: Op OrId
        ; Args: Expr list }
    
    and SetExpr = Expr list
    
    and TupleExpr = Expr list
    
    and RecordExpr = Map<string, Expr>
        
    and SolveSatisfy =
        { Annotations : Annotations }
        
    and SolveOptimise =
        { Annotations : Annotations
          Method : SolveMethod
          Objective : Expr }
        
    and SolveItem =
        | Satisfy of SolveSatisfy
        | Optimise of SolveOptimise
        
        member this.Method =
            match this with
            | Satisfy _ -> SolveMethod.Satisfy
            | Optimise o -> o.Method
            
        member this.Annotations =
            match this with
            | Satisfy s -> s.Annotations
            | Optimise o -> o.Annotations

    and IfThenElseExpr =
        { If     : Expr
        ; Then   : Expr
        ; ElseIf : Expr list
        ; Else   : Expr}
   
    and NumExpr =
        | Int         of int
        | Float       of float
        | Id          of string
        | Op          of string
        | Bracketed   of NumExpr
        | Call        of CallExpr
        | IfThenElse  of IfThenElseExpr
        | Let         of LetExpr
        | UnaryOp     of (NumericUnaryOp OrId) * NumExpr
        | BinaryOp    of NumExpr * (NumericBinaryOp OrId) * NumExpr
        | ArrayAccess of NumExpr * ArrayAccess list
            
    and NumericUnaryOp = string
    and NumericBinaryOp = string
    
    and Enum =
        { Name : string
        ; Cases : EnumCase list }
        
    and EnumCase =
        | Name of string
        | Expr of Expr
    
    /// <summary>
    /// Instantiation of a Type
    /// </summary>
    /// <remarks>
    /// We have flattened out the `ti-expr` EBNF
    /// rule here that a single class that convers
    /// everything. 
    /// </remarks>
    and TypeInst =
        { Type       : Type
          Name       : string
          IsVar      : bool
          IsSet      : bool
          IsOptional : bool 
          Dimensions : Type list }
    
    and Type =
        | Int
        | Bool
        | String
        | Float
        | Id        of string
        | Variable  of string
        | Tuple     of TypeInst list
        | Record    of TypeInst list
        | Set       of Expr list
        | Range     of lower:NumExpr * upper:NumExpr
            
    and Item =
        | Include    of string
        | Enum       of Enum
        | Alias      of AliasItem
        | Constraint of ConstraintItem
        | Assign     of AssignItem
        | Declare    of DeclareItem
        | Solve      of SolveItem
        | Predicate  of PredicateItem
        | Function   of FunctionItem
        | Test       of TestItem
        | Output     of OutputItem
        | Annotation of Annotation
        | Other      of string

    and ConstraintItem = Expr
            
    and IncludeItem = string
            
    and PredicateItem = OperationItem
    
    and TestItem = OperationItem
    
    and AliasItem = TypeInst

    and OutputItem = Expr
    
    and OperationItem =
        { Name: string
          Parameters : TypeInst list
          Body: Expr option }
        
    and FunctionItem =
        { Name: string
          Returns : TypeInst
          Parameters : TypeInst list
          Body: Expr option }
    
    and Test = unit
    
    and AssignItem = string * Expr
    
    and DeclareItem = TypeInst * Expr option
    
    and LetItem =
        | Declare of DeclareItem
        | Constraint of ConstraintItem
        
    and LetExpr =
        { Items: LetItem list;  Body: Expr }



module Parsers =
    
    open AST
    
    let simple_ident : P<string> =
        regex "_?[A-Za-z][A-Za-z0-9_]*"

    let quoted_ident : P<string> =
        regex "'[^'\x0A\x0D\x00]+'"
        
    // <ident>
    let ident : P<string> =
        regex "_?[A-Za-z][A-Za-z0-9_]*|'[^'\x0A\x0D\x00]+'"
        <?!> "identifier"

    // Parse a keyword
    // eg: 'function'
    let kw (name: string) =
        p name
        .>> notFollowedBy letter
        >>. spaces
        
    // Parse a keyword
    let kw1 (name: string) =
        p name
        .>> notFollowedBy letter
        >>. spaces1
    
    let single_line_comment : P<string> =
        p '%' >>.
        manyCharsTill (noneOf "\r\n") (skipNewline <|> eof)

    let multi_line_comment : P<string> =
        attempt (p "/*")
        >>. manyCharsTill (noneOf "*") (p "*/")

    let comment : P<string> =
        single_line_comment
        <|> multi_line_comment
        |>> (fun s -> s.Trim())
            
    // Try to parse with the given operation parser.
    // if it succeeds then Builtin constructor
    // is used, otherwise Custom.  
    let private or_id (p: P<'T>) : P<'T OrId> =
        let builtin =
            p |>> Choice1Of2
        let custom =
            simple_ident
            |> between(BACKTICK, BACKTICK)
            |>> Choice2Of2
        builtin
        <|> custom
        
    // Try to parse with the given operation parser.
    // if it succeeds then Builtin constructor
    // is used, otherwise Custom.  
    let private id_or if_id if_other p =
        let id =
            ident
            |>> if_id
        let other =
            p
            |> between(SINGLE_QUOTE, SINGLE_QUOTE)
            |> attempt
            |>> if_other
        other <|> id
    
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
        |>> (fun s ->
            float s
            )
        
    // <string-literal>
    let string_literal : P<string> =
        manySatisfy (fun c -> c <> '"')
        |> between('"', '"')
    
    // <builtin-num-un-op>
    let builtin_num_un_ops =
        [ "+" ; "-" ]
        
    let builtin_num_un_op =
        builtin_num_un_ops
        |> List.map pstring
        |> choice
    
    // <builtin-num-bin-op>
    let builtin_num_bin_ops =
         [ "+"
         ; "-"
         ; "*"
         ; "/"
         ; "div"
         ; "mod"
         ; "^"
         ; "~+"
         ; "~-"
         ; "~*"
         ; "~/"
         ; "~div" ]
        
    let builtin_num_bin_op =
         builtin_num_bin_ops
         |> List.map pstring
         |> choice
            
    // <builtin-bin-op>            
    let builtin_bin_ops = 
        [ "<->"
        ; "->"
        ; "<-"
        ; "\/"
        ; "xor"
        ; "/\\"
        ; "<"
        ; ">"
        ; "<="
        ; ">="
        ; "=="
        ; "="
        ; "!="
        ; "~="
        ; "~!="
        ; "in"
        ; "subset"
        ; "superset"
        ; "union"
        ; "diff"
        ; "symdiff"
        ; ".."
        ; "intersect"
        ; "++"
        ; "default" ]
        
    let builtin_bin_op : P<string> =
        builtin_bin_ops
        |> List.map pstring
        |> choice
        
    let builtin_un_ops =
        builtin_num_un_ops @ ["not"]
        
    let builtin_un_op : P<string> =
        builtin_un_ops
        |> List.map pstring
        |> choice

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
        createParserForwardedToRef<NumExpr, UserState>()
        
    // <num-expr-atom>
    let num_expr_atom, num_expr_atom_ref =
        createParserForwardedToRef<NumExpr, UserState>()
            
    let bracketed x =
        betweens('(', ')') x

    let unary =        
        builtin_un_op
        |> or_id 
        
    let un_op =
        unary
        .>> spaces
        .>>. expr_atom
        <?!> "un-op"

    let bin_op =
        builtin_bin_op
        |> or_id
        
    let builtin_ops =
        builtin_bin_ops @ builtin_un_ops
            
    let builtin_op : P<string> =
        builtin_ops
        |> List.map str
        |> choice
        
    // 0 .. 10
    let range_expr =
        num_expr
        .>> sps ".."
        .>>. num_expr
        |> attempt
        <?!> "range-expr"
    
    // <array1d-literal>
    let array1d_literal =
        between('[', ']', ',') expr
        <?!> "array1d"
        
    // <array2d-literal>
    let array2d_literal =
        let row_item = expr
        let row_expr = sepBy row_item (sps ',')
        let row_delim = attempt ((p '|') >>. notFollowedBy (p ']'))
        
        row_expr
        |> between(p "[|", p "|]", row_delim, ws=true, many=false)
        <?!> "array2d"
   
    // <ti-expr-and-id>
    let ti_expr_and_id : P<TypeInst> =
        ti_expr
        .>> sps ':'
        .>>. ident
        |>> (fun (expr, name) ->
            { expr with Name = name })
        <?> "ti-expr-and-id"    
    
    let parameters : P<TypeInst list> =
        ti_expr_and_id
        |> between('(', ')', ',')
    
    // <operation-item-tail>
    // eg: even(var int: x) = x mod 2 = 0;
    let operation_item_tail : P<OperationItem> =
        pipe3
            (ps ident)
            (ps parameters)
            (opt (ps "=" >>. expr))
            (fun name pars body ->
                { Name = name
                ; Parameters = pars
                ; Body = body })
        
    // <predicate-item>
    let predicate_item : P<PredicateItem> =
        kw1 "predicate" >>. operation_item_tail

    // <test_item>
    let test_item : P<TestItem> =
        kw1 "test" >>. operation_item_tail
        
    // <function-item>
    let function_item : P<FunctionItem> =
        kw1 "function"
        >>. ti_expr
        .>> sps ':'
        .>>. operation_item_tail
        |>> (fun (ti, op) ->
            { Name = op.Name
            ; Returns = ti
            ; Parameters = op.Parameters
            ; Body = op.Body })
    
    // <enum-case>
    // TODO: complex variants
    let enum_case : P<string> =
        ident
          
    // <enum-item>
    // TODO: complex constructors
    let enum_item : P<Enum> =
        let members =
            enum_case
            |> between('{', '}', ',')
            
        kw1 "enum"
        >>. ident
        .>> sps1 '='
        .>>. opt_or [] members
        |>> (fun (name, members) ->
            { Name=name
            ; Cases=List.map EnumCase.Name members })
    
    // <include-item>
    let include_item : P<string> =
        kw1 "include"
        >>. string_literal
        <?!> "include-item"
        
    // A set literal of primitives eg: {1,2,3}
    let set_literal =
        expr
        |> between('{', '}', ',')
        <?!> "set-literal"

    // <var-par>
    let var_par : P<bool> =
        [ "var" => true
        ; "par" => false]
        |> choice
        .>> spaces1
        |> opt_or false
        <?!> "var-par"
        
    // <opt-ti>        
    let opt_ti =
        ps1 "opt"
        >>% true
        |> opt_or false
        <?!> "opt-ti"
    
    // <set-ti>    
    let set_ti =
        ps1 "set"
        >>. ps1 "of"
        >>% true
        |> opt_or false
        <?!> "set-ti"
   
    // <base-ti-expr>
    let base_ti_expr : P<TypeInst> =
        pipe4
            var_par
            set_ti
            opt_ti
            base_ti_expr_tail
            (fun var set opt typ ->
                { Type = typ
                ; IsOptional = opt
                ; IsSet = set
                ; Name = ""
                ; Dimensions = []
                ; IsVar = var }
            )
        <?!> "base-ti"
    
        
    // <array-ti-expr>        
    let array_ti_expr : P<TypeInst> =
        
        let dimensions =
            base_ti_expr_tail
            |> between('[', ']', ',')
            <?!> "array-dimensions"
        
        ps  "array"
        >>.  dimensions
        .>>  sps1 "of"
        .>>. base_ti_expr
        |>> (fun (dims, ti) ->
            { ti with Dimensions = dims })
        <?!> "array-ti-expr"
    
    // <ti-expr>        
    ti_expr_ref.contents <-
        array_ti_expr
        <|> base_ti_expr
        <?!> "ti-expr"

    // <tuple-ti-expr-tail>
    let tuple_ti =
        kw "tuple"
        >>. between1('(', ')', ',') ti_expr
        <?!> "tuple-ti"
            
    // <record-ti-expr-tail>
    let record_ti =
        kw "record"
        >>. between1('(', ')', ',') ti_expr_and_id
        <?!> "record-ti"
            
    // <base-ti-expr-tail>
    base_ti_expr_tail_ref.contents <-
        [ "bool"      => Type.Bool
        ; "int"       => Type.Int
        ; "string"    => Type.String
        ; "float"     => Type.Float
        ; record_ti  |>> Type.Record
        ; tuple_ti   |>> Type.Tuple
        ; ident      |>> Type.Id
        ; set_literal   |>> Type.Set
        ; range_expr |>> Type.Range ]
        |> choice
        <?!> "base-ti-tail"

    // <call-expr>
    let call_expr =
        attempt (
            (id_or Choice1Of2 Choice2Of2 builtin_op)
            .>> spaces
            .>>. between1('(', ')', ',') expr
        )
        |>> (fun (name, args) ->
            { Name=name; Args=args })
        <?!> "call-expr"

    // <declare-item>
    let var_decl_item =
        ti_expr_and_id
        .>> spaces
        .>>. opt (ps '=' >>. expr)

    // <constraint-item>
    let constraint_item =
        kw "constraint"
        >>. expr
        <?!> "constraint"
        
    // <let-item>
    let let_item : P<LetItem> =
        (var_decl_item |>> LetItem.Declare)
        <|>
        (constraint_item |>> LetItem.Constraint)
    
    // <let-expr>
    let let_expr : P<LetExpr> =
        kw "let"
        >>. between('{', '}', ';') let_item
        .>> sps1 "in"
        .>>. expr
        |>> (fun (items, body) -> {Items=items; Body=body})
        <?!> "let-expr"
        
    // <if-then-else-expr>
    let if_else_expr : P<IfThenElseExpr> =
        
        let if_case = 
            kw1 "if"
            >>. expr
            .>> spaces1
            <?!> "if-case"
            
        let then_case =
            kw1 "then"
            >>. expr
            .>> spaces1
            <?!> "else-case"
            
        let elseif_case =
            kw1 "elseif"
            >>. expr
            .>> sps1 "then"
            >>. expr
            |> many
            <?!> "elseif-case"
            
        let else_case =
            expr
            |> betweens("else", "endif")
            <?!> "else-case"
            
        pipe4
            if_case
            then_case
            elseif_case
            else_case
            (fun if_ then_ elseif_ else_ ->
                { If = if_
                ; Then = then_
                ; ElseIf = elseif_
                ; Else = else_ })    
    
    // <num-un-op>    
    let num_un_op =
        builtin_num_un_op
        |> or_id
        .>> spaces
        .>>. num_expr_atom
        <?!> "num-un-op"
        
    let quoted_op =
        builtin_op
        |> between(SINGLE_QUOTE, SINGLE_QUOTE)
        |> attempt
    
    // <array-acces-tail>
    let array_access =
        expr
        |> between1('[', ']', ',')
        <?!> "array-access"

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
        <?!> "num-expr-atom-head"
        
    // <num-expr-atom>        
    num_expr_atom_ref.contents <-
        pipe2
            num_expr_atom_head
            (many (attempt(sp array_access)))
            (fun head access ->
                match access with
                | []->
                    head
                | _ ->
                    NumExpr.ArrayAccess (head, access)
                
            )
        <?!> "num-expr-atom"
        
    let num_bin_op =
        builtin_num_bin_op
        |> or_id
        
    // <num-expr-binop-tail>
    let num_expr_binop_tail =        
        sps num_bin_op
        .>>. num_expr
        <?!> "num-expr-binop-tail"

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
        <?!> "num-expr"
        
        
    // <expr-atom-head>    
    let expr_atom_head=
        [ float_literal   |>> Expr.Float
          int_literal     |>> Expr.Int
          bool_literal    |>> Expr.Bool
          string_literal  |>> Expr.String
          "_"              => Expr.Wildcard
          bracketed expr  |>> Expr.Bracketed
          let_expr        |>> Expr.Let
          if_else_expr    |>> Expr.IfThenElse
          call_expr       |>> Expr.Call
          array2d_literal |>> Expr.Array2d
          array1d_literal |>> Expr.Array1d
          un_op           |>> Expr.UnaryOp
          quoted_op       |>> Expr.Op
          ident           |>> Expr.Id
          ]
        |> choice
        <?!> "expr-atom-head"

    // <expr-atom>        
    expr_atom_ref.contents <-
        pipe2
            expr_atom_head
            (many (attempt(sp array_access)))
            (fun head tail ->
                match tail with
                | [] ->
                    head
                | access ->
                    Expr.Indexed (head, access)
            )
        <?!> "expr-atom"
            
    // <expr-binop-tail>
    let expr_binop_tail =
        sps bin_op .>>. expr
            
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
        <?!> "expr"
            
    let solve_method : P<SolveMethod> =
        lookup(
          "satisfy" => SolveMethod.Satisfy,
          "minimize" => SolveMethod.Minimize,
          "maximize" => SolveMethod.Maximize
        )
        <?!> "solve-method"
        
    let annotations : P<Annotations> =
        preturn []
        <?!> "annotations"
            
    // <solve-item>
    let solve_item : P<SolveItem> =
        pipe3
            (kw1 "solve" >>. annotations)
            (sps solve_method)
            (opt expr)
            (fun annos method obj ->
                match obj with
                | Some o ->
                    { Annotations = annos
                    ; Method = method
                    ; Objective = o }
                    |> SolveItem.Optimise
                | None ->
                    { Annotations = annos }
                    |> SolveItem.Satisfy)
        <?!> "solve-item"            
        
    // <assign-item>
    let assign_item =
        attempt (ident .>> sps1 '=')
        .>>. expr
        <?!> "assign-item"
        
    let unknown_item =
        manyChars (noneOf ";")
        
    // <type-inst-syn-item>
    let alias_item =
        kw1 "type"
        >>. ident
        .>> sps1 "="
        .>>. ti_expr
        |>> (fun (name, ti) -> { ti with Name = name })
        <?!> "type-alias"
        
    // <output-item>
    let output_item : P<OutputItem> =
        kw1 "output"
        >>. expr        
        <?!> "output-item"

    // <item>
    let item =
        [ enum_item       |>> Item.Enum
        ; constraint_item |>> Item.Constraint
        ; include_item    |>> Item.Include
        ; solve_item      |>> Item.Solve
        ; alias_item      |>> Item.Alias
        ; output_item     |>> Item.Output
        ; predicate_item  |>> Item.Predicate
        ; function_item   |>> Item.Function
        ; test_item       |>> Item.Test
        ; assign_item     |>> Item.Assign
        ; var_decl_item   |>> Item.Declare
        ; unknown_item    |>> Item.Other ]
        |> choice
                    
    // Parse a model from a the given string                       
    let model =
        many (item .>> sps ';')


module Parse =
                            
    // Parse the given string with the given parser            
    let string (parser: P<'t>) (input: string) =
        let lexed = input.Trim()
        let state = { Debug = { Message = ""; Indent = 0 } }
        match runParserOnString parser state "" lexed with
        | Success (value, _, _)     ->
            Result.Ok value
        | Failure (s, msg, state) ->
            let error =
                { Message = s
                ; Trace = state.Debug.Message }
            Result.Error error
            
    // Parse the given file with the given parser
    let file (parser: P<'t>) (fi: FileInfo) =
        let input = File.ReadAllText fi.FullName
        let result = string parser input
        result
        