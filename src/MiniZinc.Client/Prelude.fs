namespace MiniZinc

open System.Runtime.CompilerServices
open System.Text.RegularExpressions
open System.Threading.Tasks
open FSharp.Control


module Grep =
    
    // Returns matches for the given regex
    let matches (pattern: string) (input: string) =
                
        let regex = Regex pattern
        
        let values =
            match regex.Match input with
            | m when m.Success ->
                Seq.toList m.Groups
            | _ ->
                []
            |> List.skip 1 
            |> List.map (fun c -> c.Value)

        values
    
    
module Task =
    
    let map f a =
        task {
            let! value = a
            return f value
        }    
    
        
[<Extension>]        
type Extensions =
    
    [<Extension>]
    static member AsAsync(task: Task<'t>) =
        task
        |> Async.AwaitTask
        
    [<Extension>]
    static member Get(result: Result<'ok, 'err>) =
        Result.get result