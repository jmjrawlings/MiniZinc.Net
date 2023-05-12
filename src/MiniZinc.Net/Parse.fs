namespace MiniZinc

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
            
        static member ps (x: string) : P<string> =
            pstring x .>> spaces
            
        static member ps (x: char) : P<char> =
            pchar x .>> spaces
            
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
            
    

open ParseUtils
open type ParseUtils.P

module AST =
    type BaseType =    
        | Int
        | Bool
        | String
        | Float

    type Expr = unit
            
    type IncludeItem = string
        
    type ConstraintItem = Expr
    
    type OutputItem = Expr
                
    type SolveMethod =
        | Satisfy = 0
        | Minimize = 1
        | Maximize = 2
    
    type SolveItem =
        { Annotations : unit
          Method : SolveMethod
          Expr : Expr option }

    type IfThenElse =
        { If     : Expr
        ; Then   : Expr
        ; ElseIf : Expr list
        ; Else   : Expr}
   
    type NumExpr =
        | Int        of int
        | Float      of float
        | Id         of string
        | Bracketed  of NumExpr
        | Call       of string*Expr list
        | IfThenElse of IfThenElse
    
    type Enum =
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
    type TypeInst =
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
            
    type Item =
        | Include    of string
        | Enum       of Enum
        | Alias      of AliasItem
        | Constraint of Expr
        | Assign     of AssignItem
        | Declare    of DeclareItem
        | Solve      of SolveItem
        | Predicate  of Expr
        | Function   of TypeInst * OperationItem
        | Test       of Test
        | Annotation of Annotation
        | Other      of string
        
    and OperationItem = unit        
        
    and Annotation = unit
    
    and AliasItem = TypeInst
    
    and Test = unit
    
    and AssignItem = string * Expr
    
    and DeclareItem = TypeInst * Expr option
    
    and LetItem =
        | Declare of DeclareItem
        | Constraint of ConstraintItem
        
    and LetExpr =
        { Items: LetItem list;  Body: Expr }


