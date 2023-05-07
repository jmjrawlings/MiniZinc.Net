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
        | MzInt
        | MzBool
        | MzString
        | MzEnum    of string list
        | MzRecord  of Map<string, MzType>
        | MzArray   of MzType
        | MzArray2d of MzType*MzType
        | MzArray3d of MzType*MzType*MzType
        | MzRange   of int * int
        | MzSet     of MzType
        | MzAlias   of string
        
    
    // MiniZinc Value
    type MzValue =
        | MzInt     of int
        | MzBool    of bool
        | MzString  of string
        | MzEnum    of string
        | MzRange   of int * int
        | MzArray   of MzValue[]
        | MzArray2d of MzValue[,]
        | MzArray3d of MzValue[,,]
        | MzVar     of string

            
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
            |>> MzType.MzAlias
        
        // Parse a constraint
        let constr : P<string> =
            pstring "constraint"
            >>. ws1
            >>. many1Chars (noneOf ";:=")
            <?!> "constraint"
        
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