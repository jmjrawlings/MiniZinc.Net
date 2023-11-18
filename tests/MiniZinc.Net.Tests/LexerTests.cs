namespace MiniZinc.Tests;

using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

public class LexerTests
{
    private readonly ILogger _logger;
    
    public LexerTests(ITestOutputHelper output)
    {
        _logger = XUnitLogger.Create<LexerTests>(output);
    }

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
        TestTokens(mzn, TokenKind.Identifier);
    }

    [Fact]
    public void Test_keywords()
    {
        TestTokens(
            "if else then constraint maximize",
            TokenKind.KeywordIf,
            TokenKind.KeywordElse,
            TokenKind.KeywordThen,
            TokenKind.KeywordConstraint,
            TokenKind.KeywordMaximize
        );
    }

    [Theory]
    [InlineData(""" "abc" """)]
    [InlineData(""" "Escaped single \'quotes\' are fine" """)]
    [InlineData(""" "Escaped double \"quotes\" are fine" """)]
    public void test_string_literal(string mzn)
    {
        TestTokens(mzn, TokenKind.StringLiteral);
    }

    [Theory]
    [InlineData("1", 1)]
    [InlineData("100", 100)]
    public void test_int(string mzn, int i)
    {
        var token = Lexer.LexString(mzn).First();
        token.Int.Should().Be(i);
        token.Kind.Should().Be(TokenKind.IntLiteral);
    }

    [Theory]
    [InlineData("1.123.", 1.123)]
    [InlineData("100.0043", 100.0043)]
    public void test_float(string mzn, double d)
    {
        var token = Lexer.LexString(mzn).First();
        token.Double.Should().Be(d);
        token.Kind.Should().Be(TokenKind.FloatLiteral);
    }

    [Theory]
    [InlineData("1..10")]
    public void test_range_ti(string mzn)
    {
        TestTokens(mzn, TokenKind.IntLiteral, TokenKind.DotDot, TokenKind.IntLiteral);
    }

    [Theory]
    [InlineData("1.1..1.2.0")]
    public void test_bad_range(string mzn)
    {
        TestTokens(
            mzn,
            TokenKind.FloatLiteral,
            TokenKind.DotDot,
            TokenKind.FloatLiteral,
            TokenKind.Dot,
            TokenKind.IntLiteral
        );
    }

    [Theory]
    [InlineData("<", TokenKind.LessThan)]
    [InlineData("<=", TokenKind.LessThanEqual)]
    [InlineData("==", TokenKind.Equal)]
    [InlineData("=", TokenKind.Equal)]
    [InlineData(">=", TokenKind.GreaterThanEqual)]
    [InlineData(">", TokenKind.GreaterThan)]
    [InlineData("<>", TokenKind.Empty)]
    [InlineData("_", TokenKind.Underscore)]
    public void test_literals(string mzn, TokenKind tokenKind)
    {
        var token = Lexer.LexString(mzn).First();
        token.Kind.Should().Be(tokenKind);
    }

    [Fact]
    public void test_whitespace()
    {
        var mzn = @$" {'\r'}{'\t'}{'\n'} ";
        var tokens = Lexer.LexString(mzn).ToArray();
        tokens.Should().HaveCount(1);
        tokens[0].Kind.Should().Be(TokenKind.EOF);
    }

    [Theory]
    [ClassData(typeof(TestFiles))]
    public void test_file(FileInfo file)
    {
        _logger.LogInformation("Test {File}", file);
        var mzn = File.ReadAllText(file.FullName);
        _logger.LogInformation(mzn);
        var tokens = Lexer.LexFile(file, _logger).ToArray();
        var last = tokens.Last();
        last.Kind.Should().Be(TokenKind.EOF);
    }
    
    private string? Sanitize(string? s)
    {
        return s?.Replace("\\\"", "\\\"");
    }
    
    public List<Token> LexFile(string path)
    {
        
        var mznFile = path.ToFile();
        _logger.LogInformation("{File}", mznFile);
        var mzn = File.ReadAllText(mznFile.FullName);
        _logger.LogInformation(mzn);
        var csvFile = Path.ChangeExtension(mznFile.FullName, "csv").ToFile();
        using var csvWriter = csvFile.OpenWrite();
        using var csvStream = new StreamWriter(csvWriter);
        csvStream.WriteLine(string.Join(",","id","start", "length", "line", "column", "kind", "string", "int", "double"));
        var tokens = new List<Token>();
        var id = 0;
        foreach (var token in Lexer.LexFile(mznFile))
        {
            var msg = string.Join(',', id++, token.Start, token.Length, token.Line, token.Col, token.Kind, Sanitize(token.String),
                token.Int, token.Double);
            csvStream.WriteLine(msg);
            tokens.Add(token);
        }

        _logger.LogInformation("Reading from {File}", mznFile);
        _logger.LogInformation("Writing to {File}", csvFile);
        return tokens;
    }
    

    [Fact]
    public void test1()
    {
        var p1 = @"C:\Users\hrkn\projects\MiniZinc.Net\libminizinc\unit\general\pow_bounds.mzn";
        var t1 = LexFile(p1);
        var l1 = t1.Last();
    
        // var p2 = @"C:\Users\hrkn\projects\MiniZinc.Net\libminizinc\examples\singHoist2.mzn";
        // var t2 = LexFile(p2);
        // var l2 = t2.Last();
        
        l1.Kind.Should().Be(TokenKind.EOF);
        // l2.Kind.Should().Be(TokenKind.EOF);
        
        
    }

    [Theory]
    [InlineData("")]
    public void test_string(string mzn)
    {
        var tokens = Lexer.LexFile(mzn, _logger).ToArray();
        var last = tokens.Last();
        last.Kind.Should().Be(TokenKind.EOF);
    }
}
