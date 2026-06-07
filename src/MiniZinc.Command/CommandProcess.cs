namespace MiniZinc.Command;

using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Channels;

internal sealed class CommandProcess : IDisposable
{
    /// The originating command
    private readonly Command _command;

    /// The terminal outcome, set when the process exits
    private ProcessStatus _status;

    /// Has the process been started? (one-shot guard)
    private bool _started;

    /// Was cancellation requested? (distinguishes Cancelled from Error)
    private bool _cancelled;

    /// Time the process was started
    private DateTimeOffset _startTime;

    /// Time the process ended if it has ended
    private DateTimeOffset _endTime;

    /// Current elapsed duration or total duration if exited
    private TimeSpan _elapsed => _watch.Elapsed;

    /// The process exit code if it has exited
    private int _exitCode;

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
    internal CommandProcess(Command command)
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
    /// Run the process to completion (or until cancelled), capturing stdout and
    /// stderr into the returned <see cref="ProcessResult"/>.
    /// </summary>
    internal async Task<ProcessResult> Run(CancellationToken cancellation = default)
    {
        StringBuilder? stdout = null;
        StringBuilder? stderr = null;

        await foreach (ProcessMessage msg in Watch(cancellation))
        {
            switch (msg)
            {
                case ProcessStdOut o:
                    (stdout ??= new StringBuilder()).AppendLine(o.Text);
                    break;
                case ProcessStdErr e:
                    (stderr ??= new StringBuilder()).AppendLine(e.Text);
                    break;
            }
        }

        var result = new ProcessResult
        {
            Command = _command.ToString(),
            Status = _status,
            StdOut = stdout?.ToString() ?? string.Empty,
            StdErr = stderr?.ToString() ?? string.Empty,
            StartTime = _startTime,
            EndTime = _endTime,
            Duration = _elapsed,
            ExitCode = _exitCode,
        };
        return result;
    }

    /// <summary>
    /// Start the process and consume events until it
    /// terminates
    /// </summary>
    internal async IAsyncEnumerable<ProcessMessage> Watch(
        [EnumeratorCancellation] CancellationToken cancellation = default
    )
    {
        if (_started)
            throw new InvalidOperationException("This process has already been started");
        _started = true;

        _startTime = DateTimeOffset.Now;
        _endTime = _startTime;
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
        _processId = _process.Id;
        // Emit Started before reading begins so it is always the first message.
        _channel.Writer.TryWrite(
            new ProcessStarted { ProcessId = _processId, TimeStamp = _startTime }
        );
        _process.BeginOutputReadLine();
        _process.BeginErrorReadLine();

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
        if (!_started || _process.HasExited)
            return;

        _cancelled = true;
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

        _channel?.Writer.TryWrite(
            new ProcessStdOut
            {
                ProcessId = _processId,
                Text = e.Data,
                TimeStamp = _startTime + _elapsed,
            }
        );
    }

    private void OnError(object _, DataReceivedEventArgs e)
    {
        if (e.Data is null)
        {
            MarkEof(stdout: false);
            return;
        }

        _channel?.Writer.TryWrite(
            new ProcessStdErr
            {
                ProcessId = _processId,
                Text = e.Data,
                TimeStamp = _startTime + _elapsed,
            }
        );
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
            case { } when _cancelled:
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

        _channel?.Writer.TryWrite(
            new ProcessExited
            {
                ProcessId = _processId,
                ExitCode = _exitCode,
                TimeStamp = _startTime + _elapsed,
            }
        );
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
