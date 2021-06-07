using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Kaonavi.Net.Entities
{
    /// <summary>シート情報</summary>
    public record SheetData
    {
        public SheetData(string code, SheetRecord record) : this(code, new[]{ record }) { }

        [JsonConstructor]
        public SheetData(string code, IEnumerable<SheetRecord> records)
            => (Code, Records) = (code, records);

        /// <summary>
        /// 社員コード
        /// </summary>
        [JsonPropertyName("code")]
        public string Code { get; init; }

        /// <summary>
        /// メンバーが持つ設定値のリスト
        /// <see cref="Api.RecordType.Multiple"/>の場合にのみ複数の値が返却されます。
        /// </summary>
        [JsonPropertyName("records")]
        public IEnumerable<SheetRecord> Records { get; init; }
    }

    public record SheetRecord(
        [property: JsonPropertyName("custom_fields")] IEnumerable<CustomFieldValue> CustomFields
    );
}
