namespace MiniZinc.Net;

using System.Text;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

internal class XUnitLogger(
    ITestOutputHelper testOutputHelper,
    IExternalScopeProvider scopeProvider,
    string categoryName
) : ILogger
{
    public static ILogger Create<T>(ITestOutputHelper output) =>
        new XUnitLogger(output, new LoggerExternalScopeProvider(), nameof(T));

    public bool IsEnabled(LogLevel logLevel) => logLevel != LogLevel.None;

    public IDisposable BeginScope<TState>(TState state) => scopeProvider.Push(state);

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter
    )
    {
        var sb = new StringBuilder();
        var msg = formatter(state, exception);
        if (exception is not null)
            sb.Append('\n').Append(exception);

        testOutputHelper.WriteLine(msg);
    }

    private static string GetLogLevelString(LogLevel logLevel)
    {
        return logLevel switch
        {
            LogLevel.Trace => "trce",
            LogLevel.Debug => "dbug",
            LogLevel.Information => "info",
            LogLevel.Warning => "warn",
            LogLevel.Error => "fail",
            LogLevel.Critical => "crit",
            _ => throw new ArgumentOutOfRangeException(nameof(logLevel))
        };
    }
}
