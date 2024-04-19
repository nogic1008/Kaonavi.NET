using System.Text;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Kaonavi.Net.Generator;

public partial class SheetDataGenerator
{
    /// <summary>インデント幅</summary>
    private const int Width = 4;

    /// <summary>
    /// [CustomField]属性のついたプロパティを取得します。
    /// </summary>
    /// <inheritdoc cref="Generate"/>
    /// <returns>IDをKey, プロパティ情報をValueとしたDictionary</returns>
    private static Dictionary<int, IPropertySymbol>? GetCustomFields(TypeDeclarationSyntax syntax, INamedTypeSymbol typeSymbol, Compilation compilation, IGeneratorContext context)
    {

        // Get & Validate CustomField attributes
        var customFieldAttr = compilation.GetTypeByMetadataName(Consts.CustomField);
        var customFieldProperties = typeSymbol.GetMembers().OfType<IPropertySymbol>()
            .Select(p => (
                prop: p,
                id: p.GetAttributes()
                    .FirstOrDefault(a => a.AttributeClass?.Equals(customFieldAttr, SymbolEqualityComparer.Default) ?? false)
                    ?.ConstructorArguments[0].Value as int?
            ))
            .Where(p => p.id is not null).ToArray();

        if (customFieldProperties.Length == 0)
        {
            context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.MustHaveCustomField, syntax.Identifier.GetLocation(), typeSymbol.Name));
            return null;
        }
        var customFieldDict = new Dictionary<int, IPropertySymbol>();
        bool duplicated = false;
        foreach (var (prop, id) in customFieldProperties)
        {
            if (customFieldDict.TryGetValue(id.GetValueOrDefault(), out var prev))
            {
                context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.DuplicateCustomFieldId, prev.Locations[0], prev.Name, id, prop.Name));
                context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.DuplicateCustomFieldId, prop.Locations[0], prop.Name, id, prev.Name));
                duplicated = true;
            }
            else
            {
                customFieldDict.Add(id.GetValueOrDefault(), prop);
            }
        }
        return duplicated ? null : customFieldDict;
    }

    /// <summary>
    /// ソース生成を行います。
    /// </summary>
    /// <param name="syntax">[SheetSerializable]属性が指定された箇所のシンタックス</param>
    /// <param name="compilation">コンパイル</param>
    /// <param name="version">C#のバージョン</param>
    /// <param name="context">ソース生成コンテキストの抽象化</param>
    private static void Generate(TypeDeclarationSyntax syntax, Compilation compilation, LanguageVersion version, IGeneratorContext context)
    {
        var typeSymbol = compilation.GetSemanticModel(syntax.SyntaxTree).GetDeclaredSymbol(syntax, context.CancellationToken);
        if (typeSymbol is null)
            return;

        // Validate class definition
        if (!IsPartial(syntax))
        {
            context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.MustBePartial, syntax.Identifier.GetLocation(), typeSymbol.Name));
            return;
        }
        var sheetDataInterface = compilation.GetTypeByMetadataName(Consts.ISheetData);
        if (!IsImplemented(syntax, sheetDataInterface!))
        {
            context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.MustImplCustomSheet, syntax.Identifier.GetLocation(), typeSymbol.Name));
            return;
        }

        var customFieldDict = GetCustomFields(syntax, typeSymbol, compilation, context);
        if (customFieldDict is null)
            return;

        // Generate source code
        int lv = 0;
        var sb = new StringBuilder();

        sb.AppendLine("/// <auto-generated/>").AppendLine();
        sb.AppendLine("#nullable enable annotations");
        sb.AppendLine("#nullable disable warnings").AppendLine();
        foreach (string warning in Consts.DisableWarnings)
            sb.AppendLine($"#pragma warning disable {warning}");
        sb.AppendLine();

        bool hasNamespace = !typeSymbol.ContainingNamespace.IsGlobalNamespace;
        if (hasNamespace)
        {
            if (version >= LanguageVersion.CSharp10)
            {
                // Use file-scoped namespace (C# 10)
                sb.Append(' ', lv * Width).AppendLine($"namespace {typeSymbol.ContainingNamespace.ToDisplayString()};")
                    .AppendLine();
            }
            else
            {
                // Use nested namespace
                sb.Append(' ', lv * Width).AppendLine($"namespace {typeSymbol.ContainingNamespace.ToDisplayString()}");
                sb.Append(' ', lv++ * Width).AppendLine("{"); // start of namespace
            }
        }

        sb.Append(' ', lv * Width).AppendLine($"partial {(typeSymbol.IsRecord ? "record" : "class")} {typeSymbol.Name}");
        sb.Append(' ', lv++ * Width).AppendLine("{"); // start of class

        sb.Append(' ', lv * Width).AppendLine("/// <inheritdoc/>");
        sb.Append(' ', lv++ * Width).AppendLine($"public {Consts.ToCustomFieldsReturnType} ToCustomFields()");

        if (version >= (LanguageVersion)1200)
        {
            // Use collection expression (C# 12)
            sb.Append(' ', lv * Width).AppendLine("=>");
            sb.Append(' ', lv++ * Width).AppendLine("["); // start of collection expression
        }
        else
        {
            // Use array initializer
            sb.Append(' ', lv * Width).AppendLine("=> new[]");
            sb.Append(' ', lv++ * Width).AppendLine("{"); // start of array initializer
        }
        foreach (var kv in customFieldDict)
        {
            // Use ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) for Date objects, otherwise use ToString()
            string value = Consts.DateObjects.Contains($"{kv.Value.Type.ContainingNamespace.Name}.{kv.Value.Type.Name}")
                ? $"{kv.Value.Name}.ToString(\"{Consts.DateFormat}\", {Consts.InvariantCulture})" : $"{kv.Value.Name}.ToString()";
            sb.Append(' ', lv * Width).AppendLine($"new {Consts.CustomFieldValue}({kv.Key}, {value}),");
        }

        if (version >= (LanguageVersion)1200)
            sb.Append(' ', --lv * Width).AppendLine("];"); // end of collection expression
        else
            sb.Append(' ', --lv * Width).AppendLine("};"); // end of array initializer


        sb.Append(' ', (lv -= 2) * Width).AppendLine("}"); // end of class
        if (hasNamespace && version < LanguageVersion.CSharp10)
            sb.Append(' ', --lv * Width).AppendLine("}"); // end of namespace


        string fullType = typeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
            .Replace("global::", "")
            .Replace("<", "_")
            .Replace(">", "_");
        context.AddSource($"{fullType}.Generated.cs", sb.ToString());

        static bool IsPartial(TypeDeclarationSyntax typeDeclaration)
            => typeDeclaration.Modifiers.Any(m => m.IsKind(SyntaxKind.PartialKeyword));

        static bool IsImplemented(TypeDeclarationSyntax typeDeclaration, INamedTypeSymbol @interface)
            => typeDeclaration.BaseList?.Types.Any(t => t.Type is IdentifierNameSyntax ins && ins.Identifier.Text == @interface.Name) ?? false;
    }
}
