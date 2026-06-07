namespace MiniZinc.Command;

using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Channels;

public sealed class CommandProcess : IDisposable
{
    /// The originating command
    private readonly Command _command;

    /// Current state of the process
    private ProcessStatus _status;

    /// Time the process was started
    private DateTimeOffset _startTime;

    /// Time the process ended if it has ended
    private DateTimeOffset _endTime;

    /// Current elapsed duration or total duration if exited
    private TimeSpan _elapsed => _watch.Elapsed;

    /// The process exit code if it has exited
    private int _exitCode;

    /// If listening, the last ProcessMessage received
    private ProcessMessage _current;

    /// The Id of the process if it ever started
    private int _processId;

    private readonly ProcessStartInfo _startInfo;

    private readonly Process _process;

    private readonly Stopwatch _watch;

    /// If iterating, a channel to implement AsyncEnumerable
    private Channel<ProcessMessage>? _channel;

    // Completion gating: the channel must not complete until BOTH redirected
    // streams have reached EOF AND the process has exited. Process.Exited can
    // fire before the final async stdout/stderr callbacks, so completing on
    // exit alone truncates output. All three flags are guarded by _gate.
    private readonly object _gate = new();
    private bool _stdoutEof;
    private bool _stderrEof;
    private bool _exited;
    private bool _completed;

    /// <summary>
    /// Create a process from the given command
    /// </summary>
    internal CommandProcess(in Command command)
    {
        _watch = new Stopwatch();
        _startInfo = new ProcessStartInfo
        {
            FileName = command.Exe,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };

        // Pass arguments via ArgumentList so each token is escaped independently;
        // a single joined string mishandles values containing spaces (e.g. paths).
        foreach (string token in command.Arguments.Tokens)
            _startInfo.ArgumentList.Add(token);

        if (command.WorkingDirectory is { } path)
        {
            _startInfo.WorkingDirectory = path;
        }

        _process = new Process();
        _process.EnableRaisingEvents = true;
        _process.StartInfo = _startInfo;
        _process.OutputDataReceived += OnOutput;
        _process.ErrorDataReceived += OnError;
        _process.Exited += OnExit;
        _command = command;
    }

    /// <summary>
    /// Run the process until it either terminates or a cancellation
    /// is requested.
    /// </summary>
    internal async Task<ProcessResult> Run(
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
                        (stdout ??= new StringBuilder()).AppendLine(msg.Content);
                    break;
                case ProcessEventType.StdErr:
                    if (captureStdErr)
                        (stderr ??= new StringBuilder()).AppendLine(msg.Content);
                    break;
                case ProcessEventType.Exited:
                    break;
            }
        }

        var output = stdout?.ToString() ?? string.Empty;
        var error = stderr?.ToString() ?? string.Empty;

        var result = new ProcessResult
        {
            Command = _command.ToString(),
            Status = _status,
            StdOut = output,
            StdErr = error,
            StartTime = _startTime,
            EndTime = _endTime,
            Duration = _elapsed,
            ExitCode = _exitCode,
        };
        return result;
    }

    /// <inheritdoc cref="Run(bool,bool,System.Threading.CancellationToken)"/>
    internal async Task<ProcessResult> Run(CancellationToken cancellation = default) =>
        await Run(true, true, cancellation);

    internal ProcessResult WaitSync() => Run(CancellationToken.None).GetAwaiter().GetResult();

    /// <summary>
    /// Start the process and consume events until it
    /// terminates
    /// </summary>
    internal async IAsyncEnumerable<ProcessMessage> Watch(
        [EnumeratorCancellation] CancellationToken cancellation = default
    )
    {
        if (_status is not ProcessStatus.Idle)
            throw new InvalidOperationException("This process has already been started");

        _startTime = DateTimeOffset.Now;
        _endTime = _startTime;
        _status = ProcessStatus.Running;
        // Multiple callback threads (stdout, stderr, exit) write to the channel,
        // so SingleWriter must be false.
        _channel = Channel.CreateUnbounded<ProcessMessage>(
            new UnboundedChannelOptions
            {
                SingleWriter = false,
                SingleReader = true,
                AllowSynchronousContinuations = false,
            }
        );
        _watch.Start();
        _process.Start();
        _process.BeginOutputReadLine();
        _process.BeginErrorReadLine();
        _processId = _process.Id;
        _current = new ProcessMessage
        {
            ProcessId = _processId,
            EventType = ProcessEventType.Started,
            TimeStamp = _startTime,
        };

        CancellationTokenRegistration registration = default;
        if (cancellation.IsCancellationRequested)
            Stop();
        else
            registration = cancellation.Register(Stop, useSynchronizationContext: false);

        try
        {
            await foreach (var msg in _channel.Reader.ReadAllAsync())
                yield return msg;
        }
        finally
        {
            registration.Dispose();
            Dispose();
        }
    }

    private void Stop()
    {
        if (_status is not ProcessStatus.Running)
            return;

        if (_process.HasExited)
            return;

        _status = ProcessStatus.Signalled;
        try
        {
            _process.Kill(entireProcessTree: true);
        }
        catch { }
    }

    private void OnOutput(object _, DataReceivedEventArgs e)
    {
        // null Data signals end-of-stream for the redirected reader.
        if (e.Data is null)
        {
            MarkEof(stdout: true);
            return;
        }

        var msg = new ProcessMessage
        {
            ProcessId = _processId,
            Content = e.Data,
            EventType = ProcessEventType.StdOut,
            TimeStamp = _startTime + _elapsed,
        };
        _current = msg;
        _channel?.Writer.TryWrite(msg);
    }

    private void OnError(object _, DataReceivedEventArgs e)
    {
        if (e.Data is null)
        {
            MarkEof(stdout: false);
            return;
        }

        var msg = new ProcessMessage
        {
            ProcessId = _processId,
            Content = e.Data,
            EventType = ProcessEventType.StdErr,
            TimeStamp = _startTime + _elapsed,
        };
        _current = msg;
        _channel?.Writer.TryWrite(msg);
    }

    private void OnExit(object? _s, EventArgs _e)
    {
        _watch.Stop();
        _endTime = _startTime + _elapsed;
        _exitCode = _process.ExitCode;
        switch (_exitCode)
        {
            case 0:
                _status = ProcessStatus.Ok;
                break;
            case { } when _status is ProcessStatus.Signalled:
                _status = ProcessStatus.Cancelled;
                break;
            default:
                _status = ProcessStatus.Error;
                break;
        }

        lock (_gate)
            _exited = true;
        TryComplete();
    }

    private void MarkEof(bool stdout)
    {
        lock (_gate)
        {
            if (stdout)
                _stdoutEof = true;
            else
                _stderrEof = true;
        }
        TryComplete();
    }

    /// Emit the terminal Exited message and complete the channel, but only once
    /// both streams have drained and the process has exited.
    private void TryComplete()
    {
        lock (_gate)
        {
            if (_completed || !_stdoutEof || !_stderrEof || !_exited)
                return;
            _completed = true;
        }

        var exitMsg = new ProcessMessage
        {
            ProcessId = _processId,
            EventType = ProcessEventType.Exited,
            TimeStamp = _startTime + _elapsed,
        };
        _current = exitMsg;
        _channel?.Writer.TryWrite(exitMsg);
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
        return $"<Process \"{_command.Exe}\" | {_status} after {_watch.Elapsed.TotalSeconds}s>";
    }
}
