namespace MiniZinc.Parser.Syntax;

public enum EnumCaseType
{
    Names,
    Complex,
    Underscore,
    Anon
}

public sealed class EnumCasesSyntax : SyntaxNode
{
    public EnumCasesSyntax(in Token Start, EnumCaseType Type, SyntaxNode? Expr = null)
        : base(Start)
    {
        this.Type = Type;
        this.Expr = Expr;
    }

    public IdentifierSyntax? Constructor { get; init; } = null;
    public List<IdentifierSyntax>? Names { get; init; } = null;
    public EnumCaseType Type { get; init; }
    public SyntaxNode? Expr { get; init; }

    public void Deconstruct(out Token Start, out EnumCaseType Type, out SyntaxNode? Expr)
    {
        Start = this.Start;
        Type = this.Type;
        Expr = this.Expr;
    }
}

public sealed class EnumDeclarationSyntax : StatementSyntax, IIdentifiedSyntax
{
    public EnumDeclarationSyntax(in Token Start, IdentifierSyntax Identifier)
        : base(Start)
    {
        this.Identifier = Identifier;
    }

    public string Name => Identifier.Name;
    public List<EnumCasesSyntax> Cases { get; } = new();
    public IdentifierSyntax Identifier { get; init; }
}
