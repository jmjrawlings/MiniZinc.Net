using MiniZinc.Parser;

public sealed class WriterUnitTests
{
    [Theory]
    [InlineData("var int: a = 2;")]
    [InlineData("var bool: b = [1,2,3];")]
    [InlineData("solve satisfy   ;")]
    void test_write_minified(string input)
    {
        var ok = Parser.ParseModelFromString(input, out var model);
        ok.Should().BeTrue("Text should parse");
        var options = new WriteOptions { Minify = true };
        var output = model.Write(options);
        var a = 2;
    }

    [Fact]
    void test_write_pretty()
    {
        var input = """
            var int: a;
            solve maximize a;
            include "b.mzn";
            output ["\(a)"];
            include "a.mzn";
            """;

        var expected =
            """include "b.mzn";include "a.mzn";var int:a;solve maximize a;output["\(a)"];""";

        var ok = Parser.ParseModelFromString(input, out var model);
        ok.Should().BeTrue("Text should parse");
        var opts = new WriteOptions { Prettify = true, Minify = true };
        var output = model.Write(opts);
        output.Should().Be(expected);
    }

    [Theory]
    [InlineData("""a <-> (b \/ c)""", """a <-> b \/ c""")]
    [InlineData("""(a <-> b) \/ c""", """(a <-> b) \/ c""")]
    [InlineData("""a <-> b \/ c""", """a <-> b \/ c""")]
    [InlineData("""2 * i""", """2 * i""")]
    void test_write_precedence(string input, string expected)
    {
        var expr = Parser.ParseExpression<BinOpExpr>(input)!;
        var output = expr.Write();
        output.Should().Be(expected);
    }

    [Theory]
    [InlineData("""{A} ++ {B} ++ {C}""", """{A} ++ {B} ++ {C}""")]
    void test_write_precedence_right_assoc(string input, string expected)
    {
        var expr = Parser.ParseExpression<BinOpExpr>(input)!;
        var output = expr.Write();
        output.Should().Be(expected);
    }
}
