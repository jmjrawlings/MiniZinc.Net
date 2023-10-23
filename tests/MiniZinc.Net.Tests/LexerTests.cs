namespace MiniZinc.Tests;

public class LexerTests
{
    Token ReadToken(string mzn)
    {
        var lexer = Lexer.FromString(mzn);
        var token = lexer.LexTokens().First();
        return token;
    }

    // Test single tokenlexing the given string
    void TestKind(string mzn, params Kind[] kinds)
    {
        var lexer = Lexer.FromString(mzn);
        var tokens = lexer.LexTokens().ToArray();
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
        TestKind(mzn, Kind.StringLiteral);
    }

    [Theory]
    [InlineData("1", 1)]
    [InlineData("100", 100)]
    public void test_int(string mzn, int i)
    {
        var token = ReadToken(mzn);
        token.Int.Should().Be(i);
        token.Kind.Should().Be(Kind.IntLiteral);
    }

    [Theory]
    [InlineData("1.123.", 1.123)]
    [InlineData("100.0043", 100.0043)]
    public void test_float(string mzn, double d)
    {
        var token = ReadToken(mzn);
        token.Double.Should().Be(d);
        token.Kind.Should().Be(Kind.FloatLiteral);
    }

    [Theory]
    [InlineData("1..10")]
    public void test_range_ti(string mzn)
    {
        TestKind(mzn, Kind.IntLiteral, Kind.DotDot, Kind.IntLiteral);
    }
    
    [Theory]
    [InlineData("<", Kind.LessThan)]
    [InlineData("<=", Kind.LessThanEqual)]
    [InlineData("==", Kind.Equal)]
    [InlineData("=", Kind.Equal)]
    [InlineData(">=", Kind.GreaterThanEqual)]
    [InlineData(">", Kind.GreaterThan)]
    [InlineData("<>", Kind.Empty)]
    public void test_literals(string mzn, Kind kind)
    {
        var lexer = Lexer.FromString(mzn);
        var token = lexer.LexTokens().ToArray().First();
        token.Kind.Should().BeOneOf(kind);
    }

    [Fact]
    public void test_whitespace()
    {
        var mzn = @$" {'\r'} {'\t'} {'\n'}  ";
        var t = ReadToken(mzn);
        var a = 2;
    }
}
