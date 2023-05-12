namespace MiniZinc

open MiniZinc.Parse

module rec Model =

    open System
    open System.IO
    open AST
    
    // A MiniZinc model
    type Model =
        { Name    : string 
        ; Inputs  : Map<string, TypeInst>
        ; Outputs : Map<string, TypeInst>
        ; String  : string }
        
        static member ParseFile (file: string) =
            Model.parseFile (FileInfo file)
            
        static member ParseFile (file: FileInfo) =
            Model.parseFile file
            
        static member Parse (model: string) =
            Model.parseString model
            
        
    module Model =
        
        let empty =
            { Name = ""
            ; Inputs = Map.empty
            ; Outputs = Map.empty
            ; String = "" }
            
        let parseFile file =
            Parse.file file
            empty
            
            
        let parseString model =
            Parse.model model
            empty
                    
            