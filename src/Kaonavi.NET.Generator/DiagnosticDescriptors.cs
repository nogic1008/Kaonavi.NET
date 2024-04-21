namespace Kaonavi.Net.Generator;

/// <summary>
/// このコードジェネレーターが発出する警告/エラーの一覧。
/// </summary>
internal static class DiagnosticDescriptors
{
    private const string Category = "GenerateKaonaviNet";

    /// <summary>Resources.resxからローカライズされた文字列を取得します。</summary>
    /// <param name="resourceName">キー</param>
    private static LocalizableResourceString GetResource(string resourceName)
        => new(resourceName, Resources.ResourceManager, typeof(Resources));

    /// <summary>
    /// [SheetSerializable]属性のついたクラスがpartialでない場合に発出されます。
    /// </summary>
    public static readonly DiagnosticDescriptor MustBePartial
        = new("KAONAVI001", GetResource("KAONAVI001_Title"), GetResource("KAONAVI001_MessageFormat"), Category, DiagnosticSeverity.Warning, true);

    /// <summary>
    /// [SheetSerializable]属性のついたクラスが入れ子にされた型である場合に発出されます。
    /// </summary>
    public static readonly DiagnosticDescriptor MustBeOuterClass
        = new("KAONAVI006", GetResource("KAONAVI006_Title"), GetResource("KAONAVI006_MessageFormat"), Category, DiagnosticSeverity.Warning, true);

    /// <summary>
    /// [SheetSerializable]属性のついたクラスがISheetDataを実装していない場合に発出されます。
    /// </summary>
    public static readonly DiagnosticDescriptor MustImplCustomSheet
        = new("KAONAVI002", GetResource("KAONAVI002_Title"), GetResource("KAONAVI002_MessageFormat"), Category, DiagnosticSeverity.Warning, true);

    /// <summary>
    /// ToCustomFields()が既に実装されている場合に発出されます。
    /// </summary>
    public static readonly DiagnosticDescriptor AlreadyImplemented
        = new("KAONAVI003", GetResource("KAONAVI003_Title"), GetResource("KAONAVI003_MessageFormat"), Category, DiagnosticSeverity.Warning, true);

    /// <summary>
    /// カスタムフィールドが1つも定義されていない場合に発出されます。
    /// </summary>
    public static readonly DiagnosticDescriptor MustHaveCustomField
        = new("KAONAVI004", GetResource("KAONAVI004_Title"), GetResource("KAONAVI004_MessageFormat"), Category, DiagnosticSeverity.Warning, true);

    /// <summary>
    /// カスタムフィールドのIDが重複している場合に発出されます。
    /// </summary>
    public static readonly DiagnosticDescriptor DuplicateCustomFieldId
        = new("KAONAVI005", GetResource("KAONAVI005_Title"), GetResource("KAONAVI005_MessageFormat"), Category, DiagnosticSeverity.Warning, true);
}
