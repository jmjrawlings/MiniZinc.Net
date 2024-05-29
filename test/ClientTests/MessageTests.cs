namespace MiniZinc.Tests;

using System.Text.Json;
using System.Text.RegularExpressions;
using Xunit.Abstractions;

public class MessageTests : TestBase
{
    void test_roundtrip(string input)
    {
        var message1 = MiniZincJsonMessage.Deserialize(input);
        var string1 = JsonSerializer.Serialize(message1, MiniZincJsonMessage.JsonSerializerOptions);
        var message2 = MiniZincJsonMessage.Deserialize(string1);
        var string2 = JsonSerializer.Serialize(message2, MiniZincJsonMessage.JsonSerializerOptions);
        var norm1 = Regex.Replace(string1, @"\s", "");
        var norm2 = Regex.Replace(string2, @"\s", "");
        bool eq = String.Equals(norm1, norm2, StringComparison.OrdinalIgnoreCase);
        eq.Should().BeTrue();
    }

    [Fact]
    void test_solution_message()
    {
        test_roundtrip(
            """
            {
              "type": "solution",
              "time": 1000,
              "output": {
                "foo": "foo output section",
                "bar": "bar output section"
              },
              "sections": ["foo", "bar"]
            }
            """
        );
    }

    [Fact]
    void test_statistics_message()
    {
        test_roundtrip(
            """
            {
              "type": "statistics",
              "statistics": {
                "method": "satisfy",
                "flatTime": 1000
              }
            }
            """
        );
    }

    [Fact]
    void test_trace_message()
    {
        test_roundtrip(
            """
            {"type": "trace",
            "section": "default",
            "message": "traced message\n"}
"""
        );
    }

    public MessageTests(ITestOutputHelper output)
        : base(output) { }
}
