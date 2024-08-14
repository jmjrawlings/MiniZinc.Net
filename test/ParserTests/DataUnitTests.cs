namespace MiniZinc.Tests;

using Parser;
using static Parser.Parser;

public class DataUnitTests
{
    [Theory]
    [InlineData("{}", "{}")]
    [InlineData("{1}", "{1}")]
    void test_data_eq(string mznA, string mznB)
    {
        ParseDatum(mznA, out var a).Should().BeTrue();
        ParseDatum(mznB, out var b).Should().BeTrue();
        a.Should().Be(b);
    }

    [Theory]
    [InlineData("{1}", "1..1")]
    [InlineData("{1,2,3}", "1..3")]
    [InlineData("{1.5}", "1.5..1.5")]
    void test_data_set_eq(string mznA, string mznB)
    {
        ParseDataString(mznA, out var a);
        ParseDataString(mznB, out var b);
        a.Should().Equal(b);
        b.Should().Equal(a);
    }

    [Fact]
    void test_empty_datum()
    {
        ParseDatum("<>", out var datum);
        datum.Should().Be(Datum.Empty);
    }

    [Fact]
    void test_array_datum()
    {
        ParseDatum<IntArray>("[1,2,3,2,1]", out var array);
        array.Should().Equal(1, 2, 3, 2, 1);
    }

    [Fact]
    void test_mixed_array()
    {
        var ok = ParseDatum("[1,2.0, <>, 1]", out var array);
        ok.Should().BeFalse();
    }
}
