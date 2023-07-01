namespace MiniZinc

open System
open System.Collections.Generic
open System.IO
open MiniZinc

type CompileOptions =
    | CompileOptions    
    static member Default =
        CompileOptions

type CompileResult =
    { ModelString : string
    ; Warnings    : string list
    ; Errors      : string list }
        
    member internal this.Warn msg =
        { this with Warnings = msg :: this.Warnings }
        
    member internal this.Err msg =
        { this with Errors =  msg :: this.Errors }

            
/// <summary>
/// Functions that handle preparing
/// a <see cref="Model">model</see> for solving
/// </summary>
[<AutoOpen>]
module rec Compile =
            
    /// Compile the given model to a tempfile with '.mzn' extension
    let compileModel (model: Model) : CompileResult =
        
        let folder (model: CompileResult) (name: string) (binding: Binding) =
            match binding with
            | Binding.Variable d when d.Inst = Inst.Par && d.Expr.IsNone->
                $"Unassigned parameter \"{name}\""
                |> model.Err
            | Binding.Expr expr ->
                $"Undeclared variable {name} = {expr}"
                |> model.Err
            | Binding.Enum { Cases = []} ->
                $"Enum \"{name}\" has no cases defined"
                |> model.Err
            | Binding.Multiple bindings ->
                let n = bindings.Length
                let err =
                    bindings
                    |> List.map string
                    |> String.concat ", "
                $"""Name "{name}" was bound to {n} expressions: {err}"""
                |> model.Err
            | Binding.Variable var ->
                model
            | Binding.Enum enum ->
                model
            | Binding.Type syn ->
                model
            | Binding.Function func ->
                model
            
        let empty =
            { ModelString = ""
            ; Warnings    = []
            ; Errors      = [] }
                
        let compiled =
            (empty, model.NameSpace.Bindings)
            ||> Map.fold folder
            
        let mzn =
            model.Encode()
            
        { compiled with ModelString = mzn }
        

    /// Compile the Model with extra Data        
    let compileModelWithData (data: NameSpace) (model: Model) =
        
        let nameSpace =
            data
            |> NameSpace.merge model.NameSpace
            
        let combined =
            { model with NameSpace = nameSpace }
            
        let compiled =
            compileModel combined
            
        compiled
    
        
    module Model =
        
        let compile options model =
            compileModel model
            
        let compileWith data model =
            compileModelWithData data model
                
    type Model with
    
        /// Compile this model 
        member this.Compile() =
            compileModel this
            
        /// Compile this model with the given options
        member this.Compile(options: CompileOptions) =
            compileModel this

        /// Compile this model using the given parameters             
        member this.Compile(parameters: IDictionary<string, Expr>) =
            
            let data =
                parameters
                |> Seq.map (fun kv -> kv.Key, Binding.Expr kv.Value)
                |> NameSpace.ofSeq
            
            compileModelWithData data this        