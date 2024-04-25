namespace Kaonavi.Net.Generator;

/// <summary>
/// このコードジェネレーターが発出する警告/エラーの一覧。
/// </summary>
internal static class DiagnosticDescriptors
{
    private const string Category = "KaonaviSourceGenerate";

    private const string HelpLink = "https://github.com/nogic1008/Kaonavi.NET/tree/v2.0.0/docs/analyzer/";

    /// <summary>Resources.resxからローカライズされた文字列を取得します。</summary>
    /// <param name="resourceName">キー</param>
    private static LocalizableResourceString GetResource(string resourceName)
        => new(resourceName, Resources.ResourceManager, typeof(Resources));

    /// <summary>
    /// DiagnosticDescriptorを生成します。
    /// </summary>
    /// <remarks>Resource.resxへのリソース文字列登録が必要です。</remarks>
    /// <param name="id"><inheritdoc cref="DiagnosticDescriptor.Id" path="/summary"/></param>
    /// <param name="severity"><inheritdoc cref="DiagnosticDescriptor.DefaultSeverity" path="/summary"/></param>
    /// <param name="isEnabledByDefault"><inheritdoc cref="DiagnosticDescriptor.IsEnabledByDefault" path="/summary"/></param>
    /// <returns></returns>
    private static DiagnosticDescriptor Create(string id, DiagnosticSeverity severity, bool isEnabledByDefault)
        => new(id, GetResource($"{id}_Title"), GetResource($"{id}_MessageFormat"), Category, severity, isEnabledByDefault, helpLinkUri: $"{HelpLink}{id}.md");

    /// <summary>
    /// [SheetSerializable]属性のついたクラスがpartialでない場合に発出されます。
    /// </summary>
    public static readonly DiagnosticDescriptor MustBePartial = Create("KAONAVI001", DiagnosticSeverity.Warning, true);

    /// <summary>
    /// [SheetSerializable]属性のついたクラスが入れ子にされた型である場合に発出されます。
    /// </summary>
    public static readonly DiagnosticDescriptor MustBeOuterClass = Create("KAONAVI002", DiagnosticSeverity.Warning, true);

    /// <summary>
    /// [SheetSerializable]属性のついたクラスがISheetDataを実装していない場合に発出されます。
    /// </summary>
    public static readonly DiagnosticDescriptor MustImplSheetData = Create("KAONAVI003", DiagnosticSeverity.Warning, true);

    /// <summary>
    /// ToCustomFields()が既に実装されている場合に発出されます。
    /// </summary>
    public static readonly DiagnosticDescriptor AlreadyImplemented = Create("KAONAVI004", DiagnosticSeverity.Warning, true);

    /// <summary>
    /// カスタムフィールドが1つも定義されていない場合に発出されます。
    /// </summary>
    public static readonly DiagnosticDescriptor MustHaveCustomField = Create("KAONAVI005", DiagnosticSeverity.Warning, true);

    /// <summary>
    /// カスタムフィールドのIDが重複している場合に発出されます。
    /// </summary>
    public static readonly DiagnosticDescriptor DuplicateCustomFieldId = Create("KAONAVI006", DiagnosticSeverity.Warning, true);
}
