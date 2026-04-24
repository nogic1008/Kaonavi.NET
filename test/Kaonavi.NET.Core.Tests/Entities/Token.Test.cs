using System.Text.Json;
using Kaonavi.Net.Entities;
using JsonContext = Kaonavi.Net.Json.Context;

namespace Kaonavi.Net.Tests.Entities;

/// <summary><see cref="Token"/>の単体テスト</summary>
[Category("Entities")]
public sealed class TokenTest
{
    /// <summary>
    /// JSONからデシリアライズできる。
    /// </summary>
    [Test($"{nameof(Token)} > JSONからデシリアライズできる。")]
    [Category("JSON Deserialize")]
    public async Task CanDeserializeJSON()
    {
        // Arrange
        /*lang=json,strict*/
        var json = """
        {
          "access_token": "25396f58-10f8-c228-7f0f-818b1d666b2e",
          "token_type": "Bearer",
          "expires_in": 3600
        }
        """u8;

        // Act
        var token = JsonSerializer.Deserialize(json, JsonContext.Default.Token);

        // Assert
        await Assert.That(token).IsNotNull()
            .And.Member(static o => o.AccessToken, static o => o.IsEqualTo<string>("25396f58-10f8-c228-7f0f-818b1d666b2e"))
            .And.Member(static o => o.TokenType, static o => o.IsEqualTo<string>("Bearer"))
            .And.Member(static o => o.ExpiresIn, static o => o.IsEqualTo(3600));
    }
}
