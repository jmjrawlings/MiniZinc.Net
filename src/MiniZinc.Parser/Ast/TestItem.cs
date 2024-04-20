namespace MiniZinc.Parser.Ast;

public sealed record TestItem : Expr, INamed
{
    public required string Name { get; set; }

    public required List<DeclareStatement> Params { get; set; }

    public Node? Body { get; set; }
}
