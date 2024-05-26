namespace MiniZinc.Process;

using System.Diagnostics;
using System.Text;
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

    /// If listening, the last ProcessMessage received
    public ProcessMessage Current { get; private set; }

    /// The Id of the process if it ever started
    public int ProcessId { get; private set; }

    private readonly ProcessStartInfo _startInfo;
    private readonly SystemProcess _process;
    private readonly Stopwatch _watch;
    private readonly ILogger _logger;

    /// If iterating, a channel to implement AsyncEnumerable
    private Channel<ProcessMessage>? _channel;
    private IAsyncEnumerable<ProcessMessage>? _events;

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
    public async Task<ProcessResult> Wait(
        bool captureStdOut = true,
        bool captureStdErr = true,
        CancellationToken cancellation = default
    )
    {
        StringBuilder? stdout = null;
        StringBuilder? stderr = null;

        await foreach (var msg in Watch(cancellation))
        {
            switch (msg.EventType)
            {
                case ProcessEventType.Started:
                    break;
                case ProcessEventType.StdOut:
                    if (captureStdOut)
                        (stdout ??= new StringBuilder()).Append(msg.Content);
                    break;
                case ProcessEventType.StdErr:
                    if (captureStdErr)
                        (stderr ??= new StringBuilder()).Append(msg.Content);
                    break;
                case ProcessEventType.Exited:
                    break;
            }
        }

        var output = stdout?.ToString() ?? string.Empty;
        var error = stderr?.ToString() ?? string.Empty;

        var result = new ProcessResult
        {
            Command = Command.String,
            Status = Status,
            StdOut = output,
            StdErr = error,
            StartTime = StartTime,
            EndTime = EndTime,
            Duration = Elapsed,
            ExitCode = ExitCode
        };
        return result;
    }

    /// <inheritdoc cref="Wait(bool,bool,System.Threading.CancellationToken)"/>
    public async Task<ProcessResult> Wait(CancellationToken cancellation = default) =>
        await Wait(true, true, cancellation);

    public ProcessResult WaitSync() => Wait(CancellationToken.None).Result;

    /// <summary>
    /// Start the process and consume events until it
    /// terminates
    /// </summary>
    public IAsyncEnumerable<ProcessMessage> Watch(CancellationToken cancellation = default)
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
        _channel = Channel.CreateUnbounded<ProcessMessage>(
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
        Current = new ProcessMessage
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
        var msg = new ProcessMessage
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
        var msg = new ProcessMessage
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

        Current = new ProcessMessage
        {
            EventType = ProcessEventType.Exited,
            TimeStamp = StartTime + Elapsed
        };

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
