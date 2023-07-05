namespace MiniZinc

open System
open System.IO
open System.Runtime.CompilerServices
open Fake.Core
open Fake.IO
open Fake.Core.TargetOperators

[<AutoOpen>]
module Prelude =
    
    let fi x =
        FileInfo (string x)
        
    let di x =
        DirectoryInfo (string x)
   
    let (</>) a b =
        Path.Join(string a, string b)
        |> FileInfo
        
    let (<//>) a b =
        Path.Join(string a, string b)
        |> DirectoryInfo

    // Execute a git command in the given workdir
    let git workdir args =
        Trace.log $"git {args}"
        args
        |> CreateProcess.fromRawCommandLine "git"
        |> CreateProcess.withWorkingDirectory (string workdir) 
        |> CreateProcess.redirectOutput
        |> CreateProcess.withOutputEventsNotNull Trace.trace Trace.traceError
        |> CreateProcess.ensureExitCode
        |> Proc.run
        
        
    type FileInfo with
        member this.NameWithoutExtension =
            Path.GetFileNameWithoutExtension this.FullName
            
    [<Extension>]
    type Extensions() =
        
        [<Extension>]
        static member RelativeTo(a: string, b: string) =
            let fromUri = Uri(b)
            let toUri = Uri(a)
            let relativeUri = fromUri.MakeRelativeUri(toUri)
            let relativePath = Uri.UnescapeDataString(string relativeUri)
            let result = relativePath.Replace('/', Path.DirectorySeparatorChar)
            result

        [<Extension>]
        static member RelativeTo(a: FileInfo, b) =
            a.FullName.RelativeTo(string b)

        [<Extension>]
        static member RelativeTo(a: DirectoryInfo, b) =
            a.FullName.RelativeTo(string b)
