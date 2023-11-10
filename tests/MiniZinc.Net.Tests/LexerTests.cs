﻿namespace MiniZinc.Tests;

using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

public class LexerTests
{
    private readonly ILogger _logger;

    public LexerTests(ITestOutputHelper output)
    {
        _logger = XUnitLogger.CreateLogger<Lexer>(output);
    }

    private IEnumerable<Token> LexString(string s) => Lexer.LexString(s, _logger);

    private IEnumerable<Token> LexFile(string s) => Lexer.LexFile(s, _logger);

    private IEnumerable<Token> LexFile(FileInfo fi) => Lexer.LexFile(fi, _logger);

    // Test single tokenlexing the given string
    void TestKind(string mzn, params TokenKind[] kinds)
    {
        var tokens = LexString(mzn).ToArray();
        for (int i = 0; i < tokens.Length - 1; i++)
        {
            var token = tokens[i];
            var kind = kinds[i];
            token.Kind.Should().Be(kind);
        }
    }

    void TestFile(FileInfo file)
    {
        var tokens = LexFile(file).ToArray();
        var last = tokens[^1];
        if (last.Kind == TokenKind.EOF)
            return;

        var mzn = File.ReadAllText(file.FullName);
        var txt = mzn.AsSpan((int)last.Start).ToString();
        last.Kind.Should().Be(TokenKind.EOF);
    }

    [Theory]
    [InlineData("a")]
    [InlineData("B")]
    [InlineData("_A_NAME")]
    [InlineData("aN4m3w1thnumb3r5")]
    public void Test_identifer(string mzn)
    {
        TestKind(mzn, TokenKind.Identifier);
    }

    [Fact]
    public void Test_keywords()
    {
        TestKind(
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
        TestKind(mzn, TokenKind.StringLiteral);
    }

    [Theory]
    [InlineData("1", 1)]
    [InlineData("100", 100)]
    public void test_int(string mzn, int i)
    {
        var token = LexString(mzn).First();
        token.Int.Should().Be(i);
        token.Kind.Should().Be(TokenKind.IntLiteral);
    }

    [Theory]
    [InlineData("1.123.", 1.123)]
    [InlineData("100.0043", 100.0043)]
    public void test_float(string mzn, double d)
    {
        var token = LexString(mzn).First();
        token.Double.Should().Be(d);
        token.Kind.Should().Be(TokenKind.FloatLiteral);
    }

    [Theory]
    [InlineData("1..10")]
    public void test_range_ti(string mzn)
    {
        TestKind(mzn, TokenKind.IntLiteral, TokenKind.DotDot, TokenKind.IntLiteral);
    }

    [Theory]
    [InlineData("1.1..1.2.0")]
    public void test_bad_range(string mzn)
    {
        TestKind(
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
        var token = LexString(mzn).First();
        token.Kind.Should().Be(tokenKind);
    }

    [Fact]
    public void test_whitespace()
    {
        var mzn = @$" {'\r'}{'\t'}{'\n'} ";
        var tokens = LexString(mzn).ToArray();
        tokens.Should().HaveCount(1);
        tokens[0].Kind.Should().Be(TokenKind.EOF);
    }

    [Theory]
    [ClassData(typeof(TestFiles))]
    public void test_file(FileInfo file)
    {
        TestFile(file);
    }

    [Fact]
    public void test_path()
    {
        TestFile(
            new FileInfo(
                "C:\\Users\\hrkn\\projects\\MiniZinc.Net\\libminizinc\\examples\\singHoist2.mzn"
            )
        );
    }

    [Fact]
    public void test_xd()
    {
        var str = """
          output 
          [ "Number of used bins = ",  show( obj ), "\n"] ++ 
          [ "Items in bins = \n\t"] ++ 
          [ show(item[k, j]) ++ if j = N then "\n\t" else " " endif |
              k in 1..K, j in 1..N ] ++
          [ "\n" ];
          
          %------------------------------------------------------------------------------%
          """;

        var tokens = LexString(str).ToArray();
        var last = tokens[^1];
        if (last.Kind == TokenKind.EOF)
            return;
        Assert.Fail("EOF expected");
    }
}
