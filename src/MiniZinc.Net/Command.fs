namespace MiniZinc.Net

open System
open System.IO
open CliWrap
open CliWrap.EventStream
open FSharp.Control

[<AutoOpen>]    
module Command =    
        
    [<Struct>]
    type CommandEvent =
        | Start of id:int
        | Output of output:string
        | Error of error:string
        | Exit of code:int

    type Command with 

        static member create (command: string) =
            Cli.Wrap(command)
            
        static member create(command:string, args: string) =
            Command.create(command).WithArguments(args)
            
        static member create (command: string, [<ParamArray>] args: string[]) =
            Command.create(command, args)
                
        static member create (command: string, [<ParamArray>] args: Object[]) =
            let args =
                args
                |> Seq.map string
            Command.create(command, args)
            
        static member workdir (dir: string) =
            fun (cmd: Command) ->
                cmd.WithWorkingDirectory dir

        static member workdir (dir: DirectoryInfo) =
            fun (cmd: Command) ->
                cmd.WithWorkingDirectory dir.FullName
            
        static member exec (cmd: Command) =
            cmd.ExecuteAsync()
            
        static member listen (cmd: Command) =
            cmd.ListenAsync()
            |> AsyncSeq.ofAsyncEnum
            |> AsyncSeq.map (function
                | :? StandardOutputCommandEvent as e ->
                    CommandEvent.Output e.Text
                | :? StandardErrorCommandEvent as e ->
                    CommandEvent.Error e.Text
                | :? StartedCommandEvent as e ->
                    CommandEvent.Start e.ProcessId
                | :? ExitedCommandEvent as e ->
                    CommandEvent.Exit e.ExitCode
                | other ->
                    failwithf $"Unknown object {other}"
                )
