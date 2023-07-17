(*
Ast.fs 

Types and parsers for MiniZinc language.

This is mostly a 1:1 mapping from the MiniZinc Grammar which can be found at
https://www.minizinc.org/doc-2.7.6/en/spec.html#full-grammar.

The types in here are not really intended to be used directly, they will
be wrapped up and exposed through the `Model` class
*)

namespace MiniZinc

open System.Diagnostics
open System.IO

[<AutoOpen>]
module rec Ast =
        
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
        { Name: Id; Args: Expr list }
                
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
        Expr list
        
    type RecordExpr =
        (Id * Expr) list
        
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

    [<RequireQualifiedAccess>]
    type AnnotationItem =
        | Name of Id
        | Call of Id * Parameters
    
    type EnumItem =
        { Name : Id
        ; Annotations : Annotations
        ; Cases : EnumCases list }
        
        interface INamed with
            member this.Name = this.Name
        
    [<RequireQualifiedAccess>]        
    type EnumCases =
        | Names of Id list
        | Anon of Expr
        | Call of Id * Expr
        
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
        
    type IncludeItem =
        { Name: string
        ; File: FileInfo option
        ; Integrated : bool }
        
        static member Create(file: FileInfo) =
            match file.Exists with
            | true ->
                { Name = file.Name
                ; File = Some file
                ; Integrated = false }
            | false ->
                { Name = file.Name
                ; File = None
                ; Integrated = false }
        
        static member Create(file: string) =
            IncludeItem.Create (FileInfo file)
            
        static member Create(dir: DirectoryInfo, name: string) =
            let path = Path.Join(dir.FullName, name)
            IncludeItem.Create(path)
            
    module IncludeItem =
        let resolve (dir: DirectoryInfo) (item: IncludeItem) =
            match item.File with
            | Some fi when fi.Exists ->
                item
            | _ ->
                IncludeItem.Create(dir, item.Name)
        
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
        { Declares: DeclareItem list
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