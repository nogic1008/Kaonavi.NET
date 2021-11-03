using System.Text.Json.Serialization;

namespace Kaonavi.Net.Entities
{
    /// <summary>アクセストークン</summary>
    /// <param name="AccessToken">アクセストークン</param>
    /// <param name="TokenType">トークン種別(<c>"Bearer"</c>固定)</param>
    /// <param name="ExpiresIn">トークンの有効期限 (秒)</param>
    public record Token(
        [property: JsonPropertyName("access_token")] string AccessToken,
        [property: JsonPropertyName("token_type")] string TokenType,
        [property: JsonPropertyName("expires_in")] int ExpiresIn
    );
}
