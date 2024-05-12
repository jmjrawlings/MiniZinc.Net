namespace MiniZinc.Parser.Ast;

public sealed record BinaryOperatorSyntax : SyntaxNode
{
    public BinaryOperatorSyntax(SyntaxNode left, Operator op, SyntaxNode right)
        : base(left.Start)
    {
        Left = left;
        Op = op;
        Right = right;
    }

    public BinaryOperatorSyntax(Token start, SyntaxNode left, string id, SyntaxNode right)
        : base(start)
    {
        Left = left;
        Name = id;
        Right = right;
    }

    public SyntaxNode Left { get; }
    public Operator? Op { get; }
    public string? Name { get; }
    public SyntaxNode Right { get; }
}
