namespace MiniZinc.Net;

using System.Diagnostics;
using System.Threading.Channels;
using Microsoft.Extensions.Logging;

internal sealed class CommandWatcher : IAsyncEnumerable<CommandMessage>
{
    public readonly Command Command;
    public readonly ProcessStartInfo StartInfo;
    public readonly Process Process;
    public readonly DateTimeOffset StartTime;
    public readonly string CommandString;
    private readonly Channel<CommandMessage> _channel;
    private readonly ILogger? _logger;
    private IAsyncEnumerator<CommandMessage>? _asyncEnumerable;

    public CommandWatcher(Command cmd, ILogger? logger = null)
    {
        _logger = logger;
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
        foreach (var arg in cmd.Args)
            StartInfo.ArgumentList.Add(arg);

        Process = new Process();
        Process.EnableRaisingEvents = true;
        Process.StartInfo = StartInfo;
        Process.OutputDataReceived += OnOutput;
        Process.ErrorDataReceived += OnError;
        Process.Exited += OnExit;
        _logger?.LogInformation("Command {Command} started", CommandString);
        Process.Start();
        _channel = Channel.CreateUnbounded<CommandMessage>(
            new UnboundedChannelOptions
            {
                SingleWriter = false,
                SingleReader = false,
                AllowSynchronousContinuations = false
            }
        );
    }

    private void OnExit(object? sender, EventArgs e)
    {
        CommandOutputType outputType;
        LogLevel level;
        if (Process.ExitCode > 0)
        {
            level = LogLevel.Error;
            outputType = CommandOutputType.Failure;
        }
        else
        {
            level = LogLevel.Information;
            outputType = CommandOutputType.Success;
        }

        var msg = new CommandMessage
        {
            Command = CommandString,
            ProcessId = Process.Id,
            Content = string.Empty,
            Type = outputType,
            StartTime = StartTime,
            TimeStamp = DateTimeOffset.Now,
            Elapsed = DateTimeOffset.Now - StartTime
        };

        _logger?.Log(
            level,
            "Process exited with code {ExitCode} after {Duration}ms",
            Process.ExitCode,
            msg.Elapsed.TotalMilliseconds
        );
        Process.Dispose();
        _channel.Writer.TryWrite(msg);
        _channel.Writer.TryComplete();
    }

    private void OnError(object sender, DataReceivedEventArgs e)
    {
        if (e.Data is null)
            return;

        var msg = new CommandMessage
        {
            Command = CommandString,
            ProcessId = Process.Id,
            Content = e.Data,
            Type = CommandOutputType.StdErr,
            StartTime = StartTime,
            TimeStamp = DateTimeOffset.Now,
            Elapsed = DateTimeOffset.Now - StartTime
        };
        _logger?.LogError("{Message}", msg.Content);
        _channel.Writer.TryWrite(msg);
    }

    private void OnOutput(object sender, DataReceivedEventArgs e)
    {
        if (e.Data is null)
            return;

        var msg = new CommandMessage
        {
            Command = CommandString,
            ProcessId = Process.Id,
            Content = e.Data,
            Type = CommandOutputType.StdOut,
            StartTime = StartTime,
            TimeStamp = DateTimeOffset.Now,
            Elapsed = DateTimeOffset.Now - StartTime
        };
        _logger?.LogInformation("{Message}", msg.Content);
        _channel.Writer.TryWrite(msg);
    }

    public IAsyncEnumerator<CommandMessage> GetAsyncEnumerator(
        CancellationToken cancellationToken = new()
    )
    {
        if (_asyncEnumerable is not null)
            return _asyncEnumerable;

        var msg = new CommandMessage
        {
            Command = CommandString,
            ProcessId = Process.Id,
            Content = string.Empty,
            Type = CommandOutputType.Started,
            StartTime = DateTimeOffset.Now,
            TimeStamp = DateTimeOffset.Now,
            Elapsed = TimeSpan.Zero
        };

        _channel.Writer.TryWrite(msg);
        Process.BeginOutputReadLine();
        Process.BeginErrorReadLine();
        _asyncEnumerable = _channel
            .Reader.ReadAllAsync(cancellationToken)
            .GetAsyncEnumerator(cancellationToken);
        return _asyncEnumerable;
    }
}
