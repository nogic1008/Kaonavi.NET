using System.Text.Json.Serialization;

namespace Kaonavi.Net.Entities
{
    /// <summary>ロール情報</summary>
    public record Role(
        /// <summary>ロールID</summary>
        [property: JsonPropertyName("id")] int Id,
        /// <summary>ロール名</summary>
        [property: JsonPropertyName("name")] string Name,
        /// <summary>ロールの種別 ("Adm", "一般")</summary>
        [property: JsonPropertyName("type")] string Type
    );
}
