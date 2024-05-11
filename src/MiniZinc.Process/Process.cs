namespace MiniZinc.Process;

using System.Diagnostics;
using System.Threading.Channels;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using SystemProcess = System.Diagnostics.Process;

/// <summary>
/// Wraps and starts a System.Diagnostics.Process
/// </summary>
public sealed class Process : IDisposable
{
    /// The originating command
    public readonly Command Command;

    /// Current state of the process
    public ProcessStatus Status { get; private set; }

    /// Time the process was started
    public DateTimeOffset StartTime { get; private set; }

    /// Time the process ended if it has ended
    public DateTimeOffset EndTime { get; private set; }

    /// Current elapsed duration or total duration if exted
    public TimeSpan Elapsed => _watch.Elapsed;

    /// The process exit code if it has exited
    public int ExitCode { get; private set; }

    /// Should stdout be buffered?
    public bool BufferStdOut { get; init; }

    /// Should stderr be buffered?
    public bool BufferStdErr { get; init; }

    /// If listening, the last ProcessMessage received
    public ProcessEventMessage Current { get; private set; }

    /// The Id of the process if it ever started
    public int ProcessId { get; private set; }

    private readonly ProcessStartInfo _startInfo;
    private readonly SystemProcess _process;
    private readonly Stopwatch _watch;
    private readonly ILogger _logger;

    /// If iterating, a channel to implement AsyncEnumerable
    private Channel<ProcessEventMessage>? _channel;
    private IAsyncEnumerable<ProcessEventMessage>? _events;

    /// If running, marks the completion
    private TaskCompletionSource? _completion;

    /// <summary>
    /// Create a process from the given command
    /// </summary>
    public Process(in Command command, ILogger? logger = null)
    {
        _logger = logger ?? new NullLogger<Process>();
        _watch = new Stopwatch();
        _startInfo = new ProcessStartInfo
        {
            FileName = command.Exe,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        _logger.LogInformation("Command is {Command}", command.String);
        foreach (var arg in command.Args)
        {
            _logger.LogDebug("{Arg}", arg);
            _startInfo.ArgumentList.Add(arg.String);
        }

        if (command.WorkingDir is { } path)
        {
            _logger.LogInformation("Setting working directory to {Path}", path);
            _startInfo.WorkingDirectory = path;
        }

        _process = new SystemProcess();
        _process.EnableRaisingEvents = true;
        _process.StartInfo = _startInfo;
        _process.OutputDataReceived += OnOutput;
        _process.ErrorDataReceived += OnError;
        _process.Exited += OnExit;

        Command = command;
    }

    /// <summary>
    /// Run the process until it either terminates or a cancellation
    /// is requested.
    /// </summary>
    public async Task Run(CancellationToken cancellation = default)
    {
        if (_completion is not null)
        {
            await _completion.Task;
            return;
        }

        if (Status is not ProcessStatus.Idle)
            throw new InvalidOperationException();

        StartTime = DateTimeOffset.Now;
        EndTime = StartTime;

        _completion = new TaskCompletionSource();
        _watch.Start();
        _process.Start();
        _process.BeginOutputReadLine();
        _process.BeginErrorReadLine();
        _logger.LogInformation("ProcessId is {ProcessId}", _process.Id);

        if (cancellation.IsCancellationRequested)
            Stop();
        else
            cancellation.Register(Stop, useSynchronizationContext: false);

        await _completion.Task.ConfigureAwait(false);
    }

    /// <summary>
    /// Start the process and consume events until it
    /// terminates
    /// </summary>
    public IAsyncEnumerable<ProcessEventMessage> Listen(CancellationToken cancellation = default)
    {
        if (_events is not null)
            return _events;

        if (Status is not ProcessStatus.Idle)
            throw new InvalidOperationException();

        StartTime = DateTimeOffset.Now;
        EndTime = StartTime;
        Status = ProcessStatus.Running;
        _logger.LogInformation("Starting process");
        Status = ProcessStatus.Running;
        _channel = Channel.CreateUnbounded<ProcessEventMessage>(
            new UnboundedChannelOptions
            {
                SingleWriter = true,
                SingleReader = true,
                AllowSynchronousContinuations = true
            }
        );
        _events = _channel.Reader.ReadAllAsync();
        _watch.Start();
        _process.Start();
        _process.BeginOutputReadLine();
        _process.BeginErrorReadLine();
        _logger.LogInformation("ProcessId is {ProcessId}", _process.Id);

        ProcessId = _process.Id;
        Current = new ProcessEventMessage
        {
            EventType = ProcessEventType.Started,
            TimeStamp = StartTime
        };

        if (cancellation.IsCancellationRequested)
            Stop();
        else
            cancellation.Register(Stop, useSynchronizationContext: false);

        return _events;
    }

    private void Stop()
    {
        _logger.LogInformation("Stop requested");
        if (Status is not ProcessStatus.Running)
            return;

        if (_process.HasExited)
            return;

        _logger.LogInformation("Killing process");
        Status = ProcessStatus.Signalled;
        try
        {
            _process.Kill();
        }
        catch
        {
            _logger.LogError("Could not kill the {Exe}", Command.Exe);
        }
    }

    private void OnOutput(object _, DataReceivedEventArgs e)
    {
        if (e.Data is not { } data)
            return;

        _logger.LogDebug("{Output}", data);

        var elapsed = Elapsed;
        var msg = new ProcessEventMessage
        {
            Content = data,
            EventType = ProcessEventType.StdOut,
            TimeStamp = StartTime + elapsed
        };
        Current = msg;
        _channel?.Writer.TryWrite(msg);
    }

    private void OnError(object _, DataReceivedEventArgs e)
    {
        if (e.Data is not { } data)
            return;

        _logger.LogError("{Error}", data);

        var elapsed = Elapsed;
        var msg = new ProcessEventMessage
        {
            Content = e.Data,
            EventType = ProcessEventType.StdErr,
            TimeStamp = StartTime + elapsed
        };
        Current = msg;
        _channel?.Writer.TryWrite(msg);
    }

    private void OnExit(object? _s, EventArgs _e)
    {
        _watch.Stop();
        ExitCode = _process.ExitCode;
        switch (ExitCode)
        {
            case 0:
                Status = ProcessStatus.Ok;
                _logger.LogInformation("{Exe} succeeded after {Elapsed}", Command.Exe, Elapsed);
                break;
            case { } when Status is ProcessStatus.Signalled:
                Status = ProcessStatus.Cancelled;
                _logger.LogInformation("{Exe} was cancelled after {Elapsed}", Command.Exe, Elapsed);
                break;
            default:
                _logger.LogError(
                    "{Exe} failed with code {ExitCode} after {Elapsed}",
                    Command.Exe,
                    ExitCode,
                    Elapsed
                );
                Status = ProcessStatus.Error;
                break;
        }

        Current = new ProcessEventMessage
        {
            EventType = ProcessEventType.Exited,
            TimeStamp = StartTime + Elapsed
        };

        _completion?.SetResult();
        _channel?.Writer.TryWrite(Current);
        _channel?.Writer.TryComplete();
    }

    ///
    public void Dispose()
    {
        Stop();
        _process.Dispose();
    }

    ///
    public override string ToString()
    {
        return $"<Process \"{Command.Exe}\" | {Status} after {_watch.Elapsed.TotalSeconds}s>";
    }
}
