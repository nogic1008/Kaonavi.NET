using System.Text.Json;
using Kaonavi.Net.Entities;

namespace Kaonavi.Net.Server.Tests;

[TestCategory("Server"), TestCategory("Entities")]
[TestClass]
public sealed class KaonaviWebhookTest
{
    [TestMethod(DisplayName = $"{nameof(KaonaviWebhook)} > JSONからデシリアライズできる。"), TestCategory("JSON Deserialize")]
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
        actual.ShouldNotBeNull().ShouldSatisfyAllConditions(
            static sut => sut.Event.ShouldBe(WebhookEvent.MemberCreated),
            static sut => sut.EventTime.ShouldBe(new DateTime(2023, 4, 11, 9, 40, 45)),
            static sut => sut.MemberData.ShouldBe([new("A0001"), new("A0002")])
        );
    }
}
