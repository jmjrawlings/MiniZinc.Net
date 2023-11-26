namespace MiniZinc.Tests;

public abstract class TestBase
{
    protected readonly ILogger _logger;
    protected readonly ILoggerFactory _factory;

    protected TestBase(LoggingFixture logging, ITestOutputHelper output)
    {
        _factory = logging.Factory;
        _logger = XUnitLogger.Create<LexerTests>(output);
    }
}
