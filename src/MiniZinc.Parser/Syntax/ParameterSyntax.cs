namespace MiniZinc.Parser.Syntax;

public sealed record ParameterSyntax(TypeSyntax Type, Token? Name) : SyntaxNode(Type){
    
}