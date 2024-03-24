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

    private string? Sanitize(string? s)
    {
        return s?.Replace("\\\"", "\\\"");
    }

    // public List<Token> LexFile(string path)
    // {
    //     var mznFile = path.ToFile();
    //     _logger.LogInformation("{File}", mznFile);
    //     var mzn = File.ReadAllText(mznFile.FullName);
    //     _logger.LogInformation(mzn);
    //     var csvFile = Path.ChangeExtension(mznFile.FullName, "csv").ToFile();
    //     using var csvWriter = csvFile.OpenWrite();
    //     using var csvStream = new StreamWriter(csvWriter);
    //     csvStream.WriteLine(
    //         string.Join(
    //             ",",
    //             "id",
    //             "start",
    //             "length",
    //             "line",
    //             "column",
    //             "kind",
    //             "string",
    //             "int",
    //             "double"
    //         )
    //     );
    //     var tokens = new List<Token>();
    //     var id = 0;
    //     foreach (var token in Lexer.LexFile(mznFile))
    //     {
    //         var msg = string.Join(
    //             ',',
    //             id++,
    //             token.Start,
    //             token.Length,
    //             token.Line,
    //             token.Col,
    //             token.Kind,
    //             Sanitize(token.String),
    //             token.Int,
    //             token.Double
    //         );
    //         csvStream.WriteLine(msg);
    //         tokens.Add(token);
    //     }
    //
    //     _logger.LogInformation("Reading from {File}", mznFile);
    //     _logger.LogInformation("Writing to {File}", csvFile);
    //     return tokens;
    // }

    [Fact]
    public void xd()
    {
        var s =
            $"""output ["full var: \(x)\nvar array: \(y)\nnested: \(z)\nelement: \(z.2.1)\npartial: \(init)\ndata: \(dat)\nenumtup: \(enumtup)\n"];""";
        var x = Lexer.LexString(s);
        var a = x.ToArray();
        var z = 2;
    }

    [Theory]
    [InlineData("")]
    void test_string(string mzn)
    {
        var tokens = Lexer.LexString(mzn).ToArray();
        tokens.Should().BeEmpty();
    }

    public LexerUnitTests(ITestOutputHelper output)
        : base(output) { }
}
