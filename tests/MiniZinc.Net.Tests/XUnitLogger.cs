﻿using System.Text;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace MiniZinc.Tests;

internal class XUnitLogger(
    ITestOutputHelper testOutputHelper,
    IExternalScopeProvider scopeProvider,
    string categoryName
) : ILogger
{
    public static ILogger Create<T>(ITestOutputHelper testOutputHelper) =>
        new XUnitLogger(testOutputHelper, new LoggerExternalScopeProvider(), nameof(T));

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
        sb.Append(GetLogLevelString(logLevel))
            .Append(" [")
            .Append(categoryName)
            .Append("] ")
            .Append(formatter(state, exception));

        if (exception is not null)
            sb.Append('\n').Append(exception);

        // Append scopes
        scopeProvider.ForEachScope(
            (scope, state) =>
            {
                state.Append("\n => ");
                state.Append(scope);
            },
            sb
        );

        testOutputHelper.WriteLine(sb.ToString());
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
