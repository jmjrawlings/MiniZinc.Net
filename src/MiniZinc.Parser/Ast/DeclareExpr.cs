namespace MiniZinc.Parser.Ast;

/// <summary>
/// Declares a variable with a type and possibly a value
/// var int: x = 100;
/// </summary>
public sealed record DeclareExpr : IItem, INamed
{
    public string Name { get; set; }
    public Type Type { get; set; }
    public IExpr? Value { get; set; }
}