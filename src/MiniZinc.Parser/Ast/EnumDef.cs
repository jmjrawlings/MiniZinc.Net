namespace MiniZinc.Parser.Ast;

public record EnumDef : INamed, IStatement, IAnnotations
{
    public string Name { get; set; }
    public List<IExpr>? Annotations { get; set; }
    public List<string>? Members { get; set; }
    public IExpr Expr { get; set; }
}