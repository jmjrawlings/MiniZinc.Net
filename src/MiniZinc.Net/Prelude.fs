namespace MiniZinc.Net

open System.Text.RegularExpressions
open FSharp.Control

[<AutoOpen>]
module Prelude =
    let a = ()
    
    
module Grep =
    
    let Match pattern (s: string) =
        let pat = Regex pattern
        pat.Match s
        
    let matchAny pattern string =
        string
        |> Match pattern
        |> (fun m ->
            if m.Success
            then Some m
            else None)
    
    // Match the given pattern and store captures in an array
    let matchArray pattern string =
        
        let result =
            Match pattern string
        
        let groups =
            match result with
            | _ when result.Success ->
                Seq.toArray result.Groups
            | _ ->
                Array.empty
            |> Array.skip 1 
            |> Array.map (fun c -> c.Value)

        groups
        
    let match1 pattern =
        matchArray pattern >> (fun l -> l[0])
            
    let match2 pattern =
        matchArray pattern >> (fun l -> l[0], l[1])
        
    let match3 pattern =
        matchArray pattern >> (fun l -> l[0], l[1], l[2])
