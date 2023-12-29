namespace MiniZinc.Build;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using MiniZinc.Net.Tests;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

public sealed class LexerTests
{
    private readonly List<MethodDeclarationSyntax> _tests;
    private NamespaceDeclarationSyntax _namespace;
    private ClassDeclarationSyntax _class;

    public readonly TestSpec Spec;
    public ClassDeclarationSyntax Class => _class;
    public NamespaceDeclarationSyntax NameSpace => _namespace;

    private LexerTests(TestSpec spec)
    {
        Spec = spec;
        _namespace = NamespaceDeclaration(ParseName("MiniZinc.Net.Tests")).NormalizeWhitespace();
        _tests = new List<MethodDeclarationSyntax>();
        Using("System");
        _class = ClassDeclaration("LexerIntegrationTests");
        _class = _class.AddModifiers(Token(SyntaxKind.PublicKeyword));
        var files = spec.TestSuites
            .SelectMany(s => s.TestCases)
            .Select(c => c.Path)
            .Distinct()
            .Order()
            .ToArray();

        foreach (var file in files)
        {
            var test = CreateTest(file);
            _tests.Add(test);
            _class = _class.AddMembers(test);
        }
    }

    private void Using(string s)
    {
        var name = ParseName(s);
        var @using = UsingDirective(name);
        _namespace = _namespace.AddUsings(@using);
    }

    private MethodDeclarationSyntax CreateTest(string file)
    {
        var methodName = $"Test_{Path.GetFileNameWithoutExtension(file)}";
        var returnType = ParseTypeName("void");
        var method = MethodDeclaration(returnType, methodName);
        return null;
    }

    public static LexerTests Generate(TestSpec spec)
    {
        var tests = new LexerTests(spec);
        return tests;
    }
}
