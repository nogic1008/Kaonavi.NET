using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Kaonavi.Net.Generator;

public partial class SheetDataGenerator
{
    /// <summary>インデント幅</summary>
    private const int Width = 4;

    /// <summary>
    /// ソース生成処理を実行します。
    /// </summary>
    /// <param name="syntax">[SheetSerializable]属性が指定された箇所のシンタックス</param>
    /// <param name="compilation">現在のコンパイル結果</param>
    /// <param name="version">C#のバージョン</param>
    /// <param name="context">ソース生成コンテキスト</param>
    private static void Emit(TypeDeclarationSyntax syntax, Compilation compilation, LanguageVersion version, SourceProductionContext context)
    {
        var typeSymbol = compilation.GetSemanticModel(syntax.SyntaxTree).GetDeclaredSymbol(syntax, context.CancellationToken);
        if (typeSymbol is null)
            return;

        // Validate class definition
        if (!syntax.Modifiers.Any(m => m.IsKind(SyntaxKind.PartialKeyword)))
        {
            context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.MustBePartial, syntax.Identifier.GetLocation(), typeSymbol.Name));
            return;
        }
        if (typeSymbol.ContainingType is not null)
        {
            context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.MustBeOuterClass, syntax.Identifier.GetLocation(), typeSymbol.Name));
            return;
        }
        if (!typeSymbol.AllInterfaces.Contains(compilation.GetTypeByMetadataName(Consts.ISheetData)!))
        {
            context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.MustImplSheetData, syntax.Identifier.GetLocation(), typeSymbol.Name));
            return;
        }
        if (typeSymbol.GetMembers(Consts.ToCustomFields).OfType<IMethodSymbol>().FirstOrDefault(m => m.Parameters.Length == 0) is IMethodSymbol symbol)
        {
            context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.AlreadyImplemented, symbol.Locations[0], typeSymbol.Name));
            return;
        }

        // Get & Validate CustomField attributes
        var customFieldDict = GetCustomFields(syntax, typeSymbol, compilation, context);
        if (customFieldDict is null)
            return;

        // Generate source code
        string fullType = typeSymbol.ToDisplayString(SymbolDisplayFormat.FullyQualifiedFormat)
            .Replace("global::", "")
            .Replace("<", "_")
            .Replace(">", "_");
        context.AddSource($"{fullType}.g.cs", GenerateImplementedCode(typeSymbol, customFieldDict, version));
    }

    /// <summary>
    /// [CustomField]属性のついたプロパティを取得します。
    /// </summary>
    /// <param name="syntax">[SheetSerializable]属性が指定された箇所のシンタックス</param>
    /// <param name="typeSymbol">[SheetSerializable]属性が指定されたクラスのシンボル</param>
    /// <param name="compilation">現在のコンパイル結果</param>
    /// <param name="context">ソース生成コンテキスト</param>
    /// <returns>[CustomField]属性の設定に不備がある場合は<see langword="null"/>, 適切に設定されている場合はIDをKey, プロパティ情報をValueとしたDictionary</returns>
    private static IDictionary<int, IPropertySymbol>? GetCustomFields(TypeDeclarationSyntax syntax, INamedTypeSymbol typeSymbol, Compilation compilation, SourceProductionContext context)
    {
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

        var customFieldDict = new Dictionary<int, IPropertySymbol>(customFieldProperties.Length);
        bool hasError = false;
        foreach (var (prop, id) in customFieldProperties)
        {
            if (customFieldDict.TryGetValue(id.GetValueOrDefault(), out var prev))
            {
                context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.DuplicateCustomFieldId, prev.Locations[0], prev.Name, id, prop.Name));
                context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.DuplicateCustomFieldId, prop.Locations[0], prop.Name, id, prev.Name));
                hasError = true;
                continue;
            }
            if (prop.GetMethod is null)
            {
                context.ReportDiagnostic(Diagnostic.Create(DiagnosticDescriptors.MustHaveGetter, prop.Locations[0], prop.Name));
                hasError = true;
                continue;
            }
            customFieldDict.Add(id.GetValueOrDefault(), prop);
        }
        return hasError ? null : customFieldDict;
    }

    /// <summary>
    /// ISheetData.ToCustomFields()を実装するソースコードを生成します。
    /// </summary>
    /// <param name="typeSymbol">生成対象となるクラスのSymbol</param>
    /// <param name="customFields">[CustomField]属性のついたプロパティ</param>
    /// <param name="version">C#のバージョン</param>
    private static string GenerateImplementedCode(INamedTypeSymbol typeSymbol, IDictionary<int, IPropertySymbol> customFields, LanguageVersion version)
    {
        int lv = 0;
        var sb = new StringBuilder();

        AppendHeader(sb);

        bool hasNamespace = !typeSymbol.ContainingNamespace.IsGlobalNamespace;
        if (hasNamespace)
        {
            if (version >= LanguageVersion.CSharp10)
            {
                // Use file-scoped namespace (C# 10)
                AppendLineWithIndent(sb, lv, $"namespace {typeSymbol.ContainingNamespace.ToDisplayString()};")
                    .AppendLine();
            }
            else
            {
                // Use nested namespace
                AppendLineWithIndent(sb, lv, $"namespace {typeSymbol.ContainingNamespace.ToDisplayString()}");
                AppendLineWithIndent(sb, lv++, "{"); // start of namespace
            }
        }

        AppendLineWithIndent(sb, lv, $"partial {(typeSymbol.IsRecord ? "record" : "class")} {typeSymbol.Name}");
        AppendLineWithIndent(sb, lv++, "{"); // start of class

        AppendLineWithIndent(sb, lv, "/// <inheritdoc/>");
        AppendLineWithIndent(sb, lv++, $"public {Consts.ToCustomFieldsReturnType} ToCustomFields()");

        if (version >= (LanguageVersion)1200)
        {
            // Use collection expression (C# 12)
            AppendLineWithIndent(sb, lv, "=>");
            AppendLineWithIndent(sb, lv++, "["); // start of collection expression
        }
        else
        {
            // Use array initializer
            AppendLineWithIndent(sb, lv, "=> new[]");
            AppendLineWithIndent(sb, lv++, "{"); // start of array initializer
        }
        foreach (var kv in customFields)
        {
            string typeFullName = $"{kv.Value.Type.ContainingNamespace.Name}.{kv.Value.Type.Name}";
            bool isNullableValueType = typeFullName.StartsWith("System.Nullable");
            typeFullName = isNullableValueType ? ((INamedTypeSymbol)kv.Value.Type).TypeArguments[0].ToDisplayString() : typeFullName;
            bool isDate = Consts.DateObjects.Contains(typeFullName);
            // Use ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) for Date objects, otherwise use ToString()
            string value = $"{kv.Value.Name}{(isNullableValueType ? $".{nameof(Nullable<DateTime>.GetValueOrDefault)}()" : "")}.ToString({(isDate ? $"\"{Consts.DateFormat}\", {Consts.InvariantCulture}" : "")})";
            AppendLineWithIndent(sb, lv, $"new {Consts.CustomFieldValue}({kv.Key}, {value}),");
        }

        if (version >= (LanguageVersion)1200)
            AppendLineWithIndent(sb, --lv, "];"); // end of collection expression
        else
            AppendLineWithIndent(sb, --lv, "};"); // end of array initializer

        AppendLineWithIndent(sb, lv -= 2, "}"); // end of class
        if (hasNamespace && version < LanguageVersion.CSharp10)
            AppendLineWithIndent(sb, --lv, "}"); // end of namespace

        return sb.ToString();
    }

    /// <summary>
    /// 生成ソース共通のヘッダー(auto-generatedタグ, 警告抑制)を追加します。
    /// </summary>
    /// <param name="sb">StringBuilder</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static StringBuilder AppendHeader(StringBuilder sb)
    {
        sb.AppendLine("/// <auto-generated/>")
            .AppendLine();
        sb.AppendLine("#nullable enable annotations");
        sb.AppendLine("#nullable disable warnings")
            .AppendLine();
        foreach (string warning in Consts.DisableWarnings)
            sb.AppendLine($"#pragma warning disable {warning}");
        return sb.AppendLine();
    }

    /// <summary>インデントを挿入します。</summary>
    /// <param name="sb">インデントを挿入するStringBuilder</param>
    /// <param name="level">インデントレベル</param>
    /// <param name="value"><inheritdoc cref="StringBuilder.AppendLine(string)" path="/param[@name='value']"/></param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static StringBuilder AppendLineWithIndent(StringBuilder sb, in int level, string value) => sb.Append(' ', level * Width).AppendLine(value);
}
