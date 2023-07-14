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
    
    type Comment = string
    
    [<Struct>]
    [<DebuggerDisplay("_")>]
    type WildCard = | WildCard
            
    [<Struct>]
    [<DebuggerDisplay("<>")>]
    type Absent = | Absent
        
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
        | Absent        of Absent
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
        Expr list

    type Array2dExpr =
        Expr list list

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
        ; Else   : Expr option}

    [<RequireQualifiedAccess>]
    type NumExpr =
        | Int         of int
        | Float       of float
        | Id          of Id
        | Op          of Op
        | Bracketed   of NumExpr
        | Call        of CallExpr
        | IfThenElse  of IfThenElseExpr
        | Let         of LetExpr
        | UnaryOp     of IdOr<NumericUnaryOp> * NumExpr
        | BinaryOp    of NumExpr * IdOr<NumericBinaryOp> * NumExpr
        | ArrayAccess of NumExpr * ArrayAccess list

    type IncludeItem =
        | Include of string

    type AnnotationItem =
        | Name of Id
        | Call of Id * Parameters
    
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
    
    type NamedTypeInst =
        { Name : string
        ; TypeInst : TypeInst
        ; Annotations : Annotations }
        
    type ITyped =
        abstract member TypeInst: TypeInst

    [<RequireQualifiedAccess>]        
    type Type =
        | Int
        | Bool
        | String
        | Float
        | Ann
        | Id     of Id
        | Set    of Expr // TODO confirm with MiniZinc team
        | Tuple  of TupleType
        | Record of RecordType
        | Array  of ArrayType

    type RecordType =
        { Fields: NamedTypeInst list }
         
    type TupleType =
        { Fields: TypeInst list }
        
    type RangeExpr =
        NumExpr * NumExpr
        
    [<RequireQualifiedAccess>]
    type ArrayDim =
        | Int
        | Id of Id
        | Set of Expr
        
    type ArrayType =
        { Dimensions : ArrayDim list
        ; Elements: TypeInst }
       
    type SetLiteral =
        { Elements: Expr list }
        
    [<RequireQualifiedAccess>]    
    type Item =
        | Include    of IncludeItem
        | Enum       of EnumItem
        | Synonym    of TypeAlias
        | Constraint of ConstraintItem
        | Assign     of AssignItem
        | Declare    of DeclareItem
        | Solve      of SolveItem
        | Function   of FunctionItem
        | Test       of TestItem
        | Output     of OutputItem
        | Annotation of AnnotationItem
        | Comment    of string

    type ConstraintItem =
        { Expr: Expr
        ; Annotations: Annotations }
        
    type Parameters =
        Parameter list
        
    type Parameter =
        NamedTypeInst

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

    type TypeAlias =
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
        ; TypeInst: TypeInst
        ; Annotations: Annotations
        ; Expr: Expr option }
        
        member this.Type =
            this.TypeInst.Type
            
        member this.Inst =
            this.TypeInst.Inst
        
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
        | Variable of DeclareItem
        | Expr     of Expr
        | Enum     of EnumItem
        | Type     of TypeAlias
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
        { Bindings    : Map<string, Binding>
        ; Inputs      : Map<string, TypeInst>
        ; Outputs     : Map<string, TypeInst>
        ; Variables   : Map<string, DeclareItem>
        ; Undeclared  : Map<string, Expr>
        ; Enums       : Map<string, EnumItem> 
        ; Synonyms    : Map<string, TypeAlias> 
        ; Functions   : Map<string, FunctionItem>        
        ; Conflicts   : Map<string, Binding list> }
                
        member this.Add (x: DeclareItem) =
            NameSpace.add x.Name (Binding.Variable x) this
            
        member this.Add (x: EnumItem) =
            NameSpace.add x.Name (Binding.Enum x) this
            
        member this.Add (x: TypeAlias) =
            NameSpace.add x.Name (Binding.Type x) this

        member this.Add (x: FunctionItem) =
            NameSpace.add x.Name (Binding.Function x) this
            
        member this.Add (name: string, x: Expr) : NameSpace =
            NameSpace.add name (Binding.Expr x) this
            
        member this.Remove (name: string) : NameSpace =
            NameSpace.remove name this
        
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
                | Some (Binding.Variable var), Binding.Expr expr 
                | Some (Binding.Expr expr), Binding.Variable var ->
                    match var.Expr with
                    // Assign new value
                    | None ->
                        Binding.Variable { var with Expr = Some expr }
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
                | Binding.Variable var ->
                    match var.Inst, var.Expr with
                    | Inst.Par, None ->
                        { ns with
                            Inputs = Map.add id var.TypeInst ns.Inputs
                            Variables = Map.add id var ns.Variables }
                    | Inst.Var, None ->
                        { ns with
                            Outputs = Map.add id var.TypeInst ns.Outputs
                            Variables = Map.add id var ns.Variables }
                    | _, _ ->
                        { ns with
                            Variables = Map.add id var ns.Variables }
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
            
        let addDeclare (decl: DeclareItem) (ns: NameSpace) : NameSpace =
            add decl.Name (Binding.Variable decl) ns
            
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
        ; Includes    : Map<string, IncludedModel>
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
        
    type IncludedModel =
        
        /// Reference only - load has not been attempted
        | Reference
        
        /// Model was parsed and stored in isolation  
        | Isolated of Model
        
        /// Model was parsed and integrated into the referencing model
        | Integrated of Model