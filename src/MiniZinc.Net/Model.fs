namespace MiniZinc

module rec Model =

    open System
    open System.IO
    open Parse
    
    // A MiniZinc model
    type Model =
        { Name    : string 
        ; Inputs  : Map<string, DeclareItem>
        ; Outputs : Map<string, DeclareItem>
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
                    
            