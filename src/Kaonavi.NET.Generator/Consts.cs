namespace Kaonavi.Net.Generator;

internal static class Consts
{
    /// <summary>Namespace of Library.</summary>
    private const string Namespace = "Kaonavi.Net";

    /// <summary>Full Name of SheetSerializable attribute.</summary>
    internal const string SheetSerializable = $"{Namespace}.{nameof(SheetSerializable)}Attribute";
    /// <summary>Full Name of CustomField attribute.</summary>
    internal const string CustomField = $"{Namespace}.{nameof(CustomField)}Attribute";

    /// <summary>Full Name of ISheetData interface.</summary>
    internal const string ISheetData = $"{Namespace}.{nameof(ISheetData)}";

    internal static readonly string[] DisableWarnings = [
        "CS0612", "CS0618", // Obsolete member usage
        "CS0108", // hides inherited member
        "CS0162", // Unreachable code
        "CS0164", // This label has not been referenced
        "CS0219", // Variable assigned but never used
        "CS8602",
        "CS8619",
        "CS8620",
        "CS8631", // The type cannot be used as type parameter in the generic type or method
        "CA1050", // Declare types in namespaces.
    ];

    internal static readonly string[] DateObjects = ["System.DateTime", "System.DateTimeOffset", "System.DateOnly"];
    internal static readonly string DateFormat = "yyyy-MM-dd";

    internal const string CustomFieldValue = $"global::{Namespace}.Entities.CustomFieldValue";
    internal const string ToCustomFieldsReturnType = $"global::System.Collections.Generic.IReadOnlyList<{CustomFieldValue}>";
    internal const string InvariantCulture = "global::System.Globalization.CultureInfo.InvariantCulture";
}
