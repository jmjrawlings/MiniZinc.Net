using Microsoft.Extensions.Logging.Console;

public abstract class TestBase
{
    private readonly ITestOutputHelper _output;

    static TestBase()
    {
        Logging.Setup(config =>
        {
            config
                .AddSimpleConsole(options =>
                {
                    options.ColorBehavior = LoggerColorBehavior.Enabled;
                    options.IncludeScopes = true;
                    options.SingleLine = true;
                    options.TimestampFormat = "HH:mm:ss ";
                })
                .SetMinimumLevel(LogLevel.Debug);
        });
    }

    protected TestBase(ITestOutputHelper output)
    {
        _output = output;
    }

    protected void Write(string message)
    {
        _output.WriteLine(message);
    }

    protected void Write(string template, params object[] args)
    {
        _output.WriteLine(template, args);
    }
}
