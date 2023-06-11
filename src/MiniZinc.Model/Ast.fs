(*

Ast.fs

A (mostly) 1:1 mapping from the MiniZinc EBNF Grammar found here:
https://www.minizinc.org/doc-2.7.4/en/spec.html#full-grammar.  

Some rules have been simplified for ease of use.

*)

namespace MiniZinc

open System
open System.Diagnostics

type Id = string

[<Struct>]
[<DebuggerDisplay("_")>]
type WildCard =
    | WildCard

type Comment =
    string

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
              
type SolveType =
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

type [<RequireQualifiedAccess>] Expr =
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

and UnaryOpExpr =
    IdOr<UnaryOp> * Expr
    
and BinaryOpExpr =
    Expr * IdOr<BinaryOp> * Expr

and IndexExpr =
    | Index of Expr * ArrayAccess list
    
and ArrayAccess =
    | Access of Expr list

and Annotation =
    Expr

and Annotations =
    Annotation list

and GenCallExpr =
    { Operation: IdOr<Op>
    ; From : Generator list 
    ; Yields : Expr }
    
and ArrayCompExpr =
    { Yields : Expr         
    ; From : Generator list }

and SetCompExpr =
    { Yields : Expr         
    ; From : Generator list }

and Generator =
    { Yields : IdOr<WildCard> list
    ; From  : Expr  
    ; Where : Expr option }

and CallExpr =
    { Function: IdOr<Op>
    ; Args: Arguments }

and Array1dExpr =
    | Array1d of Expr list

and Array2dExpr =
    | Array2d of (Expr list) list

and TupleExpr =
    | TupleExpr of Expr list
    
and RecordExpr =
    | RecordExpr of (Id * Expr) list
    
and SolveMethod =
    | Sat of Annotations
    | Min of Expr * Annotations
    | Max of Expr * Annotations
    
    member this.SolveType =
        match this with
        | Sat _ -> SolveType.Satisfy
        | Min _ -> SolveType.Minimize
        | Max _ -> SolveType.Maximize
        
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
        

and IfThenElseExpr =
    { If     : Expr
    ; Then   : Expr
    ; ElseIf : (Expr * Expr) list
    ; Else   : Expr}

and NumericExpr =
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

and IncludeItem =
    | Include of string

and EnumItem =
    { Name : Id
    ; Annotations : Annotations
    ; Cases : EnumCase list }
    
and EnumCase =
    | Name of Id
    | Expr of Expr
    
and TypeInst =
    { Type  : BaseType
      Inst  : Inst
      IsSet : bool
      IsOptional : bool
      IsArray : bool }
    
and [<RequireQualifiedAccess>] BaseType =
    | Int
    | Bool
    | String
    | Float
    | Id       of Id
    | Variable of Id
    | Tuple    of TupleType
    | Record   of RecordType
    | Literal  of SetLiteral 
    | Range    of Range
    | List     of ListType
    | Array    of ArrayType

 and RecordType =
     | RecordType of (Id * TypeInst) list
     
and TupleType =
    | TupleType of TypeInst list
    
and Range =
    NumericExpr * NumericExpr
    
and ListType =
    | ListType of TypeInst
    
and ArrayType =
    | ArrayType of TypeInst list * TypeInst
    
and SetLiteral =
    | SetLiteral of Expr list
        
and [<RequireQualifiedAccess>] Item =
    | Include    of IncludeItem
    | Enum       of EnumItem
    | Synonym    of SynonymItem
    | Constraint of ConstraintItem
    | Assign     of AssignItem
    | Declare    of DeclareItem
    | Solve      of SolveMethod
    | Predicate  of PredicateItem
    | Function   of FunctionItem
    | Test       of TestItem
    | Output     of OutputItem
    | Annotation of AnnotationItem
    | Comment    of string

and AnnotationItem =
    CallExpr

and ConstraintItem =
    { Expr: Expr }
    
and Parameters =
    Parameter list
    
and Parameter =
    Id * TypeInst

and Argument =
    Expr
    
and NamedArg =
    Id * Expr

and NamedArgs =
    NamedArg list
    
and Arguments =
    Argument list
        
and PredicateItem =
    { Name: string
    ; Parameters : Parameters
    ; Annotations : Annotations
    ; Body: Expr option }
       
and TestItem =
    { Name: string
    ; Parameters : Parameters
    ; Annotations : Annotations
    ; Body: Expr option }

and SynonymItem =
    { Id : string
    ; Annotations : Annotations
    ; TypeInst: TypeInst }

and OutputItem =
    { Expr: Expr }

and OperationItem =
    { Name: string
      Parameters : Parameters
      Annotations : Annotations
      Body: Expr option }
    
and FunctionItem =
    { Name: string
      Returns : TypeInst
      Parameters : Parameters
      Body: Expr option }
    
and Test =
    unit

and AssignItem =
    NamedArg

and DeclareItem =
    { Name: string
    ; Type: TypeInst
    ; Annotations: Annotations
    ; Expr: Expr option }

and LetItem =
    | Decl of DeclareItem
    | Cons of ConstraintItem
    
and LetExpr =
    { Locals: LetItem list
      In: Expr }
    
type Ast =
    { Includes:    IncludeItem list 
    ; Enums:       EnumItem list 
    ; Synonyms:    SynonymItem list 
    ; Constraints: ConstraintItem list 
    ; Assigns:     AssignItem list 
    ; Declares:    DeclareItem list
    ; Solves:      SolveMethod list
    ; Predicates:  PredicateItem list 
    ; Functions:   FunctionItem list 
    ; Tests:       TestItem list 
    ; Outputs:     OutputItem list 
    ; Annotations: AnnotationItem list 
    ; Comments:    string list }