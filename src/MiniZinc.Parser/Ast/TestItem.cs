namespace MiniZinc.Parser.Ast;

public sealed record TestItem : Expr, INamed
{
    public string Name { get; set; }

    public List<DeclareStatement> Params { get; set; }

    public Node? Body { get; set; }
}
