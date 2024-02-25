namespace MiniZinc.Tests;

using System.Text.Json;
using MiniZinc.Client;
using MiniZinc.Client.Messages;
using Xunit.Abstractions;

public class MessageTests : TestBase
{
    void test_roundtrip(string input)
    {
        var message1 = MiniZincMessage.Deserialize(input);
        var output = JsonSerializer.Serialize(message1, MiniZincMessage.JsonSerializerOptions);
        var message2 = MiniZincMessage.Deserialize(output);
        message1.Should().Be(message2);
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
