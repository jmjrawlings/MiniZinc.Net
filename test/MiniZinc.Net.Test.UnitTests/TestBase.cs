using Microsoft.Extensions.Logging.Console;

public abstract class TestBase
{
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
}
