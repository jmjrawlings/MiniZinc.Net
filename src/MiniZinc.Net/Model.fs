namespace MiniZinc

open System.Text.RegularExpressions

module rec Model =

    open System
    open System.IO
    
    // Parameter or variable?
    type VarType =
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
    type MzVar =
        { VarType : VarType
          Name    : string
          Type    : MzType          
          Value   : MzValue option }
        
        member this.IsVar =
            this.VarType = VarType.Var
            
        member this.IsPar =
            this.VarType = VarType.Par
        

    // A MiniZinc model
    type Model =
        { Name    : string 
        ; Inputs  : Map<string, MzVar>
        ; Outputs : Map<string, MzVar>
        ; String  : string }
        
        static member ParseFile (file: string) =
            let fi = FileInfo file
            Model.parseFile fi
       
        
    module Model =
        
        let empty =
            { Name = ""
            ; Inputs = Map.empty
            ; Outputs = Map.empty
            ; String = "" }
        
        let parseString (s: string) =
            
            // https://regex101.com/r/efIy7M/1
            
            let vartype_pat = @"(?:(par|var)\s*)?"
            let type_pat = @"([a-zA-Z][\w|\s]*)"
            let name_pat = @"([a-zA-Z]\w*)"
            let value_pat = @"(?:=\s*([^;`]+))"
            let var_pat = $"{vartype_pat}{type_pat}:\s*{name_pat}\s*{value_pat}\s*;"
            let var_regex = Regex var_pat
            empty
            
        let parseFile (fi: FileInfo) =
            let contents = File.ReadAllText fi.FullName
            let name = Path.GetFileNameWithoutExtension fi.Name
            let model = parseString contents
            let model = { model with Name = name }
            model