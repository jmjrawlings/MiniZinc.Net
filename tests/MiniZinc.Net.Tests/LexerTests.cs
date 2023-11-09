namespace MiniZinc.Tests;

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
    public void test_xd()
    {
        var str = """
          /***
          !Test
          expected:
          - !Result
            solution: !Solution
              bin: [0, 1]
              item:
              - [0, 0, 0, 0]
              - [1, 1, 1, 1]
              obj: 1
              objective: 1
          - !Result
            solution: !Solution
              bin:
              - 1
              - 0
              item:
              - [1, 1, 1, 1]
              - [0, 0, 0, 0]
              obj: 1
              objective: 1
          ***/
          
          %------------------------------------------------------------------------------%
          % 2DPacking.mzn
          % Jakob Puchinger
          % October 29 2007
          % vim: ft=zinc ts=4 sw=4 et tw=0
          %------------------------------------------------------------------------------%
          % Two-dimensional Bin Packing 
          %
          % n rectangular items with given height and width have to be packed into
          % rectangular bins of size W x H
          % It is assumed that the items are sorted according to non-increasing height.
          %
          %------------------------------------------------------------------------------%
          
              % Upper Bound on the number of Bins used
          int: K;
              % Width of the Bin
          int: W;
              % Height of the Bin
          int: H;
              % Number of items to be packed
          int: N;
              % Widths of the items
          array[1..N] of int: ItemWidth;
              % Heights of the items
          array[1..N] of int: ItemHeight;
          
              % Variable indicating if bin k is used.
          array[1..K] of var 0..1: bin;
          
              % Variable indicating if item i is in bin k.
          array[1..K, 1..N] of var 0..1: item; 
          
              % The total number of bins has to be minimized
          solve minimize 
              sum( k in 1..K ) ( bin[k] ) ;
          
              % Each item has to be packed.
          constraint
              forall( j in 1..N ) (
                  sum( k in 1..K ) ( item[k, j] ) = 1
              );
          
              % subproblem constraints
          constraint
              forall( k in 1..K ) (
                  is_feasible_packing(bin[k], [ item[k, j] | j in 1..N ])
              ); 
          
              % This predicate defines a feasible packing
              % as a 2-stage guillotine pattern (level pattern).
              % A Bin consists of one or several levels 
              % and each level consist of one or several items.
              % The height of a level is given by its highest (first) item.
              % Variable x[i, j] indicates if item i is put in level j
              % x[j, j] also indicate that level j is used.
              %
              % Note, k is locally fixed, so this is the pattern for the k'th bin. 
              %
          predicate is_feasible_packing(var 0..1: s_bin,
                  array[1..N] of var 0..1: s_item) = (
              let {array[1..N, 1..N] of var 0..1: x} in (
                  forall ( i in 1..N ) (
                          % Width of items on level can not exceed W
                      sum( j in i..N ) ( ItemWidth[j] * x[i, j] ) <= W * x[i, i]
                      /\
                          % first item in level is highest
                          % XXX do not need to generate those variables (use default)
                      forall( j in 1..i-1 ) ( x[i, j] = 0 ) 
                  )
                  /\    
                      % Height of levels on bin can not exceed H
                  sum( i in 1..N ) ( ItemHeight[i] * x[i, i] ) <= s_bin * H 
                  /\
                      % for all items associate item on bin with item on level.
                  forall(j in 1..N) (
                      s_item[j] = sum( i in 1..j ) ( x[i, j] )
                  )
              )
          );
          
              % required for showing the objective function
          var int: obj;
          constraint
              obj = sum( k in 1..K )( bin[k] );
          
          output 
          [ "Number of used bins = ",  show( obj ), "\n"] ++ 
          [ "Items in bins = \n\t"] ++ 
          [ show(item[k, j]) ++ if j = N then "\n\t" else " " endif |
              k in 1..K, j in 1..N ] ++
          [ "\n" ];
          
          %------------------------------------------------------------------------------%
          %
          % Test data (Not in separate file, so that mzn2lp can handle it).
          %
          
          N = 4;
          W = 5;
          H = 10;
          K = 2;
          ItemWidth =  [ 1, 1, 2, 3 ];
          ItemHeight = [ 4, 4, 4, 3 ];
          
          %N = 6;
          %W = 5;
          %H = 10;
          %K = N;
          %ItemWidth =  [ 4, 4, 1, 1, 2, 3 ];
          %ItemHeight = [ 6, 5, 4, 4, 4, 3 ];
          """;

        var tokens = LexString(str).ToArray();
        var last = tokens[^1];
        if (last.Kind == TokenKind.EOF)
            return;
        Assert.Fail("EOF expected");
    }
}
