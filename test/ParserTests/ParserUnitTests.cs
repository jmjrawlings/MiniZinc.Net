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
        parser.ParseIncludeItem();
        parser.Model.Includes.Should().HaveCount(1);
    }

    [Fact]
    void test_parse_output_item()
    {
        var parser = Parse("output [];");
        parser.ParseOutputItem();
        parser.Model.Outputs.Should().HaveCount(1);
    }
    
    [InlineData("a")]
    void test_parse_expr(string mzn)
    {
        var parser = Parse("constraint a > 2;");
        var cons = parser.ParseConstraintItem();
    }

    [Fact]
    void test_parse_constraint()
    {
        var parser = Parse("constraint a > 2;");
        var cons = parser.ParseConstraintItem();
    }

    [Theory]
    [InlineData("solve satisfy;")]
    [InlineData("solve maximize;")]
    void test_parse_solve(string mzn)
    {
        var p = Parse(mzn);
        p.ParseSolveItem();
        p.Model.SolveItems.Should().NotBeNull();
    }
}
