namespace MiniZinc.Encode
open System
open Microsoft.FSharp.Quotations
open MiniZinc

type Encode =
    
    static member encode (x: IncludeItem) =
        string x
    
    static member encode (x: EnumItem) =
        $"enum {Encode.encode x.Name}"
                
    static member encode (x: EnumCase) =
        match x with
        | Name x -> Encode.encode x
        | Expr x -> Encode.encode x
    
    static member encode (x: SynonymItem) =
        match x with
        | (id, anns, ti) ->
            $"type {Encode.encode id} {Encode.encode anns} = {Encode.encode ti}"
        ""
        
    static member encode (x: Annotations) =
        ""
        
    static member encode (x: TypeInst) =
        ""
        
    static member encode (x: Inst) =
        match x with
        | Inst.Var -> "var"
        | Inst.Par -> "par"
        | _ -> ""
        
    static member encode(x: BaseType) =
        match x with
        | Int ->
            "int"
        | Bool ->
            "bool"
        | String ->
            "string"
        | Float ->
            "float"
        | Id s ->
            s
        | Variable s ->
            s
        | Tuple fields ->
            fields
            |> Seq.map Encode.encode
            |> String.concat ", "
            |> sprintf "(%s)"
        | Record fields ->
            fields
            |> Map.toSeq
            |> Seq.map (fun (id, ti) -> $"{Encode.encode(ti)}: {id}")
            |> String.concat ", "
            |> sprintf "record (%s)"
        | Literal exprs ->
            exprs
            |> Seq.map Encode.encode
            |> String.concat ", "
            |> sprintf "{%s}"
        | Range (lo, hi) ->
            let los = Encode.encode(lo)
            let his = Encode.encode(hi)
            let string = $"{los} .. {his}"
            string
        | List ti ->
            $"list of {Encode.encode(ti)}"
        | Array (dims, typ) ->            
            let dims =
                dims
                |> Seq.map Encode.encode
                |> String.concat ", "
                |> sprintf "[%s]"                
            let string =
                $"array[{dims}] of {Encode.encode(typ)}"                
            string
        
    static member encode(x: NumericExpr) =
        ""
            
    static member encode (x: Expr) =
        ""       
        
    static member encode (x: DeclareItem) =
        ""
        
    static member encode (x: SolveMethod) =
        ""
        
    static member encode (x: PredicateItem) =
        ""
        
    static member encode (x: FunctionItem) =
        ""
        
    static member encode (x: CallExpr) =
        ""