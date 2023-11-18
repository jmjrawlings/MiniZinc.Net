using Microsoft.Extensions.Logging;

namespace MiniZinc.Tests;

public static class Logging
{
    private static readonly ILoggerFactory _factory = LoggerFactory.Create(b => b.AddSimpleConsole(x => x.SingleLine = true));

    public static ILogger Create<T>() => _factory.CreateLogger<T>();
}
