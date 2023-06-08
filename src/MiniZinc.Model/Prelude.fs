(*

Prelude.fs

Helper functions for the rest of the codebase

*)

namespace MiniZinc

open System.IO
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
    let (|FileExists|FileNotFound|) x =
        match File.Exists x with
        | true -> FileExists (FileInfo x)
        | false -> FileNotFound

type File =
    
    static member existing(path: string) =
        match File.Exists path with
        | true -> Result.Ok path
        | false -> Error $"{path} does not exist"
    
    static member read(path: string) =
        path
        |> File.existing
        |> Result.map File.ReadAllText
        
    static member read(path: FileInfo) =
        File.read(path.FullName)
        

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
        
    let ofDict (dict: IDictionary<'k,'v>) : Map<'k, 'v> =
        seq { for kv in dict do yield (kv.Key, kv.Value) }
        |> Map.ofSeq
        
    let merge a b =
        let merged =
            Map.toSeq a
            |> Seq.append (Map.toSeq b)
            |> Map.ofSeq
        merged

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
        
    let ofOption err opt =
        match opt with
        | Some v -> Ok v
        | None -> Error err
            

// A lens for field 'u' on model 't'    
type Lens<'t, 'u>(getter: 't -> 'u, setter: 'u -> 't -> 't) =
    
    member this.get =
        getter
        
    member this.set =
        setter
        
    member this.map f (m1: 't) =
        fun m1 ->
            let v1 = this.get m1
            let v2 = f v1
            let m2 = this.set v2 m1
            m2
            

// A lens for list 'u' on model 't'    
type ListLens<'t, 'u>(getter: 't -> 'u list, setter: 'u list -> 't -> 't) =
    
    member this.get =
        getter
        
    member this.set =
        setter
        
    member this.map f (m1: 't) =
        fun m1 ->
            let v1 = this.get m1
            let v2 = f v1
            let m2 = this.set v2 m1
            m2
            
    member this.insert (x: 'u) =
        fun (m1: 't) ->
            let v1 = this.get m1
            let v2 = x :: v1
            let m2 = this.set v2 m1
            m2

    member this.insert (xs: 'u list) =
        fun (m1: 't) ->
            let v1 = this.get m1
            let v2 = xs @ v1
            let m2 = this.set v2 m1
            m2
                
    member this.append (x: 'u) =
        fun (m1: 't) ->
            let v1 = this.get m1
            let v2 = v1 @ [x]
            let m2 = this.set v2 m1
            m2
            
    member this.append (xs: 'u list) =
        fun (m1: 't) ->
            let v1 = this.get m1
            let v2 = v1 @ xs
            let m2 = this.set v2 m1
            m2


// A lens for list 'u' on model 't'    
type SetLens<'t, 'u when 'u:comparison>(getter: 't -> Set<'u>, setter: Set<'u> -> 't -> 't) =
    
    member this.get =
        getter
        
    member this.set =
        setter
        
    member this.map f (m1: 't) =
        fun m1 ->
            let v1 = this.get m1
            let v2 = f v1
            let m2 = this.set v2 m1
            m2
            
    member this.add (x: 'u) =
        fun (m1: 't) ->
            let v1 = this.get m1
            let v2 = Set.add x v1 
            let m2 = this.set v2 m1
            m2
            
    member this.remove (x: 'u) =
        fun (m1: 't) ->
            let v1 = this.get m1
            let v2 = Set.remove x v1
            let m2 = this.set v2 m1
            m2
            
            
// A lens for a Map<'k,'v> field on model 't'
type MapLens<'t, 'k, 'v when 'k:comparison>(getter: 't -> Map<'k,'v>, setter: Map<'k,'v> -> 't -> 't) =

    member this.get =
        getter
        
    member this.set =
        setter
            
    member this.map f (m1: 't) =
        let v1 = this.get m1
        let v2 = f v1
        let m2 = this.set v2 m1
        m2
    
    member this.add key value (m1: 't) =
        let v1 = this.get m1
        let v2 = v1.Add(key,value)
        let m2 = this.set v2 m1
        m2
        
    member this.remove (key: 'k) (m1: 't) =
        let v1 = this.get m1
        let v2 = v1.Remove(key)
        let m2 = this.set v2 m1
        m2
            
    member this.merge (right: Map<'k, 'v>) (m1: 't) =
        let left = this.get m1
        let merged =
            left
            |> Map.toSeq
            |> Seq.append (Map.toSeq right)
            |> Map.ofSeq
        let m2 = this.set merged m1
        m2
            
    member this.tryFind (key) =
        fun (m: 't) ->
            let map = this.get m
            let v = map.TryFind key
            v
            
    member this.tryFind (key, backup) =
        fun (m: 't) ->
            let v =
                m
                |> this.get
                |> Map.tryFind key
                |> Option.defaultValue backup
            v
            
    member this.find key =
        fun (m: 't) ->
            let v = Map.find key (this.get m)
            v
    
module Lens =
            
    // Create a lens for field field 'u on 't
    let v get set =
        Lens(get, set)
        
    // Create a lens into a Map<'k,'v> field on 't                        
    let m get set =
        MapLens(get, set)
        
    // Create a lens into a Map<'k,'v> field on 't                        
    let l get set =
        ListLens(get, set)
        
    // Create a lens into a Map<'k,'v> field on 't                        
    let s get set =
        SetLens(get, set)        
            
        
[<Extension>]
type Extensions =
    
    [<Extension>]
    static member Get(dict: IDictionary<'k, 'v>, key: 'k, backup: 'v) : 'v =
        let mutable value = Unchecked.defaultof<'v>
        match dict.TryGetValue(key, &value) with
        | true -> value
        | false -> backup
        
    [<Extension>]
    static member TryGet(dict: IDictionary<'k, 'v>, key: 'k) : 'v option =
        let mutable value = Unchecked.defaultof<'v>
        match dict.TryGetValue(key, &value) with
        | true -> Some value
        | false -> None        
        
    [<Extension>]
    static member Get(dict: IDictionary<'k, 'v>, key: 'k) : 'v =
        dict[key]        

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
        
    [<Extension>]
    static member Bind(a: Result<'ok, 'err>, f) =
        Result.bind f a
