namespace MiniZinc.Process;

using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Channels;
using CommunityToolkit.Diagnostics;
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
    public ProcessState State { get; private set; }

    /// Time the process was started
    public DateTime StartTime => _process.StartTime;

    /// Current elapsed duration or total duration if exted
    public TimeSpan Elapsed => _watch.Elapsed;

    /// Current timestamp or the completion time if exited
    public DateTime TimeStamp => StartTime + Elapsed;

    /// The process exit code if it has exited
    public int ExitCode { get; private set; }

    /// If listening, the last ProcessMessage received
    public ProcessEventMessage LastEventMessage { get; private set; }

    /// If listening, the last ProcessMessage received
    public readonly Task<ProcessResult> Result;

    /// The Id of the process if it ever started
    public int ProcessId { get; private set; }

    /// If listening, a channel to implement AsyncEnumerable
    private readonly Channel<ProcessEventMessage> _channel;

    private IAsyncEnumerable<ProcessEventMessage>? _events;

    private readonly ProcessStartInfo _startInfo;
    private readonly SystemProcess _process;
    private readonly Stopwatch _watch;
    private readonly TaskCompletionSource<ProcessResult> _tcs;
    private readonly ILogger _logger;
    private readonly CancellationToken _cancellationToken;

    /// <summary>
    /// Create a process from the given command
    /// </summary>
    public Process(
        in Command command,
        CancellationToken cancellationToken = default,
        ILogger? logger = null
    )
    {
        _logger = logger ?? new NullLogger<Process>();
        _watch = new Stopwatch();
        _tcs = new TaskCompletionSource<ProcessResult>();
        _cancellationToken = cancellationToken;
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
        Result = _tcs.Task;

        _logger.LogInformation("Starting process");
        State = ProcessState.Running;
        _channel = Channel.CreateUnbounded<ProcessEventMessage>(
            new UnboundedChannelOptions
            {
                SingleWriter = true,
                SingleReader = true,
                AllowSynchronousContinuations = true
            }
        );

        _watch.Start();
        _process.Start();
        ProcessId = _process.Id;
        LastEventMessage = new ProcessEventMessage
        {
            EventType = ProcessEventType.Started,
            TimeStamp = StartTime
        };
        if (_cancellationToken.IsCancellationRequested)
            Stop();
        else
        {
            _cancellationToken.Register(Stop);
            _channel.Writer.TryWrite(LastEventMessage);
            _process.BeginOutputReadLine();
            _process.BeginErrorReadLine();
            _logger.LogInformation("ProcessId is {ProcessId}", _process.Id);
        }
    }

    public IAsyncEnumerable<ProcessEventMessage> Events
    {
        get
        {
            if (_events is not null)
                return _events;

            _events = _channel.Reader.ReadAllAsync();
            return _events;
        }
    }

    private void SetResult()
    {
        var result = new ProcessResult
        {
            Command = Command.String,
            State = State,
            StartTime = StartTime,
            EndTime = TimeStamp,
            Duration = Elapsed,
            ExitCode = _process.ExitCode,
        };
        _tcs.SetResult(result);
    }

    /// <summary>
    /// Run the process, returning the result on termination.
    /// </summary>
    public async Task<ProcessResult> Run(
        CancellationToken cancellationToken = default,
        bool stderr = true,
        bool stdout = true
    )
    {
        if (State is not ProcessState.Idle)
            ThrowHelper.ThrowInvalidOperationException();

        if (cancellationToken.IsCancellationRequested)
        {
            State = ProcessState.Cancelled;
            SetResult();
        }
        else { }
        var result = await _tcs.Task.ConfigureAwait(false);
        return result;
    }

    private void Stop()
    {
        _logger.LogInformation("Stop requested");
        if (State is not ProcessState.Running)
            return;

        if (_process.HasExited)
            return;

        _logger.LogInformation("Killing process");
        State = ProcessState.Signalled;
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
        LastEventMessage = msg;
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
        LastEventMessage = msg;
        _channel?.Writer.TryWrite(msg);
    }

    private void OnExit(object? _s, EventArgs _e)
    {
        _watch.Stop();
        switch (_process.ExitCode)
        {
            case 0:
                State = ProcessState.Ok;
                _logger.LogInformation("{Exe} succeeded after {Elapsed}", Command.Exe, Elapsed);
                break;
            case { } when State is ProcessState.Signalled:
                State = ProcessState.Cancelled;
                _logger.LogInformation("{Exe} was cancelled after {Elapsed}", Command.Exe, Elapsed);
                break;
            case var exitCode:
                _logger.LogError(
                    "{Exe} failed with code {ExitCode} after {Elapsed}",
                    Command.Exe,
                    exitCode,
                    Elapsed
                );
                State = ProcessState.Error;
                break;
        }

        ExitCode = _process.ExitCode;
        SetResult();

        LastEventMessage = new ProcessEventMessage()
        {
            EventType = ProcessEventType.Exited,
            Content = null,
            TimeStamp = StartTime + Elapsed
        };

        if (_channel is null)
            return;

        _channel.Writer.TryWrite(LastEventMessage);
        _channel.Writer.TryComplete();
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
        return $"<Process \"{Command.Exe}\" | {State} after {_watch.Elapsed.TotalSeconds}s>";
    }

    public async ValueTask DisposeAsync()
    {
        await CastAndDispose(Result);
        await CastAndDispose(_process);

        return;

        static async ValueTask CastAndDispose(IDisposable resource)
        {
            if (resource is IAsyncDisposable resourceAsyncDisposable)
                await resourceAsyncDisposable.DisposeAsync();
            else
                resource.Dispose();
        }
    }
}
