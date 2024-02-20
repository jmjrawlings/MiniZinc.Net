namespace MiniZinc.Net;

using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Threading.Channels;
using Microsoft.Extensions.Logging;

/// <summary>
/// Starts a command and publishes the messages received
/// as an asynchronous stream
/// </summary>
internal sealed class CommandListener
{
    public readonly Command Command;
    public readonly string CommandString;
    public readonly DateTimeOffset StartTime;
    private readonly ProcessStartInfo _startInfo;
    private readonly Process _process;
    private readonly Channel<CommandMessage> _channel;
    private readonly ILogger _logger;

    public CommandListener(Command cmd)
    {
        _logger = Logging.Factory.CreateLogger<CommandListener>();
        Command = cmd;
        StartTime = DateTimeOffset.Now;
        _startInfo = new ProcessStartInfo
        {
            FileName = cmd.Exe,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        CommandString = cmd.String;
        _logger.LogInformation("Running {Exe}", cmd.Exe);
        _logger.LogInformation("{CommandString}", CommandString);

        foreach (var arg in cmd.Args)
        {
            _logger.LogDebug("{Arg}", arg);
            _startInfo.ArgumentList.Add(arg);
        }

        _process = new Process();
        _process.EnableRaisingEvents = true;
        _process.StartInfo = _startInfo;
        _process.OutputDataReceived += OnOutput;
        _process.ErrorDataReceived += OnError;
        _process.Exited += OnExit;
        _channel = Channel.CreateUnbounded<CommandMessage>(
            new UnboundedChannelOptions
            {
                SingleWriter = true,
                SingleReader = true,
                AllowSynchronousContinuations = false
            }
        );
    }

    public async IAsyncEnumerable<CommandMessage> Listen(
        [EnumeratorCancellation] CancellationToken cancellationToken = default
    )
    {
        _process.Start();
        yield return new CommandMessage
        {
            Command = CommandString,
            ProcessId = _process.Id,
            Content = string.Empty,
            MessageType = CommandMessageType.Started,
            StartTime = DateTimeOffset.Now,
            TimeStamp = DateTimeOffset.Now,
            Elapsed = TimeSpan.Zero
        };

        _process.BeginOutputReadLine();
        _process.BeginErrorReadLine();
        await foreach (var msg in _channel.Reader.ReadAllAsync(cancellationToken))
        {
            yield return msg;
        }
    }

    private void OnExit(object? sender, EventArgs e)
    {
        CommandMessageType messageType;
        LogLevel level;
        if (_process.ExitCode > 0)
        {
            level = LogLevel.Error;
            messageType = CommandMessageType.Failure;
        }
        else
        {
            level = LogLevel.Information;
            messageType = CommandMessageType.Success;
        }

        var msg = new CommandMessage
        {
            Command = CommandString,
            ProcessId = _process.Id,
            Content = string.Empty,
            MessageType = messageType,
            StartTime = StartTime,
            TimeStamp = DateTimeOffset.Now,
            Elapsed = DateTimeOffset.Now - StartTime
        };

        _logger.Log(
            level,
            "Process exited with code {ExitCode} after {Elapsed}",
            _process.ExitCode,
            msg.Elapsed
        );
        _process.Dispose();
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
            ProcessId = _process.Id,
            Content = e.Data,
            MessageType = CommandMessageType.StdErr,
            StartTime = StartTime,
            TimeStamp = DateTimeOffset.Now,
            Elapsed = DateTimeOffset.Now - StartTime
        };
        _logger.LogError("{Error}", msg.Content);
        _channel.Writer.TryWrite(msg);
    }

    private void OnOutput(object sender, DataReceivedEventArgs e)
    {
        if (e.Data is null)
            return;

        var msg = new CommandMessage
        {
            Command = CommandString,
            ProcessId = _process.Id,
            Content = e.Data,
            MessageType = CommandMessageType.StdOut,
            StartTime = StartTime,
            TimeStamp = DateTimeOffset.Now,
            Elapsed = DateTimeOffset.Now - StartTime
        };
        _logger.LogInformation("{Message}", msg.Content);
        _channel.Writer.TryWrite(msg);
    }

    public override string ToString()
    {
        return $"<CommandListener of \"{CommandString}\"";
    }
}
