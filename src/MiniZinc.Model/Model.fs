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
    | Undeclared of Expr
    | Unassigned of TypeInst
    | Assigned   of TypeInst * Expr
    | Enum       of EnumItem
    | Synonym    of TypeInst
    | Predicate  of PredicateItem
    | Function   of FunctionItem
    | Conflict   of Binding list
    

    
[<AutoOpen>]
module rec Model =
    
    // A MiniZinc model
    type Model =
        { Name        : string
        ; Includes    : string list
        ; Bindings    : Map<Id, Binding>
        ; Enums       : Map<string, EnumItem>
        ; Synonyms    : Map<string, TypeInst>
        ; Constraints : ConstraintItem list
        ; Predicates  : PredicateItem list
        ; Functions   : FunctionItem list
        ; Outputs     : OutputItem list
        ; SolveMethod : SolveMethod
        ; Assigned    : Map<string, TypeInst * Expr>
        ; Unassigned  : Map<string, TypeInst>
        ; Undeclared  : Map<string, Expr> }
                
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
            ; Constraints = []
            ; Functions = []
            ; Predicates = []
            ; SolveMethod = SolveMethod.Satisfy
            ; Bindings = Map.empty
            ; Unassigned = Map.empty
            ; Assigned = Map.empty
            ; Undeclared = Map.empty 
            ; Outputs = [] }
            
        let bindings =
            Lens.m
                (fun m -> m.Bindings)
                (fun m v -> { m with Bindings = v })
            
        let functions =
            Lens.v
                (fun m -> m.Functions)
                (fun m v -> { m with Functions = v })
            
        let predicates =
            Lens.v
                (fun m -> m.Predicates)
                (fun m v -> { m with Predicates =  v })
            
        let assigned (model: Model) =
            Lens.m
                (fun m -> m.Assigned)
                (fun m v -> { m with Assigned = v })
            
        let unassigned (model: Model) =
            Lens.m
                (fun m -> m.Unassigned)
                (fun m v -> { m with Unassigned = v })
            
        let undeclared (model: Model) =
            Lens.m
                (fun m -> m.Undeclared)
                (fun m v -> { m with Undeclared =  v })
            
        let enums (model: Model) =
            Lens.m
                (fun m -> m.Enums)
                (fun m v -> { m with Enums = v })
            
        let synonyms (model: Model) =
            Lens.m
                (fun m -> m.Synonyms)
                (fun m v -> { m with Synonyms = v })
        
        // Parse a Model from the given .mzn file
        let parseFile file : Result<Model, ParseError> =
            
            let mzn =
                File.ReadAllText file.FullName
            
            let model =
                parseString mzn
                
            model
            
        // Parse a Model from the given string
        let parseString string : Result<Model, ParseError> =
            
            let input, comments =
                Parse.sanitize string
            
            let ast =
                input
                |> Parse.string
                
            let model =
                ast
                |> Result.map fromAst
                
            model
                 
        // Add a binding to the model
        let addBinding id value (model: Model) =
                                               
            let prior =
                model.Bindings.TryFind id
                
            let binding =            
                match prior, value with
                
                // Assign an unassigned variable    
                | Some (Unassigned ti), Undeclared expr ->
                    Binding.Assigned (ti, expr)
                    
                // Update an existing variable                    
                | Some (Assigned (ti, v1)), Undeclared v2 when v1=v2 ->
                    Binding.Assigned (ti, v1)

                // Assign an unassigned enum                                    
                | Some (Enum {Cases=[]}), Enum enum ->
                    Binding.Enum enum
                
                // Identical
                | Some x, y when x = y ->
                    x

                // Already conflicted
                | Some (Conflict xs), _ ->
                    Binding.Conflict (xs @ [value])
                    
                // New clash
                | Some x, _ ->
                    Binding.Conflict ([x; value])
                
                // New binding
                | None, _ ->
                    value
        
            let bindings =
                model.Bindings
                |> Map.add id binding
                
            { model with Bindings = bindings }
            
        
        // Create a Model from the given AST            
        let fromAst (ast: Ast) : Model =
            
            let model =
                { empty with
                      Includes = ast.Includes
                      Constraints = ast.Constraints
                      Outputs = ast.Outputs }
                
            let bindings = ResizeArray()
            
            for id,expr in ast.Assigns do
                bindings.Add (id, Binding.Undeclared expr)

            for id, ti, _, expr in ast.Declares do
                match expr with
                | Some value ->
                    bindings.Add(id, Binding.Assigned (ti, value))
                | None ->
                    bindings.Add(id, Binding.Unassigned ti)
                    
            for enum in ast.Enums do
                bindings.Add (enum.Name, Binding.Enum enum)
            
            for x in ast.Functions do
                bindings.Add (x.Name, Binding.Function x)
                
            for x in ast.Predicates do
                bindings.Add (x.Name, Binding.Predicate x)
                
            for (id, _anns, ti) in ast.Synonyms do
                bindings.Add (id, Binding.Synonym ti)
            
            let model =
                bindings
                |> Seq.fold (fun m (id, b) -> addBinding id b m) model

            model
