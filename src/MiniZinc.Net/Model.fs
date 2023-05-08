namespace MiniZinc

open System.Text.RegularExpressions

module rec Model =

    open System
    open System.IO
    
    // Parameter Type
    type VarKind =
        | Par = 0
        | Var = 1
    
    type PrimitiveType =    
        | Int
        | Bool
        | String
        | Float
        | Ref of string
        
    // MiniZinc Type
    type VarType =
        | Primitive of PrimitiveType
        | Enum    of EnumType
        | Record  of RecordType
        | Array   of ArrayType
        | Set     of SetType
        
    type RangeExpr =
        { Lower : IntOrName
          Upper : IntOrName }
        
    type RecordType =
        { Fields : Map<string, VarType> }
        
    type SetType =
        | Unbounded of PrimitiveType
        | Range of RangeExpr
        | Literal of SetExpr
        
    type ArrayType =
        { Dimensions: IntOrName list
          Elements: PrimitiveType }
        
        member this.NDim =
            this.Dimensions.Length
        
    type IntOrName =
        | Int of int
        | Name of string
          
    type EnumType =
        { Name : string
        ; Members : string list }
        
    type SetExpr =
        | Primitive of PrimitiveExpr list
        | Expr of string
        
    type ArrayExpr =
        | Primitive of PrimitiveExpr[]
        | Expr of string
       
    type PrimitiveExpr =
        | Int    of int
        | Bool   of bool
        | String of string
        | Float  of float
        | Ref    of string
    
    // MiniZinc Value (Type Instantiation)
    type Value =
        | Primitive of PrimitiveExpr
        | Range     of RangeExpr
        | Array     of ArrayExpr
        | Set       of SetExpr
        | Other     of string
            
    // MiniZinc variable
    type Variable =
        { Kind  : VarKind
          Name  : string
          Type  : VarType
          Value : Value option }

        member this.IsVar =
            this.Kind = VarKind.Var
            
        member this.IsPar =
            this.Kind = VarKind.Par

        static member Var(name, typ) =
            { Kind = VarKind.Var
            ; Name = name
            ; Type = typ
            ; Value  = None }
            
        static member Par(name, typ) =
            { Kind = VarKind.Par
            ; Name = name
            ; Type = typ
            ; Value  = None }
            
        static member Par(name, typ, value) =
            { Kind = VarKind.Par
            ; Name = name
            ; Type = typ
            ; Value  = Some value }
        

    // A MiniZinc model
    type Model =
        { Name    : string 
        ; Inputs  : Map<string, Variable>
        ; Outputs : Map<string, Variable>
        ; String  : string }
        
        static member ParseFile (file: string) =
            Parse.file (FileInfo file)
            
        static member ParseFile (file: FileInfo) =
            Parse.file file
            
        static member Parse (model: string) =
            Parse.model model
            
        
    module Model =
        
        let empty =
            { Name = ""
            ; Inputs = Map.empty
            ; Outputs = Map.empty
            ; String = "" }
            
        let parseFile file =
            Parse.file file
            
        let parseString model =
            Parse.model model
      
    module Parse =
        open FParsec
        open FParsec.Primitives
        open FParsec.CharParsers
        open System.Text
        type DebugInfo = { Message: string; Indent: int }
        type UserState = { mutable Debug: DebugInfo }
        type Error = { Message: string; Trace: string; }
        type P<'t> = Parser<'t, UserState>
        type DebugType<'a> = Enter | Leave of Reply<'a>
        
        type Item =
            | Variable of Variable
            | Other of string
            | Constraint of string
        
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

        let str x = pstring x
        
        let chr c = pchar c
        
        // Parse an identifier
        let ident =
            let simple =
                many1Satisfy2
                    (fun c -> Char.IsLetter c || c = '_')
                    Char.IsLetterOrDigit

            let quoted =
                between
                    (pchar '\'')
                    (pchar '\'')
                    (many1Chars (noneOf ['\''; '\x0A'; '\x0D'; '\x00']))

            (simple <|> quoted)
            <?!> "identifier"
        
        // Parse a constraint
        let constr : P<string> =
            pstring "constraint"
            >>. spaces1
            >>. many1Chars (noneOf ";:=")
            <?!> "constraint"
            
        // Parse an range
        let range_expr : P<RangeExpr> =
            
            let bound =
                (pint32 |>> IntOrName.Int)
                <|>
                (ident |>> IntOrName.Name)
           
            attempt (bound .>>> str ".." .>>>. bound)
            |>> function | (lo,hi) -> { Lower=lo; Upper=hi }
      
        // Parse an enum
        let enum : P<EnumType> =
            let name = ident
            let members = between (chr '{') (chr '}') (sepBy ident (chr ','))
            str "enum"
            >>. spaces1
            >>. name
            .>>> chr '='
            .>>>. members
            |>> (fun (name, members) ->
                { Name=name; Members=members })

            
        let vint =
            pint32
            
        let vbool : P<bool> =
            (str "true" >>% true)
            <|>
            (str "false" >>% false)
            <?!> "vbool"
            
        let vfloat : P<float> =
            pfloat
            
        let vstr : P<string> =
            between
                (chr '"')
                (chr '"')
                (manySatisfy (fun c -> c <> '"'))
        
        let prim_expr : P<PrimitiveExpr> =
            [ vint    |>> Int
            ; vbool   |>> Bool
            ; vfloat  |>> Float
            ; vstr    |>> String ]
            |> choice
            <?!> "primitive-value"

        // Set of primitive values eg: {1,2,10}            
        let set_prim =
            let delim = spaces >>. chr ',' >>. spaces
            sepBy prim_expr delim
                
        let set_expr : P<SetExpr> =
            
            let prim =
                set_prim |>> SetExpr.Primitive
            
            let expr =
                manySatisfy (fun c -> c <> '}')
                |>> SetExpr.Expr

            between
                (chr '"' >>. spaces)
                (spaces >>. chr '"' )
                ((attempt prim) <|> expr)
                        
        let array_expr : P<ArrayExpr> =
            let prim =
                let delim = spaces >>. chr ',' >>. spaces
                sepBy prim_expr delim
            between
                (chr '[' >>. spaces)
                (spaces >>. chr ']')
                prim
            |>> List.toArray
            |>> ArrayExpr.Primitive
            
        let value : P<Value> =
            [ prim_expr  |>> Value.Primitive
            ; set_expr   |>> Value.Set
            ; array_expr |>> Value.Array 
            ; range_expr |>> Value.Range ]
            |> choice
            
        let prim_type : P<PrimitiveType> =
            [ str "bool"   >>% PrimitiveType.Bool
            ; str "int"    >>% PrimitiveType.Int
            ; str "string" >>% PrimitiveType.String
            ; str "float"  >>% PrimitiveType.Float
            ; ident |>> PrimitiveType.Ref ]
            |> choice
            <?!> "primitive-type"
            
        let set_type : P<SetType> =
            
            let unbounded =
                str "set" >>. spaces1 >>. str "of" >>. spaces1 >>. prim_type
                |>> SetType.Unbounded
                
            let range =
                range_expr
                |>> SetType.Range
                
            let literal =
                set_expr
                |>> SetType.Literal
            
            [ unbounded; range; literal ]
            |> choice
            <?!> "set-type"
            
        type ArrayIndex =
            | Set of SetType
            | Ref of string
            
        type ArrayElementType =
            | Set of SetType
            | Ref of string
            
            
            
        let array_type : P<ArrayType> =
            
            let index =
                (attempt set_type) |>> ArrayIndex.Set
                <|>
                (ident |>> ArrayIndex.Ref)
                 
            let dims =
                between
                    (chr '[' >>. spaces)
                    (spaces >>. chr ']')
                    (sepBy1 index (spaced chr ','))
            
            pipe4
                (str "array" >>. spaces1)
                dims
                (spaced1 "of")
                
                
                
                
                
        (fun _ dims ->
            match dims with
            | [] -> fail "array must have at least one dimension"
            | _ -> TArray (TInt, Some dims)) // Replace TInt with the actual base type parser when available

            
                
                
            
        let var_type : P<VarType> =
            [ prim_type   |>> VarType.Primitive
            ; set_type    |>> VarType.Set
            ; array_type  |>> VarType.Array ]
            |> choice
            
            
        
        // Parse a `var`                    
        let var : P<Variable> =
            str "var" >>. spaces1
            >>. var_type .>> spaces
            .>> chr ':' .>> spaces
            .>>. ident
            |>> (fun (typ, name) ->
                { Name = name
                ; Type = typ
                ; Value = None
                ; Kind = VarKind.Var })
            <?!> "var"
        
        // Parse a parameter    
        let par : P<Variable> =
            opt (str "par" .>> spaces1)
            >>. mz_type  .>> spaces
            .>> chr ':' .>> spaces
            .>>. ident
            |>> (fun (typ, name) ->
                { Name = name
                ; Type = typ
                ; Value = None
                ; Kind = VarKind.Par })
            <?!> "par"

        let lines =
            let other =
                many1Chars (noneOf [';'])
                <?!> "other"
            let choices =
                [ var    |>> Item.Variable
                ; constr |>> Item.Constraint
                ; other  |>> Item.Other ]
                |> choice
            sepEndBy choices (chr ';')
            .>> eof
                    
        // Parse the given string with the given parser            
        let parse parser (s: string) =
            let input = s.Trim()
            let state = { Debug = { Message = ""; Indent = 0 } }
            match runParserOnString parser state "" input with
            | Success (value, _, _) ->
                Result.Ok value
            | Failure (s, msg, state) ->
                let error =
                    { Message = s
                    ; Trace = state.Debug.Message }
                Result.Error error
                        
        // Parse a model from a the given string                       
        let model (s: string) =
            let input =
                s.Split(';')
                |> Seq.map (fun s -> s.Trim())
                |> String.concat ";"
            let result = parse lines input
            { Model.empty with String = s }
                 
        // Parse the given file
        let file (fi: FileInfo) =
            let contents = File.ReadAllText fi.FullName
            let name = Path.GetFileNameWithoutExtension fi.Name
            let model = Parse.model contents
            let model = { model with Name = name }
            model