namespace MiniZinc.TestSpec.Yaml;

public sealed class YamlParseException : Exception
{
    public int Line { get; }
    public int Column { get; }
    public string? FilePath { get; }

    public YamlParseException(string message, int line, int column, string? filePath = null)
        : base(Format(message, line, column, filePath))
    {
        Line = line;
        Column = column;
        FilePath = filePath;
    }

    private static string Format(string message, int line, int column, string? path) =>
        $"{path ?? "<yaml>"}:{line}:{column}: {message}";
}
