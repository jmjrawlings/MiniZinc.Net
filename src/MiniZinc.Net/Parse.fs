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
    type DebugType<'a> = Enter | Leave of Reply<'a>
    
    let todo<'t> () : P<'t> =
        preturn Unchecked.defaultof<'t>
    
    let addToDebug (stream:CharStream<UserState>) label dtype =
        let msgPadLen = 50
        let startIndent = stream.UserState.Debug.Indent
        let (str, curIndent, nextIndent) = 
            match dtype with
            | Enter ->
                $"Entering %s{label}", startIndent, startIndent+1
            | Leave res ->
                let str = $"Leaving  %s{label} (%A{res.Status})"
                let pad = max (msgPadLen - startIndent - 1) 0
                let resStr = $"%s{str.PadRight(pad)} {res.Result}"
                resStr, startIndent-1, startIndent-1

        let indentStr =
            let pad = max curIndent 0
            if curIndent = 0 then ""
            else "\u251C".PadRight(pad, '\u251C')

        let posStr = $"%A{stream.Position}: ".PadRight(20)
        let posIdentStr = posStr + indentStr

        // The %A for res.Result makes it go onto multiple lines - pad them out correctly
        let replaceStr = "\n" + "".PadRight(max posStr.Length 0) + "".PadRight(max curIndent 0, '\u2502').PadRight(max msgPadLen 0)
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

    let (<?!>) (p: P<'t>) label : P<'t> =
        p <?> label <!> label

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
    
    type AssignItem = string * Expr
    
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
        | Int of int
        | Float of float
        | Id of string

    // <enum-item>          
    type EnumItem =
        { Name : string
        ; Members : string list }
    
    and TypeInst =
        { Type : Type
          Var  : bool
          Set  : bool
          Dims : Type list
          Opt  : bool }
    
    and Type =
        | Int
        | Bool
        | String
        | Float
        | Id        of string
        | Variable  of string
        | Tuple     of TypeInst list
        | Record    of TypeInstAndId list
        | Set       of Expr list
        | Range     of NumExpr * NumExpr
            
    and TypeInstAndId =
        string * TypeInst
            
    type Item =
        | Include     of IncludeItem
        | Enum        of EnumItem
        | TypeSynonym of TypeSynonymItem
        | Constraint  of ConstraintItem
        | Assign      of AssignItem 
        | Declare     of DeclareItem
        | Solve       of SolveItem
        | Other       of string

    and DeclareItem =
        { Id   : string
        ; Type : TypeInst
        ; Expr : Expr option }

    and TypeSynonymItem =
        { Name: string
        ; Target: DeclareItem }

        
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
        (str "true" >>% true)
        <|>
        (str "false" >>% false)
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
            
    // <constraint-item>
    let constraint_item : P<string> =
        pstring "constraint"
        >>. spaces1
        >>. many1Chars (noneOf ";:=")
        <?!> "constraint"
        
    // <num-expr>        
    let num_expr =
        (int_literal |>> NumExpr.Int)
        <|>
        (float_literal |>> NumExpr.Float)
        <|>
        (ident |>> NumExpr.Id)
    
    let range_expr =
        attempt (num_expr .>> ( spaced "..") .>>. num_expr)
        <?!> "range-expr"

    // <enum-case>
    // TODO: complex variants
    let enum_case : P<string> =
        ident
          
    // <enum-item>
    let enum_item : P<EnumItem> =
        let members =
            enum_case
            |> betweenSep1Char '{' '}' ','
            
        p     "enum"
        .>>   spaced1 '='
        .>>>. (opt_or [] members)
        |>> (fun (name, members) ->
            { Name=name; Members=members })
    
    // <include-item>
    let include_item : P<string> =
        str "include" >>. string_literal
        <?!> "include-item"
        
    // <expr>        
    let expr : P<Expr> =
        todo<Expr>()
        
    // A set literal of primitives eg: {1,2,3}
    let set_expr =
        betweenSepChar '{' '}' ',' expr
        <?!> "set-expr"

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
            (fun var is_set is_opt typ ->
            { Type = typ
            ; Opt = is_opt
            ; Set = is_set
            ; Dims = []
            ; Var = var })
        <?!> "base-ti"
    
    // <base-type>
    let base_type : P<Type> =
        [ str "bool"   >>% Type.Bool
        ; str "int"    >>% Type.Int
        ; str "string" >>% Type.String
        ; str "float"  >>% Type.Float ]
        |> choice
        <?!> "base-type"
        
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
            { ti with Dims = dims })
        <?!> "array-ti-expr"
    
        
    ti_expr_ref.contents <-
        choice [
            array_ti_expr
            base_ti_expr 
        ]
        <?!> "ti-expr"
        
    // <ti-expr-and-id>
    let ti_expr_and_id : P<TypeInstAndId> =
        ti_expr
        .>> spaced ':'
        .>>. ident
        |>> (fun (expr, id) -> (id, expr))
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
        [ base_type  
        ; record_ti  |>> Type.Record
        ; tuple_ti   |>> Type.Tuple
        ; ident      |>> Type.Id
        ; set_expr   |>> Type.Set
        ; range_expr |>> Type.Range ]
        |> choice
        <?!> "base-ti-tail"
        
    // <solve-item>
    let solve_item : P<SolveItem> =
        todo<SolveItem>()
        
    // <solve-item>
    let assign_item =
        ident .>> spaced1 '=' .>>. expr
        
    let unknown_item =
        manyCharsTill (noneOf ";") (attempt (p ";"))
        
    // <declare-item>
    let declare_item =
        let head = ti_expr_and_id
        let tail = (p '=') >>. spaces >>. expr
        head .>> spaces .>>. (opt tail)
        |>> (fun ((id, ti), expr) ->
            { Id = id; Type = ti; Expr = expr }
            )
        
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