(*

Prelude.fs

Helper functions for the rest of the codebase

*)

namespace MiniZinc

open System.Collections.Generic
open System.Runtime.CompilerServices

type Merge =
    | Left
    | Right
    | Inner
    | Outer
    | Union

[<AutoOpen>]
module Prelude =
    let xd () =
        failwith "xd"


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

    [<Extension>]
    static member KeySet(dict: IDictionary<'k, 'v>) : Set<'k> =
        dict.Keys
        |> Set.ofSeq
        
    [<Extension>]
    static member Merge(a: Set<'t>, b: Set<'t>, how: Merge) =
        match how with
        | Left ->
            Set.difference a b
        | Right ->
            Set.difference b a
        | Inner ->
            Set.intersect a b
        | Outer ->
            let union = Set.union a b
            let isect = Set.intersect a b
            let outer = Set.difference union isect
            outer
        | Union ->
            Set.union a b
            
    [<Extension>]
    static member Venn(a: Set<'t>, b: Set<'t>) =
        let left = Set.difference a b
        let right = Set.difference b a
        let middle = Set.intersect a b
        (left, middle, right)
        
    [<Extension>]
    static member Merge(a: Map<'k, 'v>, b: Map<'k, 'v>, takeLeft: bool) =
        let leftKeys, midKeys, rightKeys = a.Venn(b)
        
        let left =
            a
            |> Map.filter (fun key _ -> leftKeys.Contains key)
            
        let right =
            b
            |> Map.filter (fun key _ -> rightKeys.Contains key)
            
        let mid =
            midKeys
            |> Seq.map (fun key ->
                match takeLeft with
                | true -> key, a[key]
                | false -> key, b[key])
            |> Map.ofSeq
                        
        (left, mid, right)

    [<Extension>]
    static member Venn(a: Map<'k, 'v>, b: Map<'k, 'v>) : Set<'k> * Set<'k> * Set<'k> =
        let ka = a.KeySet()
        let kb = b.KeySet()
        let left, mid, right = ka.Venn(kb)
        left, mid, right