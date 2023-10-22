namespace MiniZinc.Tests;

public class LexerTests
{
    Token ReadToken(string mzn)
    {
        var lexer = Lexer.FromString(mzn);
        var token = lexer.ReadToken();
        return token;
    }

    // Test single tokenlexing the given string
    void TestKind(string mzn, params Kind[] kinds)
    {
        var lexer = Lexer.FromString(mzn);
        var tokens = lexer.ReadTokens().ToArray();
        for (int i = 0; i < tokens.Length; i++)
        {
            var token = tokens[i];
            var kind = kinds[i];
            token.Kind.Should().Be(kind);
        }
    }

    [Theory]
    [InlineData("a")]
    [InlineData("B")]
    [InlineData("_A_NAME")]
    [InlineData("aN4m3w1thnumb3r5")]
    public void Test_identifer(string mzn)
    {
        TestKind(mzn, Kind.Word);
    }

    [Theory]
    [InlineData(""" "abc" """)]
    [InlineData(""" "Escaped single \'quotes\' are fine" """)]
    [InlineData(""" "Escaped double \"quotes\" are fine" """)]
    public void test_string_literal(string mzn)
    {
        TestKind(mzn, Kind.String);
    }

    [Theory]
    [InlineData("1", 1)]
    [InlineData("100", 100)]
    public void test_int(string mzn, int i)
    {
        var token = ReadToken(mzn);
        token.Int.Should().Be(i);
        token.Kind.Should().Be(Kind.Int);
    }

    // [Theory]
    // [InlineData("1..10")]
    // public void test_range_ti(string mzn)
    // {
    //     TestKind(mzn, Kind.Int, Kind.DotDot, Kind.Int);
    // }

    [Fact]
    public void test_literals()
    {
        TestKind(
            "< > <= >= <>",
            Kind.LessThan,
            Kind.GreaterThan,
            Kind.LessThanEqual,
            Kind.GreaterThanEqual,
            Kind.Empty
        );
    }

    [Fact]
    public void test_whitespace()
    {
        var mzn = @$" {'\r'} {'\t'} {'\n'}  ";
        var t = ReadToken(mzn);
        var a = 2;
    }
}
