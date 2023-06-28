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

open System.Diagnostics

[<AutoOpen>]
module rec Model =
    
    type ParseError =
        { Message : string
        ; Line    : int64
        ; Column  : int64
        ; Index   : int64
        ; Trace   : string }
    
    type Id = string
    
    [<Struct>]
    [<DebuggerDisplay("_")>]
    type WildCard =
        | WildCard

    type Comment =
        string
        
    type INamed =
        abstract member Name: string

    [<Struct>]
    // An identifier or a value of 'T
    type IdOr<'T> =
        | Id of id:string
        | Val of value:'T
                  
    type Inst =
        | Var = 0
        | Par = 1
        
    type VarKind =
        | AssignedPar = 0
        | UnassignedPar = 1
        | AssignedVar = 2
        | UnassignedVar = 3
                  
    type SolveMethod =
        | Satisfy = 0
        | Minimize = 1
        | Maximize = 2

    type NumericUnaryOp =
        | Add = 0
        | Subtract = 1
        
    type UnaryOp =
        | Add = 0
        | Subtract = 1
        | Not = 2

    type NumericBinaryOp =
        | Add = 0
        | Subtract = 1
        | Multiply = 3
        | Divide = 4
        | Div = 5
        | Mod = 6
        | Exp = 7
        | TildeAdd = 8
        | TildeSubtract = 9
        | TildeMultiply = 10
        | TildeDivide = 11
        | TildeDiv = 12
        
    type BinaryOp =
        | Add = 0
        | Subtract = 1
        | Multiply = 3
        | Divide = 4
        | Div = 5
        | Mod = 6
        | Exponent = 7
        | TildeAdd = 8
        | TildeSubtract = 9
        | TildeMultiply = 10
        | TildeDivide = 11
        | TildeDiv = 12
        | Equivalent = 13 
        | Implies = 14
        | ImpliedBy = 15
        | Or = 16
        | Xor = 17  
        | And = 18
        | LessThanEqual = 19 
        | GreaterThanEqual = 20 
        | EqualEqual = 21
        | LessThan =  22
        | GreaterThan = 23
        | Equal = 24
        | NotEqual = 25  
        | TildeEqual = 26 
        | TildeNotEqual = 27  
        | In = 28
        | Subset = 29 
        | Superset = 30
        | Union = 31
        | Diff = 32
        | SymDiff = 33 
        | DotDot = 34
        | Intersect = 35 
        | PlusPlus = 36
        | Default =  37

    type Op =     
        | Add = 0
        | Subtract = 1
        | Not = 2
        | Multiply = 3
        | Divide = 4
        | Div = 5
        | Mod = 6
        | Exponent = 7
        | TildeAdd = 8
        | TildeSubtract = 9
        | TildeMultiply = 10
        | TildeDivide = 11
        | TildeDiv = 12
        | Equivalent = 13 
        | Implies = 14
        | ImpliedBy = 15
        | Or = 16
        | Xor = 17  
        | And = 18
        | LessThanEqual = 19 
        | GreaterThanEqual = 20 
        | EqualEqual = 21
        | LessThan =  22
        | GreaterThan = 23
        | Equal = 24
        | NotEqual = 25  
        | TildeEqual = 26 
        | TildeNotEqual = 27  
        | In = 28
        | Subset = 29 
        | Superset = 30
        | Union = 31
        | Diff = 32
        | SymDiff = 33 
        | DotDot = 34
        | Intersect = 35 
        | PlusPlus = 36
        | Default =  37

    [<RequireQualifiedAccess>] 
    type Expr =
        | WildCard      of WildCard  
        | Int           of int
        | Float         of float
        | Bool          of bool
        | String        of string
        | Id            of string
        | Op            of Op
        | Bracketed     of Expr
        | Set           of SetLiteral
        | SetComp       of SetCompExpr
        | Array1d       of Array1dExpr
        | Array1dIndex
        | Array2d       of Array2dExpr
        | Array2dIndex
        | ArrayComp     of ArrayCompExpr
        | ArrayCompIndex
        | Tuple         of TupleExpr
        | Record        of RecordExpr
        | UnaryOp       of UnaryOpExpr
        | BinaryOp      of BinaryOpExpr
        | Annotation
        | IfThenElse    of IfThenElseExpr
        | Let           of LetExpr
        | Call          of CallExpr
        | GenCall       of GenCallExpr 
        | Indexed       of IndexExpr 

    type UnaryOpExpr =
        IdOr<UnaryOp> * Expr
        
    type BinaryOpExpr =
        Expr * IdOr<BinaryOp> * Expr

    type IndexExpr =
        | Index of Expr * ArrayAccess list
        
    type ArrayAccess =
        | Access of Expr list

    type Annotation =
        Expr

    type Annotations =
        Annotation list

    type GenCallExpr =
        { Operation: IdOr<Op>
        ; From : Generator list 
        ; Yields : Expr }
        
    type ArrayCompExpr =
        { Yields : Expr         
        ; From : Generator list }

    type SetCompExpr =
        { Yields : Expr         
        ; From : Generator list }

    type Generator =
        { Yields : IdOr<WildCard> list
        ; From  : Expr  
        ; Where : Expr option }

    type CallExpr =
        { Function: IdOr<Op>
        ; Args: Arguments }

    type Array1dExpr =
        | Array1d of Expr list

    type Array2dExpr =
        | Array2d of Expr list list

    type TupleExpr =
        | TupleExpr of Expr list
        
    type RecordExpr =
        | RecordExpr of (Id * Expr) list
        
    type SolveItem =
        | Sat of Annotations
        | Min of Expr * Annotations
        | Max of Expr * Annotations
        
        member this.SolveMethod =
            match this with
            | Sat _ -> SolveMethod.Satisfy
            | Min _ -> SolveMethod.Minimize
            | Max _ -> SolveMethod.Maximize
            
        member this.Annotations =
            match this with
            | Sat anns
            | Min (_, anns)
            | Max (_, anns) -> anns
            
        static member Satisfy =
            Sat []
            
        static member Minimize(expr) =
            Min (expr, [])
            
        static member Maximize(expr) =
            Max (expr, [])
            

    type IfThenElseExpr =
        { If     : Expr
        ; Then   : Expr
        ; ElseIf : (Expr * Expr) list
        ; Else   : Expr}

    type NumericExpr =
        | Int         of int
        | Float       of float
        | Id          of Id
        | Op          of Op
        | Bracketed   of NumericExpr
        | Call        of CallExpr
        | IfThenElse  of IfThenElseExpr
        | Let         of LetExpr
        | UnaryOp     of IdOr<NumericUnaryOp> * NumericExpr
        | BinaryOp    of NumericExpr * IdOr<NumericBinaryOp> * NumericExpr
        | ArrayAccess of NumericExpr * ArrayAccess list

    type IncludeItem =
        | Include of string

    type EnumItem =
        { Name : Id
        ; Annotations : Annotations
        ; Cases : EnumCase list }
        
        interface INamed with
            member this.Name = this.Name
        
    type EnumCase =
        | Name of Id
        | Expr of Expr
        
    type TypeInst =
        { Type  : Type
          Inst  : Inst
          IsSet : bool
          IsOptional : bool
          IsArray : bool }
        
        static member OfType t =
            { Type = t; Inst = Inst.Par; IsSet = false; IsOptional = false; IsArray = false }
        
    type ITyped =
        abstract member TypeInst: TypeInst

    [<RequireQualifiedAccess>]        
    type Type =
        | Int
        | Bool
        | String
        | Float
        | Id     of Id
        | Range  of Range
        | Set    of SetLiteral
        | Tuple  of TupleType
        | Record of RecordType
        | Array  of ArrayType

    type RecordType =
        { Fields: (Id * TypeInst) list }
         
    type TupleType =
        { Fields: TypeInst list }
        
    type Range =
        NumericExpr * NumericExpr
        
    [<RequireQualifiedAccess>]
    type ArrayDim =
        | Int
        | Id    of Id
        | Range of Range
        | Set   of SetLiteral
        
    type ArrayType =
        { Dimensions : ArrayDim list
        ; Elements: TypeInst }
       
    type SetLiteral =
        { Elements: Expr list }
        
    [<RequireQualifiedAccess>]    
    type Item =
        | Include    of IncludeItem
        | Enum       of EnumItem
        | Synonym    of SynonymItem
        | Constraint of ConstraintItem
        | Assign     of AssignItem
        | Declare    of DeclareItem
        | Solve      of SolveItem
        | Function   of FunctionItem
        | Test       of TestItem
        | Output     of OutputItem
        | Annotation of AnnotationItem
        | Comment    of string

    type AnnotationItem =
        CallExpr

    type ConstraintItem =
        | Constraint of Expr
        
    type Parameters =
        Parameter list
        
    type Parameter =
        Id * TypeInst

    type Argument =
        Expr
      
    type NamedArg =
        Id * Expr

    type NamedArgs =
        NamedArg list
        
    type Arguments =
        Argument list
           
    type TestItem =
        { Name: string
        ; Parameters : Parameters
        ; Annotations : Annotations
        ; Body: Expr option }
        
        interface INamed with
            member this.Name = this.Name    

    type SynonymItem =
        { Name : string
        ; Annotations : Annotations
        ; TypeInst: TypeInst }
        
        interface INamed with
            member this.Name = this.Name
        
    type OutputItem =
        { Expr: Expr }

    type OperationItem =
        { Name: string
          Parameters : Parameters
          Annotations : Annotations
          Body: Expr option }
        
    type FunctionItem =
        { Name: string
        ; Returns : TypeInst
        ; Annotations : Annotations
        ; Parameters : Parameters
        ; Body: Expr option }
        
        interface INamed with
            member this.Name = this.Name
        
    type Test =
        unit

    type AssignItem =
        NamedArg

    type DeclareItem =
        { Name: string
        ; Type: TypeInst
        ; Annotations: Annotations
        ; Expr: Expr option }
        
        interface INamed with
            member this.Name = this.Name

    type LetLocal =
        Choice<DeclareItem, ConstraintItem>
        
    type LetExpr =
        { NameSpace : NameSpace
        ; Constraints : ConstraintItem list
        ; Body: Expr }
        
    type Ast = Item list

    /// Things that a name can be bound to
    [<RequireQualifiedAccess>]
    type Binding =
        | Declare  of DeclareItem
        | Expr     of Expr
        | Enum     of EnumItem
        | Synonym  of SynonymItem
        | Function of FunctionItem
        | Multiple of Binding list

    /// <summary>
    /// Names bound to expressions (bindings)
    /// </summary>
    /// <remarks>
    /// The purpose of this type is to provide
    /// access to the expression bound to a particular name
    /// as well as maintaining strongly typed mappings for
    /// each possible binding.
    ///
    /// The two concrete use cases of namespaces are on
    /// the Model itself (each MiniZinc model has a global
    /// namespace), as well as inside of a let expression
    /// which can add new bindings to from its own namespace.
    /// </remarks>
    type NameSpace =
        { Bindings   : Map<string, Binding>  
        ; Declared   : Map<string, DeclareItem> 
        ; Enums      : Map<string, EnumItem> 
        ; Synonyms   : Map<string, SynonymItem> 
        ; Functions  : Map<string, FunctionItem> 
        ; Conflicts  : Map<string, Binding list> 
        ; Undeclared : Map<string, Expr> }
                
        member this.Add (x: DeclareItem) =
            NameSpace.add x.Name (Binding.Declare x) this
            
        member this.Add (x: EnumItem) =
            NameSpace.add x.Name (Binding.Enum x) this
            
        member this.Add (x: SynonymItem) =
            NameSpace.add x.Name (Binding.Synonym x) this

        member this.Add (x: FunctionItem) =
            NameSpace.add x.Name (Binding.Function x) this
            
        member this.Add (name: string, x: Expr) : NameSpace =
            NameSpace.add name (Binding.Expr x) this
        
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
        
        /// Get the bindings from the NameSpace     
        let bindings ns =
            ns.Bindings

        /// Add a binding to the namespace
        let add id (value: Binding) (ns: NameSpace) : NameSpace =
                                                       
            let oldBinding =
                ns.Bindings.TryFind id
                
            let newBinding =            
                match oldBinding, value with
                
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
                match newBinding with
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
                    
            let nameSpace =
                { result with
                    Bindings = Map.add id newBinding ns.Bindings }
                
            nameSpace
            
        let addDeclare (decl: DeclareItem) (ns: NameSpace) : NameSpace =
            add decl.Name (Binding.Declare decl) ns
            
        let addFunction (func: FunctionItem) (ns: NameSpace) : NameSpace =
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
            
            
    /// A MiniZinc model
    type Model = 
        { Name        : string
        ; FilePath    : string option
        ; Includes    : Map<string, LoadResult>
        ; NameSpace   : NameSpace
        ; Constraints : ConstraintItem list
        ; Outputs     : OutputItem list
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
    
    /// Specifies how models referenced with
    /// the "include" directive should be loaded
    type IncludeOptions =
        /// Reference only, do not load the model
        | Reference
        /// Parse the file, searching the given paths  
        | Parse of string list
        
        static member Default =
            IncludeOptions.Reference
      
    /// Options used during model parsing      
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
            | ParseError err -> failwithf $"{err}"
            | other -> failwith "Result was not a Model"
            
        let toOption result =
            match result with
            | Success x -> Some x
            | _ -> None            