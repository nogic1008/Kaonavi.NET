using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace Kaonavi.Net.Entities
{
    /// <summary>シート情報</summary>
    public record SheetData
    {
        /// <summary>
        /// 単一レコードシート向けに、SheetDataの新しいインスタンスを生成します。
        /// </summary>
        /// <param name="code">社員コード</param>
        /// <param name="customFields">設定値</param>
        public SheetData(string code, IReadOnlyList<CustomFieldValue> customFields)
            : this(code, new[] { new SheetRecord(customFields) }) { }

        /// <summary>
        /// 複数レコードシート向けに、SheetDataの新しいインスタンスを生成します。
        /// </summary>
        /// <param name="code">社員コード</param>
        /// <param name="records">設定値のリスト</param>
        public SheetData(string code, params IReadOnlyList<CustomFieldValue>[] records)
            : this(code, records.Select(r => new SheetRecord(r)).ToArray()) { }

        /// <summary>
        /// 複数レコードシート向けに、SheetDataの新しいインスタンスを生成します。
        /// </summary>
        /// <param name="code">社員コード</param>
        /// <param name="records">設定値のリスト</param>
        [JsonConstructor]
        public SheetData(string code, IReadOnlyList<SheetRecord> records)
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
        public IReadOnlyList<SheetRecord> Records { get; init; }
    }

    /// <summary>シート情報の設定値</summary>
    public record SheetRecord(
        [property: JsonPropertyName("custom_fields")] IReadOnlyList<CustomFieldValue> CustomFields
    );
}
