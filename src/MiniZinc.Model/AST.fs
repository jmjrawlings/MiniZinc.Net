namespace MiniZinc

open System.Diagnostics

// An Identifier
type Id = string


[<Struct>]
[<DebuggerDisplay("_")>]
type WildCard =
    | WildCard

type Comment =
    string

[<Struct>]
// A value of 'T' or an identifier
type IdOr<'T> =
    | Value of value:'T
    | Ident of name:string
              
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
    | UnaryOp       of IdOr<UnaryOp> * Expr
    | BinaryOp      of Expr * IdOr<BinaryOp> * Expr
    | Annotation
    | IfThenElse    of IfThenElseExpr
    | Let           of LetExpr
    | Call          of CallExpr
    | GenCall       of GenCallExpr 
    | Indexed       of expr:Expr * index: ArrayAccess list

and ArrayAccess =
    Expr list

and Annotation =
    Expr

and Annotations =
    Annotation list

and GenCallExpr =
    { Name: IdOr<Op>
    ; Expr : Expr 
    ; Generators : Generator list }
    
and ArrayCompExpr =
    { Yield : Expr         
    ; From : Generator list }

and SetCompExpr =
    { Yield : Expr         
    ; From : Generator list }

and Generator =
    { Yield : IdOr<WildCard> list
    ; From  : Expr
    ; Where : Expr option }

and CallExpr =
    { Name: IdOr<Op>
    ; Args: Expr list }

and SetExpr =
    Expr list

and Array1dExpr =
    Expr list

and Array2dExpr =
    Array1dExpr list

and TupleExpr =
    Expr list

and RecordExpr =
    Map<string, Expr>
    
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

and NumericExpr =
    | Int         of int
    | Float       of float
    | Id          of Id
    | Op          of Id
    | Bracketed   of NumericExpr
    | Call        of CallExpr
    | IfThenElse  of IfThenElseExpr
    | Let         of LetExpr
    | UnaryOp     of IdOr<NumericUnaryOp> * NumericExpr
    | BinaryOp    of NumericExpr * IdOr<NumericBinaryOp> * NumericExpr
    | ArrayAccess of NumericExpr * ArrayAccess list

and IncludeItem =
    string

and EnumItem =
    { Name : string
    ; Annotations : Annotations
    ; Cases : EnumCase list }
    
and EnumCase =
    | Name of string
    | Expr of Expr

and BaseType =
    | Int
    | Bool
    | String
    | Float
    | Id        of string
    | Variable  of string
    | Tuple     of TypeInst list
    | Record    of TypeInst list
    | Set       of Expr list
    | Range     of lower:NumericExpr * upper:NumericExpr


/// <summary>
/// Instantiation of a Type
/// </summary>
/// <remarks>
/// We have flattened out the `ti-expr` EBNF
/// rule here that a single class that convers
/// everything. 
/// </remarks>
and TypeInst =
    { Type        : BaseType
      Name        : string
      IsVar       : bool
      IsSet       : bool
      IsOptional  : bool
      Annotations : Annotations
      Dimensions  : BaseType list
      Value       : Expr option }

        
and Item =
    | Include    of IncludeItem
    | Enum       of EnumItem
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

and AnnotationItem =
    CallExpr

and ConstraintItem =
    Expr
        
and PredicateItem =
    OperationItem

and TestItem =
    OperationItem

and AliasItem =
    TypeInst

and OutputItem =
    Expr

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

and Test =
    unit

and AssignItem =
    string * Expr

and DeclareItem =
    TypeInst

and LetItem =
    | Declare of DeclareItem
    | Constraint of ConstraintItem
    
and LetExpr =
    { Locals: LetItem list
      In: Expr }
