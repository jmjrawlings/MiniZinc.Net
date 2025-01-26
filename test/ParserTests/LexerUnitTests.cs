using MiniZinc.Parser;
using Shouldly;
using TUnit.Assertions;
using TUnit.Assertions.Extensions;
using TUnit.Core;
using static MiniZinc.Parser.TokenKind;

public sealed class LexerUnitTests
{
    void TestTokens(string mzn, params TokenKind[] kinds)
    {
        Lexer.Lex(mzn, out var tokens);
        for (int i = 0; i < tokens.Length - 1; i++)
        {
            var token = tokens[i];
            var kind = kinds[i];
            token.Kind.ShouldBe(kind);
        }
    }

    [Test]
    [Arguments("a")]
    [Arguments("B")]
    [Arguments("_A_NAME")]
    [Arguments("aN4m3w1thnumb3r5")]
    public void Test_identifer(string mzn)
    {
        TestTokens(mzn, TOKEN_IDENTIFIER);
    }

    [Test]
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

    [Test]
    [Arguments(""" "abc" """)]
    [Arguments(""" "Escaped \'quotes\' " """)]
    [Arguments(""" "Escaped \"quotes\" " """)]
    public void test_string_literal(string mzn)
    {
        TestTokens(mzn, TOKEN_STRING_LITERAL);
    }

    [Test]
    [Arguments("1", 1)]
    [Arguments("100", 100)]
    [Arguments("0x124bce", 1199054)]
    [Arguments("0o3123", 1619)]
    public void test_int(string mzn, int i)
    {
        Lexer.Lex(mzn, out var tokens);
        var token = tokens[0];
        token.Kind.ShouldBe(TOKEN_INT_LITERAL);
        token.IntValue.ShouldBe(i);
    }

    [Test]
    [Arguments("1.123.1", 1.123)]
    [Arguments("100.0043", 100.0043)]
    public void test_float(string mzn, decimal d)
    {
        Lexer.Lex(mzn, out var tokens);
        var token = tokens[0];
        token.Kind.ShouldBe(TOKEN_FLOAT_LITERAL);
        token.FloatValue.ShouldBe(d);
    }

    [Test]
    [Arguments("1..10")]
    public void test_range_ti(string mzn)
    {
        TestTokens(mzn, TOKEN_INT_LITERAL, TOKEN_RANGE_INCLUSIVE, TOKEN_INT_LITERAL);
    }

    [Test]
    [Arguments("1.1..1.2.0")]
    public void test_dot_access(string mzn)
    {
        TestTokens(
            mzn,
            TOKEN_FLOAT_LITERAL,
            TOKEN_RANGE_INCLUSIVE,
            TOKEN_FLOAT_LITERAL,
            TOKEN_TUPLE_ACCESS
        );
    }

    [Test]
    [Arguments("<", TOKEN_LESS_THAN)]
    [Arguments("<=", TOKEN_LESS_THAN_EQUAL)]
    [Arguments("==", TOKEN_EQUAL)]
    [Arguments("=", TOKEN_EQUAL)]
    [Arguments(">=", TOKEN_GREATER_THAN_EQUAL)]
    [Arguments(">", TOKEN_GREATER_THAN)]
    [Arguments("<>", TOKEN_EMPTY)]
    [Arguments("_", TOKEN_UNDERSCORE)]
    public void test_literals(string mzn, TokenKind tokenKind)
    {
        Lexer.Lex(mzn, out var tokens);
        var token = tokens[0];
        token.Kind.ShouldBe(tokenKind);
    }

    [Test]
    public void test_whitespace()
    {
        var mzn = @$" {'\r'}{'\t'}{'\n'} ";
        Lexer.Lex(mzn, out var tokens);
        tokens.Length.ShouldBe(1);
        tokens[0].Line.ShouldBe(2);
        tokens[0].Kind.ShouldBe(TOKEN_EOF);
    }

    [Test]
    public void xd()
    {
        var s =
            $"""output ["full var: \(x)\nvar array: \(y)\nnested: \(z)\nelement: \(z.2.1)\npartial: \(init)\ndata: \(dat)\nenumtup: \(enumtup)\n"];""";
        Lexer.Lex(s, out var tokens);
    }

    [Test]
    public void test_string_interp()
    {
        var mzn = "\\([\"lala\" | i in 1..3 where b])";
        mzn = $"\"{mzn}\"";
        Lexer.Lex(mzn, out var tokens);
        tokens.Length.ShouldBe(2);
        tokens[0].Kind.ShouldBe(TOKEN_STRING_LITERAL);
        tokens[0].StringValue.ShouldBe("\\([\"lala\" | i in 1..3 where b])");
    }
}
