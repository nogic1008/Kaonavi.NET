using Kaonavi.Net.Entities;
using Moq;
using Moq.Contrib.HttpClient;

namespace Kaonavi.Net.Tests;

public sealed partial class KaonaviClientTest
{
    /// <summary><see cref="KaonaviClient.Webhook"/>の単体テスト</summary>
    [TestClass]
    public class WebhookTest
    {
        /// <summary>
        /// <see cref="KaonaviClient.Webhook.ListAsync"/>は、"/webhook"にGETリクエストを行う。
        /// </summary>
        [TestMethod($"{nameof(KaonaviClient.Webhook)}.{nameof(KaonaviClient.Webhook.ListAsync)} > GET /webhook をコールする。")]
        [TestCategory("API"), TestCategory(nameof(HttpMethod.Get)), TestCategory("Webhook設定")]
        public async Task Webhook_ListAsync_Calls_GetApi()
        {
            // Arrange
            /*lang=json,strict*/
            const string responseJson = """
            {
              "webhook_data": [
                {
                  "id": 1,
                  "url": "https://example.com/",
                  "events": ["member_created","member_deleted"],
                  "secret_token": "string",
                  "updated_at": "2021-12-01 12:00:00",
                  "created_at": "2021-11-01 12:00:00"
                },
                {
                  "id": 2,
                  "url": "https://example.com/",
                  "events": ["member_updated"],
                  "secret_token": "string",
                  "updated_at": "2021-12-01 12:00:00",
                  "created_at": "2021-11-01 12:00:00"
                }
              ]
            }
            """;
            string tokenString = GenerateRandomString();

            var handler = new Mock<HttpMessageHandler>();
            _ = handler.SetupRequest(req => req.RequestUri?.PathAndQuery == "/webhook")
                .ReturnsResponse(HttpStatusCode.OK, responseJson, "application/json");

            // Act
            var sut = CreateSut(handler, accessToken: tokenString);
            var entities = await sut.Webhook.ListAsync();

            // Assert
            _ = entities.Should().HaveCount(2);

            handler.VerifyRequest(req =>
            {
                _ = req.Should().SendTo(HttpMethod.Get, "/webhook")
                    .And.HasToken(tokenString);
                return true;
            }, Times.Once());
        }

        /// <summary>
        /// <see cref="KaonaviClient.Webhook.CreateAsync"/>は、"/webhook"にPOSTリクエストを行う。
        /// </summary>
        [TestMethod($"{nameof(KaonaviClient.Webhook)}.{nameof(KaonaviClient.Webhook.CreateAsync)} > POST /webhook をコールする。")]
        [TestCategory("API"), TestCategory(nameof(HttpMethod.Post)), TestCategory("Webhook設定")]
        public async Task Webhook_CreateAsync_Calls_PostApi()
        {
            // Arrange
            string tokenString = GenerateRandomString();
            /*lang=json,strict*/
            const string responseJson = """
            {
              "id": 1,
              "url": "https://example.com/",
              "events": [
                "member_created",
                "member_updated",
                "member_deleted"
              ],
              "secret_token": "token"
            }
            """;
            var payload = new WebhookConfigPayload(_baseUri, [WebhookEvent.MemberCreated, WebhookEvent.MemberUpdated, WebhookEvent.MemberDeleted], "token");
            /*lang=json,strict*/
            const string expectedJson = """
            {"url":"https://example.com/","events":["member_created","member_updated","member_deleted"],"secret_token":"token"}
            """;

            var handler = new Mock<HttpMessageHandler>();
            _ = handler.SetupRequest(req => req.RequestUri?.PathAndQuery == "/webhook")
                .ReturnsResponse(HttpStatusCode.OK, responseJson, "application/json");

            // Act
            var sut = CreateSut(handler, accessToken: tokenString);
            var webhook = await sut.Webhook.CreateAsync(payload);

            // Assert
            _ = webhook.Should().NotBeNull();

            handler.VerifyRequest(async req =>
            {
                _ = req.Should().SendTo(HttpMethod.Post, "/webhook")
                    .And.HasToken(tokenString);

                // Body
                string receivedJson = await req.Content!.ReadAsStringAsync();
                _ = receivedJson.Should().Be(expectedJson);

                return true;
            }, Times.Once());
        }

