namespace MiniZinc

open System.Text.RegularExpressions

module rec Model =

    open System
    open System.IO
    
    // Parameter Type
    type ParType =
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
        { ParType : ParType
          Name    : string
          Type    : MzType
          Value   : MzValue option }

        member this.IsVar =
            this.ParType = ParType.Var
            
        member this.IsPar =
            this.ParType = ParType.Par
        

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
        
        module Patterns =
            let var_type = @"(var|par)?"
            let type_name = @"([^;:]*[^;:])"
            let var_name = @"([a-zA-Z]\w*)"
            let assign = @"(=\s*([^;`]+))?"
            let var = $"{var_type}\s*{type_name}\s*:\s*{var_name}\s*{assign}\s*;"
        
        let var_regex = Regex Patterns.var
        
        // Parse a value from the given string
        let value (s: string) : MzValue option =
             MzValue.MzString s
             |> Some
             
        // Parse a value from the given string
        let var_type (s: string) : MzType option =
             MzType.MzAlias s
             |> Some
            
        // Parse a Var from the given line            
        let line (s: string) : Variable option =
            match var_regex.Match s with
            | m when m.Success ->
                
                let par_type =
                    match m.Groups[1].Value with
                    | "var" -> ParType.Var
                    | _ -> ParType.Par
                    
                let var_type =
                    m.Groups[2].Value
                    |> Parse.var_type
                    |> Option.get
                    
                let var_name =
                    m.Groups[3].Value
                    
                let value =
                    match m.Groups[4].Value with
                    | "" -> None
                    | v -> Parse.value s
                    
                let var =
                    { Name    = var_name
                    ; ParType = par_type
                    ; Type    = var_type
                    ; Value   = value }
                    
                Some var
            | _ ->
                None
            
        // Parse a model from a the given string                       
        let model (s: string) =
            
            let lines =
                s.Split(";")
                
            let inputs =
                lines
                |> Seq.map (sprintf "%s;")
                |> Seq.map (fun s -> s.Trim())
                |> Seq.choose Parse.line
                |> Seq.toList
                |> Map.withKey (fun v -> v.Name)
                
            { Model.empty with
                Inputs = inputs
                String = s }
                
                
        // Parse the given file
        let file (fi: FileInfo) : Model =
            let contents = File.ReadAllText fi.FullName
            let name = Path.GetFileNameWithoutExtension fi.Name
            let model = Parse.model contents
            let model = { model with Name = name }
            model