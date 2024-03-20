using MiniZinc.Parser.Ast;

public static class ParserExtensions
{
    public static void Check(this Parser p)
    {
        if (p._error is { } err)
            Assert.Fail(err);
    }
}

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

    [Theory]
    [InlineData("enum Letters = {A, B, C};")]
    [InlineData("enum Letters = {A, B, C} ++ {D, E, F};")]
    [InlineData("enum Anon = _(1..10) ++ anon_enum(10);")]
    [InlineData("enum Complex = C(1..10);")]
    void test_parse_enum_item(string mzn)
    {
        var parser = Parse(mzn);
        var model = new Model();
        parser.ParseEnumItem(out var @enum).Should().BeTrue();
    }

    [Fact]
    void test_parse_constraint()
    {
        var parser = Parse("constraint a > 2;");
        parser.ParseConstraintItem(out var con);
        parser.Check();
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
        p.Check();
    }

    [Theory]
    [InlineData("forall(i in 1..3)(xd[i] > 0)")]
    [InlineData("forall(i,j in 1..3)(xd[i] > 0)")]
    [InlineData("forall(i in 1..3, j in 1..3 where i > j)(xd[i] > 0)")]
    void test_parse_gencall(string mzn)
    {
        var p = Parse(mzn);
        p.ParseIdentExpr(out var expr);
        if (expr is not GenCallExpr gencall)
            Assert.Fail(p._error);
    }
}
