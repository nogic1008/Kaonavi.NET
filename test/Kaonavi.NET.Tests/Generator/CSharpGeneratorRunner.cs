using System.Runtime.CompilerServices;
using Kaonavi.Net.Generator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace Kaonavi.Net.Tests.Generator;

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
                return fileName.StartsWith("System") || fileName is "mscorlib.dll" or "netstandard.dll";
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

    public static Diagnostic[] RunGenerator(string source, LanguageVersion version = LanguageVersion.CSharp12)
    {
        var parseOptions = new CSharpParseOptions(version);

        var driver = CSharpGeneratorDriver.Create(new SheetDataGenerator()).WithUpdatedParseOptions(parseOptions);

        var inputCompilation = _baseCompilation.AddSyntaxTrees(CSharpSyntaxTree.ParseText(source, parseOptions));

        driver.RunGeneratorsAndUpdateCompilation(inputCompilation, out var outputCompilation, out var diagnostics);
        var compilationDiagnostics = outputCompilation.GetDiagnostics();
        return diagnostics.Concat(compilationDiagnostics).Where(x => x.Severity >= DiagnosticSeverity.Warning).ToArray();
    }
}
