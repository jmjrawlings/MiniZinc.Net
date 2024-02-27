namespace MiniZinc.Net;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;

/// <summary>
/// Entrypoint to setup logging, I have no idea
/// </summary>

public static class Logging
{
    private static ILoggerFactory? _factory;

    public static ILoggerFactory Factory => _factory ??= new NullLoggerFactory();

    public static ILoggerFactory Setup(Action<ILoggingBuilder> configure)
    {
        var factory = LoggerFactory.Create(configure);
        return Setup(factory);
    }

    public static ILoggerFactory Setup(ILoggerFactory factory)
    {
        if (_factory is not null)
            throw new Exception("Logging has already been setup");
        _factory = factory;
        return _factory;
    }
}
