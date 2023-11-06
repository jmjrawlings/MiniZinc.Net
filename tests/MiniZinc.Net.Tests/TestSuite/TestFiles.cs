namespace MiniZinc.Tests;

using System.Collections;

using static Prelude;

public class TestFiles : IEnumerable<object[]>
{
    readonly List<object[]> _files = new();

    public TestFiles()
    {
        var files = LibMiniZincDir.EnumerateFiles("*.mzn", SearchOption.AllDirectories).ToArray();
        foreach (var file in files)
            _files.Add(new object[] { file });
    }

    public IEnumerator<object[]> GetEnumerator() => _files.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
