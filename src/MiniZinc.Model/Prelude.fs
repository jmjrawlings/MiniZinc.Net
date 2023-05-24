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
        

// A lens for field 'u' on model 't'    
type Lens<'t, 'u> =
    { get: 't -> 'u
      set: 't -> 'u -> 't }
    
    member this.map f =
        fun x1 ->
            let v1 = this.get x1
            let v2 = f v1
            let x2 = this.set x1 v2
            x2
            
            
// A lens for a Map<'k,'v> field on model 't'
type MapLens<'t, 'k, 'v when 'k:comparison> =
    { get: 't -> Map<'k,'v>
      set: 't -> Map<'k,'v> -> 't }
    
    member this.map f =
        fun x1 ->
            let v1 = this.get x1
            let v2 = f v1
            v2
            
    member this.update f =
        fun m1 ->
            let v2 = this.map f m1
            let m2 = this.set m1 v2
            m2
    
    member this.add key value =
        fun m1 ->
            let v1 = this.get m1
            let v2 = v1.Add(key,value)
            let m2 = this.set m1 v2
            m2
        
    member this.remove (key: 'k) =
        fun m1 ->
            let v1 = this.get m1
            let v2 = v1.Remove(key)
            let m2 = this.set m1 v2
            m2
            
    member this.merge (right: Map<'k, 'v>) =
        fun m1 ->
            let left = this.get m1
            let merged =
                left
                |> Map.toSeq
                |> Seq.append (Map.toSeq right)
                |> Map.ofSeq
            let x2 = this.set m1 merged
            x2
            
    member this.tryFind (key) =
        fun m ->
            let map = this.get m
            let v = map.TryFind key
            v
            
    member this.tryFind (key, backup) =
        fun m ->
            let v =
                m
                |> this.map (Map.tryFind key)
                |> Option.defaultValue backup
            v
            
    member this.find key =
        fun m ->
            let v = this.map (Map.find key) m
            v
    
module Lens =
        
    let v get set : Lens<'t, 'u> =
        { get = get; set = set }
        
    let m get set : MapLens<'t, 'k, 'v> =
        { get = get; set = set }
            
        
[<Extension>]
type Extensions =
    
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