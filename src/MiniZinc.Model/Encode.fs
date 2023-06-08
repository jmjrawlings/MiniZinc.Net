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
    
    member this.writeIf (cond: bool) (s: string) =
        if cond then
            this.write s
        else                        
            this
            
    member this.writes (s: string) =
        this.write s
        builder.Append " "
        this
        
    member this.writeEnum (x: EnumItem) : MiniZincEncoder =
        this.writes "enum"
        this.write x.Name
        
        match x.Cases with
        | [] ->
            this.writetn()
        | cases ->
            this.write " = {"
            let n = cases.Length - 1
            for i, case in Seq.indexed cases do
                match case with
                | Name id ->
                    this.write id
                | Expr expr ->
                    this.writeExpr expr
                this.writeIf (i < n) ", "
            this.write "}"
            this.writetn()
                
    member this.writeSynonym (syn: SynonymItem) =
        this.write "type "
        this.write syn.Id
        this.write " = "
        this.writeTypeInst syn.TypeInst
        this.writetn()

    member this.write (x: Annotations) : MiniZincEncoder =
        this
        
    member this.writeTypeInst (x: TypeInst) : MiniZincEncoder =
        match x.IsArray with
        | true ->
            this.writeType x.Type
        | false ->    
            this.write x.Inst
            this.write " "
            this.writeIf x.IsSet "set of "
            this.writeIf x.IsOptional "opt "
            this.writeType x.Type
            
    member this.write (xs: TypeInst list) : MiniZincEncoder =
        let n = xs.Length - 1
        for i, x in Seq.indexed xs do
            this.writeTypeInst x
            this.writeIf (i < n) ", "
        this            
        
    member this.write (x: Inst) : MiniZincEncoder =
        match x with
        | Inst.Var ->
            this.write "var"
        | Inst.Par ->
            this
        | _ ->
            this

    member this.write (x: TupleType) : MiniZincEncoder =
        match x with
        | TupleType.TupleType fields ->
            this.write "tuple("
            this.write fields
            this.write ")"
            
    member this.write (xs: Expr list) : MiniZincEncoder =
        let n = xs.Length - 1
        for i, x in Seq.indexed xs do
            this.writeExpr x
            this.writeIf (i < n) ", "
        this
        
    member this.writeType(t: BaseType) : MiniZincEncoder =
        match t with
        | BaseType.Int ->
            this.write "int"
        | BaseType.Bool ->
            this.write "bool"
        | BaseType.String ->
            this.write "string"
        | BaseType.Float ->
            this.write "float"
        | BaseType.Id x ->
            this.write x
        | BaseType.Variable x ->
            this.write x
        | BaseType.Record x ->
            this.writeRecordType x                        
        | BaseType.Tuple x ->
            this.write x
        | BaseType.Literal x ->
            this.writeSetLit x            
        | BaseType.Range x ->
            this.writeRange x           
        | BaseType.List x ->
            this.writeListType x
        | BaseType.Array x ->
            this.writeArrayType x

    member this.writeRange (lo, hi) =
        this.writeNumExpr lo
        this.write ".."
        this.writeNumExpr hi
    
    member this.writeListType (ListType.ListType x) =
        this.write "list of "
        this.writeTypeInst x
        
    member this.writeArrayType (ArrayType.ArrayType (dims, typ)) =
        this.write "array["
        this.writeExprs dims
        this.write "] of "
        this.writeTypeInst typ
                        
    member this.writeSetLit (SetLiteral.SetLiteral exprs) =
        this.write "{"
        this.writeExprs exprs
        this.write "}"
                    
    member this.writeNumExpr (x: NumericExpr) : MiniZincEncoder =
        this
    
    member this.write (x: float) =
        this.write (string x)
        
    member this.writeWildcard (x: WildCard) =
        this.write "_"
    
    member this.write (x: bool) =
        this.write (if x then "true" else "false")
        
    member this.writeOp (x: Op) =
        let s = Encode.operators[int x]
        this.write s

    member this.writeBinOp (x: BinaryOp) =
        let s = Encode.operators[int x]
        this.write s
        
    member this.writeUnOp (x: UnaryOp) =
        let s = Encode.operators[int x]
        this.write s
                
    member this.write (x: SetCompExpr) : MiniZincEncoder =
        this
        
    // member this.writeSetLit (x: SetLiteral) : MiniZincEncoder =
    //     match x with
    //     | SetLiteral.SetLiteral exprs ->
    //         this.write "{"
    //         this.writeSolve exprs
    //         this.write "}"
        
    member this.write (x: Array1dExpr) : MiniZincEncoder =
        match x with
        | Array1d exprs -> this
        
    member this.write (x: Array2dExpr) : MiniZincEncoder =
        match x with
        | Array2d arrays -> this
        
    member this.write (x: ArrayCompExpr) : MiniZincEncoder =
        this
        
    member this.write (x: TupleExpr) : MiniZincEncoder =
        match x with
        | TupleExpr exprs ->
            this.write "("
            this.writeSolve exprs
            this.write ")"
    
    member this.write (expr: Expr, name: string) =
        this.write name
        this.write ": "
        this.writeExpr expr
        
    member this.write (x: RecordExpr) =
        match x with
        | RecordExpr.RecordExpr fields ->
            this.write "("
            this.writeExprs fields
            this.write ")"
        
    member this.write ((id, expr): UnaryOpExpr) =
        match id with
        | IdOr.Id x -> this.write x
        | IdOr.Val x -> this.write (int x)
        this.write " "
        this.writeExpr expr
        
    member this.write ((left, op, right): BinaryOpExpr) =
        this.writeExpr left
        this.write " "
        op.fold this.write this.writeBinOp
        this.write " "
        this.writeExpr right
        
    member this.write (x: IfThenElseExpr) =
        this.write "if "
        this.writeExpr x.If
        this.write " then "
        this.writeExpr x.Then
        
        for ifCase, thenCase in x.ElseIf do
            this.write "elseif "
            this.writeExpr ifCase
            this.write " then "
            this.writeExpr thenCase
            
        this.write " "
        this.writeExpr x.Else
        this.write " endif"
        
    member this.write (x: LetExpr) =
        this.writen "let {"
        this.indent()
        for local in x.Locals do
            match local with
            | Decl decl -> this.write decl
            | Cons cons -> this.writeConstraintItem cons
        this.dedent()
        this.writetn "} in "
        this.writeExpr x.In

    member this.writeSep (sep: string) (xs: 't list) (write: 't -> MiniZincEncoder) : MiniZincEncoder=
        for i in 0 .. xs.Length - 2 do
            let x = xs[i]
            write x
            this.write sep
        write (List.last xs)
        
    member this.writeGenerators (x: Generator list) =
        this.writeSep ", " x this.writeGenerator
            
    member this.writeGenCall (x: GenCallExpr) =
        this.writeIdOr this.writeOp x.Operation
        this.write "("
        this.writeGenerators x.From
        this.write ")"
        this.write "("
        this.writeExpr x.Yields
        this.write ")"
        
    member this.writeIdOr (write: 't -> MiniZincEncoder) (x: IdOr<'t>)  : MiniZincEncoder =
        match x with
        | IdOr.Id x -> this.write x
        | IdOr.Val x -> write x
        
    member this.writeGenerator (x: Generator) =
        this.writeSep ", " x.Yields (this.writeIdOr this.writeWildcard)
        this.write " in "
        this.writeExpr x.From
        match x.Where with
        | Some cond ->
            this.write " where "
            this.writeExpr cond
         | None ->
             this
    
    member this.write (x: ArrayAccess) =
        match x with
        | ArrayAccess.Access exprs ->
            this.writeSep ", " exprs this.writeExpr
        
    member this.write (x: IndexExpr) =
        match x with
        | IndexExpr.Index (expr, access) ->
            this.writeExpr expr
            this.write "["
            this.writeSep ", " access this.write
            this.write "]"
                        
    member this.writeExpr (x: Expr) : MiniZincEncoder =
        match x with
        | Expr.WildCard      x -> this.write "_"  
        | Expr.Int           x -> this.write (string x)
        | Expr.Float         x -> this.write x
        | Expr.Bool          x -> this.write x
        | Expr.String        x -> this.write x
        | Expr.Id            x -> this.write x
        | Expr.Op            x -> this.writeOp x
        | Expr.Bracketed     x -> this.writeExpr x
        | Expr.Set           x -> this.writeSetLit x
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
        | Expr.Call          x -> this.writeCall x
        | Expr.GenCall       x -> this.writeGenCall x 
        | Expr.Indexed       x -> this.write x
        
    member this.write (x: DeclareItem) =
        this.writeTypeInst x.Type
        this.write ": "
        this.write x.Name
        match x.Expr with
        | None ->
            this.writetn()
        | Some expr ->
            this.write " = "
            this.writeExpr expr
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
        this.writeParameters pred.Parameters
        this.write ")"
        if pred.Body.IsSome then
            this.writen " = "
            this.indent()
            this.writeExpr pred.Body.Value
            this.dedent()
            this.writetn()
        else
            this.writetn()
    
    // member this.write (x: Parameter) =
    //     let (name, ti) = x
    //     this.write name
    //     this.writes ":"
    //     this.writeTypeInst ti
        
    member this.writeRecordType (x: RecordType) : MiniZincEncoder =
        match x with
        | RecordType.RecordType xs ->
            this.write "record("
            this.writeParameters xs
            this.write ")"
            
    member this.writeParameter (x: Parameter) : MiniZincEncoder =
        let (id, ti) = x
        this.writeTypeInst ti
        this.write ": "
        this.write id
            
    member this.writeParameters (xs: Parameters) : MiniZincEncoder =
        this.writeSep ", " xs this.writeParameter
    
    member this.writeArg (x: NamedArg) : MiniZincEncoder =
        let (id, expr) = x
        this.write id
        this.write ": "
        this.writeExpr expr
                
    member this.writeExprs xs : MiniZincEncoder =
        this.writeSep ", " xs this.writeExpr
        
    member this.writeFunction (x: FunctionItem) =
        this.writes "function "
        this.writeTypeInst x.Returns
        this.write ": "
        this.write x.Name
        this.write "(" 
        this.writeParameters x.Parameters
        this.write ")"
        
        match x.Body with
        | None ->
            this.writetn()
        | Some body ->
            this.writen " = "
            this.indent()
            this.writeExpr body
            this.dedent()
            this.writetn()
                
    member this.writeCall (x: CallExpr) =
        this.writeIdOr x.Function this.writeOp
        this.write "("
        this.writeSep ", " x.Args this.writeExpr
        this.write ")"
        
    member this.writeConstraintItem (x: ConstraintItem) : MiniZincEncoder =
        this.write "constraint "
        this.writeExpr x.Expr
        this.writetn ()
        
    member this.writeIncludeItem (x: IncludeItem) : MiniZincEncoder =
        match x with
        | IncludeItem.Include s ->
            this.writetn $"include \"{s}\""
        
    member this.writeOutput (x: OutputItem) =
        this.write "output "
        this.writeExpr x.Expr
        this.writetn()
        
    member this.writeSolve (x: SolveMethod) : MiniZincEncoder =
        match x with
        | Sat _ ->
            this.writetn "solve satisfy"
        | Max (expr, _) ->
            this.write "solve maximize "
            this.writeExpr expr
            this.writetn()
        | Min (expr, _) ->
            this.write "solve minimize "
            this.writeExpr expr
            this.writetn()