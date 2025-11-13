using Kaonavi.Net.Entities;
using Kaonavi.Net.Json;

namespace Kaonavi.Net.Tests.Entities;

/// <summary><see cref="Token"/>の単体テスト</summary>
[TestClass, TestCategory("Entities")]
public sealed class TokenTest
{
    /// <summary>
    /// JSONからデシリアライズできる。
    /// </summary>
    [TestMethod(DisplayName = $"{nameof(Token)} > JSONからデシリアライズできる。"), TestCategory("JSON Deserialize")]
    public void CanDeserializeJSON()
    {
        // Arrange
        /*lang=json,strict*/
        const string jsonString = """
        {
          "access_token": "25396f58-10f8-c228-7f0f-818b1d666b2e",
          "token_type": "Bearer",
          "expires_in": 3600
        }
        """;

        // Act
        var token = JsonSerializer.Deserialize(jsonString, Context.Default.Token);

        // Assert
        token.ShouldNotBeNull().ShouldSatisfyAllConditions(
            static sut => sut.AccessToken.ShouldBe("25396f58-10f8-c228-7f0f-818b1d666b2e"),
            static sut => sut.TokenType.ShouldBe("Bearer"),
            static sut => sut.ExpiresIn.ShouldBe(3600)
        );
    }
}
