
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
open System.Collections.Generic
open System.Diagnostics
open System.IO
open System.Runtime.InteropServices
open System.Text

#nowarn "40"

[<AutoOpen>]    
module rec Model =

    /// A MiniZinc model
    type Model = 
        { Name        : string
        ; FilePath    : string option
        ; Includes    : Map<string, LoadResult>
        ; NameSpace   : NameSpace
        ; Constraints : ConstraintItem list
        ; Outputs     : OutputItem list
        ; SolveMethod : SolveMethod }
                
        /// Parse a Model from the given file
        static member ParseFile (filepath: string, options: ParseOptions) =
            Model.parseFile options filepath
            
        /// Parse a Model from the given file
        static member ParseFile (filepath: FileInfo, options: ParseOptions) =
            Model.parseFile options filepath.FullName

        /// Parse a Model from the given file
        static member ParseFile (filepath: string) =
            Model.parseFile ParseOptions.Default filepath
            
        /// Parse a Model from the given file
        static member ParseFile (filepath: FileInfo) =
            Model.parseFile ParseOptions.Default filepath.FullName

        /// Parse a Model from the given string
        static member ParseString (mzn: string, options: ParseOptions) =
            Model.parseString options mzn
            
        /// Parse a Model from the given string
        static member ParseString (mzn: string) =
            Model.parseString ParseOptions.Default mzn 

        member this.ToString() =
            ()
            
        member this.ToString(x: FunctionItem) =
            ()
                        
    module Model =

        /// An empty model        
        let empty : Model =
            { Name = ""
            ; FilePath = None
            ; Includes = Map.empty
            ; NameSpace = NameSpace.empty
            ; SolveMethod = SolveMethod.Satisfy
            ; Constraints = [] 
            ; Outputs = [] }

        [<AutoOpen>]
        module Lenses =

            let NameSpace_ =
                Lens.v
                    (fun m -> m.NameSpace)
                    (fun v m -> { m with NameSpace = v })
                    
            let SolveMethod_ =
                Lens.v
                    (fun m -> m.SolveMethod)
                    (fun v m -> { m with SolveMethod = v })
                    
            let Includes_ =
                Lens.m
                    (fun m -> m.Includes)
                    (fun v m -> { m with Includes = v })
                    
            let Outputs_ =
                Lens.v
                    (fun m -> m.Outputs)
                    (fun v m -> { m with Outputs = v })
                    
            let Constraints_ : ListLens<Model, ConstraintItem> =
                Lens.l
                    (fun m -> m.Constraints)
                    (fun v m -> { m with Constraints =  v })
            
            let File_ : Lens<Model, string option> = 
                Lens.v
                    (fun m -> m.FilePath)
                    (fun v m -> { m with FilePath = v })
                    
            let Name_ : Lens<Model, string> =
                Lens.v
                    (fun m -> m.Name)
                    (fun v m -> { m with Name = v })
        
        /// Parse a Model from the given MiniZinc file
        let parseFile (options: ParseOptions) (filepath: string) : LoadResult =
            
            let result =
                match File.read filepath with
                | Ok string ->
                    parseString options string
                | Error _ ->
                    FileNotFound [filepath]
                
            result
        
        /// Parse a Model from the given MiniZinc model string
        let parseString (options: ParseOptions) (mzn: string) : LoadResult =
                        
            let input, comments =
                Parse.stripComments mzn
            
            let model =
                Parse.model input
                |> Result.map (fromAst options)
                
            let result =
                match model with
                | Result.Ok model -> Success model
                | Result.Error error -> ParseError error
                
            result
            
        /// Parse a Model from the given MiniZinc model string
        let parseExn (options: ParseOptions) (mzn: string) =
            match parseString options mzn with
            | LoadResult.Success model ->
                model
            | LoadResult.ParseError error ->
                failwith error.Message
            | _ ->
                failwith "xd"
        
        /// Create a Model from the given AST
        let fromAst (options: ParseOptions) (ast: Ast) : Model =
            
            
            // Parse an included model with the given filename "model.mzn"
            let parseIncluded filename =
                
                let result =
                    match options.IncludeOptions with

                    | IncludeOptions.Reference ->
                        LoadResult.Reference

                    | IncludeOptions.Parse paths ->
                        let searchFiles =
                            paths
                            |> List.map (fun dir -> Path.Join(dir, filename))
                
                        let filepath =
                            searchFiles
                            |> List.filter File.Exists
                            |> List.tryHead
                            
                        match filepath with
                        | Some path ->
                            parseFile options path
                        | None ->
                            FileNotFound searchFiles
                            
                filename, result
                
            // Parse included models in parallel
            let inclusions =
                ast.Includes
                |> Seq.map (fun (IncludeItem.Include x) -> x)
                |> Seq.toArray
                |> Array.Parallel.map parseIncluded
                |> Map.ofSeq
                            
            // Load the model from these bindings only
            let model =
                ofNameSpace map
                |> Includes_.set inclusions
                |> Constraints_.set ast.Constraints
                |> Outputs_.set ast.Outputs                
                
            // Now merge the model with all inclusions
            let unified =
                inclusions
                |> Map.values
                |> Seq.choose (function
                    | LoadResult.Success model -> Some model
                    | _ -> None)
                |> Seq.fold merge model

            unified

        /// Merge two Models
        let merge (a: Model) (b: Model) : Model =
                            
            let nameSpace =
                NameSpace.merge a.NameSpace b.NameSpace
                
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
                { empty with
                    Name = name
                    Includes = includes
                    Constraints = constraints
                    SolveMethod = solveMethod
                    NameSpace = nameSpace }
                
            model
            
    let encode (options: EncodeOptions) (model: Model) =
        
        let mzn = MiniZincEncoder()
                                
        for incl in model.Includes.Keys do
            let item = IncludeItem.Include incl
            mzn.writeIncludeItem item

        for enum in model.NameSpace.Enums.Values do
            mzn.writeEnum enum
            
        for syn in model.NameSpace.Synonyms.Values do
            mzn.writeSynonym syn

        for x in model.NameSpace.Declared.Values do
            mzn.writeDeclareItem x
            mzn.writetn()

        for cons in model.Constraints do
            mzn.writeConstraintItem cons
            mzn.writetn()

        for func in model.NameSpace.Functions.Values do
            mzn.writeFunctionItem func
                    
        mzn.writeSolveMethod model.SolveMethod
        
        for output in model.Outputs do
            mzn.writeOutputItem output
            
        mzn.String
        
        
    type EncodeOptions =
        | EncodeOptions
        static member Default =
            EncodeOptions
    
    /// Specifies how models referenced with
    /// the "include" directive should be loaded
    type IncludeOptions =
        /// Reference only, do not load the model
        | Reference
        /// Parse the file, searching the given paths  
        | Parse of string list
        
        static member Default =
            IncludeOptions.Reference
      
    type ParseOptions =
        { IncludeOptions: IncludeOptions }
            
        static member Default =
            { IncludeOptions = IncludeOptions.Reference }

    type LoadResult =
        /// Load was successful
        | Success of Model
        /// Could not find the model
        | FileNotFound of string list
        /// Parsing failed
        | ParseError of ParseError
        /// Reference only - load has not been attempted
        | Reference
        
        member this.Model =
            LoadResult.model this
                    
    module LoadResult =
        
        /// Map the given function over the result        
        let map f result =
            match result with
            | Success model -> Success (f model)
            | other -> other
            
        /// Return the successful model or fail            
        let model result =
            match result with
            | Success x -> x
            | _ -> failwithf $"Result was not a success"
            
        let toOption result =
            match result with
            | Success x -> Some x
            | _ -> None

    /// Things that a name can be bound to
    [<RequireQualifiedAccess>]
    type Binding =
        | Declare  of DeclareItem
        | Expr     of Expr
        | Enum     of EnumItem
        | Synonym  of SynonymItem
        | Function of FunctionItem
        | Multiple of Binding list
        
    type NameSpace =
        { Bindings   : Map<string, Binding>  
        ; Declared   : Map<string, DeclareItem> 
        ; Enums      : Map<string, EnumItem> 
        ; Synonyms   : Map<string, SynonymItem> 
        ; Functions  : Map<string, FunctionItem> 
        ; Conflicts  : Map<string, Binding list> 
        ; Undeclared : Map<string, Expr> }
        
    module NameSpace =
        
        /// The empty namespace
        let empty =
            { Bindings   = Map.empty   
            ; Declared   = Map.empty  
            ; Enums      = Map.empty  
            ; Synonyms   = Map.empty  
            ; Functions  = Map.empty  
            ; Conflicts  = Map.empty  
            ; Undeclared = Map.empty }

        /// Add a binding to the namespace
        let add id value (ns: NameSpace) =
                                                       
            let previous =
                ns.Bindings.TryFind id
                
            let binding =            
                match previous, value with
                
                // Enum assignment
                | Some (Binding.Enum e), Binding.Expr expr 
                | Some (Binding.Expr expr), Binding.Enum e ->
                    let cases =
                        [EnumCase.Expr expr]
                    match e.Cases with
                    // Assign new value
                    | [] ->
                        Binding.Enum { e with Cases = cases}
                    // Existing value                    
                    | old when old = cases ->
                        Binding.Enum e
                    // Overwritten value
                    | old ->
                        Binding.Multiple
                            [ Binding.Expr expr
                            ; Binding.Enum e ]
                
                // Variable assignment    
                | Some (Binding.Declare var), Binding.Expr expr 
                | Some (Binding.Expr expr), Binding.Declare var ->
                    match var.Expr with
                    // Assign new value
                    | None ->
                        Binding.Declare { var with Expr = Some expr }
                    // Existing value                    
                    | Some old when old = expr ->
                        Binding.Expr expr
                    // Overwritten value
                    | Some other ->
                        Binding.Multiple
                            [ Binding.Expr expr
                            ; Binding.Declare var ]
                            
                // Assign an unassigned function
                | Some (Binding.Function f), Binding.Expr expr 
                | Some (Binding.Expr expr), Binding.Function f ->
                    match f.Body with
                    // Assign new value
                    | None ->
                        Binding.Function { f with Body = Some expr }
                    // Existing value                    
                    | Some old when old = expr ->
                        Binding.Function f
                    // Overwritten value
                    | old ->
                        Binding.Multiple
                            [ Binding.Expr expr
                            ; Binding.Function f ]
                            
                // Identical binding
                | Some x, y when x = y ->
                    x

                // Already conflicted
                | Some (Binding.Multiple xs), _ ->
                    Binding.Multiple (xs @ [value])
                    
                // New clash
                | Some x, _ ->
                    Binding.Multiple [x; value]
                
                // New binding
                | None, _ ->
                    value

            let result =
                match binding with
                | Binding.Declare x ->
                    { ns with Declared = Map.add id x ns.Declared }
                | Binding.Expr x ->
                    { ns with Undeclared = Map.add id x ns.Undeclared}
                | Binding.Enum x ->
                    { ns with Enums = Map.add id x ns.Enums } 
                | Binding.Synonym x ->
                    { ns with Synonyms = Map.add id x ns.Synonyms }
                | Binding.Function x ->
                    { ns with Functions = Map.add id x ns.Functions } 
                | Binding.Multiple x ->
                    { ns with Conflicts = Map.add id x ns.Conflicts }
                    
            let result =
                { result with
                    Bindings = Map.add id binding ns.Bindings }
                
            result            
                
        let ofSeq xs =
            
            let nameSpace =
                (empty, xs)
                ||> Seq.fold (fun ns (name, binding) -> add name binding ns)
                
            nameSpace
            
        let toSeq (ns: NameSpace) =
            ns.Bindings
            |> Map.toSeq
            
        /// Merge the two namespaces            
        let merge (a: NameSpace) (b: NameSpace) =
            
            let merged =
                Seq.append (toSeq a) (toSeq b)
                |> ofSeq
                
            merged
        