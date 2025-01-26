using MiniZinc.Command;
using TUnit.Assertions;
using TUnit.Assertions.Extensions;
using TUnit.Core;

public class CommandTests
{
    [Test]
    [Arguments("-v")]
    [Arguments("--okay")]
    [Arguments("-a1")]
    public async Task Parse_Flag_Only_Arg(string s)
    {
        var arg = Arg.Parse(s).First();
        await Assert.That(arg.Value).IsNull();
        await Assert.That(arg.Value).IsNull();
        await Assert.That(arg.ArgType).IsEqualTo(ArgType.FlagOnly);
        await Assert.That(arg.String).IsEqualTo(s);
    }

    [Test]
    [Arguments("-v 1")]
    [Arguments("--okay \"two\"")]
    [Arguments("-one=2")]
    public async Task Parse_Flag_And_Value_Arg(string s)
    {
        var arg = Arg.Parse(s).First();
        await Assert.That(arg.Flag).IsNotNull();
        await Assert.That(arg.Value).IsNotNull();
        await Assert.That(arg.String).IsEqualTo(s);
    }

    [Test]
    public async Task Parse_Url_Flag()
    {
        var url = @"https://github.com/MiniZinc/libminizinc.git";
        var arg = Arg.Parse(url).First();
        await Assert.That(arg.Value).IsEqualTo(url);
    }

    [Test]
    [Arguments("x")]
    [Arguments("123")]
    [Arguments("\"asdfasdf asdf\"")]
    public async Task Parse_Value_Only_Arg(string s)
    {
        var arg = Arg.Parse(s).First();
        await Assert.That(arg.Flag).IsNull();
        await Assert.That(arg.ArgType).IsEqualTo(ArgType.ValueOnly);
        await Assert.That(arg.Value).IsEqualTo(s);
    }

    [Test]
    [Arguments("--output-json")]
    public async Task Parse_Value_With_Dashes(string s)
    {
        var args = Arg.Parse(s).ToList();
        await Assert.That(args.Count).IsEqualTo(1);
        await Assert.That(args[0].ArgType).IsEqualTo(ArgType.FlagOnly);
        await Assert.That(args[0].Flag).IsEqualTo(s);
    }
}
