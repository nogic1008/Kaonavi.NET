using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Kaonavi.Net.Entities
{
    /// <summary>
    /// レイアウト定義 列項目
    /// </summary>
    public record Field
    {
        /// <summary>
        /// Fieldの新しいインスタンスを生成します。
        /// </summary>
        /// <param name="name">項目名</param>
        /// <param name="required">必須入力項目</param>
        /// <param name="type">入力タイプ ("string", "number", "date", "enum")</param>
        /// <param name="maxLength"><paramref name="type"/>が"string"の場合に設定可能な最大文字数</param>
        /// <param name="enum"><paramref name="type"/>が"enum"の場合に設定可能な値のリスト</param>
        public Field(string name, bool required, string type, int? maxLength, IReadOnlyList<string?> @enum)
            => (Name, Required, Type, MaxLength, Enum) = (name, required, type, maxLength, @enum);

        /// <summary>項目名</summary>
        [JsonPropertyName("name")]
        public string Name { get; init; }

        /// <summary>必須入力項目</summary>
        [JsonPropertyName("required")]
        public bool Required { get; init; }

        /// <summary>入力タイプ ("string", "number", "date", "enum")</summary>
        [JsonPropertyName("type")]
        public string Type { get; init; }

        /// <summary><see cref="Type"/>が"string"の場合に設定可能な最大文字数</summary>
        [JsonPropertyName("max_length")]
        public int? MaxLength { get; init; }

        /// <summary><see cref="Type"/>が"enum"の場合に設定可能な値のリスト</summary>
        [JsonPropertyName("enum")]
        public IReadOnlyList<string?> Enum { get; init; }
    }
}
