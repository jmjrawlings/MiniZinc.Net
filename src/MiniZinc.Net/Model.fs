namespace MiniZinc

open System.Text.RegularExpressions

module rec Model =

    open System
    open System.IO
    
    // Parameter Type
    type VarKind =
        | Par = 0
        | Var = 1
        
    // MiniZinc Type
    type MzType =
        | TInt
        | TBool
        | TString
        | TFloat
        | TEnum   of EnumType
        | TRecord of RecordType
        | TArray  of ArrayType
        | TRange  of Range
        | TSet    of SetType
        | TRef    of string
        
    type SetType =
        { Elements : MzType }
        
    type Range =
        { Lower : IntOrName
          Upper : IntOrName }
        
    type RecordType =
        { Name : string
          Fields : Map<string, MzType> }
        
    type ArrayType =
        { Dimensions: IntOrName list }
        
    type IntOrName =
        | Int of int
        | Name of string
          
    type EnumType =
        { Name : string
        ; Members : string list }
        
    type SetValue =
        { Expr : string }
        
    type ArrayValue =
        { Expr: string }
    
    // MiniZinc Value (Type Instantiation)
    type MzValue =
        | VInt       of int
        | VBool      of bool
        | VString    of string
        | VFloat     of float
        | VRange     of Range
        | VArray     of ArrayValue
        | VSet       of SetValue
        | VRef       of string
        
            
    // MiniZinc variable
    type Variable =
        { Kind  : VarKind
          Name  : string
          Type  : MzType
          Value : MzValue option }

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
        
        let ws = spaces
        
        // Parse at least 1 whitespace
        let ws1 = spaces1
        
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
        
        // Parse a `var`
        let var_type : P<MzType> =
            many1Chars (noneOf ";:=")
            <?!> "var-type"
            |>> MzType.TRef
        
        // Parse a constraint
        let constr : P<string> =
            pstring "constraint"
            >>. ws1
            >>. many1Chars (noneOf ";:=")
            <?!> "constraint"
            
        // Parse an range
        let range : P<Range> =
            
            let bound =
                (pint32 |>> IntOrName.Int)
                <|>
                (ident |>> IntOrName.Name)
           
            bound .>>> str ".." .>>>. bound
            |>> function | (lo,hi) -> { Lower=lo; Upper=hi }
      
        // Parse an enum
        let enum : P<EnumType> =
            let name = ident
            let members = between (chr '{') (chr '}') (sepBy ident (chr ','))
            str "enum"
            >>. ws1
            >>. name
            .>>> chr '='
            .>>>. members
            |>> (fun (name, members) ->
                { Name=name; Members=members })
        
        
        // Parse a `var`                    
        let var : P<Variable> =
            str "var"
            >>. ws1 >>. var_type .>> ws
            .>> chr ':' .>> ws
            .>>. ident
            |>> (fun (typ, name) ->
                { Name = name
                ; Type = typ
                ; Value = None
                ; Kind = VarKind.Var })
            <?!> "var"
        
        // Parse a parameter    
        let par : P<Variable> =
            str "par"     >>. ws1
            >>. var_type  .>> ws
            .>> pchar ':' .>> ws
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