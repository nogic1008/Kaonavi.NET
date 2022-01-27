namespace Kaonavi.Net.Entities;

/// <summary>レイアウト定義 カスタム列項目</summary>
/// <param name="Id">シート項目ID</param>
/// <param name="Name"><inheritdoc cref="FieldLayout" path="/param[@name='Name']"/></param>
/// <param name="Required"><inheritdoc cref="FieldLayout" path="/param[@name='Required']"/></param>
/// <param name="Type"><inheritdoc cref="FieldLayout" path="/param[@name='Type']"/></param>
/// <param name="MaxLength"><inheritdoc cref="FieldLayout" path="/param[@name='MaxLength']"/></param>
/// <param name="Enum"><inheritdoc cref="FieldLayout" path="/param[@name='Enum']"/></param>
public record CustomFieldLayout(
    [property: JsonPropertyName("id")] int Id,
    string Name,
    bool Required,
    FieldType Type,
    int? MaxLength,
    IReadOnlyList<string?> Enum
) : FieldLayout(Name, Required, Type, MaxLength, Enum);