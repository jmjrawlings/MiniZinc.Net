﻿using MiniZinc.Parser.Ast;

public class ParserUnitTests
{
    Parser Parse(string mzn)
    {
        var lexer = Lexer.LexString(mzn);
        var parser = new Parser(lexer);
        parser.Step();
        return parser;
    }

    [Fact]
    void test_namespace()
    {
        var ns = new NameSpace<int>();
        ns.Push("a", 1);
        ns.Push("a", 2);
        ns.Push("a", 3);
        ns["a"].Should().Be(3);
        ns.Pop();
        ns["a"].Should().Be(2);
        ns.Pop().Value.Should().Be(2);
        ns["a"].Should().Be(1);
        ns.Pop();
        ns.ContainsKey("a").Should().BeFalse();
    }

    [Fact]
    void test_parse_include_item()
    {
        var parser = Parse("include \"xd.mzn\";");
        var model = new Model();
        parser.ParseIncludeItem(model);
        model.Includes.Should().HaveCount(1);
    }

    [Fact]
    void test_parse_output_item()
    {
        var parser = Parse("output [];");
        var model = new Model();
        parser.ParseOutputItem(model);
        model.Outputs.Should().HaveCount(1);
    }

    [Fact]
    void test_parse_constraint()
    {
        var parser = Parse("constraint a > 2;");
        var ok = parser.ParseConstraintItem(out var con);
        ok.Should().BeTrue();
    }

    [Theory]
    [InlineData("solve satisfy;")]
    [InlineData("solve maximize;")]
    void test_parse_solve(string mzn)
    {
        var p = Parse(mzn);
        var model = new Model();
        p.ParseSolveItem(model);
        model.SolveItems.Should().NotBeNull();
    }
}
