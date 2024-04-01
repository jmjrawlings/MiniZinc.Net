public class LexerUnitTests : TestBase
{
    void TestTokens(string mzn, params TokenKind[] kinds)
    {
        var tokens = Lexer.LexString(mzn).ToArray();
        for (int i = 0; i < tokens.Length - 1; i++)
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
        TestTokens(mzn, TokenKind.IDENT);
    }

    [Fact]
    public void Test_keywords()
    {
        TestTokens(
            "if else then constraint maximize",
            TokenKind.IF,
            TokenKind.ELSE,
            TokenKind.THEN,
            TokenKind.CONSTRAINT,
            TokenKind.MAXIMIZE
        );
    }

    [Theory]
    [InlineData(""" "abc" """)]
    [InlineData(""" "Escaped \'quotes\' " """)]
    [InlineData(""" "Escaped \"quotes\" " """)]
    void test_string_literal(string mzn)
    {
        TestTokens(mzn, TokenKind.STRING_LIT);
    }

    [Theory]
    [InlineData("1", 1)]
    [InlineData("100", 100)]
    void test_int(string mzn, int i)
    {
        var token = Lexer.LexString(mzn).First();
        token.Int.Should().Be(i);
        token.Kind.Should().Be(TokenKind.INT_LIT);
    }

    [Theory]
    [InlineData("1.123.", 1.123)]
    [InlineData("100.0043", 100.0043)]
    void test_float(string mzn, double d)
    {
        var token = Lexer.LexString(mzn).First();
        token.Double.Should().Be(d);
        token.Kind.Should().Be(TokenKind.FLOAT_LIT);
    }

    [Theory]
    [InlineData("1..10")]
    void test_range_ti(string mzn)
    {
        TestTokens(mzn, TokenKind.INT_LIT, TokenKind.DOT_DOT, TokenKind.INT_LIT);
    }

    [Theory]
    [InlineData("1.1..1.2.0")]
    void test_bad_range(string mzn)
    {
        TestTokens(
            mzn,
            TokenKind.FLOAT_LIT,
            TokenKind.DOT_DOT,
            TokenKind.FLOAT_LIT,
            TokenKind.DOT,
            TokenKind.INT_LIT
        );
    }

    [Theory]
    [InlineData("<", TokenKind.LESS_THAN)]
    [InlineData("<=", TokenKind.LESS_THAN_EQUAL)]
    [InlineData("==", TokenKind.EQUAL)]
    [InlineData("=", TokenKind.EQUAL)]
    [InlineData(">=", TokenKind.GREATER_THAN_EQUAL)]
    [InlineData(">", TokenKind.GREATER_THAN)]
    [InlineData("<>", TokenKind.EMPTY)]
    [InlineData("_", TokenKind.UNDERSCORE)]
    void test_literals(string mzn, TokenKind tokenKind)
    {
        var token = Lexer.LexString(mzn).First();
        token.Kind.Should().Be(tokenKind);
    }

    [Fact]
    void test_whitespace()
    {
        var mzn = @$" {'\r'}{'\t'}{'\n'} ";
        var tokens = Lexer.LexString(mzn).ToArray();
        tokens.Should().BeEmpty();
    }

    [Fact]
    public void xd()
    {
        var s =
            $"""output ["full var: \(x)\nvar array: \(y)\nnested: \(z)\nelement: \(z.2.1)\npartial: \(init)\ndata: \(dat)\nenumtup: \(enumtup)\n"];""";
        var x = Lexer.LexString(s);
        var a = x.ToArray();
        var z = 2;
    }

    [Fact]
    void test_string_interp()
    {
        var mzn = "\\([\"lala\" | i in 1..3 where b])";
        mzn = $"\"{mzn}\"";
        var tokens = Lexer.LexString(mzn).ToArray();
        tokens.Should().HaveCount(1);
        tokens[0].Kind.Should().Be(TokenKind.STRING_LIT);
        tokens[0].String.Should().Be("\\([\"lala\" | i in 1..3 where b])");
    }

    public LexerUnitTests(ITestOutputHelper output)
        : base(output) { }
}
