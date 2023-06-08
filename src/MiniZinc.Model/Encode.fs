namespace MiniZinc

open System
open System.Collections.Generic
open System.Text
open MiniZinc

module Encode =
     
     let operators : Map<int, string> =
         [ Op.Add, "+"         
         ; Op.Subtract, "-"          
         ; Op.Multiply, "*"         
         ; Op.Divide, "/"                  
         ; Op.Exponent, "^"         
         ; Op.TildeAdd, "~+"        
         ; Op.TildeSubtract, "~-"        
         ; Op.TildeMultiply, "~*"        
         ; Op.TildeDivide, "~/"        
         ; Op.Div, "div"       
         ; Op.Mod, "mod"       
         ; Op.TildeDiv, "~div"       
         ; Op.Equivalent, "<->"       
         ; Op.Implies, "->"        
         ; Op.ImpliedBy, "<-"        
         ; Op.Or, "\/"        
         ; Op.And, "/\\"       
         ; Op.LessThanEqual, "<="        
         ; Op.GreaterThanEqual, ">="        
         ; Op.EqualEqual, "=="        
         ; Op.LessThan, "<"         
         ; Op.GreaterThan, ">"         
         ; Op.Equal, "="         
         ; Op.NotEqual, "!="        
         ; Op.TildeEqual, "~="        
         ; Op.TildeNotEqual, "~!="       
         ; Op.DotDot, ".."        
         ; Op.PlusPlus, "++"        
         ; Op.Xor, "xor"       
         ; Op.In, "in"        
         ; Op.Subset, "subset"    
         ; Op.Superset, "superset"  
         ; Op.Union, "union"     
         ; Op.Diff, "diff"      
         ; Op.SymDiff, "symdiff"   
         ; Op.Intersect, "intersect" 
         ; Op.Default, "default" ]
         |> List.map (fun (op, s) -> (int op, s))
         |> Map.ofList
        

