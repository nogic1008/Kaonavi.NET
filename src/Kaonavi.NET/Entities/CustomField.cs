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
        /// <param name="id">シート項目ID</param>
        /// <param name="name">項目名</param>
        /// <param name="required">必須入力項目</param>
        /// <param name="type">入力タイプ ("string", "number", "date", "enum")</param>
        public CustomField(int id, string name, bool required, string type)
            : base(name, required, type) => Id = id;

        /// <summary>
        /// <see cref="Type"/>が"string"である、CustomFieldの新しいインスタンスを生成します。
        /// </summary>
        /// <param name="id">シート項目ID</param>
        /// <param name="name">項目名</param>
        /// <param name="required">必須入力項目</param>
        /// <param name="maxLength">設定可能な最大文字数</param>
        public CustomField(int id, string name, bool required, int maxLength)
            : base(name, required, maxLength) => Id = id;

        /// <summary>
        /// <see cref="Type"/>が"enum"である、CustomFieldの新しいインスタンスを生成します。
        /// </summary>
        /// <param name="id">シート項目ID</param>
        /// <param name="name">項目名</param>
        /// <param name="required">必須入力項目</param>
        /// <param name="enum">設定可能な値のリスト</param>
        public CustomField(int id, string name, bool required, IReadOnlyList<string?> @enum)
            : base(name, required, @enum) => Id = id;

        /// <summary>
        /// CustomFieldの新しいインスタンスを生成します。
        /// </summary>
        /// <param name="id">シート項目ID</param>
        /// <param name="name">項目名</param>
        /// <param name="required">必須入力項目</param>
        /// <param name="type">入力タイプ ("string", "number", "date", "enum")</param>
        /// <param name="maxLength"><paramref name="type"/>が"string"の場合に設定可能な最大文字数</param>
        /// <param name="enum"><paramref name="type"/>が"enum"の場合に設定可能な値のリスト</param>
        [JsonConstructor]
        public CustomField(int id, string name, bool required, string type, int? maxLength, IReadOnlyList<string?> @enum)
            : base(name, required, type, maxLength, @enum) => Id = id;

        /// <summary>シート項目ID</summary>
        [JsonPropertyName("id")]
        public int Id { get; init; }
    }
}
