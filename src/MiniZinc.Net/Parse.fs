namespace MiniZinc

module ParseUtils =
    open FParsec
    
    type P () =
        
        static member p (x: char) =
            pchar x
            
        static member p (x: string) =
            pstring x
            
        static member ps (x: string) =
            pstring x .>> spaces
            
        static member ps (x: char) =
            pchar x .>> spaces
            
        static member ps1 (x: string) =
            pstring x .>> spaces1
            
        static member ps1 (x: char) =
            pchar x .>> spaces1            
            
        static member spaced x =
            between spaces spaces x
        
        static member spaced (x: string) =
            P.spaced (pstring x)
            
        static member spaced (c: char) =
            P.spaced (pchar c)
            
        static member spaced1 x =
            between spaces1 spaces1 x
        
        static member spaced1 (x: string) =
            P.spaced1 (pstring x)
            
        static member spaced1 (c: char) =
            P.spaced1 (pchar c)
            
        static member betweenChars (a: char) (b: char) p =
            between (pchar a) (pchar b) p
            
        static member betweenSep left right delim p =
            between (left >>. spaces) (spaces >>. right) (sepBy p (spaces >>. delim >>. spaces))
            
        static member betweenSep1 left right delim p =
            between (left >>. spaces) (spaces >>. right) (sepBy1 p (spaces >>. delim >>. spaces))
            
        static member betweenSep1Char left right delim p =
            P.betweenSep1 (pchar left) (pchar right) (pchar delim) p
            
        static member betweenSepChar left right delim p =
            P.betweenSep (pchar left) (pchar right) (pchar delim) p
        
    open type P
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

    let (.>>>) a b =
        a .>> spaces .>> b
        
    let (>>>.) a b =
        a >>. spaces >>. b
        
    let (.>>>.) a b =
        a .>> spaces .>>. b
        
    let chr x =
        pchar x
        
    let str x =
        pstring x
        
    let opt_or backup p =
        (opt p) |>> Option.defaultValue backup
    

open ParseUtils

module Parse =

    open System.IO
    open FParsec
    open ParseUtils
    open type ParseUtils.P
        
    // Parameter Type
    type VarKind =
        | Par = 0
        | Var = 1
    
    type BaseType =    
        | Int
        | Bool
        | String
        | Float

    type Expr = unit
            
    type IncludeItem = string
        
    type ConstraintItem = string
    
    type OutputItem = Expr
                
    type SolveMethod =
        | Satisfy = 0
        | Minimize = 1
        | Maximize = 2
    
    type SolveItem =
        { Annotations : unit
          Method : SolveMethod
          Expr : Expr option }

    // <num-expr>        
    type NumExpr =
        | Int       of int
        | Float     of float
        | Id        of string
        | Bracketed of NumExpr

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
        | Alias      of TypeInst
        | Constraint of Expr
        | Assign     of string * Expr
        | Declare    of TypeInst * Expr option
        | Solve      of SolveItem
        | Predicate  of Expr
        | Function   of TypeInst * OperationItem
        | Test       of Test
        | Annotation of Annotation
        | Other      of string
        
    and OperationItem = unit        
        
    and Annotation = unit
    
    and Test = unit
    
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
        |> betweenChars '"' '"'
        <?!> "string-literal"

    let num_expr, num_expr_ref =
        createParserForwardedToRef<NumExpr, UserState>()
            
    let bracketed =
        betweenChars '(' ')' num_expr

    // <builtin-num-un-op>    
    let builtin_num_un_op : P<string> =
        [ "+" ; "-" ]
        |> List.map pstring
        |> choice
    
    // <builtin-num-bin-op>
    let builtin_num_bin_op : P<string> =
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
         |> List.map pstring
         |> choice
            
    // <builtin-bin-op>            
    let builtin_bin_op : P<string> =
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
        |> List.map pstring
        |> choice
        
    let builtin_un_op : P<string> =
        [ "+" ; "-"; "not" ]
        |> List.map pstring
        |> choice
        
    let builtin_op =
        builtin_bin_op <|> builtin_un_op
        
        
    // <num-expr-atom-head>    
    let num_expr_atom_head=
        // <builtin-num-un-op> <num-expr-atom>
        //                | <ident-or-quoted-op>
        //                | <int-literal>
        //                | <float-literal>
        //                | <if-then-else-expr>
        //                | <let-expr>
        //                | <call-expr>
        //                | <gen-call-expr>
        [ int_literal   |>> NumExpr.Int
          float_literal |>> NumExpr.Float
          ident         |>> NumExpr.Id
          bracketed     |>> NumExpr.Bracketed
          
          ]
        |> choice
        
    
    //         
    // <num-expr> ::= <num-expr-atom> <num-expr-binop-tail>
    // <num-expr-atom> ::= <num-expr-atom-head> <expr-atom-tail> <annotations>
    // <num-expr-binop-tail> ::= [ <num-bin-op> <num-expr> ]
       
    
    // 0 .. 10
    let range_expr =
        num_expr
        .>> ( spaced "..")
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
            |> betweenSep1Char '{' '}' ','
            
        p "enum"
        .>> spaced1 '='
        .>>>. opt_or [] members
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
        betweenSepChar '{' '}' ',' expr
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
        (p "opt" >>. spaces1 >>% true)
        |> opt_or false
        <?!> "opt-ti"
    
    // <set-ti>    
    let set_ti =
        p "set"
        >>. spaces1
        >>. p "of"
        >>. spaces1
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
            |> betweenSepChar '[' ']' ','
            <?!> "array-dimensions"
        
        str  "array"
        >>.  spaces
        >>.  dimensions
        .>>  spaced1 "of"
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
        .>> spaced ':'
        .>>. ident
        |>> (fun (expr, name) ->
            { expr with Name = name })
        <?> "ti-expr-and-id"

    // <tuple-ti-expr-tail>
    let tuple_ti =
        ps "tuple"
        >>. betweenSep1Char '(' ')' ',' ti_expr
        <?!> "tuple-ti"
            
    // <record-ti-expr-tail>
    let record_ti =
        ps "record"
        >>. betweenSep1Char '(' ')' ',' ti_expr_and_id
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
        
    // <solve-item>
    let solve_item : P<SolveItem> =
        todo<SolveItem>()
        
    // <assign-item>
    let assign_item =
        ident
        .>> spaced1 '='
        .>>. expr
        
    let unknown_item =
        manyChars (noneOf ";")
        
    // <declare-item>
    let declare_item =
        ti_expr_and_id
        .>> spaces
        .>>. opt (ps '=' >>. expr)
        
    // <constraint-item>
    let constraint_item =
        p "constraint"
        >>. spaces1
        >>. expr
        <?!> "constraint"
        
    // <item>
    let item =
        [ declare_item    |>> Item.Declare
        ; enum_item       |>> Item.Enum
        ; constraint_item |>> Item.Constraint
        ; include_item    |>> Item.Include
        ; assign_item     |>> Item.Assign
        ; solve_item      |>> Item.Solve            
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