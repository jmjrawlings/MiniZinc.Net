namespace MiniZinc

open MiniZinc.Parse

[<AutoOpen>]
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
            
        static member ParseString (model: string) =
            Model.parseString model
            
        
    module Model =
        
        let empty =
            { Name = ""
            ; Inputs = Map.empty
            ; Outputs = Map.empty
            ; String = "" }
            
        let parseFile file =
            Parse.file Parsers.model file
            
        let parseString string =
            Parse.string Parsers.model string
                    
            