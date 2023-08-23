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
open Microsoft.FSharp.Collections
open System.Collections.Generic

[<AutoOpen>]
module rec Ast =
        
    type ParseError =
        { Message : string
        ; Line    : int64
        ; Column  : int64
        ; Index   : int64
        ; Trace   : string }
   
    type Ident = string
    
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
    type IdentOr<'T> =
        | Ident of id:string
        | Other of value:'T
        
    type VarKind =
        | AssignedPar = 0
        | UnassignedPar = 1
        | AssignedVar = 2
        | UnassignedVar = 3
                  
    type SolveMethod =
        | Satisfy = 0
        | Minimize = 1
        | Maximize = 2

    type NumUnOp =
        | Plus = 33
        | Minus = 34
        
    type UnOp =
        | Plus = 33
        | Minus = 34
        | Not = 35
        
    type NumBinOp =
        | Add = 23
        | Subtract = 24
        | Multiply = 25
        | Div = 26
        | Mod = 27
        | Divide = 28
        | Exponent = 30
        
    type BinOp =
        | Equivalent = 1
        | Implies = 2
        | ImpliedBy = 3
        | Or = 4
        | Xor = 5
        | And = 6
        | LessThan = 7
        | GreaterThan = 8
        | LessThanEqual = 9
        | GreaterThanEqual = 10
        | Equal = 11
        | NotEqual = 12
        | In = 13
        | Subset = 14
        | Superset = 15
        | Union = 16
        | Diff = 17
        | SymDiff = 18
        | ClosedRange = 19
        | LeftOpenRange = 20
        | RightOpenRange = 21
        | OpenRange = 22
        | Add = 23
        | Subtract = 24
        | Multiply = 25
        | Div = 26
        | Mod = 27
        | Divide = 28
        | Intersect = 29
        | Exponent = 30
        | Default = 31
        | Concat = 32
        | TildeNotEqual = 36 
        | TildeEqual = 37
        | TildeAdd = 38 
        | TildeSubtract = 39    
        | TildeMultiply = 40  

    type Op =     
        | Equivalent = 1
        | Implies = 2
        | ImpliedBy = 3
        | Or = 4
        | Xor = 5
        | And = 6
        | LessThan = 7
        | GreaterThan = 8
        | LessThanEqual = 9
        | GreaterThanEqual = 10
        | Equal = 11
        | NotEqual = 12
        | In = 13
        | Subset = 14
        | Superset = 15
        | Union = 16
        | Diff = 17
        | SymDiff = 18
        | ClosedRange = 19
        | LeftOpenRange = 20
        | RightOpenRange = 21
        | OpenRange = 22
        | Add = 23
        | Subtract = 24
        | Multiply = 25
        | Div = 26
        | Mod = 27
        | Divide = 28
        | Intersect = 29
        | Exponent = 30
        | Default = 31
        | Concat = 32
        | Plus = 33
        | Minus = 34
        | Not = 35
        | TildeNotEqual = 36 
        | TildeEqual = 37
        | TildeAdd = 38 
        | TildeSubtract = 39    
        | TildeMultiply = 40

    [<RequireQualifiedAccess>] 
    type Expr =
        | WildCard       of WildCard
        | Absent         of Absent
        | Int            of int
        | Float          of float
        | Bool           of bool
        | String         of string
        | Ident          of string
        | Bracketed      of Expr
        | ClosedRange    of Expr*Expr
        | LeftOpenRange  of Expr
        | RightOpenRange of Expr
        | Set            of Expr list
        | SetComp        of CompExpr
        | RecordAccess   of RecordAccessExpr
        | TupleAccess    of TupleAccessExpr
        | ArrayAccess    of ArrayAccessExpr
        | Array1DLit     of Expr[]
        | Array2DLit     of Expr[,]
        | Array3DLit     of Expr[,,]
        | IndexedArray1DLit of List<IndexedArrayItem>
        | IndexedArrayComp of IndexedArrayComp
        | Array1D        of ArrayDim * Expr[]
        | Array2D        of ArrayDim * ArrayDim * Expr[]
        | Array3D        of ArrayDim * ArrayDim * ArrayDim * Expr[]
        | Array4D        of ArrayDim * ArrayDim * ArrayDim * ArrayDim * Expr[]
        | Array5D        of ArrayDim * ArrayDim * ArrayDim * ArrayDim * ArrayDim * Expr[]
        | Array6D        of ArrayDim * ArrayDim * ArrayDim * ArrayDim * ArrayDim * ArrayDim * Expr[]
        | ArrayComp      of CompExpr
        | Tuple          of TupleExpr
        | Record         of RecordExpr
        | UnaryOp        of UnaryOpExpr
        | BinaryOp       of BinaryOpExpr
        | IfThenElse     of IfThenElseExpr
        | Let            of LetExpr
        | Call           of CallExpr
        | GenCall        of GenCallExpr
    
    type CallExpr =
        Ident * Expr list
    
    type RecordAccessExpr =
        Expr * string
    
    type TupleAccessExpr =
        Expr * uint8
    
    type ArrayAccessExpr =
        Expr * ArraySlice
    
    type BinaryOpExpr =
        Expr * BinOp * Expr
        
    type UnaryOpExpr =
        UnOp * Expr
    
    type ArraySlice =
        ValueOption<Expr> list

    type Annotation =
        Expr
                
    type Annotations =
        Annotation list

    type IndexedArrayItem =
        KeyValuePair<Expr, Expr>
        
    type IndexedArray1D =
        List<IndexedArrayItem>
    
    type GenCallExpr =
        { Id : string
        ; From : Generator list 
        ; Yields : Expr }
        
    type CompExpr =
        { Yields : Expr
        ; IsSet  : bool
        ; From   : Generator list }
        
    type IndexedArrayComp =
        { Yields : KeyValuePair<Expr, Expr>
        ; From   : Generator list }

    type Generator =
        { Yields : IdentOr<WildCard> list
        ; From  : Expr  
        ; Where : Expr option }

    type TupleExpr =
        Expr list
        
    type RecordExpr =
        NamedExpr list
        
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

    // [<RequireQualifiedAccess>]
    // type NumExpr =
    //     | Int          of int
    //     | Float        of float
    //     | Id           of Ident
    //     | Bracketed    of NumExpr
    //     | Call         of CallExpr
    //     | RecordAccess of string * NumExpr
    //     | TupleAccess  of uint8 * NumExpr
    //     | IfThenElse   of IfThenElseExpr
    //     | Let          of LetExpr
    //     | UnaryOp      of IdOr<NumericUnaryOp> * NumExpr
    //     | BinaryOp     of NumExpr * IdOr<NumericBinaryOp> * NumExpr
    //     | ArrayAccess  of ArrayAccess * NumExpr
    
    type EnumType =
        { Name : Ident
        ; Annotations : Annotations
        ; Cases : EnumCases list }
        
        interface INamed with
            member this.Name = this.Name
        
    [<RequireQualifiedAccess>]        
    type EnumCases =
        | Names of Ident list
        | Anon of Expr
        | Call of Ident * Expr
        
    type TypeInst =
        { Name : string
          Type : Type
          Annotations : Annotations
          IsVar : bool
          IsSet : bool
          IsOptional : bool
          IsArray : bool
          IsInstanced : bool
          PostFix : Annotations
          Value : Expr option }
        
        /// True if this TypeInst is a parameter (not a variable)
        member this.IsPar =
            not this.IsVar
            
        member this.IsCollection =
            this.IsSet || this.IsArray
            
        member this.IsSingleton =
            not this.IsCollection
        
        static member Empty =
            { Type = Type.Any
            ; IsVar = false
            ; IsSet = false
            ; IsOptional = false
            ; IsArray = false
            ; IsInstanced = false
            ; Name = ""
            ; PostFix = []
            ; Annotations = []
            ; Value = None }
            
        
    type ITyped =
        abstract member TypeInst: TypeInst

    [<RequireQualifiedAccess>]        
    type Type =
        | Int
        | Bool
        | String
        | Float
        | Ann
        | Annotation
        | Any
        | Generic   of Ident
        | Ident     of Ident
        | Expr      of Expr
        | Concat    of TypeInst list
        | Tuple     of TypeInst list
        | Record    of TypeInst list
        | Array1D   of ArrayDim * TypeInst
        | Array2D   of ArrayDim * ArrayDim * TypeInst
        | Array3D   of ArrayDim * ArrayDim * ArrayDim * TypeInst
        | Array4D   of ArrayDim * ArrayDim * ArrayDim * ArrayDim * TypeInst
        | Array5D   of ArrayDim * ArrayDim * ArrayDim * ArrayDim * ArrayDim * TypeInst
        | Array6D   of ArrayDim * ArrayDim * ArrayDim * ArrayDim * ArrayDim * ArrayDim * TypeInst
       
    type ArrayDim = Type
        
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
        | Enum       of EnumType
        | Synonym    of TypeAlias
        | Constraint of ConstraintExpr
        | Assign     of NamedExpr
        | Declare    of TypeInst
        | Solve      of SolveItem
        | Test       of TestItem
        | Function   of FunctionType
        | Output     of OutputExpr
        | Annotation of AnnotationType
        | Comment    of string

    type ConstraintExpr =
        { Expr: Expr
        ; Annotations: Annotations }
      
    type NamedExpr =
        KeyValuePair<Ident, Expr>
           
    type TestItem =
        { Name: string
        ; Parameters : TypeInst list
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

    type AnnotationType =
        { Name: string
        ; Params: TypeInst list
        ; Body: Expr option }
    
    type FunctionType =
        { Name: string
        ; Returns : TypeInst
        ; Annotations : Annotations
        ; Parameters : TypeInst list
        ; Ann : string
        ; Body: Expr option }
                
        interface INamed with
            member this.Name = this.Name
        
    type Test =
        unit
    
    type OutputExpr =
        { Expr: Expr; Annotation: Annotation option }
        
    type LetLocal =
        Choice<TypeInst, ConstraintExpr>
        
    type LetExpr =
        { Declares: TypeInst list
        ; Constraints : ConstraintExpr list
        ; Body: Expr }
        
    type Ast = Item list    

    /// Things that a name can be bound to
    [<RequireQualifiedAccess>]
    type Binding =
        | Variable of TypeInst
        | Expr     of Expr
        | Enum     of EnumType
        | Type     of TypeAlias
        | Function of FunctionType
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
        ; Declared   : Map<string, TypeInst>
        ; Undeclared : Map<string, Expr>
        ; Enums      : Map<string, EnumType> 
        ; Synonyms   : Map<string, TypeAlias> 
        ; Functions  : Map<string, FunctionType>        
        ; Conflicts  : Map<string, Binding list> }
        
        member this.Variables =
            this.Declared
            |> Map.filter (fun _ x -> x.IsVar)
            
        member this.Parameters =
            this.Declared
            |> Map.filter (fun _ x -> x.IsPar)