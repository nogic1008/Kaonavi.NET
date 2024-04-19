using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Kaonavi.Net.Tests;

internal static class CSharpGeneratorRunner
{
    private static Compilation _baseCompilation = default!;

    [ModuleInitializer]
    public static void InitializeCompilation()
    {
        // running .NET Core system assemblies dir path
        string baseAssemblyPath = Path.GetDirectoryName(typeof(object).Assembly.Location)!;
        var systemAssemblies = Directory.GetFiles(baseAssemblyPath)
            .Where(x =>
            {
                string fileName = Path.GetFileName(x);
                if (fileName.EndsWith("Native.dll"))
                    return false;
                return fileName.StartsWith("System") || (fileName is "mscorlib.dll" or "netstandard.dll");
            });

        var references = systemAssemblies
            .Append(typeof(SheetSerializableAttribute).Assembly.Location) // Kaonavi.Net.Core.dll
            .Select(x => MetadataReference.CreateFromFile(x))
            .ToArray();

        var compilation = CSharpCompilation.Create("generatortest",
            references: references,
            options: new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary));

        _baseCompilation = compilation;
    }

    public static (Compilation, ImmutableArray<Diagnostic>) RunGenerator(string source, string[]? preprocessorSymbols = null, AnalyzerConfigOptionsProvider? options = null)
    {
        var parseOptions = new CSharpParseOptions(LanguageVersion.CSharp11, preprocessorSymbols: preprocessorSymbols);

        var driver = CSharpGeneratorDriver.Create(new Generator.SheetDataGenerator()).WithUpdatedParseOptions(parseOptions);
        if (options is not null)
            driver = (CSharpGeneratorDriver)driver.WithUpdatedAnalyzerConfigOptions(options);

        var compilation = _baseCompilation.AddSyntaxTrees(CSharpSyntaxTree.ParseText(source, parseOptions));

        driver.RunGeneratorsAndUpdateCompilation(compilation, out var newCompilation, out var diagnostics);
        return (newCompilation, diagnostics);

        // combine diagnostics as result.(ignore warning)
        // var compilationDiagnostics = newCompilation.GetDiagnostics();
        // return diagnostics.Concat(compilationDiagnostics).Where(x => x.Severity >= DiagnosticSeverity.Warning).ToArray();
    }
}
