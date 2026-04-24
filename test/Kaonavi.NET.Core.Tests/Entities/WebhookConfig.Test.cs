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
    [TestMethod(DisplayName = $"{nameof(WebhookConfig)} > JSONからデシリアライズできる。"), TestCategory("JSON Deserialize")]
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
        var webhookConfig = JsonSerializer.Deserialize(json, Context.Default.WebhookConfig);

        // Assert
        webhookConfig.ShouldNotBeNull().ShouldSatisfyAllConditions(
            static sut => sut.Id.ShouldBe(1),
            static sut => sut.Url.ShouldBe(new Uri("https://example.com")),
            static sut => sut.Events.ShouldBe([WebhookEvent.MemberCreated, WebhookEvent.MemberUpdated, WebhookEvent.MemberDeleted]),
            static sut => sut.SecretToken.ShouldBe("string")
        );
    }
}
