using MiniZinc.Parser.Syntax;

public sealed class WriterTests
{
    [Theory]
    [InlineData("var int: a = 2;")]
    [InlineData("var bool: b = [1,2,3];")]
    [InlineData("solve satisfy   ;")]
    void test_writer_minified(string input)
    {
        var result = Parser.ParseText(input);
        result.Ok.Should().BeTrue("Text should parse");
        var tree = result.Syntax;
        var options = new WriteOptions { Minify = true };
        var output = tree.Write(options);
        var a = 2;
    }
}
