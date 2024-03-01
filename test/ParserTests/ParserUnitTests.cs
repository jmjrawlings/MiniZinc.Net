public class ParserUnitsTests
{
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
    void test_parse_include()
    {
        using var lexer = Lexer.LexString("include \"xd.mzn\";");
        var parser = new Parser(lexer);
        parser.Model.Includes.Should().HaveCount(1);
    }
}
