namespace Kaonavi.Net.Entities;

/// <summary>レイアウト定義 カスタム列項目</summary>
/// <param name="Id">シート項目ID</param>
/// <inheritdoc cref="FieldLayout"/>
public record CustomFieldLayout(
    int Id,
    string Name,
    bool Required,
    FieldType Type,
    int? MaxLength,
    IReadOnlyCollection<string?> Enum
) : FieldLayout(Name, Required, Type, MaxLength, Enum);
