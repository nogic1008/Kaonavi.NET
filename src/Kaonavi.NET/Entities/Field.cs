using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Kaonavi.Net.Entities
{
    /// <summary>
    /// レイアウト定義 列項目
    /// </summary>
    public record Field(
        /// <summary>項目名</summary>
        [property: JsonPropertyName("name")] string Name,
        /// <summary>必須入力項目</summary>
        [property: JsonPropertyName("required")] bool Required,
        /// <summary>入力タイプ ("string", "number", "date", "enum")</summary>
        [property: JsonPropertyName("type")] string Type,
        /// <summary><see cref="Type"/>が"string"の場合に設定可能な最大文字数</summary>
        [property: JsonPropertyName("max_length")] int? MaxLength,
        /// <summary><see cref="Type"/>が"enum"の場合に設定可能な値のリスト</summary>
        [property: JsonPropertyName("enum")] IEnumerable<string?> Enum
    );

    /// <summary>
    /// レイアウト定義 カスタム列項目
    /// </summary>
    public record CustomField(
        /// <summary>シート項目ID</summary>
        [property: JsonPropertyName("id")] int Id,
        string Name,
        bool Required,
        string Type,
        int? MaxLength,
        IEnumerable<string?> Enum
    ) : Field(Name, Required, Type, MaxLength, Enum);
}
