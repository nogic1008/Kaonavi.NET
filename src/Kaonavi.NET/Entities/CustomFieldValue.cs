using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Kaonavi.Net.Entities
{
    /// <summary>
    /// 基本情報のカスタム項目/シート情報の項目
    /// </summary>
    public record CustomFieldValue
    {
        public CustomFieldValue(int id, string? name, string value) : this(id, name, new[] { value }) { }

        [JsonConstructor]
        public CustomFieldValue(int id, string? name, IEnumerable<string> values)
            => (Id, Name, Values) = (id, name, values);

        /// <summary>シート項目ID</summary>
        [JsonPropertyName("id")]
        public int Id { get; init; }

        /// <summary>シート項目名</summary>
        [JsonPropertyName("name")]
        public string? Name { get; init; }

        /// <summary>
        /// シート項目値のリスト
        /// チェックボックスの場合にのみ複数の値が返却されます。
        /// </summary>
        [JsonPropertyName("values")]
        public IEnumerable<string> Values { get; init; }
    }
}
