using System.ComponentModel;
using System.Text;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace MiniZinc.Tests;

using Microsoft.CodeAnalysis;

[Generator(LanguageNames.CSharp)]
public sealed class LexerTestGenerator : IIncrementalGenerator
{
    public readonly string AttributeName;
    public readonly string AttributeSource;

    public LexerTestGenerator()
    {
        AttributeName = "LexerTestsAttribute";
        AttributeSource =
            $@"
            namespace MiniZinc.Net.Tests
            {{
                [System.AttributeUsage(System.AttributeTargets.Class)]
                public class {AttributeName} : System.Attribute
                {{
                }}
            }}";
    }

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        context.RegisterPostInitializationOutput(
            ctx =>
                ctx.AddSource(
                    $"{AttributeName}.g.cs",
                    SourceText.From(AttributeSource, Encoding.UTF8)
                )
        );
        // IncrementalValuesProvider<EnumToGenerate?> enumsToGenerate = context
        //     .SyntaxProvider.ForAttributeWithMetadataName(
        //         EnumExtensionsAttribute,
        //         predicate: (node, _) => node is EnumDeclarationSyntax,
        //         transform: GetTypeToGenerate
        //     )
        //     .WithTrackingName(TrackingNames.InitialExtraction)
        //     .Where(static m => m is not null)
        //     .WithTrackingName(TrackingNames.RemovingNulls);
        //
        // context.RegisterSourceOutput(
        //     enumsToGenerate,
        //     static (spc, enumToGenerate) => Execute(in enumToGenerate, spc)
        ;
    }

    // static void Execute(in EnumToGenerate? enumToGenerate, SourceProductionContext context)
    // {
    //     if (enumToGenerate is { } eg)
    //     {
    //         StringBuilder sb = new StringBuilder();
    //         var result = SourceGenerationHelper.GenerateExtensionClass(sb, in eg);
    //         context.AddSource(
    //             eg.Name + "_EnumExtensions.g.cs",
    //             SourceText.From(result, Encoding.UTF8)
    //         );
    //     }
    // }
    //
    // static EnumToGenerate? GetTypeToGenerate(
    //     GeneratorAttributeSyntaxContext context,
    //     CancellationToken ct
    // )
    // {
    //     INamedTypeSymbol? enumSymbol = context.TargetSymbol as INamedTypeSymbol;
    //     if (enumSymbol is null)
    //     {
    //         // nothing to do if this type isn't available
    //         return null;
    //     }
    //
    //     ct.ThrowIfCancellationRequested();
    //
    //     string name = enumSymbol.Name + "Extensions";
    //     string nameSpace = enumSymbol.ContainingNamespace.IsGlobalNamespace
    //         ? string.Empty
    //         : enumSymbol.ContainingNamespace.ToString();
    //     var hasFlags = false;
    //
    //     foreach (AttributeData attributeData in enumSymbol.GetAttributes())
    //     {
    //         if (
    //             (
    //                 attributeData.AttributeClass?.Name == "FlagsAttribute"
    //                 || attributeData.AttributeClass?.Name == "Flags"
    //             )
    //             && attributeData.AttributeClass.ToDisplayString() == FlagsAttribute
    //         )
    //         {
    //             hasFlags = true;
    //             continue;
    //         }
    //
    //         if (
    //             attributeData.AttributeClass?.Name != "EnumExtensionsAttribute"
    //             || attributeData.AttributeClass.ToDisplayString() != EnumExtensionsAttribute
    //         )
    //         {
    //             continue;
    //         }
    //
    //         foreach (
    //             KeyValuePair<string, TypedConstant> namedArgument in attributeData.NamedArguments
    //         )
    //         {
    //             if (
    //                 namedArgument.Key == "ExtensionClassNamespace"
    //                 && namedArgument.Value.Value?.ToString() is { } ns
    //             )
    //             {
    //                 nameSpace = ns;
    //                 continue;
    //             }
    //
    //             if (
    //                 namedArgument.Key == "ExtensionClassName"
    //                 && namedArgument.Value.Value?.ToString() is { } n
    //             )
    //             {
    //                 name = n;
    //             }
    //         }
    //     }
    //
    //     string fullyQualifiedName = enumSymbol.ToString();
    //     string underlyingType = enumSymbol.EnumUnderlyingType?.ToString() ?? "int";
    //
    //     var enumMembers = enumSymbol.GetMembers();
    //     var members = new List<(string, EnumValueOption)>(enumMembers.Length);
    //     HashSet<string>? displayNames = null;
    //     var isDisplayNameTheFirstPresence = false;
    //
    //     foreach (var member in enumMembers)
    //     {
    //         if (member is not IFieldSymbol field || field.ConstantValue is null)
    //         {
    //             continue;
    //         }
    //
    //         string? displayName = null;
    //         foreach (var attribute in member.GetAttributes())
    //         {
    //             if (
    //                 attribute.AttributeClass?.Name == "DisplayAttribute"
    //                 && attribute.AttributeClass.ToDisplayString() == DisplayAttribute
    //             )
    //             {
    //                 foreach (var namedArgument in attribute.NamedArguments)
    //                 {
    //                     if (
    //                         namedArgument.Key == "Name"
    //                         && namedArgument.Value.Value?.ToString() is { } dn
    //                     )
    //                     {
    //                         // found display attribute, all done
    //                         displayName = dn;
    //                         goto addDisplayName;
    //                     }
    //                 }
    //             }
    //
    //             if (
    //                 attribute.AttributeClass?.Name == "DescriptionAttribute"
    //                 && attribute.AttributeClass.ToDisplayString() == DescriptionAttribute
    //                 && attribute.ConstructorArguments.Length == 1
    //             )
    //             {
    //                 if (attribute.ConstructorArguments[0].Value?.ToString() is { } dn)
    //                 {
    //                     // found display attribute, all done
    //                     // Handle cases where contains a quote or a backslash
    //                     displayName = dn.Replace(@"\", @"\\").Replace("\"", "\\\"");
    //                     goto addDisplayName;
    //                 }
    //             }
    //         }
    //
    //         addDisplayName:
    //         if (displayName is not null)
    //         {
    //             displayNames ??= new();
    //             isDisplayNameTheFirstPresence = displayNames.Add(displayName);
    //         }
    //
    //         members.Add(
    //             (member.Name, new EnumValueOption(displayName, isDisplayNameTheFirstPresence))
    //         );
    //     }
    //
    //     return new EnumToGenerate(
    //         name: name,
    //         fullyQualifiedName: fullyQualifiedName,
    //         ns: nameSpace,
    //         underlyingType: underlyingType,
    //         isPublic: enumSymbol.DeclaredAccessibility == Accessibility.Public,
    //         hasFlags: hasFlags,
    //         names: members,
    //         isDisplayAttributeUsed: displayNames?.Count > 0
    //     );
    // }
}
