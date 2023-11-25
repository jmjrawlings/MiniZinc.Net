using System.Diagnostics;
using System.Text;

public static class ProcessUtils
{
    public static async Task<ProcessResult> Exec(
        string command,
        string arguments,
        string? workDir = null,
        int? timeoutMs = null
    )
    {
        var timeout = timeoutMs ?? (int)TimeSpan.FromMinutes(1).TotalMilliseconds;
        var result = new ProcessResult();
        using var process = new Process();
        process.StartInfo.FileName = command;
        process.StartInfo.WorkingDirectory = workDir;
        process.StartInfo.Arguments = arguments;
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardInput = true;
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;
        process.StartInfo.CreateNoWindow = true;

        var outputBuilder = new StringBuilder();
        var outputCloseEvent = new TaskCompletionSource<bool>();

        process.OutputDataReceived += (s, e) =>
        {
            // The output stream has been closed i.e. the process has terminated
            if (e.Data == null)
            {
                outputCloseEvent.SetResult(true);
            }
            else
            {
                outputBuilder.AppendLine(e.Data);
            }
        };

        var errorBuilder = new StringBuilder();
        var errorCloseEvent = new TaskCompletionSource<bool>();

        process.ErrorDataReceived += (s, e) =>
        {
            // The error stream has been closed i.e. the process has terminated
            if (e.Data == null)
            {
                errorCloseEvent.SetResult(true);
            }
            else
            {
                errorBuilder.AppendLine(e.Data);
            }
        };

        bool isStarted;

        try
        {
            isStarted = process.Start();
        }
        catch (Exception error)
        {
            // Usually it occurs when an executable file is not found or is not executable

            result.Completed = true;
            result.ExitCode = -1;
            result.Output = error.Message;

            isStarted = false;
        }

        if (isStarted)
        {
            // Reads the output stream first and then waits because deadlocks are possible
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            // Creates task to wait for process exit using timeout
            var waitForExit = WaitForExitAsync(process, timeout);

            // Create task to wait for process exit and closing all output streams
            var processTask = Task.WhenAll(
                waitForExit,
                outputCloseEvent.Task,
                errorCloseEvent.Task
            );

            // Waits process completion and then checks it was not completed by timeout
            if (
                await Task.WhenAny(Task.Delay(timeout), processTask) == processTask
                && waitForExit.Result
            )
            {
                result.Completed = true;
                result.ExitCode = process.ExitCode;

                // Adds process output if it was completed with error
                if (process.ExitCode != 0)
                {
                    result.Output = $"{outputBuilder}{errorBuilder}";
                }
            }
            else
            {
                try
                {
                    // Kill hung process
                    process.Kill();
                }
                catch { }
            }
        }

        return result;
    }

    private static Task<bool> WaitForExitAsync(Process process, int timeout)
    {
        return Task.Run(() => process.WaitForExit(timeout));
    }

    public struct ProcessResult
    {
        public bool Completed;
        public int? ExitCode;
        public string Output;
    }
}
