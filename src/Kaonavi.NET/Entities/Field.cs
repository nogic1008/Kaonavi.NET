using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Kaonavi.Net.Entities
{
    /// <summary>レイアウト定義 列項目</summary>
    public record Field
    {
        /// <summary>
        /// Fieldの新しいインスタンスを生成します。
        /// </summary>
        /// <param name="name"><inheritdoc cref="Name" path="/summary/text()"/></param>
        /// <param name="required"><inheritdoc cref="Required" path="/summary/text()"/></param>
        /// <param name="type"><inheritdoc cref="Type" path="/summary/text()"/></param>
        /// <param name="maxLength"><inheritdoc cref="MaxLength" path="/summary/text()"/></param>
        /// <param name="enum"><inheritdoc cref="Enum" path="/summary/text()"/></param>
        public Field(string name, bool required, FieldType type, int? maxLength, IReadOnlyList<string?> @enum)
            => (Name, Required, Type, MaxLength, Enum) = (name, required, type, maxLength, @enum);

        /// <summary>項目名</summary>
        [JsonPropertyName("name")]
        public string Name { get; init; }

        /// <summary>必須入力項目</summary>
        [JsonPropertyName("required")]
        public bool Required { get; init; }

        /// <summary>入力タイプ ("string", "number", "date", "enum")</summary>
        [JsonPropertyName("type")]
        public FieldType Type { get; init; }

        /// <summary><see cref="Type"/>が"string"の場合に設定可能な最大文字数</summary>
        [JsonPropertyName("max_length")]
        public int? MaxLength { get; init; }

        /// <summary><see cref="Type"/>が"enum"の場合に設定可能な値のリスト</summary>
        [JsonPropertyName("enum")]
        public IReadOnlyList<string?> Enum { get; init; }
    }
}
