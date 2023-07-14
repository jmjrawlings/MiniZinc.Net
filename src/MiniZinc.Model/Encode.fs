namespace MiniZinc

open System
open System.Text
open MiniZinc

/// <summary>
/// Functions and values that allow
/// <see cref="Model">models</see> to be
/// written to a minizinc string.
/// </summary>
[<AutoOpen>] 
module rec Encode =

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
    
            
    type Encoder() =
                
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
                    
        member this.writeSynonym (syn: TypeAlias) =
            this.write "type "
            this.write syn.Name
            this.write " = "
            this.writeTypeInst syn.TypeInst
            this.writetn()

        member this.writeAnnotation (x: Annotation) =
            this.write " ::"
            this.writeExpr x
        
        member this.writeAnnotations (x: Annotations) =
            match x with
            | [] ->
                ()
            | anns ->
                this.write " "
                for ann in anns do
                    this.writeAnnotation ann
                    
        member this.writeAnnotationItem (x: AnnotationItem) =
            this.write "annotation "
            match x with
            | AnnotationItem.Name id ->
                this.write id
            | AnnotationItem.Call (id, pars) ->
                this.write id
                this.writeParameters pars
                
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
            this.write "tuple("
            this.sepBy(", ", x.Fields, this.writeTypeInst)
            this.write ")"
            
        member this.writeType (t: Type) =
            match t with
            | Type.Int ->
                this.write "int"                
            | Type.Bool ->
                this.write "bool"                
            | Type.String ->
                this.write "string"                
            | Type.Float ->
                this.write "float"
            | Type.Ann ->
                this.write "ann"
            | Type.Id x ->
                this.write x                
            | Type.Record x ->
                this.writeRecordType x                
            | Type.Tuple x ->
                this.writeTupleType x                
            | Type.Set x ->
                this.writeExpr x
            | Type.Array x ->
                this.writeArrayType x
                
        member this.writeArrayDim(t: ArrayDim) =
            match t with
            | ArrayDim.Int ->
                this.write "int"
            | ArrayDim.Id x ->
                this.write x
            | ArrayDim.Set x ->
                this.writeExpr x

        member this.writeRange (lo, hi) =
            this.writeNumExpr lo
            this.write ".."
            this.writeNumExpr hi
                    
        member this.writeArrayType (arr: ArrayType) =
            this.write "array["
            this.sepBy(", ", arr.Dimensions, this.writeArrayDim)
            this.write "] of "
            this.writeTypeInst arr.Elements
                            
        member this.writeSetLit (set: SetLiteral) =
            this.write "{"
            this.writeExprs set.Elements
            this.write "}"

        member inline this.writeIdOrOp<'t when 't :> Enum> (x: IdOr<'t>) =
            match x with
            | IdOr.Id id ->
                this.write $"`{id}`"
            | IdOr.Val op ->
                let x = Convert.ToInt32(op)
                let s = operators[x]
                this.write s
                        
        member this.writeNumExpr (x: NumExpr) =
            match x with
            
            | NumExpr.Int i ->
                this.writeInt i
                
            | NumExpr.Float f ->
                this.writeFloat f
                
            | NumExpr.Id s ->
                this.write s
                
            | NumExpr.Op op ->
                this.writeOp op
                
            | NumExpr.Bracketed expr ->
                this.write '('
                this.writeNumExpr expr
                this.write ')'
                
            | NumExpr.Call expr ->
                this.writeCall expr
                
            | NumExpr.IfThenElse expr ->
                this.writeIfThenElse expr
                
            | NumExpr.Let expr ->
                this.writeLetExpr expr
                
            | NumExpr.UnaryOp(op, expr) ->
                this.writeIdOrOp op
                this.write " "
                this.writeNumExpr expr
                
            | NumExpr.BinaryOp(left, op, right) ->
                this.writeNumExpr left
                this.write " "
                this.writeIdOrOp op
                this.write " "
                this.writeNumExpr right
                
            | NumExpr.ArrayAccess(expr, access) ->
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
            let s = operators[x]
            this.write s
                    
        member this.writeSetComp (x: SetCompExpr) =
            this.write '{'
            this.writeExpr x.Yields
            this.write " | "
            this.sepBy(", ", x.From, this.writeGenerator)
            this.write '}'
          
        member this.writeArray1d (exprs: Array1dExpr) =
            this.write "["
            this.writeExprs exprs
            this.write "]"
            
        member this.writeExprs (exprs: Expr list) =
            this.sepBy(", ", exprs, this.writeExpr)
            
        member this.writeArray2d (arrays: Array2dExpr) =
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
                this.write " elseif "
                this.writeExpr ifCase
                this.write " then "
                this.writeExpr thenCase
                
            this.write " else "
            if x.Else.IsSome then
                this.writeExpr x.Else.Value
            this.write " endif"
            
        member this.writeLetExpr (x: LetExpr) =
            this.writen "let {"
            this.indent()
            this.sepBy(",\n", (Seq.toList x.NameSpace.Variables.Values), this.writeDeclare)
            this.sepBy(",\n", x.Constraints, this.writeConstraintItem)
            this.dedent()
            this.writen "} in"
            this.indent()
            this.writeExpr x.Body
            this.writen()
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
            | Expr.WildCard _ ->
                this.write "_"
            | Expr.Absent _ ->
                this.write "<>"
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
                this.write '('
                this.writeExpr x
                this.write ')'
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
                             
            
        member this.writeDeclare (decl: DeclareItem) =
            this.writeTypeInst decl.TypeInst
            this.write ": "
            this.write decl.Name
            match decl.Expr with
            | Some expr ->
                this.write " = "
                this.writeExpr expr
            | _ ->
                ()
                
        member this.writeSolveType (slv: SolveMethod) =
            match slv with
            | SolveMethod.Satisfy ->
                this.write "satisfy"
            | SolveMethod.Minimize ->
                this.write "minimize"
            | SolveMethod.Maximize ->
                this.write "maximize"
            | _ ->
                ()
            
        member this.writeRecordType (x: RecordType) =
            this.write "record"
            this.writeParameters x.Fields
                
        member this.writeParameter (p: Parameter) =
            this.writeTypeInst p.TypeInst
            this.write ": "
            this.write p.Name
            this.writeAnnotations p.Annotations
                
        member this.writeParameters (xs: Parameters) =
            this.write '('
            this.sepBy(", ", xs, this.writeParameter)
            this.write ')'
        
        member this.writeNamedArg (id: string, expr: Expr) =
            this.write id
            this.write ": "
            this.writeExpr expr
            
        member this.writeFunctionItem (x: FunctionItem) =
            this.write "function "
            this.writeTypeInst x.Returns
            this.write ": "
            this.write x.Name
            this.writeParameters x.Parameters
            
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
            this.writeAnnotations x.Annotations
            
        member this.writeIncludeItem (x: IncludeItem) =
            match x with
            | IncludeItem.Include s ->
                this.writetn $"include \"{s}\""
            
        member this.writeOutputItem (x: OutputItem) =
            this.write "output "
            this.writeExpr x.Expr
            this.writetn()
            
        member this.writeSolveMethod (x: SolveItem) =
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
                
    module Model =
                
        /// Encode the given model as a string
        let encode (model: Model) =
            
            let enc = Encoder()
                                                
            for incl in model.Includes.Keys do
                let item = IncludeItem.Include incl
                enc.writeIncludeItem item

            for enum in model.NameSpace.Enums.Values do
                enc.writeEnum enum
                
            for syn in model.NameSpace.Synonyms.Values do
                enc.writeSynonym syn

            for x in model.NameSpace.Variables.Values do
                enc.writeDeclare x
                enc.writetn()

            for cons in model.Constraints do
                enc.writeConstraintItem cons
                enc.writetn()

            for func in model.NameSpace.Functions.Values do
                enc.writeFunctionItem func
                        
            enc.writeSolveMethod model.SolveMethod
            
            for output in model.Outputs do
                enc.writeOutputItem output
                
            enc.String

                
    type Model with
    
        member this.Encode() =
            Model.encode this