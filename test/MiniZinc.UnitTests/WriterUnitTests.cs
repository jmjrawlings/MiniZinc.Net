using MiniZinc.Parser;
using Shouldly;
using TUnit;

public sealed class WriterUnitTests
{
    [Test]
    [Arguments("var int: a = 2;")]
    [Arguments("var bool: b = [1,2,3];")]
    [Arguments("solve satisfy   ;")]
    public void test_write_minified(string input)
    {
        var ok = Parser.ParseModelFromString(input, out var model);
        ok.ShouldBeTrue("Text s  hould parse");
        var options = new WriteOptions { Minify = true };
        var output = model.Write(options);
        var a = 2;
    }

    [Test]
    public void test_write_pretty()
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
        ok.ShouldBeTrue("Text should parse");
        var opts = new WriteOptions { Prettify = true, Minify = true };
        var output = model.Write(opts);
        output.ShouldBe(expected);
    }

    [Test]
    [Arguments("""a <-> (b \/ c)""", """a <-> b \/ c""")]
    [Arguments("""(a <-> b) \/ c""", """(a <-> b) \/ c""")]
    [Arguments("""a <-> b \/ c""", """a <-> b \/ c""")]
    [Arguments("""2 * i""", """2 * i""")]
    public void test_write_precedence(string input, string expected)
    {
        var expr = Parser.ParseExpression<BinOpExpr>(input)!;
        var output = expr.Write();
        output.ShouldBe(expected);
    }

    [Test]
    [Arguments("""{A} ++ {B} ++ {C}""", """{A} ++ {B} ++ {C}""")]
    public void test_write_precedence_right_assoc(string input, string expected)
    {
        var expr = Parser.ParseExpression<BinOpExpr>(input)!;
        var output = expr.Write();
        output.ShouldBe(expected);
    }
}
