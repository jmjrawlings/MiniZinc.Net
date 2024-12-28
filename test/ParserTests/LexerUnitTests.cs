using static MiniZinc.Parser.TokenKind;

public class LexerUnitTests : TestBase
{
    void TestTokens(string mzn, params TokenKind[] kinds)
    {
        var tokens = Lexer.Lex(mzn).ToArray();
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
        TestTokens(mzn, TOKEN_IDENTIFIER);
    }

    [Fact]
    public void Test_keywords()
    {
        TestTokens(
            "if else then constraint maximize",
            KEYWORD_IF,
            KEYWORD_ELSE,
            KEYWORD_THEN,
            KEYWORD_CONSTRAINT,
            KEYWORD_MAXIMIZE
        );
    }

    [Theory]
    [InlineData(""" "abc" """)]
    [InlineData(""" "Escaped \'quotes\' " """)]
    [InlineData(""" "Escaped \"quotes\" " """)]
    void test_string_literal(string mzn)
    {
        TestTokens(mzn, TOKEN_STRING_LITERAL);
    }

    [Theory]
    [InlineData("1", 1)]
    [InlineData("100", 100)]
    [InlineData("0x124bce", 1199054)]
    [InlineData("0o3123", 1619)]
    void test_int(string mzn, int i)
    {
        var token = Lexer.Lex(mzn).First();
        token.Kind.Should().Be(TOKEN_INT_LITERAL);
        token.IntValue.Should().Be(i);
    }

    [Theory]
    [InlineData("1.123.1", 1.123)]
    [InlineData("100.0043", 100.0043)]
    void test_float(string mzn, decimal d)
    {
        var token = Lexer.Lex(mzn).First();
        token.Kind.Should().Be(TOKEN_FLOAT_LITERAL);
        token.FloatValue.Should().Be(d);
    }

    [Theory]
    [InlineData("1..10")]
    void test_range_ti(string mzn)
    {
        TestTokens(mzn, TOKEN_INT_LITERAL, TOKEN_CLOSED_RANGE, TOKEN_INT_LITERAL);
    }

    [Theory]
    [InlineData("1.1..1.2.0")]
    void test_dot_access(string mzn)
    {
        TestTokens(
            mzn,
            TOKEN_FLOAT_LITERAL,
            TOKEN_CLOSED_RANGE,
            TOKEN_FLOAT_LITERAL,
            TOKEN_TUPLE_ACCESS
        );
    }

    [Theory]
    [InlineData("<", TOKEN_LESS_THAN)]
    [InlineData("<=", TOKEN_LESS_THAN_EQUAL)]
    [InlineData("==", TOKEN_EQUAL)]
    [InlineData("=", TOKEN_EQUAL)]
    [InlineData(">=", TOKEN_GREATER_THAN_EQUAL)]
    [InlineData(">", TOKEN_GREATER_THAN)]
    [InlineData("<>", TOKEN_EMPTY)]
    [InlineData("_", TOKEN_UNDERSCORE)]
    void test_literals(string mzn, TokenKind tokenKind)
    {
        var token = Lexer.Lex(mzn).First();
        token.Kind.Should().Be(tokenKind);
    }

    [Fact]
    void test_whitespace()
    {
        var mzn = @$" {'\r'}{'\t'}{'\n'} ";
        var tokens = Lexer.Lex(mzn).ToArray();
        tokens.Should().BeEmpty();
    }

    [Fact]
    public void xd()
    {
        var s =
            $"""output ["full var: \(x)\nvar array: \(y)\nnested: \(z)\nelement: \(z.2.1)\npartial: \(init)\ndata: \(dat)\nenumtup: \(enumtup)\n"];""";
        var x = Lexer.Lex(s);
        var a = x.ToArray();
    }

    [Fact]
    void test_string_interp()
    {
        var mzn = "\\([\"lala\" | i in 1..3 where b])";
        mzn = $"\"{mzn}\"";
        var tokens = Lexer.Lex(mzn).ToArray();
        tokens.Should().HaveCount(1);
        tokens[0].Kind.Should().Be(TOKEN_STRING_LITERAL);
        tokens[0].StringValue.Should().Be("\\([\"lala\" | i in 1..3 where b])");
    }

    public LexerUnitTests(ITestOutputHelper output)
        : base(output) { }
}
