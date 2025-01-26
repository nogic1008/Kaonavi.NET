using Kaonavi.Net.Entities;
using Kaonavi.Net.Tests.Assertions;
using Moq;
using Moq.Contrib.HttpClient;

namespace Kaonavi.Net.Tests;

public sealed partial class KaonaviClientTest
{
    /// <summary><see cref="KaonaviClient.Webhook"/>の単体テスト</summary>
    [TestClass]
    public sealed class WebhookTest
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
            var mockedApi = new Mock<HttpMessageHandler>();
            _ = mockedApi.SetupRequest(req => req.RequestUri?.PathAndQuery == "/webhook")
                .ReturnsResponse(HttpStatusCode.OK, responseJson, "application/json");

            // Act
            var sut = CreateSut(mockedApi, "token");
            var entities = await sut.Webhook.ListAsync();

            // Assert
            entities.ShouldNotBeEmpty();
            mockedApi.ShouldBeCalledOnce(
                static req => req.Method.ShouldBe(HttpMethod.Get),
                static req => req.RequestUri?.PathAndQuery.ShouldBe("/webhook")
            );
        }

        /// <summary>
        /// <see cref="KaonaviClient.Webhook.CreateAsync"/>は、"/webhook"にPOSTリクエストを行う。
        /// </summary>
        [TestMethod($"{nameof(KaonaviClient.Webhook)}.{nameof(KaonaviClient.Webhook.CreateAsync)} > POST /webhook をコールする。")]
        [TestCategory("API"), TestCategory(nameof(HttpMethod.Post)), TestCategory("Webhook設定")]
        public async Task Webhook_CreateAsync_Calls_PostApi()
        {
            // Arrange
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

            var mockedApi = new Mock<HttpMessageHandler>();
            _ = mockedApi.SetupRequest(req => req.RequestUri?.PathAndQuery == "/webhook")
                .ReturnsResponse(HttpStatusCode.OK, responseJson, "application/json");

            // Act
            var sut = CreateSut(mockedApi, "token");
            var webhook = await sut.Webhook.CreateAsync(payload);

            // Assert
            webhook.ShouldNotBeNull();
            mockedApi.ShouldBeCalledOnce(
                static req => req.Method.ShouldBe(HttpMethod.Post),
                static req => req.RequestUri?.PathAndQuery.ShouldBe("/webhook"),
                static req => req.Content!.ShouldHaveJsonBody("""
                {
                  "url": "https://example.com/",
                  "events": ["member_created", "member_updated", "member_deleted"],
                  "secret_token": "token"
                }
                """)
            );
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
            var payload = new WebhookConfig(webhookId, _baseUri, [WebhookEvent.MemberCreated, WebhookEvent.MemberUpdated, WebhookEvent.MemberDeleted], "token");

            var mockedApi = new Mock<HttpMessageHandler>();
            _ = mockedApi.SetupRequest(req => req.RequestUri?.PathAndQuery == $"/webhook/{webhookId}")
                .ReturnsResponse(HttpStatusCode.OK, responseJson, "application/json");

            // Act
            var sut = CreateSut(mockedApi, "token");
            var webhook = await sut.Webhook.UpdateAsync(payload);

            // Assert
            webhook.ShouldNotBeNull();
            mockedApi.ShouldBeCalledOnce(
                static req => req.Method.ShouldBe(HttpMethod.Patch),
                static req => req.RequestUri?.PathAndQuery.ShouldBe($"/webhook/{webhookId}"),
                static req => req.Content!.ShouldHaveJsonBody("""
                {
                  "id": 1,
                  "url": "https://example.com/",
                  "events": ["member_created", "member_updated", "member_deleted"],
                  "secret_token": "token"
                }
                """)
            );
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
            var mockedApi = new Mock<HttpMessageHandler>();
            _ = mockedApi.SetupAnyRequest().ReturnsResponse(HttpStatusCode.OK);

            // Act
            var sut = CreateSut(mockedApi);
            var act = async () => await sut.Webhook.DeleteAsync(-1);

            // Assert
            (await act.ShouldThrowAsync<ArgumentOutOfRangeException>()).ParamName.ShouldBe("id");
            mockedApi.ShouldNotBeCalled();
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

            var mockedApi = new Mock<HttpMessageHandler>();
            _ = mockedApi.SetupRequest(req => req.RequestUri?.PathAndQuery == $"/webhook/{webhookId}")
                .ReturnsResponse(HttpStatusCode.NoContent);

            // Act
            var sut = CreateSut(mockedApi, "token");
            await sut.Webhook.DeleteAsync(webhookId);

            // Assert
            mockedApi.ShouldBeCalledOnce(
                static req => req.Method.ShouldBe(HttpMethod.Delete),
                static req => req.RequestUri?.PathAndQuery.ShouldBe($"/webhook/{webhookId}")
            );
        }
    }
}
