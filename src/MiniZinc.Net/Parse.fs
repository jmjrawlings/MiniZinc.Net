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
    type BaseType =    
        | Int
        | Bool
        | String
        | Float
        
    type Expr = unit
    
    type ConstraintItem = Expr
            
    type IncludeItem = string
    
    type OutputItem = Expr
                
    type SolveMethod =
        | Satisfy = 0
        | Minimize = 1
        | Maximize = 2
        
    type Annotation = string

    type Annotations = Annotation list
        
    type SolveSatisfy =
        { Annotations : Annotations }
        
    type SolveOptimise =
        { Annotations : Annotations
          Method : SolveMethod
          Objective : Expr }
        
    type SolveItem =
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

    type IfThenElse =
        { If     : Expr
        ; Then   : Expr
        ; ElseIf : Expr list
        ; Else   : Expr}
   
    and NumExpr =
        | Int        of int
        | Float      of float
        | Id         of string
        | Bracketed  of NumExpr
        | Call       of string*Expr list
        | IfThenElse of IfThenElse
        | Let        of LetExpr
        | UnaryOp    of string * NumExpr
        | BinaryOp   of NumExpr * NumBinOp * NumExpr
        | Indexed    of NumExpr * Expr list
        
    and NumBinOp =
        | Builtin of string
        | Custom of string
    
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
        | Constraint of Expr
        | Assign     of AssignItem
        | Declare    of DeclareItem
        | Solve      of SolveItem
        | Predicate  of PredicateItem
        | Function   of FunctionItem
        | Test       of TestItem
        | Output     of OutputItem
        | Annotation of Annotation
        | Other      of string
        
    and PredicateItem = OperationItem
    
    and TestItem = OperationItem
    
    and AliasItem = TypeInst
    
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


open AST

