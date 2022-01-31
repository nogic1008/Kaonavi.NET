namespace Kaonavi.Net.Entities;

/// <summary>シート レイアウト定義情報</summary>
/// <param name="Id">シートID</param>
/// <param name="Name">シート名</param>
/// <param name="RecordType">レコードの種類</param>
/// <param name="CustomFields">シートのレイアウト定義リスト</param>
public record SheetLayout(
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("record_type")] RecordType RecordType,
    [property: JsonPropertyName("custom_fields")] IReadOnlyList<CustomFieldLayout> CustomFields
);

/// <summary><inheritdoc cref="SheetLayout" path="/param[@name='RecordType']/text()"/></summary>
public enum RecordType
{
    /// <summary>単一レコードシート</summary>
    Single = 0,
    /// <summary>複数レコードシート</summary>
    Multiple = 1,
}
