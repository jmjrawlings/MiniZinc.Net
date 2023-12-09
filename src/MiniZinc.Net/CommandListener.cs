using System.Diagnostics;
using System.Threading.Channels;

namespace MiniZinc.Net;

internal sealed class CommandListener
{
    public readonly Command Command;
    public readonly ProcessStartInfo StartInfo;
    public readonly Process Process;
    public readonly DateTimeOffset StartTime;
    public readonly string CommandString;

    public CommandListener(Command cmd)
    {
        Command = cmd;
        StartInfo = new ProcessStartInfo
        {
            FileName = cmd.Exe,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        CommandString = cmd.String;
        if (cmd.Args is { } args)
        {
            foreach (var arg in args)
                StartInfo.ArgumentList.Add(arg.String);
        }

        Process = new Process();
        Process.EnableRaisingEvents = true;
        Process.StartInfo = StartInfo;
        Process.OutputDataReceived += OnOutput;
        Process.ErrorDataReceived += OnError;
        Process.Exited += OnExit;
        Process.Start();
        StartTime = DateTimeOffset.Now;
        var msg = new CommandOutput
        {
            Command = CommandString,
            ProcessId = Process.Id,
            Message = string.Empty,
            Status = CommandStatus.Started,
            StartTime = DateTimeOffset.Now,
            TimeStamp = DateTimeOffset.Now,
            Elapsed = TimeSpan.Zero
        };

        // channel.Writer.TryWrite(startMessage);
        Process.BeginOutputReadLine();
        Process.BeginErrorReadLine();
        // channel.Reader.ReadAllAsync();
        // return;
    }

    private void OnExit(object? sender, EventArgs e)
    {
        CommandStatus status;
        if (Process.ExitCode > 0)
            status = CommandStatus.Failure;
        else
        {
            status = CommandStatus.Success;
        }

        var msg = new CommandOutput
        {
            Command = CommandString,
            ProcessId = Process.Id,
            Message = string.Empty,
            Status = status,
            StartTime = StartTime,
            TimeStamp = DateTimeOffset.Now,
            Elapsed = DateTimeOffset.Now - StartTime
        };

        Process.Dispose();
        // channel.Writer.TryWrite(msg);
        // channel.Writer.Complete();
    }

    private void OnError(object sender, DataReceivedEventArgs e)
    {
        throw new NotImplementedException();
    }

    private void OnOutput(object sender, DataReceivedEventArgs e)
    {
        if (e.Data is null)
            return;

        var msg = new CommandOutput
        {
            Command = CommandString,
            ProcessId = Process.Id,
            Message = e.Data,
            Status = CommandStatus.StdOut,
            StartTime = StartTime,
            TimeStamp = DateTimeOffset.Now,
            Elapsed = DateTimeOffset.Now - StartTime
        };
    }

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
}
