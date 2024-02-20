namespace MiniZinc.Net;

using System.Diagnostics;
using System.Text;
using Microsoft.Extensions.Logging;

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
    private readonly StringBuilder? _stderr;
    private readonly StringBuilder? _stdout;
    public readonly Task<CommandResult> Task;
    private readonly ILogger _logger;

    public CommandRunner(in Command cmd, bool captureStdErr = true, bool captureStdOut = true)
    {
        _logger = Logging.Factory.CreateLogger<CommandRunner>();

        if (captureStdErr)
            _stderr = new StringBuilder();

        if (captureStdOut)
            _stdout = new StringBuilder();

        _tcs = new TaskCompletionSource<CommandResult>();
        Task = _tcs.Task;
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
        _logger.LogInformation("Running {Exe}", cmd.Exe);
        _logger.LogInformation("{CommandString}", CommandString);

        foreach (var arg in cmd.Args)
        {
            _logger.LogDebug("{Arg}", arg);
            StartInfo.ArgumentList.Add(arg);
        }

        Process = new Process();
        Process.EnableRaisingEvents = true;
        Process.StartInfo = StartInfo;
        Process.OutputDataReceived += OnOutput;
        Process.ErrorDataReceived += OnError;
        Process.Exited += OnExited;
        StartTime = DateTimeOffset.UtcNow;
    }

    public async Task<CommandResult> Run()
    {
        Process.Start();
        Process.BeginOutputReadLine();
        Process.BeginErrorReadLine();
        _logger.LogInformation("ProcessId is {ProcessId}", Process.Id);
        var result = await _tcs.Task;
        return result;
    }

    private void OnOutput(object? sender, DataReceivedEventArgs e)
    {
        if (e.Data is not { } data)
            return;
        _logger.LogDebug("{Output}", data);
        _stdout?.Append(data);
    }

    private void OnError(object sender, DataReceivedEventArgs e)
    {
        if (e.Data is not { } data)
            return;
        _logger.LogError("{Error}", data);
        _stderr?.Append(data);
    }

    private void OnExited(object? sender, EventArgs e)
    {
        var endTime = DateTimeOffset.Now;
        var result = new CommandResult
        {
            Command = CommandString,
            StartTime = StartTime,
            EndTime = endTime,
            Duration = endTime - StartTime,
            ExitCode = Process.ExitCode,
            StdOut = _stdout?.ToString() ?? string.Empty,
            StdErr = _stderr?.ToString() ?? string.Empty
        };

        _logger.Log(
            result.IsError ? LogLevel.Error : LogLevel.Information,
            "Process exited with code {ExitCode} after {Elapsed}",
            result.ExitCode,
            result.Duration
        );

        Process.Dispose();
        _tcs.SetResult(result);
    }

    public override string ToString()
    {
        return $"<CommandRunner for \"{CommandString}\">";
    }
}
