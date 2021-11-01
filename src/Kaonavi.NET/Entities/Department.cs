using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Kaonavi.Net.Entities
{
    /// <summary>主務/兼務情報</summary>
    public record Department
    {
        /// <summary>
        /// Departmentの新しいインスタンスを生成します。
        /// </summary>
        /// <param name="code"><inheritdoc cref="Code" path="/summary/text()"/></param>
        /// <param name="name"><inheritdoc cref="Name" path="/summary/text()"/></param>
        /// <param name="names"><inheritdoc cref="Names" path="/summary/text()"/></param>
        public Department(string code, string? name = null, IReadOnlyList<string>? names = null)
            => (Code, Name, Names) = (code, name, names);

        /// <summary>所属コード</summary>
        [JsonPropertyName("code")]
        public string Code { get; init; }

        /// <summary>親所属を含む全ての所属名を半角スペース区切りで返却</summary>
        [JsonPropertyName("name")]
        public string? Name { get; init; }

        /// <summary>親所属を含む全ての所属名を配列で返却</summary>
        [JsonPropertyName("names")]
        public IReadOnlyList<string>? Names { get; init; }
    }
}
