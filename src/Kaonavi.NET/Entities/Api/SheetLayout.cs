using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Kaonavi.Net.Entities.Api
{
    /// <summary>
    /// シート レイアウト定義情報
    /// </summary>
    public record SheetLayout(
        /// <summary>シートID</summary>
        [property: JsonPropertyName("id")] int Id,
        /// <summary>シート名</summary>
        [property: JsonPropertyName("name")] string Name,
        /// <summary>レコードの種類</summary>
        [property: JsonPropertyName("record_type")] RecordType RecordType,
        /// <summary>シートのレイアウト定義リスト</summary>
        [property: JsonPropertyName("custom_fields")] IEnumerable<CustomField> CustomFields
    );

    /// <summary>レコードの種類</summary>
    public enum RecordType
    {
        /// <summary>単一レコードシート</summary>
        Single = 0,
        /// <summary>複数レコードシート</summary>
        Multiple = 1,
    }
}
