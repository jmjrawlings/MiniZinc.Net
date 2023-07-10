namespace MiniZinc.Tests

open System
open System.IO
open System.Reflection
open System.Runtime.CompilerServices

[<AutoOpen>]
module Prelude =
            
    let inline (?=) a b =
        if a <> b then
            failwithf $"Values were not equal"
        
    let (</>) a b =
        Path.Join(string a, string b)
        |> FileInfo
    
    let (<//>) a b =
        Path.Join(string a, string b)
        |> DirectoryInfo
        
    let private assembly =
        Assembly.GetExecutingAssembly().Location
        |> Path.GetFullPath
        |> FileInfo
        
    let project_dir =
        let dir = assembly.Directory.Parent.Parent.Parent.Parent.Parent
        dir
        
    let tests_dir =
        project_dir <//> "tests"
        
        
[<Extension>]
type Extensions() =
    
    [<Extension>]
    static member AssertOk(result: Result<'ok, 'err>, msg: string) =
        match result with
        | Ok value -> value
        | Error err -> failwith $"{msg}: Error was {err}"
        
    [<Extension>]
    static member AssertOk(result: Result<'ok, 'err>) =
        match result with
        | Ok value -> value
        | Error err -> failwith $"{err}"
        
    [<Extension>]
    static member AssertErr(result: Result<'ok, 'err>) =
        match result with
        | Ok value -> failwith $"Expected an error but got {value}"
        | Error err -> ()
                    
    [<Extension>]
    static member AssertEmpty(seq: 't seq, msg: string) =
        match Seq.isEmpty seq with
        | true -> ()
        | false -> failwith msg
        
    [<Extension>]
    static member AssertEmpty(seq: 't seq) =
        match Seq.isEmpty seq with
        | true -> ()
        | false -> failwith "Sequence should be empty"
        
    [<Extension>]
    static member AssertNonEmpty(seq: 't seq, msg: string) =
        match Seq.isEmpty seq with
        | true -> failwith msg
        | false -> ()
        
    [<Extension>]
    static member AssertNonEmpty(seq: 't seq) =
        match Seq.isEmpty seq with
        | true -> failwith "Sequence should not be empty"
        | false -> ()
        
    [<Extension>]
    static member inline AssertEquals(a, b) =
        if a <> b then
            failwithf $"{a} does not equal {b}"        
    
    [<Extension>]
    static member inline StringEquals(a, b: string) =
        if String.Equals(string a, b, StringComparison.OrdinalIgnoreCase) then
            ()
        else
            failwithf $"{a} does not equal {b}"