        /// <summary>
        /// <see cref="KaonaviClient.Webhook.UpdateAsync"/>は、"/webhook/{webhookId}"にPATCHリクエストを行う。
        /// </summary>
        [TestMethod($"{nameof(KaonaviClient.Webhook)}.{nameof(KaonaviClient.Webhook.UpdateAsync)} > PATCH /webhook/:webhookId をコールする。")]
        [TestCategory("API"), TestCategory(nameof(HttpMethod.Patch)), TestCategory("Webhook設定")]
        public async Task Webhook_UpdateAsync_Calls_PatchApi()
        {
            // Arrange
            const int webhookId = 1;
            /*lang=json,strict*/
            const string responseJson = """
            {
              "id": 1,
              "url": "https://example.com/",
              "events": [
                "member_created",
                "member_updated",
                "member_deleted"
              ],
              "secret_token": "token"
            }
            """;
            string tokenString = GenerateRandomString();
            var payload = new WebhookConfig(webhookId, _baseUri, [WebhookEvent.MemberCreated, WebhookEvent.MemberUpdated, WebhookEvent.MemberDeleted], "token");
            /*lang=json,strict*/
            const string expectedJson = """
            {"id":1,"url":"https://example.com/","events":["member_created","member_updated","member_deleted"],"secret_token":"token"}
            """;

            var handler = new Mock<HttpMessageHandler>();
            _ = handler.SetupRequest(req => req.RequestUri?.PathAndQuery == $"/webhook/{webhookId}")
                .ReturnsResponse(HttpStatusCode.OK, responseJson, "application/json");

            // Act
            var sut = CreateSut(handler, accessToken: tokenString);
            var webhook = await sut.Webhook.UpdateAsync(payload);

            // Assert
            _ = webhook.Should().NotBeNull();

            handler.VerifyRequest(async req =>
            {
                _ = req.Should().SendTo(HttpMethod.Patch, $"/webhook/{webhookId}")
                    .And.HasToken(tokenString);

                // Body
                string receivedJson = await req.Content!.ReadAsStringAsync();
                _ = receivedJson.Should().Be(expectedJson);

                return true;
            }, Times.Once());
        }

        /// <summary>
        /// <inheritdoc cref="KaonaviClient.IWebhook.DeleteAsync" path="/param[@name='id']"/>が<c>0</c>未満のとき、
        /// <see cref="KaonaviClient.Webhook.DeleteAsync"/>は<see cref="ArgumentOutOfRangeException"/>をスローする。
        /// </summary>
        [TestMethod($"{nameof(KaonaviClient.Webhook)}.{nameof(KaonaviClient.Webhook.DeleteAsync)} > ArgumentOutOfRangeExceptionをスローする。")]
        [TestCategory("API"), TestCategory(nameof(HttpMethod.Delete)), TestCategory("Webhook設定")]
        public async Task When_Id_IsNegative_Webhook_DeleteAsync_Throws_ArgumentOutOfRangeException()
        {
            // Arrange
            var handler = new Mock<HttpMessageHandler>();
            _ = handler.SetupAnyRequest().ReturnsResponse(HttpStatusCode.OK);

            // Act
            var sut = CreateSut(handler);
            var act = async () => await sut.Webhook.DeleteAsync(-1);

            // Assert
            _ = await act.Should().ThrowExactlyAsync<ArgumentOutOfRangeException>()
                .WithParameterName("id");

            handler.VerifyAnyRequest(Times.Never());
        }

        /// <summary>
        /// <see cref="KaonaviClient.Webhook.DeleteAsync"/>は、"/webhook/{webhookId}"にDELETEリクエストを行う。
        /// </summary>
        [TestMethod($"{nameof(KaonaviClient.Webhook)}.{nameof(KaonaviClient.Webhook.DeleteAsync)} > DELETE /webhook/:webhookId をコールする。")]
        [TestCategory("API"), TestCategory(nameof(HttpMethod.Delete)), TestCategory("Webhook設定")]
        public async Task Webhook_DeleteAsync_Calls_DeleteApi()
        {
            // Arrange
            const int webhookId = 1;
            string tokenString = GenerateRandomString();

            var handler = new Mock<HttpMessageHandler>();
            _ = handler.SetupRequest(req => req.RequestUri?.PathAndQuery == $"/webhook/{webhookId}")
                .ReturnsResponse(HttpStatusCode.NoContent);

            // Act
            var sut = CreateSut(handler, accessToken: tokenString);
            await sut.Webhook.DeleteAsync(webhookId);

            // Assert
            handler.VerifyRequest(req =>
            {
                _ = req.Should().SendTo(HttpMethod.Delete, $"/webhook/{webhookId}")
                    .And.HasToken(tokenString);
                return true;
            }, Times.Once());
        }
    }
}
