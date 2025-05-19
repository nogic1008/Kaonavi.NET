using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Kaonavi.Net.Generator.Tests;

/// <summary>コード生成の実行を行います。</summary>
internal static class CSharpGeneratorRunner
{
    private static Compilation _baseCompilation = default!;

    [ModuleInitializer]
    public static void InitializeCompilation()
    {
        var references = AppDomain.CurrentDomain.GetAssemblies()
            .Where(x => !x.IsDynamic && !string.IsNullOrWhiteSpace(x.Location))
            .Select(x => MetadataReference.CreateFromFile(x.Location))
            .Concat([
                MetadataReference.CreateFromFile(typeof(SheetSerializableAttribute).Assembly.Location), // Kaonavi.Net.Core.dll
            ]);

        var compilation = CSharpCompilation.Create("generator_test",
            references: references,
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        _baseCompilation = compilation;
    }

    public static (Compilation, ImmutableArray<Diagnostic>) RunGenerator(string source, LanguageVersion version = LanguageVersion.CSharp12)
    {
        var parseOptions = new CSharpParseOptions(version);

        var driver = CSharpGeneratorDriver.Create(new SheetDataGenerator()).WithUpdatedParseOptions(parseOptions);

        var inputCompilation = _baseCompilation.AddSyntaxTrees(CSharpSyntaxTree.ParseText(source, parseOptions));

        driver.RunGeneratorsAndUpdateCompilation(inputCompilation, out var compilation, out var diagnostics);
        return (compilation, diagnostics);
    }
}
