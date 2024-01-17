namespace MiniZinc.Net;

using System.Diagnostics;
using System.Text;

/// <summary>
/// Runs the given command to completion
/// </summary>
internal sealed class CommandRunner
{
    public readonly Command Command;
    public readonly ProcessStartInfo StartInfo;
    public readonly Process Process;
    public readonly DateTimeOffset StartTime;
    public readonly string CommandString;
    private readonly TaskCompletionSource<CommandResult> _tcs;

    public CommandRunner(Command cmd)
    {
        var stderr = new StringBuilder();
        var stdout = new StringBuilder();
        _tcs = new TaskCompletionSource<CommandResult>();
        Command = cmd;
        StartInfo = new ProcessStartInfo
        {
            FileName = cmd.Exe,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };
        if (cmd.WorkingDir is not null)
            StartInfo.WorkingDirectory = cmd.WorkingDir.FullName;

        CommandString = cmd.String;
        if (cmd.Args is not null)
            foreach (var arg in cmd.Args)
                StartInfo.ArgumentList.Add(arg);

        Process = new Process();
        Process.EnableRaisingEvents = true;
        Process.StartInfo = StartInfo;
        Process.OutputDataReceived += (_, args) =>
        {
            if (args.Data is null)
                return;
            stdout.Append(args.Data);
        };
        Process.ErrorDataReceived += (_, args) =>
        {
            if (args.Data is null)
                return;
            stderr.Append(args.Data);
        };
        Process.Exited += (_, _) =>
        {
            var endTime = DateTimeOffset.Now;
            var result = new CommandResult
            {
                Command = CommandString,
                StartTime = StartTime,
                EndTime = endTime,
                Duration = endTime - StartTime,
                ExitCode = Process.ExitCode,
                IsError = Process.ExitCode > 0,
                StdOut = stdout.ToString(),
                StdErr = stderr.ToString()
            };
            Process.Dispose();
            _tcs.SetResult(result);
        };

        StartTime = DateTimeOffset.UtcNow;
    }

    public async Task<CommandResult> Run()
    {
        Process.Start();
        Process.BeginOutputReadLine();
        Process.BeginErrorReadLine();
        var result = await _tcs.Task;
        return result;
    }
}
