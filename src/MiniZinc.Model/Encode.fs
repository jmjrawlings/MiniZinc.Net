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
            
        member this.write (i: int) =
            ignore (builder.Append i)
            
        member this.write (i: uint8) =
            ignore (builder.Append i)
            
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
        
        member this.writeSep (sep: string, xs: 't seq, write: 't -> unit) =
            let n = Seq.length xs 
            let mutable i = 0
            for x in xs do
                i <- i + 1
                write x
                if i < n then
                    this.write sep
            ()
                
        member this.writeColl (left:string, right:string, sep: string, xs: 't list, write: 't -> unit) =
            this.write left
            this.writeSep(sep, xs, write)
            this.write right
                
        member this.writeEnumType (x: EnumType) =
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
            this.writeExpr x
        
        member this.writeAnnotations (x: Annotations) =
            match x with
            | [] ->
                ()
            | anns ->
                this.write " "
                for ann in anns do
                    this.writeAnnotation ann
                
        member this.writeTypeInst (ti: TypeInst) =
            match ti.IsArray with
            | true ->
                this.writeType ti.Type
            | false ->
                if ti.IsVar then
                    this.write "var "
                if ti.IsSet then
                    this.write "set of "
                if ti.IsOptional then
                    this.write "opt "
                this.writeType ti.Type

        member this.writeTupleType fields =
            this.write "tuple("
            this.writeSep(", ", fields, this.writeTypeInst)
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
            | Type.Any ->
                this.write "any"
            | Type.Ident x
            | Type.Generic x ->
                this.write x                
            | Type.Record x ->
                this.writeRecordType x                
            | Type.Tuple x ->
                this.writeTupleType x                
            | Type.Set x ->
                this.writeExpr x
            | Type.Array (i, ti) ->
                this.writeArrayType(ti, i)
            | Type.Array2D (i, j, ti) ->
                this.writeArrayType(ti, i, j)
            | Type.Array3D (i, j, k, ti) ->
                this.writeArrayType(ti, i, j, k)
            | Type.Array4D (i, j, k, l, ti) ->
                this.writeArrayType(ti, i, j, k, l)
            | Type.Array5D (i, j, k, l, m, ti) ->
                this.writeArrayType(ti, i, j, k, l, m)
            | Type.Array6D (i, j, k, l, m, n, ti) ->
                this.writeArrayType(ti, i, j, k, l, m, n)
                
        member this.writeArrayDim(t: ArrayDim) =
            match t with
            | ArrayDim.Int ->
                this.write "int"
            | ArrayDim.Id x ->
                this.write x
            | ArrayDim.Set x ->
                this.writeExpr x
                    
        member this.writeArrayType (elements: TypeInst, [<ParamArray>]dimensions: ArrayDim[]) =
            this.write "array["
            this.writeSep(", ", dimensions, this.writeArrayDim)
            this.write "] of "
            this.writeTypeInst elements
                            
        member this.writeSetLit (set: Expr list) =
            this.write "{"
            this.writeExprs set
            this.write "}"

        member inline this.writeIdOrOp<'t when 't :> Enum> (x: IdOr<'t>) =
            match x with
            | IdOr.Id id ->
                this.write $"`{id}`"
            | IdOr.Val op ->
                let x = Convert.ToInt32(op)
                let s = operators[x]
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
            this.writeSep(", ", x.From, this.writeGenerator)
            this.write '}'
          
        member this.writeArray1d (exprs: Expr[]) =
            this.write "["
            this.writeExprs exprs
            this.write "]"
            
        member this.writeExprs(exprs: Expr seq) =
            this.writeSep(", ", exprs, this.writeExpr)
            
        member this.writeArray2d (array: Expr[,]) =
            this.write "[|"
            let I = Array2D.length1 array
            let J = Array2D.length2 array
            for i in 0 .. I-1 do
                let a = Array2D.map
                let row = array.[i,*]
                this.writeExprs(row)
                this.write "\n|"
                    
            this.write "|]"
            
        member this.writeArray3d (array: Expr[,,]) =
            let I = Array3D.length1 array
            let J = Array3D.length2 array
            let K = Array3D.length3 array
            this.write $"array3d(1..{I}, 1..{J}, 1..{K}, "
            for i in 0 .. I - 1 do
                for j in 0 .. J - 1 do
                    for k in 0 .. K - 1 do
                        this.writeExpr (array.[i, j, k])
                        this.write ", "
                
            // Remove trailing comma
            builder.Remove(builder.Length - 2, 2)
            this.write ")"
            ()
           
        member this.writeArrayComp (x: ArrayCompExpr) =
            this.write '['
            this.writeExpr x.Yields
            this.write " | "
            this.writeGenerators x.From
            this.write ']'
            
        member this.writeTuple (tuple: TupleExpr) =
            this.writeArgs tuple
            
        member this.writeRecordField (id: Ident, expr: Expr) =
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
        
        member this.writeArrayAccess (exprs: ArrayAccess) =
            this.write '['
            this.writeExprs exprs
            this.write ']'
                            
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
            | Expr.Ident x ->
                this.write x
            | Expr.Bracketed x ->
                this.write '('
                this.writeExpr x
                this.write ')'
            | Expr.Set x ->
                this.writeSetLit x
            | Expr.SetComp x ->
                this.writeSetComp x
            | Expr.Array1D x ->
                this.writeArray1d x
            | Expr.Array2D x ->
                this.writeArray2d x
            | Expr.Array3D x ->
                this.writeArray3d x
            | Expr.ArrayComp x ->
                this.writeArrayComp x
            | Expr.Array1DIndex ->
                ()
            | Expr.Array2DIndex ->
                ()
            | Expr.ArrayCompIndex ->
                ()
            | Expr.Tuple x ->
                this.writeTuple x
            | Expr.Record x ->
                this.writeRecord x
            | Expr.RecordAccess (field, expr) ->
                this.writeExpr expr
                this.write "."
                this.write field
            | Expr.TupleAccess (item, expr) ->
                this.writeExpr expr
                this.write "."
                this.write item                
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
            | Expr.ArrayAccess (access, expr) ->
                this.writeExpr expr
                this.write '['
                this.writeExprs access
                this.write ']'
            
        member this.writeDeclare (ti: TypeInst) =
            this.writeTypeInst ti
            this.write ": "
            this.write ti.Name
            this.writeAnnotations ti.Annotations
            match ti.Value with
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
            
        member this.writeRecordType fields =
            this.write "record"
            this.writeParameters fields
                
        member this.writeParameter (ti: TypeInst) =
            this.writeTypeInst ti
            this.write ": "
            this.write ti.Name
            this.writeAnnotations ti.Annotations
                
        member this.writeParameters (xs: TypeInst list) =
            if xs.IsEmpty then
                ()
            else
                this.write '('
                this.writeSep(", ", xs, this.writeParameter)
                this.write ')'
        
        member this.writeNamedArg (id: string, expr: Expr) =
            this.write id
            this.write ": "
            this.writeExpr expr
            
        member this.writeOutput (x: OutputExpr) =
            this.write "output "
            match x.Annotation with
            | Some ann ->
                this.writeAnnotation (Expr.String ann)
            | _ ->
                ()
            this.writeExpr x.Expr                
                
        member this.writeFunction (x: FunctionType) =
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
                    
        member this.writeCall (ident, args) =
            match ident with
            | IdOr.Id s -> this.write s
            | IdOr.Val v -> this.writeOp v
            
            this.writeArgs args
            
        member this.writeArgs (args: Expr list) =
            this.write "("
            this.writeExprs args
            this.write ")"
            
        member this.writeConstraint (x: ConstraintExpr) =
            this.write "constraint "
            this.writeExpr x.Expr
            this.writeAnnotations x.Annotations
            
        member this.writeIncludeItem (x: IncludeItem) =
            this.write $"include \"{x.Name}\""
            
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
    
        member this.writeAssign (id:string, expr) =
            this.write id
            this.write " = "
            this.writeExpr expr
            
        member this.writeDeclareAnnotation (x: AnnotationType) =
            this.write "annotation "
            this.write x.Name
            this.writeParameters x.Params            
                
        member this.writeItem (x: Item) =
            match x with
            | Item.Include x ->  
                this.writeIncludeItem x
            | Item.Enum x -> 
                this.writeEnumType x 
            | Item.Synonym x -> 
                this.writeSynonym x
            | Item.Constraint x -> 
                this.writeConstraint x
            | Item.Assign x -> 
                this.writeAssign x
            | Item.Declare x -> 
                this.writeDeclare x
            | Item.Solve x ->
                this.writeSolveMethod x
            | Item.Output x -> 
                this.writeOutput x
            | Item.Function x -> 
                this.writeFunction x
            | Item.Annotation x -> 
                this.writeDeclareAnnotation x
            | _ ->
                ()
