using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace MiniZinc.Build;

public sealed class TestDatabase
{
    public readonly DirectoryInfo Directory;
    public readonly TestSpec Spec;

    private TestDatabase(TestSpec spec, DirectoryInfo dir)
    {
        Spec = spec;
        Directory = dir;
    }

    /// Compile the given test spec at the given directory
    public static void Compile(TestSpec spec, DirectoryInfo dir) { }
}
