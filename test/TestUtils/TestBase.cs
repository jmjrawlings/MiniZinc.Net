namespace MiniZinc.Tests;

using Xunit.Abstractions;

public abstract class TestBase
{
    protected readonly ITestOutputHelper _output;

    protected TestBase(ITestOutputHelper output)
    {
        _output = output;
    }

    protected void WriteLn(string? message = null)
    {
        if (message is null)
            _output.WriteLine("");
        else
            _output.WriteLine(message);
    }

    protected void WriteLn(string template, params object?[] args)
    {
        _output.WriteLine(template, args);
    }

    protected void WriteSection()
    {
        _output.WriteLine(new string('-', 80));
    }
}
