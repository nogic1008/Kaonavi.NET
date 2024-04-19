using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Kaonavi.Net.Generator;

/// <summary>
/// [Kaonavi.Net.SheetSerializable] 属性を対象に実行されるソース ジェネレーター
/// </summary>
[Generator(LanguageNames.CSharp)]
public partial class SheetDataGenerator : IIncrementalGenerator
{
    /// <inheritdoc/>
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var typeDeclarations = context.SyntaxProvider.ForAttributeWithMetadataName(
            Consts.SheetSerializable,
            predicate: static (node, token) =>
            {
                token.ThrowIfCancellationRequested();
                return node is ClassDeclarationSyntax or RecordDeclarationSyntax;
            },
            transform: static (context, token) =>
            {
                token.ThrowIfCancellationRequested();
                return (TypeDeclarationSyntax)context.TargetNode;
            }
        );
        var parseOptions = context.ParseOptionsProvider.Select(static (parseOptions, token) =>
        {
            token.ThrowIfCancellationRequested();
            return ((CSharpParseOptions)parseOptions).LanguageVersion;
        });

        var source = typeDeclarations
            .Combine(context.CompilationProvider)
            .WithComparer(Comparer.Instance)
            .Combine(parseOptions);

        context.RegisterSourceOutput(source, static (context, source) =>
        {
            var token = context.CancellationToken;
            token.ThrowIfCancellationRequested();

            var ((syntax, compilation), languageVersion) = source;
            Emit(syntax, compilation, languageVersion, new GeneratorContext(context, languageVersion));
        });
    }

    private class Comparer : IEqualityComparer<(TypeDeclarationSyntax, Compilation)>
    {
        public static readonly Comparer Instance = new();
        public bool Equals((TypeDeclarationSyntax, Compilation) x, (TypeDeclarationSyntax, Compilation) y) => x.Item1.Equals(y.Item1);
        public int GetHashCode((TypeDeclarationSyntax, Compilation) obj) => obj.Item1.GetHashCode();
    }

    private class GeneratorContext(SourceProductionContext context, LanguageVersion languageVersion) : IGeneratorContext
    {
        public CancellationToken CancellationToken => context.CancellationToken;
        public LanguageVersion LanguageVersion { get; } = languageVersion;
        public void AddSource(string hintName, string source) => context.AddSource(hintName, source);
        public void ReportDiagnostic(Diagnostic diagnostic) => context.ReportDiagnostic(diagnostic);
    }
}
