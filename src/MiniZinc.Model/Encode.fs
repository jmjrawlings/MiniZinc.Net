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

        member this.writeSep (sep: string, xs: 't list, write: 't -> unit) =
            if xs.IsEmpty then
                ()
            else
                let n = xs.Length
                for i in 0 .. n - 2 do
                    write xs[i]
                    this.write sep
                write xs[n-1]
                ()
                
        member this.writeColl (left:string, right:string, sep: string, xs: 't list, write: 't -> unit) =
            this.write left
            this.writeSep(sep, xs, write)
            this.write right
                
        member this.writeEnum (x: EnumItem) =
            this.write "enum "
            this.write x.Name
                        
            match x.Cases with
            | [] ->
                ()
            | cases ->
                this.write " = "
                this.writeSep(" ++ ", cases, this.writeEnumCase)
                
        member this.writeEnumCase (ecase: EnumCases) =
            match ecase with
            | EnumCases.Anon expr ->
                this.write "_("
                this.writeExpr expr
                this.write ")"
            | EnumCases.Call (id, expr) ->
                this.write $"{id}("
                this.writeExpr expr
                this.write ")"
            | EnumCases.Names names ->
                this.write "{"
                this.writeSep(",", names, this.write)
                this.write "}"
            
        member this.writeSynonym (syn: TypeAlias) =
            this.write "type "
            this.write syn.Name
            this.write " = "
            this.writeTypeInst syn.TypeInst

        member this.writeAnnotation (x: Annotation) =
            this.write " ::"
            this.write x.Name
            match x.Args with
            | [] ->
                ()
            | args ->
                this.writeArgs args
        
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
            this.writeSep(", ", x.Fields, this.writeTypeInst)
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
            this.writeSep(", ", arr.Dimensions, this.writeArrayDim)
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
            this.writeSep(", ", x.From, this.writeGenerator)
            this.write '}'
          
        member this.writeArray1d (exprs: Array1dExpr) =
            this.write "["
            this.writeExprs exprs
            this.write "]"
            
        member this.writeExprs (exprs: Expr list) =
            this.writeSep(", ", exprs, this.writeExpr)
            
        member this.writeArray2d (arrays: Array2dExpr) =
            this.write "[|"
            this.writeSep("\n| ", arrays, this.writeExprs)
            this.write "|]"
            
        member this.writeArrayComp (x: ArrayCompExpr) =
            this.write '['
            this.writeExpr x.Yields
            this.write " | "
            this.writeGenerators x.From
            this.write ']'
            
        member this.writeTuple (tuple: TupleExpr) =
            this.writeArgs tuple
            
        member this.writeRecordField (id: Id, expr: Expr) =
            this.write id
            this.write ": "
            this.writeExpr expr
            
        member this.writeRecord (record: RecordExpr) =
            this.write "("
            this.writeSep(", ", record, this.writeRecordField)
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
            this.writeSep(",\n", x.Declares, this.writeDeclare)
            this.writeSep(",\n", x.Constraints, this.writeConstraint)
            this.dedent()
            this.writen "} in"
            this.indent()
            this.writeExpr x.Body
            this.writen()
            this.dedent()
            
        member this.writeTypeInsts (xs: TypeInst list) : unit=
            this.writeSep (", ", xs, this.writeTypeInst)
            
        member this.writeGenerators (xs: Generator list) =
            this.writeSep(", ", xs, this.writeGenerator)
                
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
            this.writeSep(", ", gen.Yields, this.writeYield)
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
                this.write x
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
            this.writeSep(", ", xs, this.writeParameter)
            this.write ')'
        
        member this.writeNamedArg (id: string, expr: Expr) =
            this.write id
            this.write ": "
            this.writeExpr expr
            
        member this.writeFunction (x: FunctionItem) =
            this.write "function "
            this.writeTypeInst x.Returns
            this.write ": "
            this.write x.Name
            this.writeParameters x.Parameters
            
            match x.Body with
            | Some body ->
                this.writen " = "
                this.indent()
                this.writeExpr body
                this.dedent()
            | _ ->
                ()
                    
        member this.writeCall (x: CallExpr) =
            match x.Function with
            | IdOr.Id s -> this.write s
            | IdOr.Val v -> this.writeOp v
            
            this.writeArgs x.Args
            
        member this.writeArgs (args: Expr list) =
            this.write "("
            this.writeExprs args
            this.write ")"
            
        member this.writeConstraint (x: ConstraintItem) =
            this.write "constraint "
            this.writeExpr x.Expr
            this.writeAnnotations x.Annotations
            
        member this.writeIncludeItem (x: IncludeItem) =
            this.write $"include \"{x.Name}\""
            
        member this.writeOutputItem (x: OutputItem) =
            this.write "output "
            this.writeExpr x.Expr
            
        member this.writeSolveMethod (x: SolveItem) =
            this.write "solve"
            this.writeAnnotations x.Annotations
            this.write " "
            match x with
            | Sat _ ->
                this.write "satisfy"
            | Max (expr, _) ->
                this.write "maximize "
                this.writeExpr expr
            | Min (expr, _) ->
                this.write "minimize "
                this.writeExpr expr
                
    