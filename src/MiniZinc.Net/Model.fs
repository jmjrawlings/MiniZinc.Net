namespace MiniZinc

open MiniZinc.Parse
open System.Runtime.InteropServices

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
        
        /// <summary>
        /// Parse a Model from the given file
        /// </summary>
        static member ParseFile (file: string) =
            Model.parseFile (FileInfo file)
        
        /// <summary>
        /// Parse a Model from the given file
        /// </summary>    
        static member ParseFile (file: FileInfo) =
            Model.parseFile file

        /// <summary>
        /// Parse a Model from the given string
        /// </summary>
        /// <param name="input"></param>
        /// MiniZinc model as a string
        /// <param name="allowEmpty">
        /// If true, an empty model will be returned
        /// in the case of an empty/null string.
        ///
        /// Defaults is false.
        /// </param>
        static member ParseString (input: string, [<Optional; DefaultParameterValue(false)>] allowEmpty: bool) =
            if String.IsNullOrWhiteSpace input then
                if allowEmpty then
                    []
                else
                    nullArg (nameof input)
            else
                Model.parseStringExn input
        
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
            
        let parseStringExn string =
            match parseString string with
            | Ok model ->
                model
            | Error err ->
                raise (ParseException err)
