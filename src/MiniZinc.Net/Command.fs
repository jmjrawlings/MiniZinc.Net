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

        // Create a Command
        static member create (command: string) =
            Cli.Wrap(command)
            
        // Create a Command
        static member create(command:string, args: string) =
            Command.create(command).WithArguments(args)
        
        // Create a Command    
        static member create (command: string, [<ParamArray>] args: string[]) =
            Command.create(command, args)
                
        // Create a Command
        static member create (command: string, [<ParamArray>] args: Object[]) =
            let args =
                args
                |> Seq.map string
            Command.create(command, args)
        
        // Set the working directory    
        static member workdir (dir: string) =
            fun (cmd: Command) ->
                cmd.WithWorkingDirectory dir

        // Set the working directory
        static member workdir (dir: DirectoryInfo) =
            fun (cmd: Command) ->
                cmd.WithWorkingDirectory dir.FullName
            
        // Execute the given Command            
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

        // Stream standard out from the given command
        static member stdout (cmd: Command) =
            cmd
            |> Command.listen
            |> AsyncSeq.choose (fun ev ->
                match ev with
                | CommandEvent.Output x ->
                    Some x
                | _ ->
                    None
                )
