namespace MiniZinc.Parser

open System
open System.Collections.Generic
open System.IO
open System.Runtime.CompilerServices
open System.Text
open FParsec
open MiniZinc

[<AutoOpen>]    
module Parsers =
    
    open ParseUtils
    open type ParseUtils.P
    
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
    /// This function returns the TypeInst with the correctly
    /// inferred Vars 
    /// </remarks>
    let rec private resolveTypeInst (ti: TypeInst) =
        match resolveType ti.Type with
        // If the type is a Var it overrides the setting here
        | ty, true ->
            { ti with Type = ty; IsVar = true }
        // Otherwise use the existing value
        | ty, _ ->
            { ti with Type = ty; }
            
    and private resolveType (ty: Type) =
        match ty with
        | Type.Int 
        | Type.Bool 
        | Type.String 
        | Type.Float 
        | Type.Ident _ 
        | Type.Expr _
        | Type.Any
        | Type.Generic _
        | Type.Annotation
        | Type.Ann ->
            ty, false
            
        | Type.Concat xs as ty ->
            ty, List.exists (fun x -> x.IsVar) xs
        
        // Any var field means a var tuple
        | Type.Tuple x ->
            let mutable isVar = false
            let fields =
                x
                |> List.map (fun field ->
                    match resolveTypeInst field with
                    | ti when ti.IsVar ->
                        isVar <- true
                        ti
                    | ti ->
                        ti
                    )
                
            (Type.Tuple fields), isVar
                
        // Any var field means a var record
        | Type.Record x ->
            let mutable isVar = false
            
            let resolved =
                x
                |> List.map (fun field ->
                    let ti = resolveTypeInst field
                    if ti.IsVar then
                        isVar <- true
                    ti)
                
            (Type.Record resolved), isVar
            
        | Type.Array1D (i, ti) ->
            let ti = resolveTypeInst ti
            (Type.Array1D (i, ti)), ti.IsVar
            
        | Type.Array2D (i, j, ti) ->
            let ti = resolveTypeInst ti
            (Type.Array2D (i, j, ti)), ti.IsVar
            
        | Type.Array3D (i, j, k, ti) ->
            let ti = resolveTypeInst ti
            (Type.Array3D (i, j, k, ti)), ti.IsVar
        
        | Type.Array4D (i, j, k, l, ti) ->
            let ti = resolveTypeInst ti
            (Type.Array4D (i, j, k, l, ti)), ti.IsVar
        
        | Type.Array5D (i, j, k, l, m, ti) ->
            let ti = resolveTypeInst ti
            (Type.Array5D (i, j, k, l, m, ti)), ti.IsVar
            
        | Type.Array6D (i, j, k, l, m, n, ti) ->
            let ti = resolveTypeInst ti
            (Type.Array6D (i, j, k, l, m, n, ti)), ti.IsVar

                        
    let opt_or backup p =
        (opt p) |>> Option.defaultValue backup
       
    let simple_ident : Parser<Ident> =
        let options = IdentifierOptions(isAsciiIdStart = fun c -> c = '_' || Char.IsLetter c )
        identifier options

    let quoted_ident : Parser<Ident> =
        between
            (skipChar '\'')
            (skipChar '\'')
            (many1Chars (noneOf "'\x0A\x0D\x00"))
            |>> (fun s -> $"\'{s}\'")
               
    let ident : Parser<Ident> =
        simple_ident <|> quoted_ident
    
    // An identifier but also return its keyword
    let ident_kw : Parser<struct(string*Keyword)> =
        fun stream ->
            let reply = ident stream
            if reply.Status <> Ok then
                Reply(reply.Status, reply.Error)
            else
                let mutable word = Keyword.NONE
                Keyword.byName.TryGetValue(reply.Result, &word)
                Reply(struct(reply.Result, word))
    
    // let (=>) (kw: Keyword) b =
    //     let name = Keyword.byValue[kw]
    //     stringReturn name b
    //
    let (==>) (kw: Keyword) b =
        skip1 kw >>. (preturn b)
    
    // Parse the given operator and return the value
    // where value is an enum
    let (=!>) (operator: string) (value: 't) =
         attempt (stringReturn operator value)
            
    let name_or_quoted_value (p: Parser<'T>) : Parser<IdentOr<'T>> =
                        
        let name =
            ident
            |>> IdentOr.Ident
        
        let value =
            p
            |> between (skipChar ''') (skipChar ''') 
            |>> IdentOr.Other
        
        value <|> name
            
    // <float-literal> or <int-literal>
    let number_lit : Parser<Expr> =

        let format =
           NumberLiteralOptions.AllowFraction
           ||| NumberLiteralOptions.AllowExponent
           ||| NumberLiteralOptions.AllowHexadecimal
           ||| NumberLiteralOptions.AllowOctal
           
        fun stream ->
            let reply = numberLiteral format "number" stream
            let num = reply.Result
            // Parsing failed
            if reply.Status <> Ok then
                Reply(reply.Status, reply.Error)
            // Integer                
            elif num.IsInteger then
                Reply(Expr.Int (int32 num.String))
            // Float with a trailing dot (not allowed)
            elif num.String[num.String.Length - 1] = '.' then
                stream.Seek(stream.Index - 1L)
                let expr =
                    num.String.Substring(0, num.String.Length - 1)
                    |> int32
                    |> Expr.Int
                Reply(expr)
            else
                let expr = Expr.Float(float num.String)
                Reply(expr)
            
    // <string-literal>
    let string_lit : Parser<string> =
        let error = expected "string-literal"
        let invalidEscape = messageError "Invalid escape sequence"
        fun stream ->
            let mutable inExpr = false
            if stream.Read() <> '"' then
                 Reply(Error, error)
            else
                let buffer = StringBuilder()
                let rec loop () =
                    match stream.Peek() with
                    | '"' when not inExpr ->
                        stream.Skip()
                        Reply(buffer.ToString())
                    | '\\' ->
                        stream.Skip()
                        match stream.Peek() with
                        | 'n' ->
                            buffer.Append('\n')
                            stream.Skip()
                            loop ()
                        | 't' ->
                            buffer.Append('\t')
                            stream.Skip()
                            loop ()
                        | '"' ->
                            buffer.Append("\\\"")
                            stream.Skip()
                            loop ()
                        | '(' ->
                            buffer.Append("\\\"")
                            stream.Skip()
                            inExpr <- true
                            loop ()
                        | ''' ->
                            buffer.Append("\\\'")
                            stream.Skip()
                            loop ()
                        | _   ->
                            Reply(ReplyStatus.Error, invalidEscape)
                    | CharStream.EndOfStreamChar ->
                        Reply(Error, messageError "Unexpected end of input")
                        
                    | ')' when inExpr ->
                        inExpr <- false
                        buffer.Append(')')
                        stream.Skip()
                        loop ()
                        
                    | c ->
                        buffer.Append(c)
                        stream.Skip()
                        loop ()

                loop ()
    
    let string_annotation : Parser<Expr> =
        skip "::"
        >>. string_lit
        |>> Expr.String
    
    // <ti-expr>
    let ti_expr, ti_expr_ref =
        createParserForwardedToRef<TypeInst, ParserState>()

    // <expr-atom-ref>
    let expr_atom, expr_atom_ref =
        createParserForwardedToRef<Expr, ParserState>()

    // <ti-expr>
    let base_ti_expr_tail, base_ti_expr_tail_ref =
        createParserForwardedToRef<Type, ParserState>()

    // <annotation>
    let annotation, annotation_ref =
        createParserForwardedToRef<Annotation, ParserState>()
    
    let opp = OperatorPrecedenceParser<Expr, string, ParserState>()
    opp.TermParser <- (expr_atom .>> ws)
    
    let expr = opp.ExpressionParser
    let LeftAssoc = Associativity.Left
    let RightAssoc = Associativity.Right
    let NoAssoc = Associativity.None
           
    let addInfix (s: string) (op: BinOp) (precedence: int) (assoc: Associativity) =

        let create _ left right =
            Expr.BinaryOp(left, op, right)

        // For 'word' operators, eg `not` we need
        // to ensure it is not followed by more letters
        let after =
            if (Char.IsLetter s[0]) then
                notFollowedBy letter
                >>. ws >>. preturn ""
            else
                ws >>. preturn ""
                
        // TODO - Ask MiniZinc team about non-associative operators
        // as leaving it in causes expressions such as this
        // to fail:
        // ```R in row /\ C in col```
        let assoc =
            if assoc = NoAssoc then
                LeftAssoc
            else
                assoc

        let op =
            InfixOperator(s, after, precedence, assoc, (), create)
            
        opp.AddOperator op
        
    addInfix "<->"       BinOp.Equivalent       1200 LeftAssoc
    addInfix "->"        BinOp.Implies          1100 LeftAssoc
    addInfix "<-"        BinOp.ImpliedBy        1100 LeftAssoc
    addInfix "\\/"       BinOp.Or               1000 LeftAssoc
    addInfix "xor"       BinOp.Xor              1000 LeftAssoc
    addInfix "/\\"       BinOp.And              0900 LeftAssoc
    addInfix "<"         BinOp.LessThan         0800 NoAssoc
    addInfix ">"         BinOp.GreaterThan      0800 NoAssoc
    addInfix "<="        BinOp.LessThanEqual    0800 NoAssoc
    addInfix ">="        BinOp.GreaterThanEqual 0800 NoAssoc
    addInfix "=="        BinOp.Equal            0800 NoAssoc
    addInfix "="         BinOp.Equal            0800 NoAssoc
    addInfix "!="        BinOp.NotEqual         0800 NoAssoc
    addInfix "in"        BinOp.In               0700 NoAssoc
    addInfix "subset"    BinOp.Subset           0700 NoAssoc
    addInfix "superset"  BinOp.Superset         0700 NoAssoc
    addInfix "union"     BinOp.Union            0600 LeftAssoc
    addInfix "diff"      BinOp.Diff             0600 LeftAssoc
    addInfix "symdiff"   BinOp.SymDiff          0600 LeftAssoc
    addInfix ".."        BinOp.ClosedRange      0500 NoAssoc
    addInfix "<.."       BinOp.LeftOpenRange    0500 NoAssoc
    addInfix "..<"       BinOp.RightOpenRange   0500 NoAssoc
    addInfix "<..<"      BinOp.OpenRange        0500 NoAssoc
    addInfix "+"         BinOp.Add              0400 LeftAssoc
    addInfix "-"         BinOp.Subtract         0400 LeftAssoc
    addInfix "*"         BinOp.Multiply         0300 LeftAssoc
    addInfix "div"       BinOp.Div              0300 LeftAssoc
    addInfix "mod"       BinOp.Mod              0300 LeftAssoc
    addInfix "/"         BinOp.Divide           0300 LeftAssoc
    addInfix "intersect" BinOp.Intersect        0300 LeftAssoc
    addInfix "^"         BinOp.Exponent         0200 LeftAssoc
    addInfix "default"   BinOp.Default          0070 LeftAssoc
    addInfix "~!="       BinOp.TildeNotEqual    0800 NoAssoc
    addInfix "~="        BinOp.TildeEqual       0800 NoAssoc
    addInfix "~+"        BinOp.TildeAdd         0400 NoAssoc
    addInfix "~-"        BinOp.TildeSubtract    0400 NoAssoc
    addInfix "~*"        BinOp.TildeMultiply    0300 NoAssoc
    
    let concat =
        
        let after : Parser<string> =
            let error = messageError "Cannot use ++ within TypeInst expressions"
            fun stream ->
                let context = stream.UserState.Context
                if context = Context.TypeInst then
                    Reply(ReplyStatus.Error, error)
                else
                    stream.SkipWhitespace()
                    Reply("")

        let create _ left right =
            Expr.BinaryOp(left, BinOp.Concat, right)
        
        let op =
            InfixOperator("++", after, 100, RightAssoc, (), create)
            
        opp.AddOperator op
    
    let quoted_binop =
        
        let simple_ident =
            let options = IdentifierOptions(isAsciiIdStart = Char.IsLetter)
            identifier options

        let quoted_ident =
            simple_ident
            .>> (skipChar '`')
            .>> ws
            
        let create ident left right =
            Expr.Call(ident, [left;right])
            
        let op : InfixOperator<Expr, string, ParserState> =
            InfixOperator("`", quoted_ident, 50, LeftAssoc, (), create)
            
        opp.AddOperator op
        
    let addPrefix (s: string) precedence f =
        let operator =
            PrefixOperator(s, ws >>. preturn "", precedence, true, f)
        
        opp.AddOperator operator

    addPrefix "+"   2000 (fun expr -> Expr.UnaryOp (UnOp.Plus, expr))
    addPrefix "-"   2000 (fun expr -> Expr.UnaryOp (UnOp.Minus, expr))
    addPrefix "not" 2000 (fun expr -> Expr.UnaryOp (UnOp.Not, expr))
    addPrefix ".."  2000 (fun expr -> Expr.RightOpenRange expr)
    
    let right_open_range =
        
        let after : Parser<string> =
            pstring "."
            .>> ws
            
        let op =
            PostfixOperator(".", after, 1, false, Expr.RightOpenRange)
                    
        opp.AddOperator op   
            
    // <annotations>
    // eg: `:: output :: x(2)`
    let annotations : Parser<Annotations> =
        many (annotation .>> ws)
    
    // <annotations>
    // eg: `:: output :: x(2)`
    let ann_capture : Parser<Ident> =
        skip "ann"
        >>. skip ':'
        >>. ident
    
    // A TypeInst with a name
    // <ti-expr-and-id>
    // eg: `var int: x`
    let ti_expr_and_id : Parser<TypeInst> =
        fun stream ->
            let reply = ti_expr stream
            if reply.Status <> Ok then
                Reply(reply.Status, reply.Error)
            else
                stream.SkipWhitespace()
                stream.SkipWs(':')
                match ident stream with
                | r when r.Status = Ok ->
                    let ti = {reply.Result with Name = r.Result}
                    Reply(ti)
                | r ->
                    Reply(r.Status, r.Error)
        
    // A TypeInst with an optional name
    // eg: function(int, a:bool)                
    let parameter_ti : Parser<TypeInst> =
        pipe(
            ti_expr,
            opt_or "" (skip ':' >>. ident),
            annotations,
            fun ti id anns ->
                { ti with
                    Name = id
                    Annotations = anns }
        )
        
    let assign_tail : Parser<Expr> =
        let error = expected "=[=] expr"
        fun stream ->
            if stream.Skip('=') then
                stream.Skip('=')
                stream.SkipWhitespace()
                expr stream
            else
                Reply(ReplyStatus.Error, error)
       
    // eg: `(int: a, var bool: b)`        
    let parameters : Parser<TypeInst list> =
        parameter_ti
        |> commaSep
        |> betweenWs('(', ')')
        <!> "parameters"
        
    // eg: `(1, 2, "abc")`
    let tupled_args : Parser<Expr list> =
        expr
        |> commaSep
        |> betweenWs('(', ')')
        <!> "tupled-args"
        
    // <predicate-item>
    // eg: `predicate isOk(int x) = x > 2`
    let predicate_item : Parser<Item> =
        pipe(
            ident,
            parameters,
            opt_or "" ann_capture,
            annotations,
            opt assign_tail,
            fun id pars ann anns body ->
                { Name = id
                ; Parameters = pars
                ; Ann = ann
                ; Annotations = anns
                ; Returns =
                    { TypeInst.Empty
                        with
                            Type = Type.Bool
                            IsVar = true  }
                ; Body = body }
                |> Item.Function
                )
        <!> "predicate"

    // <test_item>
    let test_item : Parser<Item> =
        pipe(
            ident,
            parameters,
            annotations,
            opt assign_tail,
            fun id pars anns body ->
                { Name = id
                ; Parameters = pars
                ; Annotations = anns
                ; Body = body }
                |> Item.Test
                )
        <!> "test"
        
    // <function-item>
    (*
    TODO - combine this with variable parsing because the optional function keyword is making it hard
    *)
    let function_item : Parser<Item> =
        pipe(
            ti_expr_and_id,
            parameters,
            opt_or "" ann_capture,
            annotations,
            opt assign_tail,
            (fun ti pars ann anns body ->
                { Name = ti.Name
                ; Returns = { ti with Name = ""}
                ; Ann = ann
                ; Annotations = anns 
                ; Parameters = pars
                ; Body = body }
                |> Item.Function
                )
        )
        <!> "function"
          
    // <enum-item>
    let enum_item : Parser<Item> =
        
        // <enum-case-list>    
        let enum_cases : Parser<EnumCases list> =
                    
            let enum_names : Parser<EnumCases> =
                ident
                |> commaSep
                |> betweenWs('{', '}')
                |>> EnumCases.Names

            let anon_enum : Parser<EnumCases> =
                skip "_"
                <|> skip "anon_enum"
                >>. betweenWs('(', ')') expr
                |>> EnumCases.Anon
                
            let enum_call : Parser<EnumCases> =
                ident
                .>>. betweenWs('(', ')') expr
                |>> EnumCases.Call
            
            let enum_case =
                [ enum_names; anon_enum; enum_call ]
                |> choice
                .>> ws
                
            sepBy enum_case (skip "++")
        
        pipe(
            ident,
            annotations,
            (opt_or [] (skip '=' >>. enum_cases)),
            fun name anns cases ->
                { Name = name
                ; Annotations = anns
                ; Cases = cases }
                |> Item.Enum)
    
    // <include-item>
    let include_item : Parser<Item> =
        string_lit
        |>> (IncludeItem.Create >> Item.Include)
           
    // <base-ti-expr>
    let base_ti_expr_atom : Parser<TypeInst> =
        
        let is_var =
            choice
                [ Keyword.VAR ==> true
                ; Keyword.PAR ==> false
                ; preturn false ]
        
        let is_opt =
            Keyword.OPT ==> true
            <|> preturn false
    
        let is_set =
            skip1 Keyword.SET
            >>. skip Keyword.OF
            >>% true
            <|> preturn false
        
        pipe(
            is_var,
            is_set,
            is_opt,
            base_ti_expr_tail,
            fun inst set opt typ ->
                { Type = typ
                ; Name = ""
                ; IsOptional = opt
                ; IsSet = set
                ; IsArray = false
                ; Annotations = []
                ; PostFix = [] 
                ; IsVar = inst
                ; Value = None
                ; IsInstanced = false }
        )
        <!> "base-ti"
        
    let generic_ident =
        let options = IdentifierOptions(isAsciiIdStart = fun c -> c = '$' || Char.IsLetter c )
        skipChar '$'
        >>. identifier options
    
    let base_ti_expr : Parser<TypeInst> =
                          
        let any_ti : Parser<TypeInst> =
            let error = expected "ti-variable: eg $$x"
            skip1 "any"
            >>. generic_ident
            |>> (fun name -> { TypeInst.Empty with Type = Type.Generic name  })

        // Combine the given TypeInsts 
        let foldTypeInsts (a: TypeInst) _ (b: TypeInst) =
              
            let ty =
                match a.Type, b.Type with
              
                | Type.Record ra, Type.Record rb when a.IsSingleton && b.IsSingleton ->
                    Type.Record (ra @ rb)
                  
                | Type.Tuple ta, Type.Tuple tb when a.IsSingleton && b.IsSingleton ->
                    Type.Tuple (ta @ tb)
                  
                // TODO - handle somehow
                | Type.Concat xs, _ ->
                    Type.Concat (xs @ [b])
                  
                | _, _ ->
                    Type.Concat [a; b]
                  
            let ti = { a with Type = ty }
            let ti = resolveTypeInst ti
            ti
            
        any_ti
        <|>
        Inline.SepBy(
          stateFromFirstElement = id,
          foldState = foldTypeInsts,
          resultFromState = id,
          elementParser = (base_ti_expr_atom .>> ws),
          separatorParser = skip "++",
          separatorMayEndSequence = true
        )
        
    // <array-ti-expr>        
    let array_ti_expr : Parser<TypeInst> =
        
        let list_ti : Parser<TypeInst> =
            fun stream ->
                let error = expected "list-ti"
                if stream.Skip("list ") then
                    stream.SkipWhitespace()
                    if stream.Skip("of") then
                        stream.SkipWhitespace()
                        base_ti_expr stream
                    else
                        Reply(ReplyStatus.Error, error)
                else
                    Reply(ReplyStatus.Error, error)
            
        let array_dim_expr =
            ti_expr |>> (fun ti -> ti.Type)
                    
        let array_dims =
            array_dim_expr
            |> commaSep
            |> betweenWs ('[', ']')
            <!> "array-dims"
        
        let array_ti =
            pipe(
                skip Keyword.ARRAY,
                array_dims,
                skip Keyword.OF,
                base_ti_expr,
                fun _ dims _ ti ->
                    (dims, resolveTypeInst ti))
            >>= (fun (dims, ti) ->
                    let arr ty =
                        preturn
                            { TypeInst.Empty with
                                IsArray = true
                                IsVar = ti.IsVar
                                Type = ty }
                            
                    match dims with
     
                    | [i] ->
                        arr <| Type.Array1D (i, ti)
                    | [i;j] ->
                        arr <| Type.Array2D (i, j, ti)
                    | [i;j;k]->
                        arr <| Type.Array3D (i, j, k, ti)
                    | [i;j;k;l]->
                        arr <| Type.Array4D (i, j, k, l, ti)
                    | [i;j;k;l;m] ->
                        arr <| Type.Array5D (i, j, k, l, m, ti)
                    | [i;j;k;l;m;n] ->
                        arr <| Type.Array6D (i, j, k, l, m, n, ti)
                    | xs ->
                        fail $"Number of array dimension must be between 1 and 6 (got {xs.Length})."
            )
        list_ti
        <|> array_ti
        <!> "array-ti"
    
    // <ti-expr>        
    ti_expr_ref.contents <-
        [ array_ti_expr
        ; base_ti_expr  ]
        |> choice
        |> withContext Context.TypeInst        
        <!> "ti"

    // <tuple-ti-expr-tail>
    let tuple_ti : Parser<Type> =
        ti_expr
        |> commaSep1
        |> betweenWs ('(', ')')
        |>> Type.Tuple
        <!> "tuple-ti"
            
    // <record-ti-expr-tail>
    let record_ti : Parser<Type> =
        ti_expr_and_id
        |> commaSep1
        |> betweenWs('(', ')')
        |>> Type.Record
        <!> "record-ti"
        
    let left_open_range =
        skip ".."
        >>. expr
        |>> Expr.LeftOpenRange
            
    // <base-ti-expr-tail>
    base_ti_expr_tail_ref.contents <-
        
        // TypeInst declared with an expression
        // eg: ```1..(1+3)```
        let expr_ti : Parser<Type> =
            fun stream ->
                let reply = expr stream
                if reply.Status = Ok then
                    Reply(Type.Expr reply.Result)
                else
                    Reply(reply.Status, reply.Error)
            
        let generic =
            generic_ident
            |>> Type.Generic
            
        fun stream ->
            let stateTag = stream.StateTag
            let context = stream.UserState.Context
            
            let reply = ident_kw stream
            let struct(id, keyword) = reply.Result
                        
            if reply.Status = Ok then
                match keyword with 
                | Keyword.INT ->
                    Reply(Type.Int)
                | Keyword.BOOL ->
                    Reply(Type.Bool)
                | Keyword.FLOAT ->
                    Reply(Type.Float)
                | Keyword.STRING ->
                    Reply(Type.String)
                | Keyword.ANN ->
                    Reply(Type.Ann)
                | Keyword.ANNOTATION ->
                    Reply(Type.Annotation)
                | Keyword.RECORD ->
                    record_ti stream
                | Keyword.TUPLE ->
                    tuple_ti stream
                | _ ->
                    // Must be an expression (eg: `Foo`, `{1,2,3}`)
                    stream.Seek(stream.Index - (int64)id.Length)
                    expr_ti stream
                      
            elif reply.Status = Error && stateTag = stream.StateTag then
              match stream.Peek() with
              | '$' -> generic stream 
              | _ -> expr_ti stream
                  
            else 
              Reply(reply.Status, reply.Error)
    
        //
        // |>> (fun (name, args) ->
        //     // Handle some special known cases of function application
        //     match name, args with
        //     | (Id "array1d"), [i; Expr.Array1DLit arr] ->
        //         Expr.Array1D (i, arr)
        //     | (Id "array2d"), [i; j; Expr.Array1DLit arr] ->
        //         Expr.Array2D (i, j, arr)
        //     | (Id "array3d"), [i; j; k; Expr.Array1DLit arr] ->
        //         Expr.Array3D (i, j, k, arr)
        //     | (Id "array4d"), [i; j; k; l; Expr.Array1DLit arr] ->
        //         Expr.Array4D (i, j, k, l, arr)
        //     | (Id "array5d"), [i; j; k; l; m; Expr.Array1DLit arr] ->
        //         Expr.Array5D (i, j, k, l, m, arr)
        //     | (Id "array6d"), [i; j; k;l;m;n; Expr.Array1DLit arr] ->
        //         Expr.Array6D (i, j, k, l, m, n, arr)
        //     | _, _ ->
        //         Expr.Call(name,args)
        // )
        // <!> "call-expr"
        
    let wildcard : Parser<WildCard> =
        skipChar '_'
        >>. notFollowedBy letter
        >>% WildCard.WildCard
    
    let absent : Parser<Absent> =
        attempt (skip "<>")
        >>% Absent
        
    /// Generators
    /// Used in generator calls and list or set comprehensions
    /// eg: ```sum(i in 1..3 where i < 2)(i)```
    let generators : Parser<Generator list> =
                
        let gen_var =
            [ wildcard |>> IdentOr.Other 
            ; ident    |>> IdentOr.Ident ]
            |> choice
            <!> "gen-var"
            
        let gen_vars =
            gen_var
            |> commaSep1            
            <!> "gen-vars"
            
        let gen_where =
            skip "where "
            >>. expr
            <!> "gen-where"
            
        let generator =    
            pipe(
                gen_vars,
                skip Keyword.IN,
                expr,
                opt gen_where,
                (fun idents _ source filter ->
                    { Yields = idents
                    ; From = source
                    ; Where = filter })
            )
            <!> "generator"
            
        generator
        |> commaSep1
    
    // <declare-item>
    let declare_item : Parser<Item> =
        
        let declare_any : Parser<Item> =
            skip "any"
            >>. skip ':'
            >>. pipe(
                ident,
                annotations,
                assign_tail,
                fun name anns expr ->
                    Item.Declare
                     { TypeInst.Empty with
                        Type = Type.Any
                        Name = name
                        Annotations = anns
                        Value = Some expr }
                )
            <!> "declare-any"        

        declare_any
        <|>
        pipe(
            ti_expr_and_id,
            opt parameters,
            annotations,
            opt assign_tail,
            annotations,
            fun ti args ti_anns expr expr_anns ->
                let ti = resolveTypeInst ti
                match args with
                // No arguments, not a function
                | None ->
                    Item.Declare
                        { ti with
                            Annotations = ti_anns
                            PostFix = expr_anns 
                            Value=expr }
                // Arguments, must be a function                    
                | Some args ->
                    Item.Function
                        { Name = ti.Name
                        ; Returns = {ti with Name = ""}
                        ; Ann = ""
                        ; Annotations = ti_anns
                        ; Parameters = args
                        ; Body = expr}
        )
        <!> "declare-item"

    let constraint_tail : Parser<ConstraintExpr> =
        pipe(
            expr,
            annotations,
            fun expr anns ->
              { Expr = expr
              ; Annotations = anns }
        )
        
    // <constraint-item>
    let constraint_item =
        constraint_tail |>> Item.Constraint
       
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
            skip Keyword.CONSTRAINT
            >>. constraint_tail
            |>> Choice2Of2
            
        // Let constraint must go first            
        let_constraint <|> let_declare
    
    // <let-expr>
    let let_expr_tail : Parser<LetExpr> =
                
        let item =
            let_item .>>> opt (anyOf ";,")
            
        let items =
            many1 (item .>> ws)
            |> betweenWs('{', '}')
        
        ws >>. pipe(items, skip "in", expr,
            fun items _b body ->
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
                )

        |> withContext Context.LetExpr        
        <!> "let-expr"
            
    // <if-then-else-expr>
    let if_else_expr_tail : Parser<IfThenElseExpr> =
        
        let else_case =
            skip Keyword.ELSE
            >>. expr
            
        let elseif_case =
            skip Keyword.ELSEIF
            >>. expr
            
        let then_case =
            skip Keyword.THEN
            >>. expr
            
        let elseif_then_case =
            tuple(elseif_case, then_case)
            
        let elseif_cases =
            many elseif_then_case
        
        fun stream ->
            
            let mutable ite =
                { If = Unchecked.defaultof<Expr>
                  Then = Unchecked.defaultof<Expr>
                  ElseIf = []
                  Else = None }
                        
            stream.SkipWhitespace()
            let mutable reply =
                Reply(ReplyStatus.Error, ite, NoErrorMessages)
            
            // If-Case
            let ifReply = expr stream
            if ifReply.Status = Ok then
                stream.SkipWhitespace()
                ite <- { ite with If = ifReply.Result }
                
                // Then-case
                let thenReply = then_case stream
                if thenReply.Status = Ok then
                    stream.SkipWhitespace()
                    ite <- { ite with Then = thenReply.Result }
                    let stateTag = stream.StateTag
                    
                    // ElseIf-Cases
                    let elseIfReply = elseif_cases stream
                    if (elseIfReply.Status = Ok || stateTag = stream.StateTag) then
                        if elseIfReply.Status = Ok then
                            stream.SkipWhitespace()
                            ite <- { ite with ElseIf =  elseIfReply.Result }
                        let stateTag = stream.StateTag
                        
                        // Else-Case
                        let elseReply = else_case stream
                        if (elseReply.Status = Ok || stream.StateTag = stateTag) then
                            if elseReply.Status = Ok then
                                stream.SkipWhitespace()
                                ite <- {ite with Else = Some elseReply.Result }
                            if stream.Skip("endif") then
                                reply.Status <- Ok
                                reply.Result <- ite
                            else
                                reply.Status <- ReplyStatus.Error
                                reply.Error <- expected "endif"
                        else
                            reply.Status <- elseIfReply.Status
                            reply.Error <- elseIfReply.Error
                    else
                        reply.Status <- elseIfReply.Status
                        reply.Error <- elseIfReply.Error
                else
                    reply.Status <- thenReply.Status
                    reply.Error <- thenReply.Error
            else
                reply.Status <- ifReply.Status
                reply.Error <- ifReply.Error
            
            reply
    
    let tuple_access_tail =
        attempt (skip '.' >>. puint8)
        <!> "tuple-access"
        
    let record_access_tail =
        attempt (skip '.' >>. ident)
        <!> "record-access"
        
    let array_access_tail =

        let item =
            stringReturn ".." ValueNone
            <|>
            (expr |>> ValueSome)
            
        item            
        |> commaSep1
        |> betweenWs ('[', ']')
   
    // <expr-atom-tail>
    let rec expr_atom_tail expr : Parser<Expr> =
        
        let tup =
            tuple_access_tail
            |>> fun field -> Expr.TupleAccess(expr, field)
            
        let record =
            record_access_tail
            |>> fun field -> Expr.RecordAccess(expr, field)
            
        let array_access =   
           array_access_tail
           |>> fun slice -> Expr.ArrayAccess(expr, slice)
            
        let tail =
            choice [ tup; record; array_access ]

        (tail >>== expr_atom_tail)
        <|>
        preturn expr
    
    [<Struct>]
    type private BracketState =
        | Empty
        | Expr of expr: Expr
        | Tuple of tuple: ResizeArray<Expr>
        | Record of record: ResizeArray<struct(string * Expr)>
        
    let expr_in_brackets : Parser<Expr> =
                
        fun stream ->
            stream.SkipWs('(')
            let state = BracketState.Empty
            let mutable error = expected "Bracketed expr, tuple, or record"
            let mutable comma = false
            let rec loop state =
                match stream.Peek() with
                | ')' ->
                    stream.Skip()
                    match state with
                    | Expr expr when comma ->
                        Reply(Expr.Tuple [expr])
                    | Expr expr ->
                        Reply(Expr.Bracketed expr)
                    | Empty ->
                        Reply(ReplyStatus.Error, error)
                    | Record fields ->
                        fields
                        |> (List.ofSeq >> Expr.Record >> Reply)
                    | Tuple fields ->
                        fields
                        |> (List.ofSeq >> Expr.Tuple >> Reply)
                
                | ',' ->
                    comma <- true
                    stream.SkipWs(1)
                    loop state
                    
                | _ ->
                    let reply = expr stream
                    if reply.Status <> Ok then
                        Reply(reply.Status, reply.Error)
                    else
                        stream.SkipWhitespace()
                        comma <- false
                        match reply.Result with
                        
                        // Record items eg: `a:2`
                        | Expr.Ident id when stream.Peek() = ':' ->
                            stream.SkipWs(1)
                            match expr stream with
                            | r when r.Status = Ok ->
                                let value = r.Result
                                stream.SkipWhitespace()
                                match state with
                                | Empty ->
                                    let fields = ResizeArray<struct (string*Expr)>(4)
                                    fields.Add(struct(id, value))
                                    loop (BracketState.Record fields)
                                | Record xs ->
                                    xs.Add(struct(id, value))
                                    loop (BracketState.Record xs)
                                | _ ->
                                    Reply(ReplyStatus.Error, error)
                            | r ->
                                Reply(r.Status, r.Error)
                                
                        | expr ->
                            match state with
                            | Empty ->
                                loop (Expr expr)
                            | Tuple fields ->
                                fields.Add(expr)
                                loop (Tuple fields)
                            | Expr first ->
                                let fields = ResizeArray<Expr>(4)
                                fields.Add(first)
                                fields.Add(expr)
                                loop (Tuple fields)
                            | _ ->
                                Reply(ReplyStatus.Error, error)
                                
            let reply = loop Empty
            reply
                
    type [<Struct>] Array3DState =
        | Begin
        | End
        | ParseElement of optional:bool
        | Element of expr:Expr
        | NoElement
        
    let bool_lit =
        stringReturn "true" true
        <|>
        stringReturn "false" false
        |>> Expr.Bool
        
    let int_lit =
        pint32
        |>> Expr.Int
        
    let float_lit =
        number_lit
                
    let array3d_lit : Parser<Expr[,,]> =
        
        let endElementError =
            messageError "array elements must be followed by ',', '|', '|,' or '| |]"
        
        let expectedArrayElement =
            expected "Array element expr"
            
        let expectedEndOfArray =
            expected "|]"
            
        let rowEndError =
            expected "Array1D row end: | or ','"
            
        let expectedMatrixSep =
            expected "Array2D separator: |"
            
        let expectedElementOrEndOfArray =
            expected "Array element or end of array '| |]'"
        
        fun stream ->
            
            let mutable i = 0 // Current first index
            let mutable j = 0 // Current second index
            let mutable k = 0 // Current third index
            let mutable n = 0 // Total element count
                                                                                    
            let mutable errors = NoErrorMessages
            let mutable rowContinue = false
            let mutable fin = false
            let mutable elements = List<Expr>()
                                                                                                                        
            while not fin do
                let tag = stream.StateTag
                let exprReply = expr stream
                // Successful element parse
                if exprReply.Status = Ok then
                    stream.SkipWhitespace()
                    elements.Add exprReply.Result
                    k <- k + 1
                    n <- n + 1
                                                                                
                    let comma = stream.SkipWs(',')
                    let pipe = stream.SkipWs('|')
                                                                    
                    // End of current 1D array                        
                    if pipe then
                        rowContinue <- false
                                                                                                                                                                                                                  
                        // Start of new 2D array
                        if stream.SkipWs(',') then
                            if stream.SkipWs('|') then
                                k <- 0
                                i <- i + 1
                                j <- 0
                            else
                                errors <- expectedMatrixSep
                        
                        // End of the 3d array
                        elif stream.Skip("|]") then
                            fin <- true
                            
                        // Start of new row
                        else
                            k <- 0
                            j <- j + 1
                            
                    // Continue 1D array
                    elif comma then
                        rowContinue <- true
                    
                    // Error    
                    else
                        errors <- rowEndError
                        fin <- true

                // Element parse failed and changed state                        
                elif tag <> stream.StateTag then
                    errors <- exprReply.Error
                    fin <- true
                    
                // Element parse failed when one was expected
                elif rowContinue then
                    errors <- expectedArrayElement
                    fin <- true

                // No element but it could be an empty array                
                elif n = 0 then
                    if stream.SkipWs('|') && stream.Skip("|]") then
                        fin <- true
                    else
                        fin <- true
                        errors <- expectedElementOrEndOfArray
                    
                // No element - must be end
                elif stream.Skip("|]") then
                    fin <- true
                    
                // Error
                else
                    errors <- expectedEndOfArray
                    fin <- true
            
            if errors <> NoErrorMessages then
                Reply(ReplyStatus.Error, errors)
            elif n = 0 then
                Reply(Array3D.zeroCreate 1 0 0)
            else
                let I = i + 1
                let J = j + 1 
                let K = k
                let N = I * J * K
                if N <> n then
                    let msg = $"Array[{I},{J},{K}] should have {N} elements but parsed {n}"
                    Reply(ReplyStatus.Error, messageError msg)
                else
                    let array = Array3D.zeroCreate I J K
                    let mutable z = 0
                    for i in 0 .. (I-1) do
                        for j in 0 .. (J-1) do
                            for k in 0 .. (K-1) do
                                array[i,j,k] <- elements[z]
                                z <- z + 1
                    Reply(array)
    
    type Array1DState =
        | ParseElement 
        | RetryElement
        | Continue
    
    // Parse a 1D array literal using the provided element parser
    // as the first priority.  Internally this uses an
    // (element <|> expr) to still allow complex expressions 
    let private array1d_lit_of (element: ValueOption<Parser<Expr>>): Parser<List<Expr>> =
        let delimError = expected ", or ]"
        let elemError = expected "array element"
        let pElement, pBackup, retry =
            match element with
            | ValueSome p ->
                p, expr, true
            | ValueNone _ ->
                expr, Unchecked.defaultof<Parser<Expr>>, false

        fun stream ->
            let mutable elements = List<Expr>()
            let rec loop (state: Array1DState) =
                match state with
                // Attempt with the initial parser (eg: int, float)
                | ParseElement ->
                    let tag = stream.StateTag
                    let idx = stream.Index                        
                    let reply = pElement stream
                    // Successful element parse
                    if reply.Status = Ok then
                        stream.SkipWhitespace()
                        // Check ahead in case the element on matched partially
                        // for example it could succeed but be part of a binary
                        // operation.
                        let c = stream.Peek()
                        if not retry then
                            elements.Add reply.Result
                            loop Continue
                        elif c = ',' then
                            elements.Add reply.Result
                            loop Continue
                        elif c = ']' then
                            elements.Add reply.Result
                            loop Continue
                        else
                            stream.Seek(idx)
                            loop RetryElement
                        
                    // Failed and changed state
                    elif stream.StateTag <> tag then
                        reply.Error
                    // Parser failed but we can retry
                    elif retry then
                        loop RetryElement
                    // Parser failed no retry                                            
                    else
                        reply.Error
                        
                // Retry with the full expression parser if required
                | RetryElement ->
                    let peek = stream.Peek2()
                    match peek.Char0, peek.Char1 with
                    // Check for wildcard 
                    | '_', c when not (Char.IsLetter c) ->
                        elements.Add (Expr.WildCard WildCard)
                        stream.SkipWs(1)
                        loop Continue
                    // Check for absent 
                    | '<', '>' ->
                        elements.Add (Expr.Absent Absent)
                        stream.SkipWs(2)
                        loop Continue
                    | _, _ ->
                        let reply = pBackup stream
                        // Retry succeded
                        if reply.Status = Ok then
                            elements.Add reply.Result
                            loop Continue
                        // Retry failed
                        else
                            reply.Error

                // Check for end or separator                    
                | Continue ->
                    if stream.SkipWs(',') then
                        if stream.Skip(']') then
                            NoErrorMessages
                        else
                            loop ParseElement
                    elif stream.Skip(']') then
                        NoErrorMessages
                    else
                        elemError
            
            let error = loop Continue
            if error = NoErrorMessages then
                Reply(elements)
            else
                Reply(ReplyStatus.Error, error)
                
    // <array-literal-2d>
    let array2d_lit : Parser<Expr[,]> =
                                            
        let row : Parser<Expr list> =
            commaSep1 expr
        
        let sep : Parser<unit> =
            fun stream ->
                let error = expectedString "|"
                let next = stream.Peek2()
                if next.Char0 = '|' && next.Char1 <> ']' then
                    stream.SkipWs(1)
                    Reply(())
                else
                    Reply(ReplyStatus.Error, error)
        
        let rows : Parser<Expr list list> =
            sepEndBy row sep

        let uniformError =
            expected "2D array with uniform dimensions"
        
        let closeError =
            expected "|] or ]"
        
        fun stream ->
            let reply = rows stream
            if reply.Status = Ok then
                
                // Directly follows a trailing sep
                let closeOk =
                    if stream.Skip(']') then
                        true
                    else
                        stream.SkipWhitespace()
                        stream.Skip("|]")
                
                if closeOk then    
                    try
                        let array = array2D reply.Result
                        Reply(array)
                    with
                    | exn ->
                        Reply(ReplyStatus.Error, uniformError)
                else
                    Reply(ReplyStatus.Error, closeError)
            else
                Reply(reply.Status, reply.Error)
    
           
    let array_expr : Parser<Expr> =
            
        let error = expectedString "["
        
        let array_comp_tail =
            generators .>> ws .>> skipChar ']'
            
        let expectedArrayExpr =
            expected "array expression (literal or comp)"
            
        let array3d =            
            array3d_lit |>> Expr.Array3DLit 
        
        let array2d =            
            array2d_lit |>> Expr.Array2DLit
        
        fun stream ->
                        
            // Empty 1D array
            if stream.Skip('[') then
                
                // 2D or 3D array 
                if stream.SkipWs("|") then
                                    
                    // Empty 2D Array
                    if stream.Skip("|]") then
                        Reply(Expr.Array1DLit Array.empty)
                        
                    elif stream.SkipWs('|') then
                        array3d stream                        
                    else
                        array2d stream

                // Empty 1D Array
                elif stream.WsSkip(']') then 
                    Reply(Expr.Array1DLit Array.empty)
                                    
                // Non-empty 1D Array or Comprehension
                else
                    let reply = expr stream
                    if reply.Status = Ok then
                        stream.SkipWhitespace()
                        // Pipe indicates a comprehension
                        if stream.SkipWs('|') then
                            match array_comp_tail stream with
                            | r when r.Status = Ok ->
                                { Yields = reply.Result
                                ; IsSet = false
                                ; From = r.Result }
                                |> Expr.ArrayComp
                                |> Reply
                            | r ->
                                Reply(r.Status, r.Error)
                            
                        else
                            // Comma separated means array literal
                            let first = reply.Result
                            let element =
                                match first with
                                | Expr.Bool _ -> ValueSome bool_lit
                                | Expr.Int _ -> ValueSome int_lit
                                | Expr.Float _ -> ValueSome float_lit
                                | _ -> ValueNone
                            
                            match array1d_lit_of element stream with
                            | r when r.Status = Ok ->
                                let arr = r.Result
                                arr.Insert(0, reply.Result)
                                let set = 
                                    arr
                                    |> List.ofSeq
                                    |> Expr.Set
                                
                                Reply set
                            | r ->
                                Reply(r.Status, r.Error)

                    else
                        Reply(reply.Status, reply.Error)
            else
                Reply(ReplyStatus.Error, expectedArrayExpr)    
        
   
    // <expr-atom-head>    
    let expr_atom_head : Parser<Expr> =
            
        let wildcard =
            wildcard |>> Expr.WildCard
            
        let absent =             
            absent |>> Expr.Absent
        
        let string_lit =
            string_lit |>> Expr.String
            
        let wildcard =
            charReturn '_' (Expr.WildCard WildCard)
            
        let absent =
            stringReturn "<>" (Expr.Absent Absent)
            
        let id =
            ident_kw .>> ws
                    
        let set_expr : Parser<Expr> =
            
            let set_lit_tail =
                commaSep1 expr .>> skipChar '}'
                
            let set_comp_tail =
                generators .>> ws .>> skipChar '}'
            
            fun stream ->
                stream.SkipWs('{')

                // Empty set literal
                if stream.Skip('}') then
                    Reply(Expr.Set [])

                // Could be set lit or set comp                    
                else
                    let reply = expr stream
                    if reply.Status = Ok then
                        stream.SkipWhitespace()
                        // Comma separated means set literal
                        if stream.SkipWs(',') then
                            match set_lit_tail stream with
                            | r when r.Status = Ok ->
                                reply.Result :: r.Result
                                |> Expr.Set
                                |> Reply
                            | r ->
                                Reply(r.Status, r.Error)
                                
                        // Pipe indicated set comprehension
                        elif stream.SkipWs('|') then
                            match set_comp_tail stream with
                            | r when r.Status = Ok ->
                                { Yields = reply.Result
                                ; IsSet = true
                                ; From = r.Result }
                                |> Expr.SetComp
                                |> Reply
                            | r ->
                                Reply(r.Status, r.Error)
                            
                        // End of set literal
                        elif stream.SkipWs('}') then
                             Reply(Expr.Set[reply.Result])

                        else
                            let c = stream.Peek()
                            Reply(ReplyStatus.Error, unexpected (string c))
                             
                    else
                        Reply(reply.Status, reply.Error)

        let gen_call_tail =
            tuple(
                betweenWs('(', ')') generators,
                betweenWs('(', ')') expr
            )
            
        let let_expr =
            let_expr_tail |>> Expr.Let
        
        let if_else_expr =
            if_else_expr_tail |>> Expr.IfThenElse
                        
        fun stream ->
            let tag = stream.StateTag
            let reply = ident_kw stream
            if reply.Status = Ok then
                let struct(id, keyword) = reply.Result
                match keyword with
                
                | Keyword.LET ->
                    let_expr stream
                    
                | Keyword.IF ->
                    if_else_expr stream
                    
                | Keyword.TRUE ->
                    Reply(Expr.Bool true)
                    
                | Keyword.FALSE ->
                    Reply(Expr.Bool false)
                    
                | Keyword.NOT ->
                    stream.SkipWhitespace()
                    let reply = expr stream
                    if reply.Status <> Ok then
                        Reply(reply.Status, reply.Error)
                    else
                        Reply(Expr.UnaryOp (UnOp.Not, reply.Result))
                                    
                // <ident>, <call-expr>, or <gen-call>
                | Keyword.NONE ->
                    stream.SkipWhitespace()
                    if stream.Peek() = '(' then
                        let mutable state = CharStreamState(stream)
                        let reply = gen_call_tail stream
                        if reply.Status = Ok then
                            { Id = id
                            ; From = fst reply.Result
                            ; Yields = snd reply.Result }
                            |> Expr.GenCall
                            |> Reply
                        else
                            stream.BacktrackTo(&state)
                            match tupled_args stream with
                            | r when r.Status = Ok ->
                                Expr.Call (id, r.Result)
                                |> Reply
                            | r ->
                                Reply(r.Status, r.Error)
                    else
                        Reply(Expr.Ident id)
                    
                // TODO This is an error usually except when used as an annotation eg: `:: output`
                | word ->
                    Reply(Expr.Ident id)
                
            else if stream.StateTag = tag then
                match stream.Peek() with
                | '(' -> expr_in_brackets stream
                | '{' -> set_expr stream
                | '[' -> array_expr stream
                | '_' -> wildcard stream 
                | '<' -> absent stream
                | '"' -> string_lit stream
                | _other -> number_lit stream
                
            else
                Reply(reply.Status, reply.Error)
            
    // <annotations>
    annotation_ref.contents <-
        skip "::"
        >>. expr_atom
        <!> "annotation"

    expr_atom_ref.contents <-        
        expr_atom_head 
        >>== expr_atom_tail
        <!> "expr-atom"
        
    //         
    // let quoted_ident_or_binop =
    //     quoted_ident_or builtin_bin_op
    //     <!> "binop-or-id"
    //         
    // // <expr>
    // expr_ref.contents <-
    //     
    //     let binop_or_quoted_id =
    //         quoted_ident_or builtin_bin_op
    //                 
    //     let rec expr_binop_tail (left: Expr) (stream: CharStream<_>) =
    //         stream.SkipWhitespace()
    //         let mutable state = stream.State
    //         let stateTag = stream.StateTag
    //         let reply = binop_or_quoted_id stream
    //         if reply.Status = Ok then
    //             match reply.Result with
    //             
    //             (* ".." is not actually a 'binop' because it 
    //             can be used in three ways:
    //             `left..`
    //             `left..right`
    //             `..right`
    //             *)
    //             | Val BinOp.DotDot as op ->
    //                 stream.SkipWhitespace()
    //                 let stateTag = stream.StateTag
    //                 let reply = expr stream
    //                 if reply.Status = Ok then
    //                     let right = reply.Result
    //                     let binop = Expr.BinaryOp (left, op, right)
    //                     expr stream
    //                 elif stream.StateTag = stateTag then
    //                     Reply(Expr.RightOpenRange left)
    //                 else
    //                     Reply(reply.Status, reply.Error)
    //                 
    //             | operator ->
    //                 stream.SkipWhitespace()
    //                 let reply = expr stream
    //                 if reply.Status = Ok then
    //                     let right = reply.Result
    //                     let binop = Expr.BinaryOp (left, operator, right)
    //                     expr_binop_tail binop stream
    //                 else
    //                     Reply(reply.Status, reply.Error)
    //                     
    //         elif stream.StateTag <> stateTag then
    //             Reply(reply.Status, reply.Error)
    //             
    //         else
    //             Reply(left)
    //     
    //     (expr_atom <|> left_open_range)
    //     >>= expr_binop_tail
    //     <!> "expr"
            
    let solve_type : Parser<SolveMethod> =
        choice
          [ stringReturn "satisfy" SolveMethod.Satisfy
          ; stringReturn "minimize" SolveMethod.Minimize
          ; stringReturn "maximize" SolveMethod.Maximize ]
        
    // <solve-item>
    let solve_item : Parser<Item> =
        pipe(
            annotations,
            solve_type,
            opt expr,
            fun anns solveType obj ->
                match (solveType, obj) with
                | SolveMethod.Maximize, Some exp ->
                    SolveItem.Max (exp, anns)
                | SolveMethod.Minimize, Some exp ->
                    SolveItem.Min (exp, anns)
                | _ ->
                    SolveItem.Sat anns
                |> Item.Solve)
        
    // <type-inst-syn-item>
    let alias_item : Parser<Item> =
        pipe(
            ident,
            annotations,
            skip '=',
            ti_expr,
            fun id anns _ ti ->
                { Name = id
                ; Annotations = anns
                ; TypeInst = ti }
                |> Item.Synonym
                )
        
    // <output-item>
    let output_item : Parser<Item> =
        pipe(
            opt string_annotation,
            expr,
            fun ann expr ->
                { Expr = expr
                ; Annotation = ann }
                |> Item.Output)
                
    // <annotation_item>
    let ann_item : Parser<Item> =
        pipe(
            ident,
            opt_or [] parameters,
            opt assign_tail,
            fun name pars body ->
                { Name = name
                ; Body = body
                ; Params = pars }
                |> Item.Annotation)
        <!> "annotation-type"

    // <item>
    let item : Parser<Item> =
        let err = expected "item"
        let idkw = ident_kw .>> ws
        fun stream ->
            let mutable state = CharStreamState(stream)
            let tag = stream.StateTag
            let reply = idkw stream
            if reply.Status = Ok then
                let struct(name, word) = reply.Result
                match word with
                | Keyword.ENUM ->
                    enum_item stream
                | Keyword.CONSTRAINT ->
                    constraint_item stream
                | Keyword.INCLUDE ->
                    include_item stream
                | Keyword.SOLVE ->
                    solve_item stream
                | Keyword.TYPE ->
                    alias_item stream
                | Keyword.OUTPUT ->
                    output_item stream
                | Keyword.PREDICATE ->
                    predicate_item stream
                | Keyword.FUNCTION ->
                    function_item stream
                | Keyword.TEST ->
                    test_item stream
                | Keyword.NONE when stream.Peek() = '=' ->
                    let reply = assign_tail stream
                    if reply.Status = Ok then
                        Item.Assign (name, reply.Result)
                        |> Reply
                    else
                        Reply(reply.Status, reply.Error)
                | _other ->
                    stream.BacktrackTo(&state)
                    declare_item stream
  
            // Not an identifier, assume part of declare
            // eg: 1..10: x;
            else
              declare_item stream
                          
    let ast : Parser<Ast> =
        
        let sep = skip ';'

        ws
        >>. sepEndBy (item .>> ws) sep
        .>> eof
        
    let private assign_expr : Parser<NamedExpr> =
        let error = expected "Assignment (id = expr)"
        fun stream ->
            match ident stream with
            | r when r.Status = Ok ->
                stream.SkipWhitespace()
                let id = r.Result
                let peek = stream.Peek2()
                let c0 = peek.Char0
                let c1 = peek.Char1
                if c0 <> '=' then
                    Reply(ReplyStatus.Error, error)
                else
                    stream.Skip(if c1 = '=' then 2 else 1)
                    match expr stream with
                    | e when e.Status = Ok ->
                        stream.SkipWhitespace()
                        Reply(struct(id, e.Result))
                    | e ->
                        Reply(e.Status, e.Error)
                
            | r ->
                Reply(r.Status, r.Error)
        
        
    let data : Parser<NamedExpr list> =
        ws
        >>. sepEndBy assign_expr (skip ';')
        .>> ws 
        .>> eof
        
    [<Struct>]        
    type Statement =
        | Comment of c:string
        | Statement of s:string
        
    let line_comment : Parser<string> =
        skip '%' >>. restOfLine true
        
    let block_comment : Parser<string> =
        let pStart = attempt <| skip "/*"
        let pEnd = attempt <| skipString "*/"
        pStart
        >>. manyCharsTill anyChar pEnd
        
    let non_comment : Parser<string> =
        let pEnd =
            (skipChar '%')
            <|> (attempt (skipString "/*"))
            <|> eof
        
        many1CharsTill anyChar (followedBy pEnd)
        
    let statement =
        [ line_comment  |>> Comment
        ; block_comment |>> Comment
        ; string_lit |>> Statement
        ; non_comment   |>> Statement ] 
        |> choice
        
    let statements =
        ws
        >>. many (statement .>> ws)
        .>> eof
        
        
open Parsers

[<AutoOpen>]       
module Parse =
        
    // Parse a string with the given parser
    let parseWith (parser: Parser<'t>) (options: ParseOptions) (input: string) : Result<'t, ParseError> =
        
        let state = ParserState(options)
                        
        match runParserOnString parser state "" input with
                
        | ParserResult.Success (value, _state, _pos) ->
            Result.Ok value
            
        | ParserResult.Failure (msg, err, state) ->
            
            let err =
                { Message = msg
                ; Line = err.Position.Line
                ; Column = err.Position.Column
                ; Index = err.Position.Index
                ; Trace = state.DebugString }
                
            Result.Error err
    
    [<Struct>]
    type PPX =
        | Mzn
        | LineComment
        | BlockOpenCheck
        | BlockCloseCheck
        | Block
        | StringLit
        | StringEscape
        
    // Parse and remove comments from the given model string
    let parseComments (mzn: string) : string * string =
        let comments = StringBuilder()
        let source = StringBuilder()
        let chars = mzn.AsSpan()
        let mutable state = PPX.Mzn
        let mutable char = 'a'
        
        for c in chars do
            match state, c with
            
            | PPX.LineComment, '\n' ->
                source.AppendLine()
                state <- PPX.Mzn
                
            | PPX.LineComment, _ ->
                comments.Append c
                state <- PPX.LineComment
                
            | PPX.Mzn, '%' ->
                state <- PPX.LineComment
                
            | PPX.Mzn, '/' ->
                state <- PPX.BlockOpenCheck
                
            | PPX.BlockOpenCheck, '*' ->
                state <- PPX.Block
                
            | PPX.BlockOpenCheck, _ ->
                source.Append '/'
                source.Append c
                state <- PPX.Mzn
                ()
            
            | PPX.Block, '*' ->
                state <- PPX.BlockCloseCheck
                
            | PPX.BlockCloseCheck, '/' ->
                state <- PPX.Mzn
            
            | PPX.BlockCloseCheck, _ ->
                comments.Append '*'
                state <- PPX.Block
               
            | PPX.Block, _ ->
                comments.Append c
                state <- PPX.Block
                
            | PPX.Mzn, '"' ->
                source.Append '"'
                state <- PPX.StringLit
                
            | PPX.StringLit, '\\' ->
                source.Append '\\'
                state <- PPX.StringEscape
                
            | PPX.StringEscape, _ ->
                source.Append c
                state <- PPX.StringLit
                
            | PPX.StringLit, '"' ->
                source.Append '"'
                state <- PPX.Mzn
                
            | PPX.StringLit, _ ->
                source.Append c
                ()
                
            | PPX.Mzn, '\n' when char = '\n' ->
                ()
                
            | PPX.Mzn, _ ->
                source.Append c
                ()
    
            char <- c                
                

        let source = source.ToString().Trim()
        let comments = comments.ToString()
        source, comments
         
    /// Parse '.dzn' style model data from a string        
    let parseDataString (options: ParseOptions) (dzn: string) : Result<NamedExpr list, ParseError> =
                            
        let source, comments =
            parseComments dzn
            
        let result =
            parseWith data options source
            
        result            
            
    /// Parse a '.dzn' model data file            
    let parseDataFile (options: ParseOptions) (filepath: string) : Result<NamedExpr list, ParseError> =
        let dzn = File.ReadAllText filepath
        let data = parseDataString options dzn
        data