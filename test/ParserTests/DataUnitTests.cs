namespace MiniZinc.Tests;

using Parser;

public class DataUnitTests
{
    [Theory]
    [InlineData("{}", "{}")]
    [InlineData("{1}", "{1}")]
    void test_data_eq(string mznA, string mznB)
    {
        Parser.ParseDataString(mznA, out var a).Ok.Should().BeTrue();
        Parser.ParseDataString(mznB, out var b).Ok.Should().BeTrue();
        a.Should().Equal(b);
    }

    [Theory]
    [InlineData("{1}", "1..1")]
    [InlineData("{1,2,3}", "1..3")]
    [InlineData("{1.5}", "1.5..1.5")]
    void test_data_set_eq(string mznA, string mznB)
    {
        Parser.ParseDataString(mznA, out var a);
        Parser.ParseDataString(mznB, out var b);
        a.Should().Equal(b);
        b.Should().Equal(a);
    }
}
