namespace MiniZinc.Parser.Syntax;

public sealed class LetSyntax : ExpressionSyntax
{
    public LetSyntax(in Token start, List<ILetLocalSyntax>? locals, SyntaxNode body)
        : base(start)
    {
        Locals = locals;
        Body = body;
    }

    public List<ILetLocalSyntax>? Locals { get; }

    public SyntaxNode Body { get; }
}
