namespace MiniZinc.Tests;

using Xunit.Abstractions;

public abstract class TestBase
{
    private readonly ITestOutputHelper _output;

    protected TestBase(ITestOutputHelper output)
    {
        _output = output;
    }

    protected void Write(string message)
    {
        _output.WriteLine(message);
    }

    protected void Write(string template, params object?[] args)
    {
        _output.WriteLine(template, args);
    }
}
