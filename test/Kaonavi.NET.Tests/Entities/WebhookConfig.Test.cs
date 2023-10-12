using Kaonavi.Net.Entities;

namespace Kaonavi.Net.Tests.Entities;

/// <summary>
/// <see cref="WebhookConfig"/>の単体テスト
/// </summary>
public class WebhookConfigTest
{
    /// <summary>
    /// JSONから<see cref="WebhookConfig"/>にデシリアライズできる。
    /// </summary>
    [Fact(DisplayName = $"{nameof(WebhookConfig)} > JSONからデシリアライズできる。")]
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
