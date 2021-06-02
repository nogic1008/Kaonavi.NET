using System.Text.Json.Serialization;

namespace Kaonavi.Net.Entities
{
    public record Token(
        [property: JsonPropertyName("access_token")] string AccessToken,
        [property: JsonPropertyName("token_type")] string TokenType,
        [property: JsonPropertyName("expires_in")] int ExpiresIn
    );
}
