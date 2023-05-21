namespace MiniZinc

open System.Diagnostics

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
    | Id_ of id:string
    | Value_ of value:'T

    member this.Value =
        match this with
        | Value_ v -> v
        | _ -> Unchecked.defaultof<'T>
        
    member this.Id =
        match this with
        | Id_ id -> id
        | _ -> ""
        
    member this.IsId =
        match this with
        | Id_ _ -> true | _ -> false

    member this.IsValue =
        match this with
        | Value_ _ -> true | _ -> false
              
type Inst =
    | Var = 0
    | Par = 1
              
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
    | Op            of Op
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
    | Sat of SolveSatisfy
    | Opt of SolveOptimise
    
    member this.Method =
        match this with
        | Sat _ -> SolveMethod.Satisfy
        | Opt o -> o.Method
        
    member this.Annotations =
        match this with
        | Sat s -> s.Annotations
        | Opt o -> o.Annotations
        
    static member Satisfy =
        Sat {Annotations = [] }
        
    static member Minimize(expr) =
        { Annotations = []
        ; Objective =  expr
        ; Method=SolveMethod.Minimize  }
        |> Opt
        
    static member Maximize(expr) =
        { Annotations = []
        ; Objective =  expr
        ; Method=SolveMethod.Maximize  }
        |> Opt
        

and IfThenElseExpr =
    { If     : Expr
    ; Then   : Expr
    ; ElseIf : Expr list
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
    string

and EnumItem =
    { Name : Id
    ; Annotations : Annotations
    ; Cases : EnumCase list }
    
and EnumCase =
    | Name of Id
    | Expr of Expr

and MzType =
    | Array of ArrayType
    | Base of BaseType
    
    static member DetermineInst(ty: MzType) =
        match ty with
        | Array arr -> MzType.DetermineInst arr.Type
        | Base ty -> MzType.DetermineInst ty
    
    static member DetermineInst(ty:BaseType) =
        match ty.Inst with
        | Inst.Var ->
            Inst.Var
        | _ ->
            MzType.DetermineInst ty.Type
        
    static member DetermineInst(ty: BaseTypeTail) =
        match ty with
        | Int 
        | Bool 
        | String 
        | Float 
        | Id _ 
        | Literal _ 
        | Range _
        | Variable _ ->
            Inst.Par
        
        // Any var item means a var tuple    
        | Tuple items ->
            items
            |> Seq.map MzType.DetermineInst
            |> Seq.exists ((=) Inst.Var)
            |> function
                | true -> Inst.Var
                | false -> Inst.Par

        // Any var field means a var record                            
        | Record fields ->
            fields
            |> Map.values
            |> Seq.map MzType.DetermineInst
            |> Seq.exists ((=) Inst.Var)
            |> function
                | true -> Inst.Var
                | false -> Inst.Par

    
and BaseType =
    { Type       : BaseTypeTail
      Inst       : Inst
      IsSet      : bool
      IsOptional : bool }
    
and ArrayType =
    { Dimensions: MzType list
    ; Type: BaseType }

and BaseTypeTail = 
    | Int
    | Bool
    | String
    | Float
    | Id       of Id
    | Variable of Id
    | Tuple    of TupleType
    | Record   of RecordType
    | Literal  of Expr list 
    | Range    of RangeExpr

 and RecordType =
     Map<Id, MzType>
     
and TupleType =
    MzType list
    
and RangeExpr =
    NumericExpr*NumericExpr
        
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
    { Type : MzType
    ; Annotations : Annotations
    ; Name : Id }

and OutputItem =
    Expr

and OperationItem =
    { Name: string
      Parameters : Map<string, MzType>
      Annotations : Annotations
      Body: Expr option }
    
and FunctionItem =
    { Name: string
      Returns : MzType
      Parameters : Map<string, MzType>
      Body: Expr option }

and Test =
    unit

and AssignItem =
    string * Expr

and DeclareItem =
    { Name: Id
    ; Type: MzType
    ; Inst: Inst
    ; Annotations: Annotations
    ; Value : Expr option }

and LetItem =
    | Decl of DeclareItem
    | Cons of ConstraintItem
    
and LetExpr =
    { Locals: LetItem list
      In: Expr }

type Ast = Item list