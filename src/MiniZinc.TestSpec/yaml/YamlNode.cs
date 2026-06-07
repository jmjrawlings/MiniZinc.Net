namespace MiniZinc.TestSpec.Yaml;

public enum ScalarStyle
{
    Plain,
    SingleQuoted,
    DoubleQuoted,
    Literal,
    Folded,
}

public abstract record YamlNode(int Line, int Column, string? Tag);

public sealed record YamlScalar(int Line, int Column, string? Tag, string Value, ScalarStyle Style)
    : YamlNode(Line, Column, Tag)
{
    public bool IsNull => Tag is null && Style is ScalarStyle.Plain && Value is "" or "null" or "~";
}

public sealed record YamlMapping(
    int Line,
    int Column,
    string? Tag,
    IReadOnlyList<YamlMappingEntry> Entries
) : YamlNode(Line, Column, Tag);

public sealed record YamlSequence(
    int Line,
    int Column,
    string? Tag,
    IReadOnlyList<YamlNode> Items
) : YamlNode(Line, Column, Tag);

public sealed record YamlMappingEntry(YamlScalar Key, YamlNode Value);
