namespace MiniZinc.Process;

using System.Runtime.CompilerServices;

/// <summary>
///
/// </summary>
public static class ProcessExtensions
{
    /// <summary>
    /// Run this command as a Process
    /// </summary>
    public static async Task<ProcessResult> Run(
        this Command cmd,
        CancellationToken token = default,
        bool stdout = true,
        bool stderr = true
    )
    {
        using var process = new Process(cmd);
        var result = await process.Run(token, stdout, stderr);
        return result;
    }

    /// <summary>
    /// Run this command and listen to the outputs
    /// </summary>
    public static async IAsyncEnumerable<ProcessEvent> Listen(
        this Command cmd,
        [EnumeratorCancellation] CancellationToken token = default
    )
    {
        using var process = new Process(cmd);
        await foreach (var msg in process.Listen(token))
        {
            yield return msg;
        }
    }
}
