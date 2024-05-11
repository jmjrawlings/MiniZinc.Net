namespace MiniZinc.Parser.Ast;

public sealed record ParameterSyntax(TypeSyntax Type, Token Name) : SyntaxNode(Type){
    
}