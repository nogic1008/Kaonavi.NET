using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Kaonavi.Net.Entities
{
    /// <summary>
    /// 基本情報のカスタム項目/シート情報の項目
    /// </summary>
    public record CustomFieldValue
    {
        /// <summary>
        /// 単一の項目値を持つ、CustomFieldValueの新しいインスタンスを生成します。
        /// </summary>
        /// <param name="id">シート項目ID</param>
        /// <param name="value">シート項目値</param>
        /// <param name="name">シート項目名</param>
        public CustomFieldValue(int id, string value, string? name = null) : this(id, new[] { value }, name) { }

        /// <summary>
        /// 複数の項目値を持つ、CustomFieldValueの新しいインスタンスを生成します。
        /// </summary>
        /// <param name="id">シート項目ID</param>
        /// <param name="values">シート項目値のリスト</param>
        /// <param name="name">シート項目名</param>
        [JsonConstructor]
        public CustomFieldValue(int id, IReadOnlyList<string> values, string? name = null)
            => (Id, Values, Name) = (id, values, name);

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
        public IReadOnlyList<string> Values { get; init; }
    }
}
