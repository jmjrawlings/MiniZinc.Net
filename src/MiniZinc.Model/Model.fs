namespace MiniZinc

open System.Runtime.InteropServices
open System
open System.IO
open MiniZinc.Ast

    
[<AutoOpen>]
module rec Model =
    
    open MiniZinc.Ast
    
    // A MiniZinc model
    type Model =
        { Name         : string
        ; Inputs       : Map<string, MzType>
        ; Outputs      : OutputItem list
        ; Includes     : IncludeItem list
        ; Constraints  : ConstraintItem list
        ; Declarations : DeclareItem list
        ; Enums        : EnumItem list
        ; Assigns      : AssignItem list
        ; Predicates   : PredicateItem list
        ; Functions    : FunctionItem list
        ; Aliases      : AliasItem list
        ; Solve        : SolveItem
        ; String       : string }
        
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
                    Result.Ok Model.empty
                else
                    Model.parseString input
                
                
    module Model =
        
        let empty =
            { Inputs = Map.empty
            ; Outputs = []
            ; String = ""
            ; Name = ""
            ; Aliases = []
            ; Assigns = []
            ; Constraints = []
            ; Declarations = []
            ; Enums = []
            ; Functions = []
            ; Includes = []
            ; Predicates = []
            ; Solve = SolveItem.Satisfy }
        
        let parseFile file : Result<Model, ParseError> =
            let model =
                file
                |> Parse.file Parsers.ast
                |> Result.map fromAst
            model
            
            
        let parseString string : Result<Model, ParseError> =
            let model =
                string
                |> Parse.string Parsers.ast
                |> Result.map fromAst
            model                
            
        let fromAst ast : Model =
            addAst empty ast
            
        let rec addAst (model: Model) (ast: Ast) : Model =
            match ast with
            | [] ->
                model
            | item :: rest ->
                match item with
                | Include x ->
                    addAst {model with Includes = x :: model.Includes} rest
                | Enum x ->
                    addAst {model with Enums = x :: model.Enums} rest
                | Alias x ->
                    addAst {model with Aliases =  x :: model.Aliases} rest
                | Constraint x ->
                    addAst {model with Constraints =  x :: model.Constraints} rest
                | Assign x ->
                    addAst {model with Assigns =  x :: model.Assigns} rest
                | Declare x ->
                    addAst {model with Declarations =   x :: model.Declarations} rest
                | Solve x ->
                    addAst {model with Solve = x} rest
                | Predicate x ->
                    addAst {model with Predicates =  x :: model.Predicates} rest
                | Function x ->
                    addAst {model with Functions =   x :: model.Functions} rest
                | Output x ->
                    addAst {model with Outputs = x :: model.Outputs} rest
                | _ ->
                    addAst model rest
