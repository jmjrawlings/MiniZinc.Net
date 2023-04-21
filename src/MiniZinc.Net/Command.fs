namespace MiniZinc.Net

open System
open System.IO
open CliWrap.EventStream
open FSharp.Control
open System.Threading.Tasks
open CliWrap.Buffered

[<AutoOpen>]    
module CommandExtensions =
            
    [<Struct>]
    type CommandEvent =
        | Start of id:int
        | Output of output:string
        | Error of error:string
        | Exit of code:int

open CliWrap
type CliCommand = CliWrap.Command

type Command =

    // Create a Command
    static member create (command: string) =
        Cli.Wrap(command).WithValidation(CommandResultValidation.None)
        
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
        
    // Add arugments
    static member withArgs (args: string) (cmd: CliCommand) =
        cmd.WithArguments(args)
    
    // Set the working directory    
    static member workdir (dir: string) =
        fun (cmd: CliCommand) ->
            cmd.WithWorkingDirectory dir

    // Set the working directory
    static member workdir (dir: DirectoryInfo) =
        Command.workdir dir.FullName
        
    // Execute the given Command asynchronously
    static member execAsync (cmd: CliCommand) =
        task {
            let! x = cmd.ExecuteBufferedAsync()
            return x
        }
        |> Async.AwaitTask
        
    // Execute the given Command in a synchronous manner            
    static member exec (cmd: CliCommand) =
        cmd
        |> Command.execAsync
        |> Async.RunSynchronously
        
    static member listen (cmd: CliCommand) =
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
    static member stdout (cmd: CliCommand) =
        cmd
        |> Command.listen
        |> AsyncSeq.choose (fun ev ->
            match ev with
            | CommandEvent.Output x ->
                Some x
            | _ ->
                None
            )