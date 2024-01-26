namespace MiniZinc.Net.Tests;

using System.ComponentModel;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

[Generator(LanguageNames.CSharp)]
public sealed class LexerTestGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        // get the additional text provider
        var additionalTexts = context.AdditionalTextsProvider;

        // apply a 1-to-1 transform on each text, extracting the path
        var transformed = additionalTexts.Select(static (text, _) => (text.Path));

        // take the file paths from the above batch and make some user visible syntax
        context.RegisterSourceOutput(
            transformed,
            static (sourceProductionContext, path) =>
            {
                var name = Path.GetFileName(path);
                var p = $"\"{path}\"";
                sourceProductionContext.AddSource(
                    $"{name}.cs",
                    $@"
public partial static class Texts
{{
    public const string {name} = {p};
}}"
                );
            }
        );
    }
}
