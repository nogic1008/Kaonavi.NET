using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Kaonavi.Net.Entities
{
    /// <summary>基本情報のカスタム項目/シート情報の項目</summary>
    public record CustomFieldValue
    {
        /// <summary>
        /// 単一の項目値を持つ、CustomFieldValueの新しいインスタンスを生成します。
        /// </summary>
        /// <param name="id"><inheritdoc cref="Id" path="/summary/text()"/></param>
        /// <param name="value">シート項目値</param>
        /// <param name="name"><inheritdoc cref="Name" path="/summary/text()"/></param>
        public CustomFieldValue(int id, string value, string? name = null) : this(id, new[] { value }, name) { }

        /// <summary>
        /// 複数の項目値を持つ、CustomFieldValueの新しいインスタンスを生成します。
        /// </summary>
        /// <param name="id"><inheritdoc cref="Id" path="/summary/text()"/></param>
        /// <param name="values"><inheritdoc cref="Values" path="/summary/text()"/></param>
        /// <param name="name"><inheritdoc cref="Name" path="/summary/text()"/></param>
        [JsonConstructor]
        public CustomFieldValue(int id, IReadOnlyList<string> values, string? name = null)
            => (Id, Values, Name) = (id, values, name);

        /// <summary><inheritdoc cref="CustomField" path="/param[@name='Id']/text()"/></summary>
        [JsonPropertyName("id")]
        public int Id { get; init; }

        /// <summary>シート項目名</summary>
        [JsonPropertyName("name")]
        public string? Name { get; init; }

        /// <summary>シート項目値のリスト</summary>
        /// <remarks>チェックボックスの場合にのみ複数の値が返却されます。</remarks>
        [JsonPropertyName("values")]
        public IReadOnlyList<string> Values { get; init; }
    }
}
