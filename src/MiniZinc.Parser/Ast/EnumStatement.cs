namespace MiniZinc.Parser.Ast;

public enum EnumCaseType
{
    Names,
    Complex,
    Underscore,
    Anon
}

public record EnumCases : Node
{
    public EnumCaseType Type { get; set; }
    public List<string>? Names { get; set; }
    public Expr? Expr { get; set; }
}

public sealed record EnumStatement : Node, INamed
{
    public required string Name { get; set; }

    public List<EnumCases> Cases { get; } = new();
}
