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


module rec Variables =

    [<DebuggerDisplay("<{ToString()}>")>]
    type Variables(map: Map<string, Variable>) =
                                        
        let by_kind =
            Dictionary<VarKind, Variables>()
                
        member this.Map =
            map
            
        member this.Names =
            map.Keys
            
        member this.Filter f =
            Variables.filter f this
            
        member this.Values =
            map.Values
         
        member this.Count =
            map.Count
                
        member this.OfKind (kind: VarKind) =
            match by_kind.TryGet kind with
            | Some vars ->
                vars
            | None ->
                let vars =
                    this.Filter (fun v -> v.Kind = kind)
                by_kind[kind] <- vars
                vars
                
        override this.ToString() =
            Variables.toString this
                
        static member (+) (a: Variables, b: Variable) =
            Variables.add b a
            
        static member (+) (a: Variable, b: Variables) =
            Variables.add a b
            
        static member (+) (a: Variables, b: Variables) =
            Variables.merge a b
            
        interface IEnumerable<Variable> with
            member this.GetEnumerator() = 
                (this.Values :> IEnumerable<_>).GetEnumerator()
            
        interface IEnumerable with
            member this.GetEnumerator() : IEnumerator = 
                (this.Values :> IEnumerable).GetEnumerator()
                
        interface IReadOnlyDictionary<Id, Variable> with
            member this.ContainsKey key =
                map.ContainsKey key 
            member this.GetEnumerator(): IEnumerator<KeyValuePair<Id,Variable>> =
                (map :> IEnumerable<KeyValuePair<Id,Variable>>).GetEnumerator()
            member this.TryGetValue(key, value) =
                map.TryGetValue(key, &value)
            member this.Count =
                map.Count
            member this.Item with get key =
                map[key]
            member this.Keys =
                map.Keys
            member this.Values =
                map.Values
                    
    module Variables =
        
        let empty =
            Variables Map.empty
        
        let ofMap map =
            Variables(map)
            
        let ofSeq (vars: Variable seq) =
            vars
            |> Map.withKey (fun v -> v.Name)
            |> ofMap
        
        let ofList (vars: Variable list) =
            ofSeq vars
            
        let apply f (vars: Variables) =
            vars.Map
            |> f
            |> ofMap
        
        let map f (vars: Variables) =
            apply (Map.map f) vars
            
        let filter f (vars: Variables) =
            apply (Map.filter <| fun _ -> f) vars
            
        let names (vars: Variables) =
            vars.Map.Keys
            
        let add (var: Variable) (vars :Variables) =
            apply (Map.add var.Name var) vars
            
        let merge (a: Variables) (b: Variables) =
            let vars =
                a.Values
                |> Seq.append b.Values
                |> ofSeq
            vars
        
        let assign name value (vars: Variables) =
            
            let f (map: Map<_,_>) =
                match map.TryFind name with
                | None ->
                    // TODO: Warning? Error?
                    map
                | Some var ->
                    Map.add name {var with Value = Some value } map
            
            let result =
                apply f vars
                
            result

            
        let toString (vars: Variables) =
            $"{vars.Count} Vars"

    
[<AutoOpen>]
module rec Model =
    
    open Variables
    
    // A MiniZinc model
    type Model =
        { Name        : string
        ; Includes    : IncludeItem list
        ; Enums       : EnumItem list
        ; Aliases     : AliasItem list
        ; Variables   : Variables
        ; Assigns     : AssignItem list
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
        
        let empty =
            { Name = ""
            ; Includes = []
            ; Enums = []
            ; Aliases = []
            ; Variables = Variables.empty
            ; Constraints = []
            ; Functions = []
            ; Predicates = []
            ; Assigns = []
            ; Solve = SolveItem.Satisfy 
            ; Outputs = [] }
        
        let parseFile file : Result<Model, ParseError> =
            
            let string =
                File.ReadAllText file.FullName
            
            let model =
                parseString string
                
            model
            
        let parseString string : Result<Model, ParseError> =
            
            let input, comments =
                Parse.sanitize string
            
            let model =
                input
                |> Parse.string Parsers.ast
                |> Result.map fromAst
                
            model                

        // Create a Model from the given AST            
        let fromAst ast : Model =
            addAst ast empty
            
        // Incorporate the given AST into the model
        let addAst (ast: Ast) (model: Model) : Model =
            
            let includes = ResizeArray()
            let aliases = ResizeArray()
            let enums = ResizeArray()
            let constraints = ResizeArray()
            let assigns = ResizeArray()
            let declares = ResizeArray()
            let variables = ResizeArray()
            let predicates = ResizeArray()
            let functions = ResizeArray()
            let outputs = ResizeArray()
            let mutable solve = Solve.Satisfy
                        
            for item in ast do
                match item with
                | Include x ->
                    includes.Add x
                | Enum x ->
                    enums.Add x
                | Alias x ->
                    aliases.Add x
                | Constraint x ->
                    constraints.Add x
                | Assign x ->
                    assigns.Add x
                | Declare x ->
                    declares.Add x
                | Solve x ->
                    solve <- x
                | Predicate x ->
                    predicates.Add x
                | Function x ->
                    functions.Add x 
                | Output x ->
                    outputs.Add x
                | Annotation _ 
                | Comment _ 
                | Test _ ->
                    ()

            let variables =
                variables
                |> Variables.ofSeq
                
            let assign (id, expr) map =
                match Map.tryFind id map with
                | None ->
                    map
                | Some var ->
                    Map.add id { var with Value = Some expr }
                
            let assigned =
                variables
                |> Seq.fold assign assigns
                
            
                
            let result =
                reconciled
    