module Parse =
    
    open AST
    
    // <ident>
    let ident =
        regex "_?[A-Za-z][A-Za-z0-9_]*|'[^'\x0A\x0D\x00]+'"
        <?!> "identifier"

    // <int-literal>
    let int_literal =
        many1Satisfy Char.IsDigit
        |>> int
        <?!> "int-literal"
    
    // <bool-literal>    
    let bool_literal : P<bool> =
        (p "true" >>% true)
        <|>
        (p "false" >>% false)
        <?!> "bool-literal"
        
    // <float-literal>        
    let float_literal : P<float> =
        regex "[0-9]+.[0-9]+"
        |>> float
        <?!> "float-literal"
        
    // <string-literal>
    let string_literal : P<string> =
        manySatisfy (fun c -> c <> '"')
        |> between('"', '"')
        <?!> "string-literal"

    let num_expr, num_expr_ref =
        createParserForwardedToRef<NumExpr, UserState>()
        
    let num_expr_atom, num_expr_atom_ref =
        createParserForwardedToRef<NumExpr, UserState>()
            
    let bracketed =
        betweens('(', ')') num_expr

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
    
    // <ti-expr>
    let ti_expr, ti_expr_ref =
        createParserForwardedToRef<TypeInst, UserState>()

    // <ti-expr>
    let base_ti_expr_tail, base_ti_expr_tail_ref =
        createParserForwardedToRef<Type, UserState>()
        
    // <expr>        
    let expr, expr_ref =
        createParserForwardedToRef<Expr, UserState>()
    
         
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
        ps1 "predicate" >>. operation_item_tail

    // <test_item>
    let test_item : P<TestItem> =
        ps1 "test" >>. operation_item_tail
        
    // <function-item>
    let function_item : P<FunctionItem> =
        ps1 "function"
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
        
    // A set literal of primitives eg: {1,2,3}
    let set_expr =
        expr
        |> between('{', '}', ',')
        <?!> "set-expr"

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
        choice [
            array_ti_expr
            base_ti_expr 
        ]
        <?!> "ti-expr"

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
        |> attempt
        <?!> "call-expr"

    // <declare-item>
    let var_decl_item =
        ti_expr_and_id
        .>> spaces
        .>>. opt (ps '=' >>. expr)

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
    let if_else_expr : P<IfThenElse> =
        let if_case = 
            ps1 "if"
            >>. expr
            .>> spaces1
            <?!> "if-case"
        let then_case =
            ps1 "then"
            >>. expr
            .>> spaces1
            <?!> "else-case"
        let elseif_case =
            ps1 "elseif"
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
        .>> spaces
        .>>. num_expr_atom
        <?!> "num-un-op"
        
    // <num-expr-atom-head>    
    let num_expr_atom_head=
        [ float_literal |>> NumExpr.Float
          int_literal   |>> NumExpr.Int
          bracketed     |>> NumExpr.Bracketed
          let_expr      |>> NumExpr.Let
          if_else_expr  |>> NumExpr.IfThenElse
          num_un_op     |>> NumExpr.UnaryOp
          call_expr     |>> NumExpr.Call
          ident_or_op   |>> NumExpr.Id
          ]
        |> choice
        <?!> "num-expr-atom-head"
        
    // <num-expr-atom>        
    num_expr_atom_ref.contents <-
        pipe2
            (num_expr_atom_head .>> spaces) 
            (opt (between1('[', ']', ',') expr))
            (fun head tail ->
                match tail with
                | Some index ->
                    NumExpr.Indexed (head, index)
                | None ->
                    head
            )
        <?!> "num-expr-atom"
        
    let num_bin_op : P<NumBinOp> =
        let builtin =
            builtin_num_bin_op
            |>> NumBinOp.Builtin
        let custom =
            between(''',''') ident
            |>> NumBinOp.Custom
        builtin
        <|> custom
        <?!> "num-bin-op"
            
    // <num-expr>
    num_expr_ref.contents <-
        pipe2
            num_expr_atom
            (opt (sps num_bin_op .>>. num_expr))
            (fun head tail ->
                match tail with
                | None ->
                    head
                | Some (op, right) ->
                    NumExpr.BinaryOp (head, op, right)
            )
        <?!> "num-expr"            
        
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
            (ps1 "solve" >>. annotations)
            (sps1 solve_method)
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
        
    // <assign-item>
    let assign_item =
        ident
        .>> sps1 '='
        .>>. expr
        
    let unknown_item =
        manyChars (noneOf ";")
        
    // <type-inst-syn-item>
    let alias_item =
            ps1 "type"
        >>. ident
        .>> sps1 "="
        .>>. ti_expr
        |>> (fun (name, ti) -> { ti with Name = name })
        <?!> "type-alias"
        
    // <output-item>
    let output_item : P<OutputItem> =
        ps1 "output"
        >>. expr

    // <item>
    let item =
        [ var_decl_item   |>> Item.Declare
        ; enum_item       |>> Item.Enum
        ; constraint_item |>> Item.Constraint
        ; include_item    |>> Item.Include
        ; assign_item     |>> Item.Assign
        ; solve_item      |>> Item.Solve
        ; alias_item      |>> Item.Alias
        ; output_item     |>> Item.Output
        ; predicate_item  |>> Item.Predicate
        ; function_item   |>> Item.Function
        ; test_item       |>> Item.Test
        ; unknown_item    |>> Item.Other ]
        |> choice
    
                
    // Parse the given string with the given parser            
    let parseString (p: P<'t>) (input: string) =
        let state = { Debug = { Message = ""; Indent = 0 } }
        match runParserOnString p state "" input with
        | Success (value, _, _) ->
            Result.Ok value
        | Failure (s, msg, state) ->
            let error =
                { Message = s
                ; Trace = state.Debug.Message }
            Result.Error error
        
    // Turn the given parser into one that applies line by line                      
    let by_line (p: P<'t>) =
        sepEndBy p (sps ';')
        .>> eof
                    
    // Parse a model from a the given string                       
    let model (input: string) =
        parseString (by_line item) input
             
    // Parse the given file
    let file (fi: FileInfo) =
        let contents = File.ReadAllText fi.FullName
        let name = Path.GetFileNameWithoutExtension fi.Name
        let model = model contents
        model