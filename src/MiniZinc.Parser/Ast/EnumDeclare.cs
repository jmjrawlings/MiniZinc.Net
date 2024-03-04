namespace MiniZinc.Parser.Ast;

public enum EnumCaseType
{
    Name,
    Anonymous,
    Complex
}

public readonly record struct EnumCase
{
    public EnumCaseType Type { get; init; }
    public string? Name { get; init; }
    public IExpr? Expr { get; init; }
}

public record EnumDeclare : INamed, IAnnotations, IExpr
{
    public string Name { get; set; }
    public List<IExpr>? Annotations { get; set; }
    public List<EnumCase> Cases { get; set; } = new();
}
