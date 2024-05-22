﻿using MiniZinc.Parser.Syntax;

public sealed class WriterTests
{
    [Theory]
    [InlineData("var int: a = 2;")]
    [InlineData("var bool: b = [1,2,3];")]
    [InlineData("solve satisfy   ;")]
    void test_write_minified(string input)
    {
        var result = Parser.ParseText(input);
        result.Ok.Should().BeTrue("Text should parse");
        var tree = result.Syntax;
        var options = new WriteOptions { Minify = true };
        var output = tree.Write(options);
        var a = 2;
    }

    [Fact]
    void test_write()
    {
        var input = "var int: a; constraint a > 100";
        var expected = """
            var int: a;
            constraint
                a > 100;
            """;

        var result = Parser.ParseText(input);
        result.Ok.Should().BeTrue("Text should parse");
        var tree = result.Syntax;
        var output = tree.Write();
        output.Should().Be(expected);
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
            """include "b.mzn";include "a.mzn";var int: a;solve maximize a;output ["\(a)"];""";

        var result = Parser.ParseText(input);
        result.Ok.Should().BeTrue("Text should parse");
        var tree = result.Syntax;
        var opts = new WriteOptions { Prettify = true, Minify = true };
        var output = tree.Write(opts);
        output.Should().Be(expected);
    }

    [Fact]
    void test_write_precedence()
    {
        var input = "a  <-> (2 + b)";
        var expr = Parser.ParseNode<BinaryOperatorSyntax>(input)!;
        var output = expr.Write();
        output.Should().Be("a - 2 > 100 <-> b * 10 < c");
    }
}
