namespace MiniZinc.Net.Tests;

using System.Collections;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Xml;
using CommunityToolkit.Diagnostics;

public sealed class TestSpec : IEnumerable<TestSuite>
{
    public required string FileName { get; set; }

    public required List<TestSuite> TestSuites { get; set; }

    public IEnumerable<TestCase> TestCases => TestSuites.SelectMany(s => s.TestCases);

    public IEnumerator<TestSuite> GetEnumerator() => TestSuites.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public static TestSpec ParseYaml(FileInfo file)
    {
        var parser = new TestSpecParser(file);
        var spec = parser.Spec;
        return spec;
    }

    public static async Task<TestSpec> ParseJson(FileInfo file)
    {
        await using var stream = file.OpenRead();
        var suites = await JsonSerializer.DeserializeAsync<List<TestSuite>>(stream);
        Guard.IsNotNull(suites);
        var spec = new TestSpec { FileName = file.FullName, TestSuites = suites };
        return spec;
    }

    public static async Task<FileInfo> WriteJson(TestSpec spec, FileInfo file)
    {
        var options = new JsonSerializerOptions();
        options.WriteIndented = true;
        options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        await using var stream = file.OpenWrite();
        await JsonSerializer.SerializeAsync(stream, spec, options);
        return file;
    }
}
