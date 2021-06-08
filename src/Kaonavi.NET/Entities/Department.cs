using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Kaonavi.Net.Entities
{
    /// <summary>主務/兼務情報</summary>
    public record Department(
        /// <summary>所属コード</summary>
        [property: JsonPropertyName("code")] string Code,
        /// <summary>親所属を含む全ての所属名を半角スペース区切りで返却</summary>
        [property: JsonPropertyName("name")] string? Name = null,
        /// <summary>親所属を含む全ての所属名を配列で返却</summary>
        [property: JsonPropertyName("names")] IEnumerable<string>? Names = null
    );
}
