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
open System.IO
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
    
type Bindings = Map<Id, Binding>

module Bindings =
    
    let empty = Map.empty
    
    let add id value (bindings: Bindings) =
                                           
        let prior =
            bindings.TryFind id
            
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
                Binding.Conflict [x; value]
            
            // New binding
            | None, _ ->
                value
    
        let result =
            bindings.Add(id, binding)
        
        result            
            
    let ofSeq xs =
        
        let bindings =
            (Map.empty, xs)
            ||> Seq.fold (fun map (id, bnd) -> add id bnd map)
            
        bindings
        
    let merge (a: Bindings) (b: Bindings) =
        
        let merged =
            Map.toSeq a
            |> Seq.append (Map.toSeq b)
            |> ofSeq
            
        merged

    
module rec Model =
            
    type LoadResult =
        // Load was successful
        | Success of Model
        // Could not find the model to load
        | FileNotFound of string list
        // Parsing failed
        | ParseError of ParseError
        // Reference only
        | Reference 
            
    // A MiniZinc model
    type Model =
        { Name        : string
        ; File        : string option
        ; Includes    : Map<string, LoadResult>
        ; Bindings    : Map<Id, Binding>
        ; Enums       : Map<string, EnumItem>
        ; Synonyms    : Map<string, TypeInst>
        ; Constraints : ConstraintItem list
        ; Predicates  : Map<string, PredicateItem>
        ; Functions   : Map<string, FunctionItem>
        ; Outputs     : OutputItem list
        ; SolveMethod : SolveMethod
        ; Assigned    : Map<string, TypeInst * Expr>
        ; Unassigned  : Map<string, TypeInst>
        ; Undeclared  : Map<string, Expr> 
        ; Conflicts   : Map<string, Binding list> }
                
        /// <summary>
        /// Parse a Model from the given file
        /// </summary>
        static member ParseFile (file: string) =
            Model.parseFile file

        /// <summary>
        /// Parse a Model from the given string
        /// </summary>
        /// <param name="allowEmpty">
        /// If true, an empty model will be returned
        /// in the case of an empty/null string.
        ///
        /// Defaults is false.
        /// </param>
        static member ParseString ([<Optional; DefaultParameterValue(false)>] allowEmpty: bool) =
            fun input ->
                if String.IsNullOrWhiteSpace input && allowEmpty then
                    Result.Ok Model.empty
                else
                    Model.parseString input
                
                
    module Model =

        // An empty model        
        let empty =
            { Name = ""
            ; File = None
            ; Includes = Map.empty
            ; Enums = Map.empty
            ; Synonyms = Map.empty
            ; Constraints = []
            ; Functions = Map.empty
            ; Predicates = Map.empty
            ; SolveMethod = SolveMethod.Satisfy
            ; Bindings = Map.empty
            ; Unassigned = Map.empty
            ; Assigned = Map.empty
            ; Undeclared = Map.empty
            ; Conflicts = Map.empty 
            ; Outputs = [] }

        [<AutoOpen>]
        module Lenses =
            let Bindings_ =
                Lens.m
                    (fun m -> m.Bindings)
                    (fun v m -> { m with Bindings = v })
                
            let Functions_ =
                Lens.m
                    (fun m -> m.Functions)
                    (fun v m -> { m with Functions = v })
                    
            let SolveMethod_ =
                Lens.v
                    (fun m -> m.SolveMethod)
                    (fun v m -> { m with SolveMethod = v })                
            
            let Conflicts_ =
                Lens.m
                    (fun m -> m.Conflicts)
                    (fun v m -> { m with Conflicts = v })
                
            let Predicates_ =
                Lens.m
                    (fun m -> m.Predicates)
                    (fun v m -> { m with Predicates =  v })
                
            let Assigned_ =
                Lens.m
                    (fun m -> m.Assigned)
                    (fun v m -> { m with Assigned = v })
                
            let Unassigned_ =
                Lens.m
                    (fun m -> m.Unassigned)
                    (fun v m -> { m with Unassigned = v })
                
            let Undeclared_ =
                Lens.m
                    (fun m -> m.Undeclared)
                    (fun v m -> { m with Undeclared =  v })
                
            let Enums_ =
                Lens.m
                    (fun m -> m.Enums)
                    (fun v m -> { m with Enums = v })
                
            let Synonyms_ =
                Lens.m
                    (fun m -> m.Synonyms)
                    (fun v m -> { m with Synonyms = v })
                    
            let Includes_ =
                Lens.m
                    (fun m -> m.Includes)
                    (fun v m -> { m with Includes = v })
                    
            let Outputs_ =
                Lens.v
                    (fun m -> m.Outputs)
                    (fun v m -> { m with Outputs = v })
                    
            let Constraints_ = 
                Lens.l
                    (fun m -> m.Constraints)
                    (fun v m -> { m with Constraints =  v })
            
            let File_ = 
                Lens.v
                    (fun m -> m.File)
                    (fun v m -> { m with File = v })
                    
            let Name_ =
                Lens.v
                    (fun m -> m.Name)
                    (fun v m -> { m with Name = v })
        
        // Parse a Model from the given .mzn file
        let parseFile (file: string) =
            
            let mzn =
                File.read file
            
            let model =
                mzn.Bind parseString
                
            model
            
        // Parse a Model from the given string
        let parseString string =
                        
            let input, comments =
                Parse.sanitize string
            
            let ast =
                Parse.string input
                |> Result.mapError (fun e -> e.Message)
                
            let model =
                ast
                |> Result.map fromAst
                
            model
            
        
        // Create a Model from the given AST            
        let fromAst (ast: Ast) : Model =
                
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
                
            for id, _anns, ti in ast.Synonyms do
                bindings.Add (id, Binding.Synonym ti)
                
            let map =
                Bindings.ofSeq bindings
                
            let includes =
                ast.Includes
                |> Seq.map (fun i -> (i, LoadResult.Reference))
                |> Map.ofSeq
                            
            let model =
                fromBindings map
                |> Includes_.set includes
                |> Constraints_.set ast.Constraints
                |> Outputs_.set ast.Outputs

            model
            
                
        // Create a Model from the given AST            
        let fromBindings (bindings: Bindings) : Model =
                    
            let rec loop (bindings: (string*Binding) list) model =
                match bindings with
                | [] ->
                    model
                | (id, binding) :: rest ->
                    match binding with
                    | Undeclared s ->
                        model
                        |> Undeclared_.add id s 
                        |> loop rest
                    | Unassigned s ->
                        model
                        |> Unassigned_.add id s
                        |> loop rest
                    | Assigned (ti,expr) ->
                        model
                        |> Assigned_.add id (ti,expr)
                        |> loop rest
                    | Enum e ->
                        model
                        |> Enums_.add id e
                        |> loop rest
                    | Synonym s ->
                        model
                        |> Synonyms_.add id s
                        |> loop rest
                    | Predicate p ->
                        model
                        |> Predicates_.add id p
                        |> loop rest   
                    | Function f ->
                        model
                        |> Functions_.add id f
                        |> loop rest   
                    | Conflict c ->
                        model
                        |> Conflicts_.add id c
                        |> loop rest
                        
            let model =
                loop (Map.toList bindings) empty
                
            model
            
            
        // Merge the two models            
        let merge (a: Model) (b: Model) : Model =
                            
            let bindings =
                Bindings.merge a.Bindings b.Bindings
                
            let name =
                $"{a.Name} and {b.Name}"
                
            let includes =
                Map.merge a.Includes b.Includes

            let constraints =
                a.Constraints @ b.Constraints
                                
            let solveMethod =                
                match a.SolveMethod, b.SolveMethod with
                    | SolveMethod.Sat _ , other
                    | other, SolveMethod.Sat _ -> other
                    | left, right -> right
                
            let model =
                fromBindings bindings
                |> Constraints_.set constraints
                |> Includes_.set includes
                |> Name_.set name
                |> SolveMethod_.set solveMethod
                
            model
            
        // Load an included model into this one
        let loadIncluded filename (searchDirs: string seq) (model: Model) =
                        
            let includeFile =
                searchDirs
                |> Seq.map (fun dir -> Path.Join(dir, filename))
                |> Seq.filter File.Exists
                |> Seq.tryHead
                |> Result.ofOption $"Could not find {filename} in any of the search directories"
                
            let includeModel =
                includeFile
                |> Result.bind (parseFile >> Result.mapError string)
                
            let result =
                match includeModel with
                | Ok incl ->
                    let merged = merge model incl
                    let includes = List.filter ((<>) filename) model.Includes
                    Ok {merged with Includes = includes }
                | Error err ->
                    Error err
            
            result
            
            
        // Load all included models
        let loadIncludedModels (searchDirs: string seq) (model: Model) =
            model