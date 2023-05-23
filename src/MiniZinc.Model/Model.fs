(*

Model.fs

Contains the primary types and functions we will use to
create, analyze and manipulate MiniZinc models.

The AST is cumbersome to manipulate directly so this module forms
an API over the top of it.  We can readily convert between Model and 
AST as required.

*)

namespace MiniZinc


open System
open System.Diagnostics
open System.IO
open System.Collections
open System.Collections.Generic
open System.Runtime.InteropServices
    
type Binding =
    | UndeclaredVar of Expr
    | UnassignedVar of TypeInst
    | AssignedVar of TypeInst * Expr
    | Enum of EnumItem
    | Synonym of TypeInst
    | Predicate of PredicateItem
    | Function of FunctionItem
    
    
[<AutoOpen>]
module rec Model =
    
    // A MiniZinc model
    type Model =
        { Name        : string
        ; Includes    : IncludeItem list
        ; Bindings    : Map<Id, Binding>
        ; Constraints : ConstraintItem list
        ; Predicates  : PredicateItem list
        ; Functions   : FunctionItem list
        ; Outputs     : OutputItem list 
        ; Solve       : SolveItem }
                
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

        // An empty model        
        let empty =
            { Name = ""
            ; Includes = []
            ; Enums = Map.empty
            ; Synonyms = Map.empty
            ; Declares = Map.empty
            ; Assignments = Map.empty
            ; Variables = Map.empty 
            ; Constraints = []
            ; Functions = []
            ; Predicates = []
            ; Solve = SolveItem.Satisfy 
            ; Outputs = [] }
        
        // Parse a Model from the given .mzn file
        let parseFile file : Result<Model, ParseError> =
            
            let string =
                File.ReadAllText file.FullName
            
            let model =
                parseString string
                
            model
            
        // Parse a Model from the given string
        let parseString string : Result<Model, ParseError> =
            
            let input, comments =
                Parse.sanitize string
            
            let model =
                input
                |> Parse.string Parsers.ast
                |> Result.map fromAst
                
            model
                    

        // Create a Model from the given AST            
        let fromAst (ast: Ast) : Model =
            
            let rec loop (ast: Ast) (model: Model) =
                
                match ast with
                | [] ->
                    model
                | item :: rest ->
                    let loop = loop rest
                    match item with
                    | Include x ->
                        loop {
                            model with
                                Includes = x :: model.Includes
                        } 
                    | Enum x ->
                        loop {
                            model with
                                Enums = model.Enums.Add (x.Name, x)
                        }
                    | Alias (id, _, ti) ->
                        loop {
                            model with
                                Synonyms = model.Synonyms.Add (id, ti)
                        }
                    | Constraint x ->
                        loop {
                            model with
                                Constraints =  x :: model.Constraints
                        } 
                    | Assign (id, expr) ->
                        loop {
                            model with
                                Assignments = model.Assignments.Add(id,expr)
                        }
                    | Declare (id, ti, anns, Some expr) ->
                        loop {
                            model with
                                Declares = model.Declares.Add (id, ti)
                                Assignments = model.Assignments.Add (id, expr) 
                        } 
                    | Declare (id, ti, anns, None) ->
                        loop {
                            model with
                                Declares =  model.Declares.Add (id, ti)
                        }
                    | Solve x ->
                        loop {
                            model with
                                Solve = x
                        }
                    | Predicate x ->
                        loop {
                            model with
                                Predicates =  x :: model.Predicates
                        }
                    | Function x ->
                        loop {
                            model with
                                Functions =   x :: model.Functions
                        } 
                    | Output x ->
                        loop {
                            model with
                                Outputs = x :: model.Outputs
                        } 
                    | _ ->
                        loop model
                        
            let model = loop ast empty
            
            model
            
    let variables (model: Model) : Model =
        
        