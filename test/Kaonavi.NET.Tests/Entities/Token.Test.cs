using System.Text.Json;
using FluentAssertions;
using Kaonavi.Net.Entities;
using Xunit;

namespace Kaonavi.Net.Tests.Entities
{
    /// <summary>
    /// Unit test for <see cref="Token"/>
    /// </summary>
    public class TokenTest
    {
        [Fact]
        public void CanDeserializeJSON()
        {
            // Arrange
            const string jsonString = "{"
            + "\"access_token\": \"25396f58-10f8-c228-7f0f-818b1d666b2e\","
            + "\"token_type\": \"Bearer\","
            + "\"expires_in\": 3600"
            + "}";

            // Act
            var token = JsonSerializer.Deserialize<Token>(jsonString);

            // Assert
            token.Should().NotBeNull();
            token!.AccessToken.Should().Be("25396f58-10f8-c228-7f0f-818b1d666b2e");
            token.TokenType.Should().Be("Bearer");
            token.ExpiresIn.Should().Be(3600);
        }
    }
}
