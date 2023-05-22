(*

Prelude.fs

Helper functions for the rest of the codebase

*)

namespace MiniZinc

open System.Collections.Generic
open System.Runtime.CompilerServices


[<AutoOpen>]
module Prelude =
    let a = ()


module Map =
    let withKey f xs =
        xs
        |> Seq.map (fun x -> (f x), x)
        |> Map
        
    let toDict (map: Map<'k, 'v>) : Dictionary<'k,'v> =
        let dict = Dictionary()
        for (k,v) in Map.toSeq map do
            dict[k] <- v
        dict


module Result =
    
    let get (result: Result<'ok, 'err>) =
        match result with
        | Ok ok ->
            ok
        | Error err ->
            failwith $"{err}"
        
    let ofSeq (results : Result<'ok, 'err> seq) =
        let oks = ResizeArray()
        let errs = ResizeArray()
        for result in results do
            match result with
            | Ok v -> oks.Add v
            | Error err -> errs.Add err
        match errs.Count with
        | 0 -> Ok (Seq.toList oks)
        | _ -> Error (Seq.toList errs)
        
        
[<Extension>]
type Extensions =
    
    [<Extension>]
    static member TryGet(dict: IDictionary<'k, 'v>, key: 'k) : Option<'v> =
        let mutable value = Unchecked.defaultof<'v>
        match dict.TryGetValue(key, &value) with
        | true -> Some value
        | false -> None

    [<Extension>]
    static member Get(dict: IDictionary<'k, 'v>, key: 'k, backup: 'v) : 'v =
        let mutable value = Unchecked.defaultof<'v>
        match dict.TryGetValue(key, &value) with
        | true -> value
        | false -> backup
