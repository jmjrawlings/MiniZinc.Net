namespace MiniZinc

open System
open System.Text
open MiniZinc

module Encode =
     
     let operators : Map<int, string> =
         [ Op.Add, "+"         
         ; Op.Subtract, "-"
         ; Op.Not, "not"
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
    
    let builder = StringBuilder()
    
    member this.String =
        builder.ToString()
        
    member this.indent() =
        indentLevel <- indentLevel + 1
        
    member this.dedent() =
        indentLevel <- indentLevel - 1
        
    member this.write (c: char) =
        ignore (builder.Append c)

    member this.write (s: string) =
        ignore (builder.Append s)
        
    member this.writen (s: string) =
        ignore (builder.AppendLine s)

    member this.writen () =
        ignore (builder.AppendLine "")
            
    member this.writet () =
        this.write ";"
        
    member this.writet (s: string) =
        this.write s
        this.write ";"
        
    member this.writetn () =
        ignore (builder.AppendLine ";")
        
    member this.writetn (s: string) =
        this.write s
        this.writetn()
    
    member this.writeIf (cond: bool) (s: string) =
        if cond then
            this.write s

    member this.sepBy (sep: string, xs: 't list, write: 't -> unit) =
        if xs.IsEmpty then
            ()
        else
            let n = xs.Length
            for i in 0 .. n - 2 do
                write xs[i]
                this.write sep
            write xs[n-1]
            ()
            
    member this.writeEnum (x: EnumItem) =
        this.write "enum "
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

    member this.writeAnnotations (x: Annotations) =
        ()
       
    member this.writeTypeInst (ti: TypeInst) =
        match ti.IsArray with
        | true ->
            this.writeType ti.Type
        | false ->
            
            match ti.Inst with
            | Inst.Var ->
                this.write "var "
            | _ ->
                ()
            this.writeIf ti.IsSet "set of "
            this.writeIf ti.IsOptional "opt "
            this.writeType ti.Type        

    member this.writeTupleType (x: TupleType) =
        match x with
        | TupleType.TupleType fields ->
            this.write "tuple("
            this.sepBy(", ", fields, this.writeTypeInst)
            this.write ")"
        
    member this.writeType(t: BaseType) =
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
            this.writeTupleType x
            
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
        this.sepBy(", ", dims, this.writeTypeInst)
        this.write "] of "
        this.writeTypeInst typ
                        
    member this.writeSetLit (SetLiteral.SetLiteral exprs) =
        this.write "{"
        this.writeExprs exprs 
        this.write "}"

    member inline this.writeIdOrOp<'t when 't :> Enum> (x: IdOr<'t>) =
        match x with
        | IdOr.Id id ->
            this.writeId id
        | IdOr.Val op ->
            let x = Convert.ToInt32(op)
            let s = Encode.operators[x]
            this.write s
                    
    member this.writeNumExpr (x: NumericExpr) =
        match x with
        
        | Int i ->
            this.writeInt i
            
        | Float f ->
            this.writeFloat f
            
        | Id s ->
            this.write s
            
        | Op op ->
            this.writeOp op
            
        | Bracketed expr ->
            this.write '('
            this.writeNumExpr expr
            this.write ')'
            
        | Call expr ->
            this.writeCall expr
            
        | IfThenElse expr ->
            this.writeIfThenElse expr
            
        | Let expr ->
            this.writeLetExpr expr
            
        | UnaryOp(op, expr) ->
            this.writeIdOrOp op
            this.write " "
            this.writeNumExpr expr
            
        | BinaryOp(left, op, right) ->
            this.writeNumExpr left
            this.write " "
            this.writeIdOrOp op
            this.write " "
            this.writeNumExpr right
            
        | ArrayAccess(expr, access) ->
            this.writeNumExpr expr
            for acc in access do
                this.writeArrayAccess acc
    
    member this.writeId (s: string) =
        this.write s
    
    member this.writeString (s: string) =
        this.write $"\"{s}\""
    
    member this.writeInt (x: int) =
        this.write(string x)
    
    member this.writeFloat (x: float) =
        this.write (string x)
        
    member this.writeWildcard (x: WildCard) =
        this.write "_"
    
    member this.writeBool (x: bool) =
        match x with
        | true -> this.write "true"
        | false -> this.write "false"
        
    member this.writeOp (x: Enum) =
        let x = Convert.ToInt32(x)
        let s = Encode.operators[x]
        this.write s
                
    member this.writeSetComp (x: SetCompExpr) =
        this.write '{'
        this.writeExpr x.Yields
        this.write " | "
        this.sepBy(", ", x.From, this.writeGenerator)
        this.write '}'
      
    member this.writeArray1d (Array1dExpr.Array1d exprs) =
        this.write "["
        this.writeExprs exprs
        this.write "]"
        
    member this.writeExprs (exprs: Expr list) =
        this.sepBy(", ", exprs, this.writeExpr)
        
    member this.writeArray2d (Array2dExpr.Array2d arrays) =
        this.write "[|"
        this.sepBy("\n| ", arrays, this.writeExprs)
        this.write "|]"
        
    member this.writeArrayComp (x: ArrayCompExpr) =
        this.write '['
        this.writeExpr x.Yields
        this.write " | "
        this.writeGenerators x.From
        this.write ']'
        
    member this.writeTuple (TupleExpr.TupleExpr exprs) =
        this.write "("
        this.sepBy(", ", exprs, this.writeExpr)
        this.write ")"
    
    member this.writeRecordField (id: Id, expr: Expr) =
        this.write id
        this.write ": "
        this.writeExpr expr
        
    member this.writeRecord (RecordExpr.RecordExpr fields) =
        this.write "("
        this.sepBy(", ", fields, this.writeRecordField)
        this.write ")"

    member this.writeUnaryOp ((id, expr): UnaryOpExpr) =
        match id with
        | IdOr.Id x -> this.write x
        | IdOr.Val x -> this.writeOp x
        this.write " "
        this.writeExpr expr
        
    member this.writeBinaryOp ((left, op, right): BinaryOpExpr) =
        this.writeExpr left
        this.write " "
        this.writeIdOrOp op
        this.write " "
        this.writeExpr right
        
    member this.writeIfThenElse (x: IfThenElseExpr) =
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
        
    member this.writeLetExpr (x: LetExpr) =
        this.writen "let {"
        this.indent()
        
        let writeLocal =
            function
            | Decl decl -> this.writeDeclareItem(decl)
            | Cons cons -> this.writeConstraintItem(cons)
        
        this.sepBy(";\n", x.Locals, writeLocal)
        this.dedent()
        this.writen "} in"
        this.indent()
        this.writeExpr x.In
        this.writetn()
        this.dedent()
        
    member this.writeTypeInsts (xs: TypeInst list) : unit=
        this.sepBy (", ", xs, this.writeTypeInst)
        
    member this.writeGenerators (xs: Generator list) =
        this.sepBy(", ", xs, this.writeGenerator)
            
    member this.writeGenCall (x: GenCallExpr) =
        match x.Operation with
        | IdOr.Id id ->this.write id
        | IdOr.Val v -> this.writeOp v
        this.write "("
        this.writeGenerators x.From
        this.write ")"
        this.write "("
        this.writeExpr x.Yields
        this.write ")"
        
    member this.writeYield (id: IdOr<WildCard>) =
        match id with
        | IdOr.Id id -> this.write id
        | IdOr.Val value -> this.writeWildcard value
    
    member this.writeGenerator (gen: Generator) =
        this.sepBy(", ", gen.Yields, this.writeYield)
        this.write " in "
        this.writeExpr gen.From
        match gen.Where with
        | Some cond ->
            this.write " where "
            this.writeExpr cond
         | None ->
             ()
    
    member this.writeArrayAccess (ArrayAccess.Access exprs) =
        this.write '['
        this.writeExprs exprs
        this.write ']'
        
    member this.writeIndexExpr (IndexExpr.Index (expr, access)) =
        this.writeExpr expr
        for acc in access do
            this.writeArrayAccess acc
                        
    member this.writeExpr (x: Expr) =
        match x with
        | Expr.WildCard x ->
            this.write "_"  
        | Expr.Int x ->
            this.writeInt x
        | Expr.Float x ->
            this.writeFloat x
        | Expr.Bool x ->
            this.writeBool x
        | Expr.String x ->
            this.writeString x
        | Expr.Id x ->
            this.writeId x
        | Expr.Op x ->
            this.writeOp x
        | Expr.Bracketed x ->
            this.writeExpr x
        | Expr.Set x ->
            this.writeSetLit x
        | Expr.SetComp x ->
            this.writeSetComp x
        | Expr.Array1d x ->
            this.writeArray1d x
        | Expr.Array2d x ->
            this.writeArray2d x
        | Expr.ArrayComp x ->
            this.writeArrayComp x
        | Expr.Array1dIndex ->
            ()
        | Expr.Array2dIndex ->
            ()
        | Expr.ArrayCompIndex ->
            ()
        | Expr.Tuple x ->
            this.writeTuple x
        | Expr.Record x ->
            this.writeRecord x
        | Expr.UnaryOp x ->
            this.writeUnaryOp x
        | Expr.BinaryOp x ->
            this.writeBinaryOp x
        | Expr.Annotation ->
            ()
        | Expr.IfThenElse x ->
            this.writeIfThenElse x
        | Expr.Let x ->
            this.writeLetExpr x
        | Expr.Call x ->
            this.writeCall x
        | Expr.GenCall x ->
            this.writeGenCall x 
        | Expr.Indexed x ->
            this.writeIndexExpr x
        
    member this.writeDeclareItem (decl: DeclareItem) =
        this.writeTypeInst decl.Type
        this.write ": "
        this.write decl.Name
        match decl.Expr with
        | Some expr ->
            this.write " = "
            this.writeExpr expr
        | _ ->
            ()
            
    member this.writeSolveType (slv: SolveType) =
        match slv with
        | SolveType.Satisfy ->
            this.write "satisfy"
        | SolveType.Minimize ->
            this.write "minimize"
        | SolveType.Maximize ->
            this.write "maximize"
        | _ ->
            ()
        
    member this.writePredicate (pred: PredicateItem) =
        this.write "predicate "
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
        
    member this.writeRecordType (RecordType.RecordType pars) =
        this.write "record("
        this.writeParameters pars
        this.write ")"
            
    member this.writeParameter ((id,ti): Parameter) =
        this.writeTypeInst ti
        this.write ": "
        this.write id
            
    member this.writeParameters (xs: Parameters) =
        this.sepBy(", ", xs, this.writeParameter)
    
    member this.writeNamedArg (id: string, expr: Expr) =
        this.write id
        this.write ": "
        this.writeExpr expr
        
    member this.writeFunctionItem (x: FunctionItem) =
        this.write "function "
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
        match x.Function with
        | IdOr.Id s -> this.write s
        | IdOr.Val v -> this.writeOp v
        this.write "("
        this.writeExprs x.Args
        this.write ")"
        
    member this.writeConstraintItem (x: ConstraintItem) =
        this.write "constraint "
        this.writeExpr x.Expr
        
    member this.writeIncludeItem (x: IncludeItem) =
        match x with
        | IncludeItem.Include s ->
            this.writetn $"include \"{s}\""
        
    member this.writeOutputItem (x: OutputItem) =
        this.write "output "
        this.writeExpr x.Expr
        this.writetn()
        
    member this.writeSolveMethod (x: SolveMethod) =
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
            