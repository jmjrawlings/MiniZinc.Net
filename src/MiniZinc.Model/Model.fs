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
        ; Predicates  : Map<string, PredicateItem>
        ; Functions   : Map<string, FunctionItem>
        ; Outputs     : OutputItem list
        ; SolveMethod : SolveMethod
        ; Assigned    : Map<string, TypeInst * Expr>
        ; Conflicts    : Map<string, Binding list>
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
            ; Functions = Map.empty
            ; Predicates = Map.empty
            ; SolveMethod = SolveMethod.Satisfy
            ; Bindings = Map.empty
            ; Unassigned = Map.empty
            ; Assigned = Map.empty
            ; Undeclared = Map.empty
            ; Conflicts = Map.empty 
            ; Outputs = [] }
            
        let bindings =
            Lens.m
                (fun m -> m.Bindings)
                (fun v m -> { m with Bindings = v })
            
        let functions =
            Lens.m
                (fun m -> m.Functions)
                (fun v m -> { m with Functions = v })
        
        let conflicts =
            Lens.m
                (fun m -> m.Conflicts)
                (fun v m -> { m with Conflicts = v })
            
        let predicates =
            Lens.m
                (fun m -> m.Predicates)
                (fun v m -> { m with Predicates =  v })
            
        let assigned =
            Lens.m
                (fun m -> m.Assigned)
                (fun v m -> { m with Assigned = v })
            
        let unassigned =
            Lens.m
                (fun m -> m.Unassigned)
                (fun v m -> { m with Unassigned = v })
            
        let undeclared =
            Lens.m
                (fun m -> m.Undeclared)
                (fun v m -> { m with Undeclared =  v })
            
        let enums =
            Lens.m
                (fun m -> m.Enums)
                (fun v m -> { m with Enums = v })
            
        let synonyms =
            Lens.m
                (fun m -> m.Synonyms)
                (fun v m -> { m with Synonyms = v })
                
        let includes =
            Lens.v
                (fun m -> m.Includes)
                (fun v m -> { m with Includes = v })
                
        let outputs =
            Lens.v
                (fun m -> m.Outputs)
                (fun v m -> { m with Outputs = v })
                
        let constraints =
            Lens.v
                (fun m -> m.Constraints)
                (fun v m -> { m with Constraints =  v })
                
        let name =
            Lens.v
                (fun m -> m.Name)
                (fun v m -> { m with Name = v })
        
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
                            
            let model =
                fromBindings map
                |> includes.set ast.Includes
                |> constraints.set ast.Constraints
                |> outputs.set ast.Outputs

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
                        |> undeclared.add id s 
                        |> loop rest
                    | Unassigned s ->
                        model
                        |> unassigned.add id s
                        |> loop rest
                    | Assigned (ti,expr) ->
                        model
                        |> assigned.add id (ti,expr)
                        |> loop rest
                    | Enum e ->
                        model
                        |> enums.add id e
                        |> loop rest
                    | Synonym s ->
                        model
                        |> synonyms.add id s
                        |> loop rest
                    | Predicate p ->
                        model
                        |> predicates.add id p
                        |> loop rest   
                    | Function f ->
                        model
                        |> functions.add id f
                        |> loop rest   
                    | Conflict c ->
                        model
                        |> conflicts.add id c
                        |> loop rest
                        
            let model =
                loop (Map.toList bindings) empty
                
            model