/// Encode MiniZinc to String
type MiniZincEncoder() =
    
    let mutable indentLevel = 0

    let contexts = Stack<IDisposable>()

    let builder = StringBuilder()
    
    member this.String =
        builder.ToString()
    
    /// Pop the last context off the stack        
    member private this.pop() =
        contexts.Pop()
        this
            
    /// Add a new context to the stack            
    member private this.push context =
        contexts.Push context
        this

    /// Add a context for 1 level of indentation            
    member this.indented () =
        indentLevel <- indentLevel + 1
        let context = 
            { new IDisposable with
                 member IDisposable.Dispose () =
                     indentLevel <- indentLevel - 1
                     () }
        this.push context
        
    member this.indent() =
        indentLevel <- indentLevel + 1
        this
        
    member this.dedent() =
        indentLevel <- indentLevel - 1
        this       
        
    /// Add a context that surrounds with the given strings                
    member this.enclose (prefix: string) (suffix: string) =
        builder.Append prefix
        let context =
            { new IDisposable with
                 member IDisposable.Dispose () =
                     builder.Append suffix
                     () }
        this.push context
        context

    member this.write (s: string) =
        for i in 1 .. indentLevel do
            builder.Append "\t"
        builder.Append s
        this
        
    member this.writen (s: string) =
        builder.AppendLine s
        this

    member this.writen () =
        builder.AppendLine ""
        this
            
    member this.writet () =
        this.write ";"
        
    member this.writet (s: string) =
        this.write s
        this.write ";"
        
    member this.writetn () =
        builder.AppendLine ";"
        this
        
    member this.writetn (s: string) =
        this.write s
        builder.AppendLine ";"
        this
    
    member this.writeif (cond: bool) (s: string) =
        if cond then
            this.write s
        else                        
            this
            
    member this.writes (s: string) =
        this.write s
        builder.Append " "
        this
        
    member this.write (x: EnumItem) : MiniZincEncoder =
        this.writes "enum"
        this.write x.Name
        
        match x.Cases with
        | [] ->
            this.writetn()
        | cases ->
            this.write "{"
            let n = cases.Length - 1
            for i, case in Seq.indexed cases do
                match case with
                | Name id ->
                    this.write id
                | Expr expr ->
                    this.write expr
                this.writeif (i < n) ", "
            this.write "}"
            this.writetn()
                
    member this.write (syn: SynonymItem) =
        this.write "type "
        this.write syn.Id
        this.write " = "
        this.write syn.TypeInst
        this.writetn()

    member this.write (x: Annotations) : MiniZincEncoder =
        this
        
    member this.write (x: TypeInst) : MiniZincEncoder =
        this.write x.Inst
        this.write " "
        this.writeif x.IsSet "set of "
        this.writeif x.IsOpt "opt "
        this.write x.Type
        
    member this.write (x: Inst) : MiniZincEncoder =
        match x with
        | Inst.Var ->
            this.write "var"
        | Inst.Par ->
            this.write "par"
        | _ ->
            this
        
    member this.write(x: BaseType) =
        match x with
        | Int ->
            this.write "int"
        | Bool ->
            this.write "bool"
        | String ->
            this.write "string"
        | Float ->
            this.write "float"
        | Id s ->
            this.write s
        | Variable s ->
            this.write s
        | Tuple fields ->
            this.write "("
            let n = fields.Length - 1
            for i, field in Seq.indexed fields do
                this.write field
                this.writeif (i < n) ", "
            this.write ")"
        | Record fields ->
            this.write "record("
            this.write fields
            this.write ")"            
        | Literal exprs ->
            this.write "{"
            let n = exprs.Length - 1
            for i, expr in Seq.indexed exprs do
                this.write expr
                this.writeif (i < n) ", "
            this.write "}"
        | Range (lo, hi) ->
            this.write lo
            this.write ".."
            this.write hi
        | List ti ->
            this.write "list of "
            this.write ti
        | Array (dims, typ) ->
            this.write "array["
            let n = dims.Length - 1
            for (i, dim) in Seq.indexed dims do
                this.write dim
                this.writeif (i < n) ", "
            this.write "] of "
            this.write typ
                    
    member this.write (x: NumericExpr) : MiniZincEncoder =
        this.write ""
    
    member this.write (x: float) =
        this.write (string x)
        
    member this.write (x: WildCard) =
        this.write "_"
    
    member this.write (x: bool) =
        this.write (if x then "true" else "false")
        
    member this.write (x: Op) =
        let s = Encode.operators[int x]
        this.write s

    member this.write (x: BinaryOp) =
        let s = Encode.operators[int x]
        this.write s
        
    member this.write (x: UnaryOp) =
        let s = Encode.operators[int x]
        this.write s        
                
    member this.write (x: SetCompExpr) =
        this
        
    member this.write (SetExpr.Set exprs) =
        this.write "{"
        for expr in exprs do
            this.write expr
        this.write "}"
        
    member this.write (Array1dExpr.Array1d x) =
        this
        
    member this.write (Array2dExpr.Array2d x) =
        this
        
    member this.write (x: ArrayCompExpr) =
        this
        
    member this.write (TupleExpr.Tuple items) =
        this.write "("
        for item in items do
            this.write item
        this.write ")"

    member this.writeSep (xs: 't list) (write: 't -> MiniZincEncoder) sep =
        let n = xs.Length
        let mutable i = 0
        for x in xs do
            i <- i + 1
            write(x)
            this.writeif (i < n) sep
        this
    
    member this.write (expr: Expr, name: string) =
        this.write name
        this.write ": "
        this.write expr
        
    member this.write (RecordExpr.Record fields) =
        this.write "("
        this.writeSep fields this.write ", "
        this.write ")"
        
    member this.write ((id, expr): UnaryOpExpr) =
        match id with
        | IdOr.Id x -> this.write x
        | IdOr.Val x -> this.write (int x)
        this.write " "
        this.write expr
        
    member this.write ((left, op, right): BinaryOpExpr) =
        this.write left
        this.write " "
        op.fold this.write this.write
        this.write " "
        this.write right
        
    member this.write (x: IfThenElseExpr) =
        this.write "if "
        this.write x.If
        this.write " then "
        this.write x.Then
        
        for ifCase, thenCase in x.ElseIf do
            this.write "elseif "
            this.write ifCase
            this.write " then "
            this.write thenCase
            
        this.write " "
        this.write x.Else
        this.write " endif"
        
    member this.write (x: LetExpr) =
        this.writen "let {"
        this.indent()
        for local in x.Locals do
            match local with
            | Decl decl -> this.write decl
            | Cons cons -> this.write cons
        this.dedent()
        this.writetn "} in "
        this.write x.In

    member this.writesep (sep: string) (xs: 't list) (write: 't -> MiniZincEncoder) : MiniZincEncoder=
        let n = xs.Length
        let mutable i = 0
        for x in xs do
            i <- i + 1
            write x
            this.writeif (i < n) sep
        this
        
    member this.write (x: Generator list) =
        this.writesep ", " x this.write
            
    member this.write (x: GenCallExpr) =
        this.write x.Operation
        this.write "("
        this.write x.From
        this.write ")"
        this.write "("
        this.write x.Yields
        this.write ")"
        
    member this.write (x: IdOr<Op>) : MiniZincEncoder =
        match x with
        | IdOr.Id x -> this.write x
        | IdOr.Val x -> this.write x
        
    member this.write (x: IdOr<BinaryOp>) =
        match x with
        | IdOr.Id x -> this.write x
        | IdOr.Val x -> this.write x

    member this.write (x: IdOr<WildCard>) =
        match x with
        | IdOr.Id x -> this.write x
        | IdOr.Val x -> this.write x        
        
    member this.write (x: Generator) =
        this.writesep ", " x.Yields this.write
        this.write " in "
        this.write x.From
        match x.Where with
        | Some cond ->
            this.write " where "
            this.write cond
         | None ->
             this
    
    member this.write (x: ArrayAccess) =
        match x with
        | ArrayAccess.Access exprs ->
            this.writesep ", " exprs this.write
        
    member this.write (x: IndexExpr) =
        match x with
        | IndexExpr.Index (expr, access) ->
            this.write expr
            this.write "["
            this.writesep ", " access this.write
            this.write "]"
                        
    member this.write (x: Expr) : MiniZincEncoder =
        match x with
        | Expr.WildCard      x -> this.write "_"  
        | Expr.Int           x -> this.write (string x)
        | Expr.Float         x -> this.write x
        | Expr.Bool          x -> this.write x
        | Expr.String        x -> this.write x
        | Expr.Id            x -> this.write x
        | Expr.Op            x -> this.write x
        | Expr.Bracketed     x -> this.write x
        | Expr.Set           x -> this.write x
        | Expr.SetComp       x -> this.write x
        | Expr.Array1d       x -> this.write x
        | Expr.Array1dIndex    -> this
        | Expr.Array2d       x -> this.write x
        | Expr.Array2dIndex    -> this
        | Expr.ArrayComp     x -> this.write x
        | Expr.ArrayCompIndex  -> this
        | Expr.Tuple         x -> this.write x
        | Expr.Record        x -> this.write x
        | Expr.UnaryOp       x -> this.write x
        | Expr.BinaryOp      x -> this.write x
        | Expr.Annotation      -> this
        | Expr.IfThenElse    x -> this.write x
        | Expr.Let           x -> this.write x
        | Expr.Call          x -> this.write x
        | Expr.GenCall       x -> this.write x 
        | Expr.Indexed       x -> this.write x
        
    member this.write (x: DeclareItem) =
        this.write x.Type
        this.write ": "
        this.write x.Name
        match x.Expr with
        | None ->
            this.writetn()
        | Some expr ->
            this.write " = "
            this.write expr
            this.writetn()
            
    member this.write (x: SolveType) : MiniZincEncoder =
        match x with
        | SolveType.Satisfy ->
            this.write "satisfy"
        | SolveType.Minimize ->
            this.write "minimize"
        | SolveType.Maximize ->
            this.write "maximize"
        | _ ->
            this
        
    member this.write (pred: PredicateItem) =
        this.writes "predicate"
        this.write pred.Name
        this.write "("
        this.write pred.Parameters
        this.write ")"
        if pred.Body.IsSome then
            this.writen " = "
            this.indent()
            this.write pred.Body.Value
            this.dedent()
            this.writetn()
        else
            this.writetn()
    
    member this.write (name: string, ti: TypeInst) =
        this.write name
        this.writes ":"
        this.write ti
        
    member this.write (x: Map<string, TypeInst>) : MiniZincEncoder =
        let n = x.Count - 1
        for i, kv in Seq.indexed x do
            this.write kv.Value
            this.write ": "
            this.write kv.Key
            this.writeif (i < n) ", "
        this
    
    member this.write (x: FunctionItem) =
        this.writes "function "
        this.write x.Returns
        this.write ": "
        this.write x.Name
        this.write "(" 
        this.write x.Parameters
        this.write ")"
        
        match x.Body with
        | None ->
            this.writetn()
        | Some body ->
            this.writen " = "
            this.indent()
            this.write body
            this.dedent()
            this.writetn()
                
    member this.write (x: CallExpr) =
        this.write x.Function
        this.write "("
        this.writesep ", " x.Args this.write
        this.write ")"
        
    member this.write (x: ConstraintItem) : MiniZincEncoder =
        this.write "constraint "
        this.write x.Expr
        this.writetn ()
        
    member this.write (x: IncludeItem) : MiniZincEncoder =
        match x with
        | IncludeItem.Include s ->
            this.writetn $"include \"{s}\""
        
    member this.write (x: OutputItem) =
        this.write "output "
        this.write x.Expr
        this.writetn()
        
    member this.write (x: SolveMethod) =
        match x with
        | Sat _ ->
            this.writetn "solve satisfy"
        | Max (expr, _) ->
            this.write "solve maximize "
            this.write expr
            this.writetn()
        | Min (expr, _) ->
            this.write "solve minimize "
            this.write expr
            this.writetn()