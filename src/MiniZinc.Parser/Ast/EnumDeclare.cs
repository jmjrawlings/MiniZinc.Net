namespace MiniZinc.Parser.Ast;

public enum EnumCaseType
{
    Names,
    Complex,
    Underscore,
    Anon
}

public interface IEnumCases : INode
{
    EnumCaseType Type { get; }
}

public readonly struct NamedEnumCases(List<string> names) : IEnumCases
{
    public EnumCaseType Type => EnumCaseType.Names;
    public List<string> Names => names;
}

public readonly struct ComplexEnumCase(INode expr, EnumCaseType type) : IEnumCases
{
    public EnumCaseType Type => type;
    public INode Expr => expr;
}

public record EnumDeclare : Expr, INamed
{
    public string Name { get; set; }

    public List<IEnumCases> Cases { get; set; } = new();
}
