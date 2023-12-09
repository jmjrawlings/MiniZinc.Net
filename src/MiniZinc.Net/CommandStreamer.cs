using System.Diagnostics;
using System.Threading.Channels;

namespace MiniZinc.Net;

internal sealed class CommandStreamer
{
    public readonly Command Command;
    public readonly ProcessStartInfo StartInfo;
    public readonly Process Process;
    public readonly DateTimeOffset StartTime;
    public readonly string CommandString;
    private readonly Channel<CommandOutput> _channel;

    public CommandStreamer(Command cmd)
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
        _channel = Channel.CreateUnbounded<CommandOutput>(
            new UnboundedChannelOptions
            {
                SingleWriter = false,
                SingleReader = false,
                AllowSynchronousContinuations = false
            }
        );
    }

    public async IAsyncEnumerable<CommandOutput> Stream()
    {
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

        _channel.Writer.TryWrite(msg);
        Process.BeginOutputReadLine();
        Process.BeginErrorReadLine();

        await foreach (var output in _channel.Reader.ReadAllAsync())
        {
            yield return output;
        }
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
        _channel.Writer.TryWrite(msg);
        _channel.Writer.TryComplete();
    }

    private void OnError(object sender, DataReceivedEventArgs e)
    {
        if (e.Data is null)
            return;

        var msg = new CommandOutput
        {
            Command = CommandString,
            ProcessId = Process.Id,
            Message = e.Data,
            Status = CommandStatus.StdErr,
            StartTime = StartTime,
            TimeStamp = DateTimeOffset.Now,
            Elapsed = DateTimeOffset.Now - StartTime
        };
        _channel.Writer.TryWrite(msg);
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
        _channel.Writer.TryWrite(msg);
    }
}
