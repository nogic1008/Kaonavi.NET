namespace Kaonavi.Net.Generator;

/// <summary>
/// Shared context for SourceGenerator and IncrementalGenerator
/// </summary>
public interface IGeneratorContext
{
    CancellationToken CancellationToken { get; }
    void ReportDiagnostic(Diagnostic diagnostic);
    void AddSource(string hintName, string source);
    LanguageVersion LanguageVersion { get; }
}
