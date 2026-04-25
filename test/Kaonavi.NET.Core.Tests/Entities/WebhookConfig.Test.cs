using System.Text.Json;
using Kaonavi.Net.Entities;
using JsonContext = Kaonavi.Net.Json.Context;

namespace Kaonavi.Net.Tests.Entities;

/// <summary><see cref="WebhookConfig"/>の単体テスト</summary>
[Category("Entities")]
public sealed class WebhookConfigTest
{
    /// <summary>
    /// JSONから<see cref="WebhookConfig"/>にデシリアライズできる。
    /// </summary>
    [Test($"{nameof(WebhookConfig)} > JSONからデシリアライズできる。")]
    [Category("JSON Deserialize")]
    public async Task WebhookConfig_CanDeserializeJSON()
    {
        // Arrange
        /*lang=json,strict*/
        var json = """
        {
            "id": 1,
            "url": "https://example.com",
            "events": ["member_created", "member_updated", "member_deleted"],
            "secret_token": "string"
        }
        """u8;

        // Act
        var webhookConfig = JsonSerializer.Deserialize(json, JsonContext.Default.WebhookConfig);

        // Assert
        await Assert.That(webhookConfig).IsNotNull()
            .And.Member(static o => o.Id, static o => o.IsEqualTo(1))
            .And.Member(static o => o.Url, static o => o.IsEqualTo(new("https://example.com")))
            .And.Member(static o => o.Events, static o => o.IsEquivalentTo([WebhookEvent.MemberCreated, WebhookEvent.MemberUpdated, WebhookEvent.MemberDeleted]))
            .And.Member(static o => o.SecretToken, static o => o.IsEqualTo<string>("string"));
    }
}
