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
    
    let matchList pattern string =
        string
        |> Match pattern
        |> (fun m ->
            if m.Success
            then Seq.toList m.Captures
            else [])
        |> List.map (fun c -> c.Value)
        
    let match1 pattern =
        matchList pattern >> List.head
            
    let match2 pattern =
        matchList pattern >> (fun l -> l[0], l[1])
        
    let match3 pattern =
        matchList pattern >> (fun l -> l[0], l[1], l[3])
