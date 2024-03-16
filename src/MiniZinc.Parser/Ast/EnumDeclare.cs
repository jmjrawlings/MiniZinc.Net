namespace MiniZinc.Parser.Ast;

public enum EnumCaseType
{
    Name,
    Anonymous,
    Underscore,
    Complex
}

public sealed record EnumCase
{
    public EnumCaseType Type;
    public List<string>? Names;
    public IExpr? Expr;
}

public record EnumDeclare : INamed, IAnnotations, IExpr
{
    public string Name { get; set; }
    public List<IExpr>? Annotations { get; set; }
    public List<EnumCase> Cases { get; set; } = new();
}
