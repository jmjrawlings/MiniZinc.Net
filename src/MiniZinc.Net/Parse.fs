﻿namespace MiniZinc

open System
open System.Diagnostics
open System.IO
open System.Runtime.InteropServices
open FParsec

type ParseError =
    { Message: string
    ; Trace: string }

exception ParseException of ParseError

type ParseResult<'T> = Result<'T, ParseError>


module ParseUtils =
    type DebugInfo = { Message: string; Indent: int }
    type UserState = { mutable Debug: DebugInfo }
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
        p <?> label
    #endif        
        
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
        static member ps x =
            x .>> spaces
        
        // parse spaces    
        static member ps (x: string) : P<string> =
            pstring x .>> spaces
            
        // parse spaces
        static member ps (x: char) : P<char> =
            pchar x .>> spaces
        
        // parse spaces1
        static member ps1 x =
            x .>> spaces1
        
        // parse spaces1    
        static member ps1 (x: string) : P<string> =
            pstring x .>> spaces1

        // parse spaces1                        
        static member ps1 (x: char) : P<char> =
            pchar x .>> spaces1            
        
        // space parse space    
        static member sps x =
            between spaces spaces x
        
        // space parse space
        static member sps (x: string) : P<string> =
            P.sps (pstring x)
            
        // space parse space            
        static member sps (c: char) : P<char> =
            P.sps (pchar c)
            
        // space1 parse space1
        static member sps1 x =
            between spaces1 spaces1 x

        // space1 parse space1        
        static member sps1 (x: string) : P<string> =
            P.sps1 (pstring x)
            
        // space1 parse space1
        static member sps1 (c: char) : P<char> =
            P.sps1 (pchar c)
        
        // space parse     
        static member sp x =
            spaces >>. x

        // space parse     
        static member sp (c: char) =
            P.sp (pchar c)

        // space parse     
        static member sp (s: string) =
            P.sp (pstring s)
            
        // space1 parse
        static member sp1 x =
            spaces1 >>. x
            
        // space1 parse
        static member sp1 (c: char) =
            P.sp1 (pchar c)
            
        // space1 parse            
        static member sp1 (s: string) =
            P.sp1 (pstring s)

        // Parse between 'start' and 'end' with optional whitespace 
        static member between (pStart: P<'a>, pEnd: P<'a>, [<Optional; DefaultParameterValue(false)>] ws : bool) =
            let left, right =
                match ws with
                | true -> (pStart .>> spaces), (spaces >>. pEnd)
                | false -> pStart,pEnd
            between left right

        // Parse between 'start' and 'end' with optional whitespace 
        static member between (a: char, b: char, [<Optional; DefaultParameterValue(false)>] ws : bool) =
            P.between(pchar a, pchar b, ws)

        // Parse between 'start' and 'end' with optional whitespace 
        static member between (a: string, b: string, [<Optional; DefaultParameterValue(false)>] ws : bool) =
            P.between (pstring a, pstring b, ws)

        // Parse between 'start' and 'end' with whitespace 
        static member betweens (a: char, b: char) =
            P.between(a, b, ws=true)

        // Parse between 'start' and 'end' with whitespace
        static member betweens (a: string, b: string) =
            P.between (a, b, ws=true)

        // Parse 0 or more 'p' between 'start' and 'end' with optional whitespace            
        static member between (pStart: P<'a>, pEnd: P<'a>, pDelim: P<'b>, [<Optional; DefaultParameterValue(false)>] ws : bool, [<Optional; DefaultParameterValue(false)>] many : bool) =
            fun p ->
                
                let item, delim =
                    match ws with
                    | true ->
                        (p .>> spaces), (pDelim .>> spaces)
                     | false ->
                         p, pDelim
                
                let items =
                    match many with
                    | true -> sepBy1 item delim
                    | false -> sepBy item delim
                    
                P.between(pStart, pEnd, ws=ws) items                    
                
        // Parse 0 or more between 'start' and 'end' with optional whitespace                
        static member between (a: char, b: char, c: char, [<Optional; DefaultParameterValue(false)>] many : bool) =
            P.between(pchar a, pchar b, pchar c, ws=true, many=many)

        // Parse 0 or more between 'start' and 'end' with optional whitespace            
        static member between (a:string, b:string, c:string, [<Optional; DefaultParameterValue(false)>] many : bool) =
            P.between(pstring a, pstring b, pstring c, ws=true, many=many)
        
        // Parse 1 or more between 'start' and 'end' with optional whitespace    
        static member between1 (a:string, b:string, c:string) =
            P.between(a, b, c, many=true)
            
        // Parse 1 or more between 'start' and 'end' with optional whitespace
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
    
    type Ident = string
    
    [<Struct>]
    [<DebuggerDisplay("_")>]
    type WildCard =
        | WildCard
    
    type Comment = string
    
    type BinaryOp = string
        
    type UnaryOp = string
    
    type Op = string
    
    [<DebuggerDisplay("{String}")>]
    [<Struct>]
    // A value of 'T' or an identifier
    type IdentOr<'T> =
        | Value of value:'T
        | Ident of name:string
        
        member this.String =
            match this with
            | Value v -> string v
            | Ident s -> s
            
        
        override this.ToString() =
            this.String
            
        
                
    type SolveMethod =
        | Satisfy = 0
        | Minimize = 1
        | Maximize = 2
   
    type Expr =
        | WildCard      of WildCard  
        | Int           of int
        | Float         of float
        | Bool          of bool
        | String        of string
        | Id            of string
        | Op            of string
        | Bracketed     of Expr
        | Set           of SetExpr
        | SetComp       of SetCompExpr
        | Array1d       of Array1dExpr
        | Array1dIndex
        | Array2d       of Array2dExpr
        | Array2dIndex
        | ArrayComp     of ArrayCompExpr
        | ArrayCompIndex
        | Tuple         of TupleExpr
        | Record        of RecordExpr
        | UnaryOp       of IdentOr<UnaryOp> * Expr
        | BinaryOp      of Expr * IdentOr<BinaryOp> * Expr
        | Annotation
        | IfThenElse    of IfThenElseExpr
        | Let           of LetExpr
        | Call          of CallExpr
        | GenCall       of GenCallExpr 
        | Indexed       of expr:Expr * index: ArrayAccess list
    
    and ArrayAccess = Expr list
    
    and Annotation = Expr
    
    and Annotations = Annotation list
    
    and GenCallExpr =
        { Name: IdentOr<Op>
        ; Expr : Expr 
        ; Generators : Generator list }  
        
    and ArrayCompExpr =
        { Expr : Expr         
        ; Generators : Generator list }
    
    and SetCompExpr =
        { Expr : Expr         
        ; Generators : Generator list }

    // eg "x,y in array where x > y"
    and Generator =
        { Idents : IdentOr<WildCard> list
        ; Source : Expr
        ; Where  : Expr option }
    
    and CallExpr =
        { Name: IdentOr<Op>
        ; Args: Expr list }
    
    and SetExpr = Expr list
    
    and Array1dExpr = Expr list
    
    and Array2dExpr = Array1dExpr list
    
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
        | UnaryOp     of IdentOr<NumericUnaryOp> * NumExpr
        | BinaryOp    of NumExpr * IdentOr<NumericBinaryOp> * NumExpr
        | ArrayAccess of NumExpr * ArrayAccess list
            
    and NumericUnaryOp = string
    and NumericBinaryOp = string
    
    and Enum =
        { Name : string
        ; Annotations : Annotations
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
        { Type        : Type
          Name        : string
          IsVar       : bool
          IsSet       : bool
          IsOptional  : bool
          Annotations : Annotations
          Dimensions  : Type list
          Expr        : Expr option }
    
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
        | Comment    of string
        

    and ConstraintItem = Expr
            
    and IncludeItem = string
            
    and PredicateItem = OperationItem
    
    and TestItem = OperationItem
    
    and AliasItem = TypeInst

    and OutputItem = Expr
    
    and OperationItem =
        { Name: string
          Parameters : TypeInst list
          Annotations : Annotations
          Body: Expr option }
        
    and FunctionItem =
        { Name: string
          Returns : TypeInst
          Parameters : TypeInst list
          Body: Expr option }
    
    and Test = unit
    
    and AssignItem = string * Expr
    
    and DeclareItem = TypeInst
    
    and LetItem =
        | Declare of DeclareItem
        | Constraint of ConstraintItem
        
    and LetExpr =
        { Items: LetItem list;  Body: Expr }



module Parsers =
    
    open AST
    
    let simple_ident : P<Ident> =
        regex "_?[A-Za-z][A-Za-z0-9_]*"

    let quoted_ident : P<Ident> =
        regex "'[^'\x0A\x0D\x00]+'"
        
    // <ident>
    let ident : P<Ident> =
        regex "_?[A-Za-z][A-Za-z0-9_]*|'[^'\x0A\x0D\x00]+'"
        <?!> "identifier"

    // Parse a keyword, ensures its not a part of a larger string
    let kw (name: string) =
        p name
        .>> notFollowedBy letter
        >>. spaces
        
    // Parse a keyword, ensures its not a part of a larger string
    let kw1 (name: string) =
        p name
        .>> notFollowedBy letter
        >>. spaces1
    
    let line_comment : P<string> =
        p '%' >>.
        manyCharsTill (noneOf "\r\n") (skipNewline <|> eof)

    let block_comment : P<string> =
        regex "\/\*([\s\S]*?)\*\/"

    let comment : P<string> =
        line_comment
        <|> block_comment
        |>> (fun s -> s.Trim())
        
    let (=>) (key: string) (value) =
        pstring key >>% value
            
    let value_or_quoted_name (p: P<'T>) : P<IdentOr<'T>> =
        
        let value =
            p |>> IdentOr.Value
        
        let name =
            simple_ident
            |> between(BACKTICK, BACKTICK)
            |> attempt
            |>> IdentOr.Ident
            
        name <|> value
    
      
    let name_or_quoted_value (p: P<'T>) : P<IdentOr<'T>> =
        
        let name =
            ident |>> IdentOr.Ident
        
        let value =
            p
            |> between(SINGLE_QUOTE, SINGLE_QUOTE)
            |> attempt
            |>> IdentOr.Value
        
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
        ; "<="
        ; ">="
        ; "=="
        ; "<"
        ; ">"
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
        @ builtin_num_bin_ops
        
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
        
    // <num-expr-atom>
    let annotations, annotations_ref =
        createParserForwardedToRef<Annotations, UserState>()
                
    let bracketed x =
        betweens('(', ')') x

    let op p =
        value_or_quoted_name p
        
    let un_op =
        op builtin_un_op
        .>> spaces
        .>>. expr_atom
        <?!> "un-op"

    let builtin_ops =
        builtin_bin_ops @ builtin_un_ops
            
    let builtin_op : P<string> =
        builtin_ops
        |> List.map p
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
        |> attempt
        <?!> "array1d-literal"
        
    // <set-literal>
    let set_literal =
        between('{', '}', ',') expr
        |> attempt
        <?!> "set-literal"
        
    // <array2d-literal>
    let array2d_literal =
        let row =
            sepBy1 (ps expr) (ps ',')
        let delim =
            attempt (p '|' >>. notFollowedBy (p ']'))
        row
        |> between(p "[|", p "|]", delim, ws=true, many=false)
        <?!> "array2d-literal"
   
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
        pipe4
            (ps ident)
            (ps parameters)
            (ps annotations)
            (opt (ps "=" >>. expr))
            (fun name pars anns body ->
                { Name = name
                ; Parameters = pars
                ; Annotations = anns 
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
            
        pipe3
            (kw1 "enum" >>. ident .>> sps '=')
            (ps annotations)
            (opt_or [] members)
            (fun name anns cases ->
                { Name = name
                ; Annotations = anns
                ; Cases = List.map EnumCase.Name cases
                })
    
    // <include-item>
    let include_item : P<string> =
        kw1 "include"
        >>. string_literal
        <?!> "include-item"
    
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
                ; IsVar = var
                ; Annotations = [] 
                ; Expr = None }
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
    
    let id_or_op =
        name_or_quoted_value builtin_op
    
    // <call-expr>
    let call_expr =
        
        let operation =
            ps id_or_op
        
        // let args =
        //     ps '('
        //     >>. sepBy1 (ps expr) (ps ',')
        //     .>> p ')'
        //     <?!> "call-args"
        let args =
            between('(', ')', ',', many=true) expr
            <?!> "call-args"
        
        pipe2
            operation
            args
            (fun name args ->
                { Name=name; Args=args })
        |> attempt
        <?!> "call-expr"
        
    let wildcard : P<WildCard> =
        p '_'
        >>. notFollowedBy letter
        >>% WildCard.WildCard
        
        
    // <comp-tail>
    let comp_tail : P<Generator list> =
        let var =
            (wildcard |>> IdentOr.Value)
            <|>
            (ident |>> IdentOr.Ident)
            <?!> "gen-var"
        let vars =
            sepBy1 (ps var) (ps ",")
            <?!> "gen-vars"
        let where =
            kw1 "where" >>. expr
            <?!> "gen-where"
        let generator =    
            pipe3
                (vars .>> sps "in")
                (ps expr)
                (opt where)
                (fun idents source filter ->
                    { Idents = idents
                    ; Source = source
                    ; Where = filter })
            <?!> "generator"
            
        let generators =
            sepBy1 generator (sps ",")
            
        generators
        <?!> "comp_tail"
            
    // <gen-call-expr>
    let gen_call_expr =
        pipe3
            (ps id_or_op)
            (ps (betweens('(', ')') comp_tail))
            (betweens('(', ')') expr)
            (fun name gens expr ->
                { Name = name
                ; Generators = gens
                ; Expr = expr })
        |> attempt
        <?!> "gen-call"
    
    // <array-comp>
    let array_comp : P<ArrayCompExpr> =
        (expr .>> sps '|' .>>. comp_tail)
        |> betweens('[', ']')
        |> attempt
        |>> (fun (expr, gens) -> 
             { Expr=expr
             ; Generators = gens })
        <?!> "array-comp"

    // <set-comp>
    let set_comp : P<SetCompExpr> =
        (expr .>> sps '|' .>>. comp_tail)
        |> betweens('(', ')')
        |> attempt
        |>> (fun (expr, gens) -> 
             { Expr=expr
             ; Generators = gens })
        <?!> "set-comp"
            
    // <declare-item>
    let var_decl_item =
        pipe3
            (ps ti_expr_and_id)
            (ps annotations)
            (opt (ps '=' >>. expr))
            (fun ti anns expr ->
                { ti with
                    Annotations = anns;
                    Expr = expr
                })

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
        .>> sps "in"
        .>>. expr
        |>> (fun (items, body) -> {Items=items; Body=body})
        <?!> "let-expr"
        
    // <if-then-else-expr>
    let if_else_expr : P<IfThenElseExpr> =
        
        let if_case = 
            kw "if"
            >>. expr
            .>> spaces1
            <?!> "if-case"
            
        let then_case =
            kw "then"
            >>. expr
            .>> spaces1
            <?!> "else-case"
            
        let elseif_case =
            kw "elseif"
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
        op builtin_num_un_op
        .>> spaces
        .>>. num_expr_atom
        <?!> "num-un-op"
        
    let quoted_op =
        builtin_op
        |> between(SINGLE_QUOTE, SINGLE_QUOTE)
        |> attempt
    
    // <array-acces-tail>
    let array_access : P<ArrayAccess> =
        expr
        |> between1('[', ']', ',')
        <?!> "array-access"
        
    let expr_atom_tail =
        (many (attempt (sp array_access)))

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
            expr_atom_tail            
            (fun head access ->
                match access with
                | []->
                    head
                | _ ->
                    NumExpr.ArrayAccess (head, access)
                
            )
        <?!> "num-expr-atom"
        
    // <num-expr-binop-tail>
    let num_expr_binop_tail =        
        sps (op builtin_num_bin_op)
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
          set_literal     |>> Expr.Set
          un_op           |>> Expr.UnaryOp
          quoted_op       |>> Expr.Op
          ident           |>> Expr.Id
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
                    Expr.Indexed (head, access)
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
        <?!> "expr"
            
    let solve_method : P<SolveMethod> =
        lookup(
          "satisfy" => SolveMethod.Satisfy,
          "minimize" => SolveMethod.Minimize,
          "maximize" => SolveMethod.Maximize
        )
        <?!> "solve-method"
        
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
        
    // <type-inst-syn-item>
    let alias_item : P<TypeInst> =
        pipe3
            (kw1 "type" >>. ident .>> spaces)
            (annotations .>> sps1 "=")
            ti_expr
            (fun name anns ti ->
                { ti with
                    Name = name
                    Annotations = anns
                  })
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
        ; block_comment   |>> Item.Comment ]
        |> choice
                    
    // Parse a model from a the given string                       
    let model =
        spaces
        >>. sepEndBy1 item (sps ';')
        .>> eof


module Parse =
    
    open System.Text.RegularExpressions
    open AST
    
    // Sanitize the input string by removing any
    // blank lines and comments
    let sanitize (input: string) : string * List<Comment> =
        
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
            Regex.Replace(input, pattern, evaluator, RegexOptions.Multiline)

        // let no_empty_lines =
        //     Regex.Replace(no_comments, "^\s*$", "", RegexOptions.Multiline)
        //
        let comments = Seq.toList comments
        output, comments
        
    // Parse the given string with the given parser
    let string (parser: P<'t>) (input: string) : ParseResult<'t> =
        let state = { Debug = { Message = ""; Indent = 0 } }
        match runParserOnString parser state "" input with
        | Success (value, _state, _pos) ->
            Result.Ok value
        | Failure (msg, err, state) ->
            let err = { Message = msg; Trace = state.Debug.Message }
            Result.Error err
            
    // Parse the given file with the given parser
    let file (parser: P<'t>) (fi: FileInfo) : ParseResult<'t> =
        let input = File.ReadAllText fi.FullName
        let source, comments = sanitize input
        let result = string parser source
        result
        