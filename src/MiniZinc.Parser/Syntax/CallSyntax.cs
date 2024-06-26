using System.Data.SqlTypes;

namespace MiniZinc.Parser.Syntax;

public sealed class CallSyntax : ExpressionSyntax, IIdentifiedSyntax
{
    public IdentifierSyntax Identifier { get; }

    public readonly IReadOnlyList<ExpressionSyntax>? Args;

    public string Name => Identifier.Name;

    public CallSyntax(IdentifierSyntax identifier, IReadOnlyList<ExpressionSyntax>? args = null)
        : base(identifier.Start)
    {
        Args = args;
        Identifier = identifier;
    }
}
