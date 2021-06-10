using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Kaonavi.Net.Entities.Api
{
    /// <summary>
    /// シート レイアウト定義情報
    /// </summary>
    public record SheetLayout
    {
        /// <summary>
        /// SheetLayoutの新しいインスタンスを生成します。
        /// </summary>
        /// <param name="id">シートID</param>
        /// <param name="name">シート名</param>
        /// <param name="recordType">レコードの種類</param>
        /// <param name="customFields">シートのレイアウト定義リスト</param>
        public SheetLayout(int id, string name, RecordType recordType, IEnumerable<CustomField> customFields)
            => (Id, Name, RecordType, CustomFields) = (id, name, recordType, customFields);

        /// <summary>シートID</summary>
        [JsonPropertyName("id")]
        public int Id { get; init; }

        /// <summary>シート名</summary>
        [JsonPropertyName("name")]
        public string Name { get; init; }

        /// <summary>レコードの種類</summary>
        [JsonPropertyName("record_type")]
        public RecordType RecordType { get; init; }

        /// <summary>シートのレイアウト定義リスト</summary>
        [JsonPropertyName("custom_fields")]
        public IEnumerable<CustomField> CustomFields { get; init; }
    }

    /// <summary>レコードの種類</summary>
    public enum RecordType
    {
        /// <summary>単一レコードシート</summary>
        Single = 0,
        /// <summary>複数レコードシート</summary>
        Multiple = 1,
    }
}
