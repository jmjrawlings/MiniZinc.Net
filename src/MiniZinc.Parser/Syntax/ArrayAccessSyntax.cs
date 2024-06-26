namespace MiniZinc.Parser.Syntax;

public sealed class ArrayAccessSyntax : ExpressionSyntax
{
    public readonly ExpressionSyntax Array;
    public readonly List<ExpressionSyntax> Access;

    public ArrayAccessSyntax(ExpressionSyntax array, List<ExpressionSyntax> access)
        : base(array.Start)
    {
        Array = array;
        Access = access;
    }
}
