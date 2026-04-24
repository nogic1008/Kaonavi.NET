using System.Text.Json;
using Kaonavi.Net.Entities;

namespace Kaonavi.Net.Server.Tests;

[Category("Server"), Category("Entities")]
public sealed class KaonaviWebhookTest
{
    [Test($"{nameof(KaonaviWebhook)} > JSONからデシリアライズできる。"), Category("JSON Deserialize")]
    public async Task Can_Deserialize_JSON()
    {
        // Arrange
        const string json = /*lang=json,strict*/ """
        {
          "event": "member_created",
          "event_time": "2023-04-11 09:40:45",
          "member_data": [
            {
              "code": "A0001"
            },
            {
              "code": "A0002"
            }
          ]
        }
        """;

        // Act
        var actual = JsonSerializer.Deserialize(json, Context.Default.KaonaviWebhook);

        // Assert
        await Assert.That(actual)
            .Member(static o => o.Event, static o => o.IsEqualTo(WebhookEvent.MemberCreated))
            .And.Member(static o => o.EventTime, static o => o.IsEqualTo(new DateTime(2023, 4, 11, 9, 40, 45)))
            .And.Member(static o => o.MemberData, static o => o.IsEquivalentTo((Member[])[new("A0001"), new("A0002")]));
    }
}