module Parse =
    
    open AST
    
    // <ident>
    let ident =
        regex "_?[A-Za-z][A-Za-z0-9_]*|'[^'\x0A\x0D\x00]+'"
        <?!> "identifier"

    // <int-literal>
    let int_literal =
        pint32
        <?!> "int-literal"
    
    // <bool-literal>    
    let bool_literal : P<bool> =
        (p "true" >>% true)
        <|>
        (p "false" >>% false)
        <?!> "bool-literal"
        
    // <float-literal>        
    let float_literal : P<float> =
        pfloat
        <?!> "float-literal"
        
    // <string-literal>
    let string_literal : P<string> =
        manySatisfy (fun c -> c <> '"')
        |> between('"', '"')
        <?!> "string-literal"

    let num_expr, num_expr_ref =
        createParserForwardedToRef<NumExpr, UserState>()
            
    let bracketed =
        between('(', ')') num_expr

    // <builtin-num-un-op>
    let builtin_num_un_ops =
        [ "+" ; "-" ]
        
    let builtin_num_un_op : P<string> =
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
         
    let builtin_num_bin_op : P<string> =
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
        
    let builtin_ops =
        builtin_bin_ops @ builtin_un_ops
            
    let builtin_op : P<string> =
        builtin_ops
        |> List.map str
        |> choice
        
    // <ident-or-quoted-op>        
    let ident_or_op =
        builtin_op
        |> between(''', ''')
        |> attempt
        <|> ident
       
        
    
    //         
    // <num-expr> ::= <num-expr-atom> <num-expr-binop-tail>
    // <num-expr-atom> ::= <num-expr-atom-head> <expr-atom-tail> <annotations>
    // <num-expr-binop-tail> ::= [ <num-bin-op> <num-expr> ]
       
    
    // 0 .. 10
    let range_expr =
        num_expr
        .>> sps ".."
        .>>. num_expr
        |> attempt
        <?!> "range-expr"

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
            
        ps "enum"
        >>. ident
        .>> sps1 '='
        .>>. opt_or [] members
        |>> (fun (name, members) ->
            { Name=name
            ; Cases=List.map EnumCase.Name members })
    
    // <include-item>
    let include_item : P<string> =
        ps1 "include" >>. string_literal
        <?!> "include-item"
        
    // <expr>        
    let expr : P<Expr> =
        todo<Expr>()
        
    // A set literal of primitives eg: {1,2,3}
    let set_expr =
        expr
        |> between('{', '}', ',')
        <?!> "set-expr"

    // <var-par>
    let var_par : P<bool> =
        let var = p "var" >>% true
        let par = p "par" >>% false
        (var <|> par)
        .>> spaces1
        |> opt_or false
        <?!> "var-par"

    // <ti-expr>
    let ti_expr, ti_expr_ref =
        createParserForwardedToRef<TypeInst, UserState>()

    // <ti-expr>
    let base_ti_expr_tail, base_ti_expr_tail_ref =
        createParserForwardedToRef<Type, UserState>()
        
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
    
        
    ti_expr_ref.contents <-
        choice [
            array_ti_expr
            base_ti_expr 
        ]
        <?!> "ti-expr"
        
    // <ti-expr-and-id>
    let ti_expr_and_id : P<TypeInst> =
        ti_expr
        .>> sps ':'
        .>>. ident
        |>> (fun (expr, name) ->
            { expr with Name = name })
        <?> "ti-expr-and-id"

    // <tuple-ti-expr-tail>
    let tuple_ti =
        ps "tuple"
        >>. between1('(', ')', ',') ti_expr
        <?!> "tuple-ti"
            
    // <record-ti-expr-tail>
    let record_ti =
        ps "record"
        >>. between1('(', ')', ',') ti_expr_and_id
        <?!> "record-ti"
            
    // <base-ti-expr-tail>
    base_ti_expr_tail_ref.contents <-
        [ p "bool"     >>% Type.Bool
        ; p "int"      >>% Type.Int
        ; p "string"   >>% Type.String
        ; p "float"    >>% Type.Float
        ; record_ti    |>> Type.Record
        ; tuple_ti     |>> Type.Tuple
        ; ident        |>> Type.Id
        ; set_expr     |>> Type.Set
        ; range_expr   |>> Type.Range ]
        |> choice
        <?!> "base-ti-tail"

    // <call-expr>
    let call_expr =
        ident_or_op
        .>> spaces
        .>>. between1('(', ')', ',') expr
       
    
    // <num-expr-atom-head>    
    let num_expr_atom_head=
        // <builtin-num-un-op> <num-expr-atom>
        //                | <ident-or-quoted-op>
        //                | <if-then-else-expr>
        //                | <let-expr>
        //                | <gen-call-expr>
        [ int_literal   |>> NumExpr.Int
          float_literal |>> NumExpr.Float
          ident         |>> NumExpr.Id
          bracketed     |>> NumExpr.Bracketed
          call_expr     |>> NumExpr.Call
          ]
        |> choice
        
            
    // <solve-item>
    let solve_item : P<SolveItem> =
        todo<SolveItem>()
        
    // <assign-item>
    let assign_item =
        ident
        .>> sps1 '='
        .>>. expr
        
    let unknown_item =
        manyChars (noneOf ";")
        
    // <declare-item>
    let var_decl_item =
        ti_expr_and_id
        .>> spaces
        .>>. opt (ps '=' >>. expr)
        
    // <type-inst-syn-item>
    let alias_item =
            ps1 "type"
        >>. ident
        .>> sps1 "="
        .>>. ti_expr
        |>> (fun (name, ti) -> { ti with Name = name })
        <?!> "type-alias"
        
    // <constraint-item>
    let constraint_item =
        ps1 "constraint"
        >>. expr
        <?!> "constraint"
        
    // <let-item>
    let let_item : P<LetItem> =
        (var_decl_item |>> LetItem.Declare)
        <|>
        (constraint_item |>> LetItem.Constraint)
    
    // <let-expr>
    let let_expr : P<LetExpr> =
        ps "let"
        >>. between('{', '}', ';') let_item
        .>> sps1 "in"
        .>>. expr
        |>> (fun (items, body) -> {Items=items; Body=body})
        
    // <if-then-else-expr>
    let if_then_else_expr : P<IfThenElse> =
        let if_case = 
            ps1 "if" >>. expr .>> spaces1
        let then_case =
            ps1 "then" >>. expr .>> spaces1
        let elseif_case =
            ps1 "elseif"
            >>. expr
            .>> sps1 "then"
            >>. expr
            |> many
        let else_case =
            expr
            |> betweens("else", "endif")
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
            

    // <item>
    let item =
        [ var_decl_item   |>> Item.Declare
        ; enum_item       |>> Item.Enum
        ; constraint_item |>> Item.Constraint
        ; include_item    |>> Item.Include
        ; assign_item     |>> Item.Assign
        ; solve_item      |>> Item.Solve
        ; alias_item      |>> Item.Alias
        ; unknown_item    |>> Item.Other ]
        |> choice
    
    // <item>    
    let items =
        sepEndBy item (chr ';')
        .>> eof
                
    // Parse the given string with the given parser            
    let parseString (p: P<'t>) (s: string) =
        let input = s.Trim()
        let state = { Debug = { Message = ""; Indent = 0 } }
        match runParserOnString p state "" input with
        | Success (value, _, _) ->
            Result.Ok value
        | Failure (s, msg, state) ->
            let error =
                { Message = s
                ; Trace = state.Debug.Message }
            Result.Error error
    
    // Parse lines of the given string with the given parser                       
    let parseLines (p: P<'t>) (s: string) =
        let input =
            s.Split(';')
            |> Seq.map (fun s -> s.Trim())
            |> String.concat ";"
        let parser =
            sepEndBy p (chr ';') .>> eof
        let result =
            parseString parser input
        result
        
    // Parse a single line with the given paser
    // a ';' will be added to the end                       
    let parseLine (p: P<'t>) (s: string) =
        let input = s.Trim()
        let result = parseString p input
        result
                    
    // Parse a model from a the given string                       
    let model (s: string) =
        parseLines items s
             
    // Parse the given file
    let file (fi: FileInfo) =
        let contents = File.ReadAllText fi.FullName
        let name = Path.GetFileNameWithoutExtension fi.Name
        let model = model contents
        model