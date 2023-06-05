namespace MiniZinc

open System
open System.Collections.Generic
open System.Text
open MiniZinc

/// Encode MiniZinc to String
type MiniZincEncoder() =
    
    let mutable indentLevel = 0
    let contexts = Stack<IDisposable>()
    let string = StringBuilder()
    
    /// Pop the last context off the stack        
    member private this.pop() =
        contexts.Pop()
        this
            
    /// Add a new context to the stack            
    member private this.push context =
        contexts.Push context
        this

    /// Add a context for 1 level of indentation            
    member this.indent () =
        indentLevel <- indentLevel + 1
        let context = 
            { new IDisposable with
                 member IDisposable.Dispose () =
                     indentLevel <- indentLevel - 1
                     () }
        this.push context
        
    /// Add a context that surrounds with the given strings                
    member this.enclose (prefix: string) (suffix: string) =
        string.Append prefix
        let context =
            { new IDisposable with
                 member IDisposable.Dispose () =
                     string.Append suffix
                     () }
        this.push context
        context

    
    member this.write (s: string) =
        string.Append s
        this
        
    member this.write (c: char) =
        string.Append c
        this
        
    member this.writeif (cond: bool) (s: string) =
        if cond then
            this.write s
        else                        
            this
        
    member this.writeln (s: string) =
        string.AppendLine s
        this
        
    member this.writes (s: string) =
        string.Append s
        string.Append " "
        this
    
    member this.write (x: EnumItem) : MiniZincEncoder =
        this.write "enum "
        this.write x.Name
        
        match x.Cases with
        | [] ->
            this
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
                
    member this.write (syn: SynonymItem) =
        this.write "type "
        this.write syn.Id
        this.write " = "
        this.write syn.TypeInst

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
            
    member this.write (x: Expr) =
        this.write ""       
        
    member this.write (x: DeclareItem) =
        this.write ""
        
    member this.write (x: SolveType) =
        match x with
        | SolveType.Satisfy -> "satisfy"
        | SolveType.Minimize -> "minimize"
        | SolveType.Maximize -> "maximize"
        | _ -> ""
        
    member this.write (pred: PredicateItem) =
        this.writes "predicate"
        this.write pred.Name
        this.write "("
        this.write pred.Parameters
        this.write ")"
        this
    
    member this.write (name: string, ti: TypeInst) =
        this.write name
        this.writes ":"
        this.write ti
        
    member this.write (x: Map<string, TypeInst>) : MiniZincEncoder =
        let n = x.Count - 1
        for i, kv in Seq.indexed x do
            this.write kv.Key
            this.write ": "
            this.write kv.Value
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
            this
        | Some body ->
            this.writeln " = "
            this.indent()
            this.write body
            this.pop()
                
    member this.write (x: CallExpr) =
        this.write ""
        
    member this.write (con: ConstraintItem) =
        this.write "constraint "
        this.write con.Expr
        
    member this.write (x: IncludeItem) =
        this.write "include "
        this.write x.FileName
        
    member this.write (x: OutputItem) =
        this.write "output "
        this.write x.Expr
        
    member this.write (x: SolveMethod) =
        match x with
        | Sat _ ->
            this.write "solve satisfy"
        | Max (expr, _) ->
            this.write "solve maximize "
            this.write expr
        | Min (expr, _) ->
            this.write "solve minimize "
            this.write expr