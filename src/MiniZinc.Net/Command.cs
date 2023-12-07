namespace MiniZinc.Net;

using System;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Collections;
using System.Threading.Channels;

public enum CommandStatus {
    Started = 0,
    StdOut = 1,
    StdErr = 2,
    Success = 3,
    Failure = 4,
}

public readonly record struct CommandOutput
{
    public required string Command { get; init;} 
    public required int ProcessId { get; init;}
    public required string Message { get; init;} 
    public required DateTimeOffset StartTime { get; init;} 
    public required DateTimeOffset TimeStamp { get; init;} 
    public required TimeSpan Elapsed { get; init;} 
    public required CommandStatus Status { get; init;}
}

public readonly record struct CommandResult {
    public required string Command   {get;init;} 
    public required DateTimeOffset StartTime {get;init;} 
    public required DateTimeOffset EndTime   {get;init;} 
    public required TimeSpan Duration  {get;init;} 
    public required string StdOut    {get;init;} 
    public required string StdErr    {get;init;} 
    public required int ExitCode  {get;init;} 
    public required bool IsError   {get;init;}  
}

public enum FlagType
    {
    Short, Long, None
    }

public readonly record struct Arg
{
    public required string Flag { get; init; }
    public required string Value { get; init; }
    public required string Sep { get; init; }
}

/// Command line arguments
public class Args : IEnumerable<Arg>
{
    public readonly List<Arg> List;

    private Args()
    {
        List = new List<Arg>();
    }

    public IEnumerator<Arg> GetEnumerator()
    {
        return List.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return ((IEnumerable)List).GetEnumerator();
    }
}

public readonly struct Command
{
    public readonly string Exe;
    private readonly Arg[]? _args;
    public IReadOnlyCollection<Arg> Args => _args;
    public readonly string String;
    
    private Command(string exe, params Arg[]? args) 
    {
        Exe = exe;
        _args = args;
        if (args is null)
        {
            String = exe;
        }
        else
        {
            String = exe + string.Join(" ", args);
        }
    }

    public override string ToString() => String;

        // /// Create a ProcessStartInfo for the given command        
        // let toStartInfo (cmd: Command) =
        //     let info = ProcessStartInfo()
        //     info.FileName <- cmd.Exe
        //     info.RedirectStandardOutput <- true
        //     info.RedirectStandardError <- true
        //     info.UseShellExecute <- false
        //     info.CreateNoWindow <- true
        //     for arg in cmd.Args do
        //         info.ArgumentList.Add (string arg)
        //     info
        //
        // /// Create a new Process for the given command
        // let toProcess (cmd: Command) =
        //     let proc = new Process()
        //     proc.EnableRaisingEvents <- true
        //     proc.StartInfo <- toStartInfo cmd
        //     proc
        //     
        // /// Start the given command and stream the output back asynchronously
        // let stream (cmd: Command) : IAsyncEnumerable<CommandOutput> =
        //     
        //     let proc =
        //         toProcess cmd
        //         
        //     let channel =
        //         Channel.CreateUnbounded<CommandOutput>()
        //         
        //     let mutable startMessage =
        //         Unchecked.defaultof<CommandOutput>
        //         
        //     let handleData status (args: DataReceivedEventArgs) =
        //         match args.Data with
        //         | null ->
        //             ()
        //         | text ->
        //             let message =
        //                 { startMessage with
        //                     Message = text
        //                     Status = status
        //                     TimeStamp = DateTimeOffset.Now
        //                     Elapsed = DateTimeOffset.Now - startMessage.StartTime }
        //
        //             channel.Writer.TryWrite message
        //             |> ignore
        //             
        //     let handleExit _ =
        //         let message =
        //             { startMessage with
        //                 Status = if proc.ExitCode > 0 then CommandStatus.Success else CommandStatus.Failure
        //                 TimeStamp = DateTimeOffset.Now
        //                 Elapsed = DateTimeOffset.Now - startMessage.StartTime }
        //             
        //         proc.Dispose()
        //         channel.Writer.TryWrite message
        //         channel.Writer.Complete()
        //
        //     proc.OutputDataReceived.Add (handleData CommandStatus.StdOut)
        //     proc.ErrorDataReceived.Add (handleData CommandStatus.StdErr)
        //     proc.Exited.Add handleExit
        //     proc.Start()
        //     
        //     startMessage <-
        //         { Command = cmd.Statement
        //         ; ProcessId = proc.Id
        //         ; Message = ""
        //         ; Status = CommandStatus.Started
        //         ; StartTime = DateTimeOffset.Now 
        //         ; TimeStamp = DateTimeOffset.Now
        //         ; Elapsed = TimeSpan.Zero }
        //     
        //     channel.Writer.TryWrite startMessage
        //     proc.BeginOutputReadLine()
        //     proc.BeginErrorReadLine()
        //     channel.Reader.ReadAllAsync()        
        //             
        // /// Execute the given Command 
        // let run (cmd: Command) =
        //     let proc = toProcess cmd
        //     let statement = Command.statement cmd
        //     let stdout = StringBuilder()
        //     let stderr = StringBuilder()
        //     let complete = TaskCompletionSource<bool>()
        //     
        //     let handleData (builder: StringBuilder) (args: DataReceivedEventArgs) =
        //         match args.Data with
        //         | null ->
        //             ()
        //         | msg ->
        //             ignore (builder.Append msg)
        //     
        //     proc.OutputDataReceived.Add (handleData stdout)
        //     proc.ErrorDataReceived.Add (handleData stderr)
        //     proc.Exited.Add (fun _ -> complete.SetResult true)
        //             
        //     task {
        //         let start_time = DateTimeOffset.Now
        //         
        //         proc.Start()
        //         proc.BeginOutputReadLine()
        //         proc.BeginErrorReadLine()
        //                 
        //         let! _ = complete.Task
        //         let end_time = DateTimeOffset.Now
        //                         
        //         let result =
        //             { Command = cmd.Statement
        //             ; StartTime = start_time
        //             ; EndTime = end_time
        //             ; Duration = end_time - start_time
        //             ; ExitCode = proc.ExitCode
        //             ; IsError = proc.ExitCode > 0 
        //             ; StdOut = string stdout
        //             ; StdErr = string stderr }
        //
        //         proc.Dispose()
        //         return result
        //     }
        //     
        // /// Execute the given Command and wait for the result 
        // let runSync (cmd: Command) =
        //     use proc = toProcess cmd
        //     let statement = Command.statement cmd
        //     let stdout = StringBuilder()
        //     let stderr = StringBuilder()
        //                             
        //     let handleData (builder: StringBuilder) (args: DataReceivedEventArgs) =
        //         match args.Data with
        //         | null ->
        //             ()
        //         | msg ->
        //             ignore (builder.Append msg)
        //     
        //     proc.OutputDataReceived.Add (handleData stdout)
        //     proc.ErrorDataReceived.Add (handleData stderr)
        //             
        //     let start_time = DateTimeOffset.Now
        //     
        //     proc.Start()
        //     proc.BeginOutputReadLine()
        //     proc.BeginErrorReadLine()
        //     proc.WaitForExit()
        //     
        //     let end_time = DateTimeOffset.Now
        //     
        //     let result =
        //         { Command = statement 
        //         ; StartTime = start_time
        //         ; EndTime = end_time
        //         ; Duration = end_time - start_time
        //         ; ExitCode = proc.ExitCode
        //         ; IsError = proc.ExitCode > 0 
        //         ; StdOut = string stdout
        //         ; StdErr = string stderr }
        //     
        //     result