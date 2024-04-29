using System.Text.Json;
using System.Text.Json.Serialization;

namespace Kaonavi.Net.Server.Tests;

[TestCategory("Server"), TestCategory("Entities")]
[TestClass]
public sealed class KaonaviWebhookTest
{
    [TestMethod($"{nameof(KaonaviWebhook)} > JSONからデシリアライズできる。"), TestCategory("JSON Deserialize")]
    public void Can_Deserialize_JSON()
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
        actual.Should().NotBeNull();
        actual!.Event.Should().Be(WebhookEvent.MemberCreated);
        actual.EventTime.Should().Be(new DateTime(2023, 4, 11, 9, 40, 45));
        actual.MemberData.Should().Equal(new Member("A0001"), new Member("A0002"));
    }
}

[JsonSerializable(typeof(KaonaviWebhook))]
public partial class Context : JsonSerializerContext;
