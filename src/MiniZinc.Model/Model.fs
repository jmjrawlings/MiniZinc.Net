(*

Model.fs

Domain types for MiniZinc which includes those only used
in the parsing phase (eg: LetLocal).  This is mostly a 
1:1 mapping from the MiniZinc Grammar which can be found at
https://www.minizinc.org/doc-2.7.6/en/spec.html#full-grammar.

The `Model` type is the core datastructure we will deal with
past the parsing phase. 
*)

namespace MiniZinc

open System.IO
open FParsec.CharParsers

[<AutoOpen>]
module rec Model =
        
    module NameSpace =
                
        /// The empty namespace
        let empty =
            { Bindings   = Map.empty
            ; Inputs     = Map.empty
            ; Outputs    = Map.empty
            ; Variables  = Map.empty
            ; Undeclared = Map.empty 
            ; Enums      = Map.empty  
            ; Synonyms   = Map.empty  
            ; Functions  = Map.empty  
            ; Conflicts  = Map.empty }
        
        /// Get the bindings from the NameSpace     
        let bindings ns =
            ns.Bindings
            
        /// Remove the binding from the namespace     
        let remove name ns =
            { ns with
                Bindings   = ns.Bindings.Remove name 
                Outputs  = ns.Outputs.Remove name 
                Undeclared = ns.Undeclared.Remove name 
                Enums      = ns.Enums.Remove name 
                Synonyms   = ns.Synonyms.Remove name 
                Functions  = ns.Functions.Remove name 
                Conflicts  = ns.Conflicts.Remove name }

        /// Add a binding to the namespace
        let add id (value: Binding) (ns: NameSpace) : NameSpace =
                                                       
            let oldBinding =
                ns.Bindings.TryFind id
                
            let newBinding =            
                match oldBinding, value with
                
                // Identical binding
                | Some x, y when x = y ->
                    x
                
                // New binding
                | None, _ ->
                    value
                    
                // Enum assignment
                | Some (Binding.Enum e), Binding.Expr expr 
                | Some (Binding.Expr expr), Binding.Enum e ->
                    
                    // let names = ResizeArray<string>()
                    //
                    // let state = {| names:   |}
                    //
                    // let rec parseCases cases expr =
                    //     match expr with
                    //     | Expr.Call ()                            
                    //     
                    //     
                    value
                    
                    // match e.Cases with
                    // // Assign new value
                    // | [] ->
                    //     Binding.Enum { e with Cases = cases}
                    // // Existing value                    
                    // | old when old = cases ->
                    //     Binding.Enum e
                    // // Overwritten value
                    // | old ->
                    //     Binding.Multiple
                    //         [ Binding.Expr expr
                    //         ; Binding.Enum e ]
                
                // Variable assignment    
                | Some (Binding.Variable var), Binding.Expr expr 
                | Some (Binding.Expr expr), Binding.Variable var ->
                    match var.Value with
                    // Assign new value
                    | None ->
                        Binding.Variable { var with Value = Some expr }
                    // Existing value                    
                    | Some old when old = expr ->
                        Binding.Expr expr
                    // Overwritten value
                    | Some other ->
                        Binding.Multiple
                            [ Binding.Expr expr
                            ; Binding.Variable var ]
                            
                // Assign an unassigned function
                | Some (Binding.Function f), Binding.Expr expr 
                | Some (Binding.Expr expr), Binding.Function f ->
                    match f.Body with
                    
                    // Assign a new value
                    | None ->
                        Binding.Function { f with Body = Some expr }
                        
                    // No change in value                    
                    | Some old when old = expr ->
                        Binding.Function f
                        
                    // Overwrite existing value?
                    | old ->
                        Binding.Multiple
                            [ Binding.Expr expr
                            ; Binding.Function f ]
                
                // Already conflicted
                | Some (Binding.Multiple xs), _ ->
                    Binding.Multiple (xs @ [value])
                    
                // New clash
                | Some x, _ ->
                    Binding.Multiple [x; value]
                

            let result =
                match newBinding with
                | Binding.Variable ti ->
                    match ti.IsVar, ti.Value with
                    | false, None ->
                        { ns with
                            Inputs = Map.add id ti ns.Inputs
                            Variables = Map.add id ti ns.Variables }
                    | true, None ->
                        { ns with
                            Outputs = Map.add id ti ns.Outputs
                            Variables = Map.add id ti ns.Variables }
                    | _, _ ->
                        { ns with
                            Variables = Map.add id ti ns.Variables }
                | Binding.Expr x ->
                    { ns with
                        Undeclared = Map.add id x ns.Undeclared }
                | Binding.Enum x ->
                    { ns with
                        Enums = Map.add id x ns.Enums } 
                | Binding.Type x ->
                    { ns with
                        Synonyms = Map.add id x ns.Synonyms }
                | Binding.Function x ->
                    { ns with
                        Functions = Map.add id x ns.Functions } 
                | Binding.Multiple x ->
                    { ns with
                        Conflicts = Map.add id x ns.Conflicts }
                    
            let nameSpace =
                { result with
                    Bindings = Map.add id newBinding ns.Bindings }
                
            nameSpace
            
        let addDeclare (decl: TypeInst) (ns: NameSpace) : NameSpace =
            add decl.Name (Binding.Variable decl) ns
            
        let addFunction (func: FunctionType) (ns: NameSpace) : NameSpace =
            add func.Name (Binding.Function func) ns
        
        /// Create a NameSpace from the given bindings        
        let ofSeq (xs: (string * Binding) seq) : NameSpace =
            
            let fold ns (name, binding) =
                add name binding ns
            
            let nameSpace =
                Seq.fold fold empty xs
                
            nameSpace
            
        /// Return the bindings as a sequence  
        let toSeq (ns: NameSpace) =
            ns.Bindings
            |> Map.toSeq
            
        /// Merge the two namespaces            
        let merge (a: NameSpace) (b: NameSpace) =
            
            let merged =
                Seq.append (toSeq a) (toSeq b)
                |> ofSeq
                
            merged
    
    type NameSpace with
                
        member this.Add (x: TypeInst) =
            NameSpace.add x.Name (Binding.Variable x) this
            
        member this.Add (x: EnumType) =
            NameSpace.add x.Name (Binding.Enum x) this
            
        member this.Add (x: TypeAlias) =
            NameSpace.add x.Name (Binding.Type x) this

        member this.Add (x: FunctionType) =
            NameSpace.add x.Name (Binding.Function x) this
            
        member this.Add (name: string, x: Expr) : NameSpace =
            NameSpace.add name (Binding.Expr x) this
            
        member this.Remove (name: string) : NameSpace =
            NameSpace.remove name this
                
    /// A MiniZinc model
    type Model = 
        { Name        : string
        ; FilePath    : string option
        ; Includes    : Map<string, IncludeItem>
        ; NameSpace   : NameSpace
        ; Constraints : ConstraintExpr list
        ; Outputs     : OutputExpr list
        ; SolveMethod : SolveItem }
                        
    module Model =

        /// An empty model        
        let empty : Model =
            { Name = ""
            ; FilePath = None
            ; Includes = Map.empty
            ; NameSpace = NameSpace.empty
            ; SolveMethod = SolveItem.Satisfy
            ; Constraints = [] 
            ; Outputs = [] }
        
        /// Merge two Models
        let merge (a: Model) (b: Model) : Model =
                            
            let nameSpace =
                NameSpace.merge a.NameSpace b.NameSpace
                
            let name =
                $"{a.Name} and {b.Name}"
                
            let includes =
                Map.merge a.Includes b.Includes

            let constraints =
                List.distinct (a.Constraints @ b.Constraints)
                                
            let solveMethod =                
                match a.SolveMethod, b.SolveMethod with
                    | SolveItem.Sat _ , other
                    | other, SolveItem.Sat _ -> other
                    | left, right -> right
                
            let model =
                { empty with
                    Name = name
                    Includes = includes
                    Constraints = constraints
                    SolveMethod = solveMethod
                    NameSpace = nameSpace }
                
            model
                    
        let fromAst (ast: Ast) : Model =
                        
            let fold (model:Model) (item: Item) =
                match item with
                | Item.Include x ->
                    { model with Includes = model.Includes.Add (x.Name, x) }
                | Item.Enum x ->
                    { model with NameSpace = model.NameSpace.Add x }
                | Item.Synonym x ->
                    { model with NameSpace = model.NameSpace.Add x }
                | Item.Declare x ->
                    { model with NameSpace = model.NameSpace.Add x }
                | Item.Function x ->
                    { model with NameSpace = model.NameSpace.Add x }
                | Item.Assign (name, expr) ->
                    { model with NameSpace = model.NameSpace.Add(name, expr) }
                | Item.Constraint x ->
                    { model with Constraints = x :: model.Constraints }
                | Item.Solve x ->
                    { model with SolveMethod = x} 
                | Item.Test x ->
                    model
                | Item.Output x ->
                    { model with Outputs = x :: model.Outputs }
                | Item.Annotation x ->
                    model
                | Item.Comment x ->
                    model

            let model =
                List.fold fold Model.empty ast
                
            model
    
        /// Encode the given model as a string
        let encode (model: Model) =
            
            let enc = Encoder()
                                                
            for item in model.Includes.Values do
                enc.writeIncludeItem item
                enc.writetn()

            for enum in model.NameSpace.Enums.Values do
                enc.writeEnumType enum
                enc.writetn()
                
            for syn in model.NameSpace.Synonyms.Values do
                enc.writeSynonym syn
                enc.writetn()

            for x in model.NameSpace.Variables.Values do
                enc.writeDeclare x
                enc.writetn()

            for cons in model.Constraints do
                enc.writeConstraint cons
                enc.writetn()

            for func in model.NameSpace.Functions.Values do
                enc.writeFunction func
                enc.writetn()
                        
            enc.writeSolveMethod model.SolveMethod
            
            for output in model.Outputs do
                enc.writeOutput output
                enc.writetn()
                
            enc.String
                            
    let parseModelString (mzn: string) : Result<Model, ParseError> =
                                            
        let source, comments =
            parseComments mzn
        
        let model =
            source
            |> parseWith Parsers.ast
            |> Result.map Model.fromAst
            
        model
        
    let parseModelFile (filepath: string) : Result<Model, ParseError> =
                
        if File.Exists filepath then
            let mzn = File.ReadAllText filepath
            let model = parseModelString mzn
            model
        else
            failwithf $"{filepath} does not exist"
       
        
    type Model with

        /// Parse a Model from the given file
        static member ParseFile (filepath: string) =
            parseModelFile filepath
            
        /// Parse a Model from the given file
        static member ParseFile (filepath: FileInfo) =
            parseModelFile filepath.FullName

        /// Parse a Model from the given string
        static member ParseString (mzn: string) =
            parseModelString mzn
            
        member this.Encode() =
            Model.encode this