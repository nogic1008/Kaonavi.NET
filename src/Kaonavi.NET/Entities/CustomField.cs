using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Kaonavi.Net.Entities
{
    /// <summary>
    /// レイアウト定義 カスタム列項目
    /// </summary>
    public record CustomField : Field
    {
        /// <summary>
        /// CustomFieldの新しいインスタンスを生成します。
        /// </summary>
        /// <param name="id"><inheritdoc cref="Id" path="/summary"/></param>
        /// <inheritdoc cref="Field(string, bool, string, int?, IReadOnlyList{string?})"/>
        public CustomField(int id, string name, bool required, string type, int? maxLength, IReadOnlyList<string?> @enum)
            : base(name, required, type, maxLength, @enum) => Id = id;

        /// <summary>シート項目ID</summary>
        [JsonPropertyName("id")]
        public int Id { get; init; }
    }
}
