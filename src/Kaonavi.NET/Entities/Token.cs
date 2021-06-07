using System.Text.Json.Serialization;

namespace Kaonavi.Net.Entities
{
    /// <summary>アクセストークン</summary>
    public record Token(
        /// <summary>アクセストークン</summary>
        [property: JsonPropertyName("access_token")] string AccessToken,
        /// <summary>トークン種別(<c>"Bearer"</c>固定)</summary>
        [property: JsonPropertyName("token_type")] string TokenType,
        /// <summary>トークンの有効期限 (秒)</summary>
        [property: JsonPropertyName("expires_in")] int ExpiresIn
    );
}
