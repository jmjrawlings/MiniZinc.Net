namespace MiniZinc.AST

open System.Diagnostics

[<Struct>]
type BaseType =    
    | Int
    | Bool
    | String
    | Float

type Ident = string

[<Struct>]
[<DebuggerDisplay("_")>]
type WildCard =
    | WildCard

type Comment = string

type BinaryOp = string
    
type UnaryOp = string

type Op = string


[<Struct>]
// A value of 'T' or an identifier
type IdentOr<'T> =
    | Value of value:'T
    | Ident of name:string
            
type SolveMethod =
    | Satisfy = 0
    | Minimize = 1
    | Maximize = 2

type Expr =
    | WildCard      of WildCard  
    | Int           of int
    | Float         of float
    | Bool          of bool
    | String        of string
    | Id            of string
    | Op            of string
    | Bracketed     of Expr
    | Set           of SetExpr
    | SetComp       of SetCompExpr
    | Array1d       of Array1dExpr
    | Array1dIndex
    | Array2d       of Array2dExpr
    | Array2dIndex
    | ArrayComp     of ArrayCompExpr
    | ArrayCompIndex
    | Tuple         of TupleExpr
    | Record        of RecordExpr
    | UnaryOp       of IdentOr<UnaryOp> * Expr
    | BinaryOp      of Expr * IdentOr<BinaryOp> * Expr
    | Annotation
    | IfThenElse    of IfThenElseExpr
    | Let           of LetExpr
    | Call          of CallExpr
    | GenCall       of GenCallExpr 
    | Indexed       of expr:Expr * index: ArrayAccess list

and ArrayAccess = Expr list

and Annotation = Expr

and Annotations = Annotation list

and GenCallExpr =
    { Name: IdentOr<Op>
    ; Expr : Expr 
    ; Generators : Generator list }
    
and ArrayCompExpr =
    { Yield : Expr         
    ; From : Generator list }

and SetCompExpr =
    { Yield : Expr         
    ; From : Generator list }

and Generator =
    { Yield : IdentOr<WildCard> list
    ; From  : Expr
    ; Where : Expr option }

and CallExpr =
    { Name: IdentOr<Op>
    ; Args: Expr list }

and SetExpr = Expr list

and Array1dExpr = Expr list

and Array2dExpr = Array1dExpr list

and TupleExpr = Expr list

and RecordExpr = Map<string, Expr>
    
and SolveSatisfy =
    { Annotations : Annotations }
    
and SolveOptimise =
    { Annotations : Annotations
      Method : SolveMethod
      Objective : Expr }
    
and SolveItem =
    | Satisfy of SolveSatisfy
    | Optimise of SolveOptimise
    
    member this.Method =
        match this with
        | Satisfy _ -> SolveMethod.Satisfy
        | Optimise o -> o.Method
        
    member this.Annotations =
        match this with
        | Satisfy s -> s.Annotations
        | Optimise o -> o.Annotations

and IfThenElseExpr =
    { If     : Expr
    ; Then   : Expr
    ; ElseIf : Expr list
    ; Else   : Expr}

and NumExpr =
    | Int         of int
    | Float       of float
    | Id          of string
    | Op          of string
    | Bracketed   of NumExpr
    | Call        of CallExpr
    | IfThenElse  of IfThenElseExpr
    | Let         of LetExpr
    | UnaryOp     of IdentOr<NumericUnaryOp> * NumExpr
    | BinaryOp    of NumExpr * IdentOr<NumericBinaryOp> * NumExpr
    | ArrayAccess of NumExpr * ArrayAccess list
        
and NumericUnaryOp = string

and NumericBinaryOp = string

and Enum =
    { Name : string
    ; Annotations : Annotations
    ; Cases : EnumCase list }
    
and EnumCase =
    | Name of string
    | Expr of Expr

/// <summary>
/// Instantiation of a Type
/// </summary>
/// <remarks>
/// We have flattened out the `ti-expr` EBNF
/// rule here that a single class that convers
/// everything. 
/// </remarks>
and TypeInst =
    { Type        : Type
      Name        : string
      IsVar       : bool
      IsSet       : bool
      IsOptional  : bool
      Annotations : Annotations
      Dimensions  : Type list
      Value       : Expr option }

and Type =
    | Int
    | Bool
    | String
    | Float
    | Id        of string
    | Variable  of string
    | Tuple     of TypeInst list
    | Record    of TypeInst list
    | Set       of Expr list
    | Range     of lower:NumExpr * upper:NumExpr
        
and Item =
    | Include    of string
    | Enum       of Enum
    | Alias      of AliasItem
    | Constraint of ConstraintItem
    | Assign     of AssignItem
    | Declare    of DeclareItem
    | Solve      of SolveItem
    | Predicate  of PredicateItem
    | Function   of FunctionItem
    | Test       of TestItem
    | Output     of OutputItem
    | Annotation of AnnotationItem
    | Comment    of string

and AnnotationItem = CallExpr

and ConstraintItem = Expr
        
and IncludeItem = string
        
and PredicateItem = OperationItem

and TestItem = OperationItem

and AliasItem = TypeInst

and OutputItem = Expr

and OperationItem =
    { Name: string
      Parameters : TypeInst list
      Annotations : Annotations
      Body: Expr option }
    
and FunctionItem =
    { Name: string
      Returns : TypeInst
      Parameters : TypeInst list
      Body: Expr option }

and Test = unit

and AssignItem = string * Expr

and DeclareItem = TypeInst

and LetItem =
    | Declare of DeclareItem
    | Constraint of ConstraintItem
    
and LetExpr =
    { Locals: LetItem list
      In: Expr }


