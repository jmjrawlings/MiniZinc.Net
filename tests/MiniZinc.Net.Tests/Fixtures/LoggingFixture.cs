namespace MiniZinc.Tests;

public sealed class LoggingFixture
{
    public readonly ILoggerFactory Factory;

    public LoggingFixture()
    {
        Factory = LoggerFactory.Create(b => b.AddSimpleConsole(x => x.SingleLine = true));
    }
}
