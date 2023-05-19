﻿namespace MiniZinc

open System.Runtime.InteropServices
open System
open System.IO

    
[<AutoOpen>]
module rec Model =
    open MiniZinc
    
    // A MiniZinc model
    type Model =
        { Name    : string 
        ; Inputs  : Map<string, MzType>
        ; Outputs : Map<string, MzType>
        ; String  : string }
        
        /// <summary>
        /// Parse a Model from the given file
        /// </summary>
        static member parseFile (file: string) =
            Model.parseFile (FileInfo file)

        /// <summary>
        /// Parse a Model from the given string
        /// </summary>
        /// <param name="allowEmpty">
        /// If true, an empty model will be returned
        /// in the case of an empty/null string.
        ///
        /// Defaults is false.
        /// </param>
        static member parseString ([<Optional; DefaultParameterValue(false)>] allowEmpty: bool) =
            fun input ->
                if String.IsNullOrWhiteSpace input && allowEmpty then
                    Result.Ok []
                else
                    Model.parseString input
                
                
    module Model =
        
        let empty =
            { Inputs = Map.empty
            ; Outputs = Map.empty
            ; String = ""
            ; Name = "" }
        
        let parseFile file =
            Parse.file Parsers.model file
            
        let parseString string =
            Parse.string Parsers.model string
             