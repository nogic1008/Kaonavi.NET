using Kaonavi.Net.Entities;
using Kaonavi.Net.Json;

namespace Kaonavi.Net.Tests.Entities;

/// <summary><see cref="WebhookConfig"/>の単体テスト</summary>
[TestClass, TestCategory("Entities")]
public sealed class WebhookConfigTest
{
    /// <summary>
    /// JSONから<see cref="WebhookConfig"/>にデシリアライズできる。
    /// </summary>
    [TestMethod($"{nameof(WebhookConfig)} > JSONからデシリアライズできる。"), TestCategory("JSON Deserialize")]
    public void WebhookConfig_CanDeserializeJSON()
    {
        // Arrange
        /*lang=json,strict*/
        const string json = """
        {
            "id": 1,
            "url": "https://example.com",
            "events": ["member_created", "member_updated", "member_deleted"],
            "secret_token": "string"
        }
        """;

        // Act
        var config = JsonSerializer.Deserialize(json, Context.Default.WebhookConfig);

        // Assert
        _ = config.Should().NotBeNull();
        _ = config!.Id.Should().Be(1);
        _ = config.Url.Should().Be(new Uri("https://example.com"));
        _ = config.Events.Should().Equal(WebhookEvent.MemberCreated, WebhookEvent.MemberUpdated, WebhookEvent.MemberDeleted);
        _ = config!.SecretToken.Should().Be("string");
    }
}
