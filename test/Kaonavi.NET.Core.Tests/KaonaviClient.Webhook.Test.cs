using Kaonavi.Net.Entities;
using Kaonavi.Net.Tests.Assertions;

namespace Kaonavi.Net.Tests;

public sealed partial class KaonaviClientTest
{
    /// <summary><see cref="KaonaviClient.Webhook"/>の単体テスト</summary>
    [Category("API"), Category("Webhook設定")]
    public sealed class WebhookTest
    {
        /// <summary>
        /// <see cref="KaonaviClient.Webhook.ListAsync"/>は、"/webhook"にGETリクエストを行う。
        /// </summary>
        /// <param name="cancellationToken"><inheritdoc cref="KaonaviClient.IWebhook.ListAsync" path="/param[@name='cancellationToken']"/></param>
        [Test($"{nameof(KaonaviClient.Webhook)}.{nameof(KaonaviClient.Webhook.ListAsync)} > GET /webhook をコールする。")]
        [Category(nameof(HttpMethod.Get))]
        public async ValueTask Webhook_ListAsync_Calls_GetApi(CancellationToken cancellationToken = default)
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
            using var client = Mock.HttpClient(BaseUriString);
            client.Handler.OnGet("/webhook").RespondWithJson(responseJson);

            // Act
            var sut = CreateSut(client, "token");
            var entities = await sut.Webhook.ListAsync(cancellationToken);

            // Assert
            await Assert.That(entities).Count().IsEqualTo(2);
            client.Handler.Verify(r => r.Method(HttpMethod.Get).Path("/webhook"), Times.Once);
        }

        /// <summary>
        /// <see cref="KaonaviClient.Webhook.CreateAsync"/>は、"/webhook"にPOSTリクエストを行う。
        /// </summary>
        /// <param name="cancellationToken"><inheritdoc cref="KaonaviClient.IWebhook.CreateAsync" path="/param[@name='cancellationToken']"/></param>
        [Test($"{nameof(KaonaviClient.Webhook)}.{nameof(KaonaviClient.Webhook.CreateAsync)} > POST /webhook をコールする。")]
        [Category(nameof(HttpMethod.Post))]
        public async ValueTask Webhook_CreateAsync_Calls_PostApi(CancellationToken cancellationToken = default)
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

            using var client = Mock.HttpClient(BaseUriString);
            client.Handler.OnPost("/webhook").RespondWithJson(responseJson);

            // Act
            var sut = CreateSut(client, "token");
            var webhook = await sut.Webhook.CreateAsync(payload, cancellationToken);

            // Assert
            await Assert.That(webhook).IsNotNull();
            client.Handler.Verify(r => r.Method(HttpMethod.Post).Path("/webhook"), Times.Once);
            await Assert.That(client.Handler.Requests[0].Body).IsValidJsonObject()
                .And.IsJsonEquals("""
                {
                  "url": "https://example.com/",
                  "events": ["member_created", "member_updated", "member_deleted"],
                  "secret_token": "token"
                }
                """u8);
        }

        /// <summary>
        /// <see cref="KaonaviClient.Webhook.UpdateAsync"/>は、"/webhook/{webhookId}"にPATCHリクエストを行う。
        /// </summary>
        /// <param name="cancellationToken"><inheritdoc cref="KaonaviClient.IWebhook.UpdateAsync" path="/param[@name='cancellationToken']"/></param>
        [Test($"{nameof(KaonaviClient.Webhook)}.{nameof(KaonaviClient.Webhook.UpdateAsync)} > PATCH /webhook/:webhookId をコールする。")]
        [Category(nameof(HttpMethod.Patch))]
        public async Task Webhook_UpdateAsync_Calls_PatchApi(CancellationToken cancellationToken = default)
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

            using var client = Mock.HttpClient(BaseUriString);
            client.Handler.OnRequest(req => req.Method(HttpMethod.Patch).Path($"/webhook/{webhookId}")).RespondWithJson(responseJson);

            // Act
            var sut = CreateSut(client, "token");
            var webhook = await sut.Webhook.UpdateAsync(payload, cancellationToken);

            // Assert
            await Assert.That(webhook).IsNotNull();
            client.Handler.Verify(r => r.Method(HttpMethod.Patch).Path($"/webhook/{webhookId}"), Times.Once);
            await Assert.That(client.Handler.Requests[0].Body).IsValidJsonObject().And.IsJsonEquals("""
            {
              "id": 1,
              "url": "https://example.com/",
              "events": ["member_created", "member_updated", "member_deleted"],
              "secret_token": "token"
            }
            """u8);
        }

        /// <summary>
        /// <inheritdoc cref="KaonaviClient.IWebhook.DeleteAsync" path="/param[@name='id']"/>が<c>0</c>未満のとき、
        /// <see cref="KaonaviClient.Webhook.DeleteAsync"/>は<see cref="ArgumentOutOfRangeException"/>をスローする。
        /// </summary>
        /// <param name="cancellationToken"><inheritdoc cref="KaonaviClient.IWebhook.DeleteAsync" path="/param[@name='cancellationToken']"/></param>
        [Test($"{nameof(KaonaviClient.Webhook)}.{nameof(KaonaviClient.Webhook.DeleteAsync)} > ArgumentOutOfRangeExceptionをスローする。")]
        [Category(nameof(HttpMethod.Delete))]
        public async Task When_Id_IsNegative_Webhook_DeleteAsync_Throws_ArgumentOutOfRangeException(CancellationToken cancellationToken = default)
        {
            // Arrange
            using var client = Mock.HttpClient(BaseUriString);
            client.Handler.OnAnyRequest().Respond(HttpStatusCode.OK);

            // Act - Assert
            var sut = CreateSut(client);
            await Assert.That(async () => await sut.Webhook.DeleteAsync(-1, cancellationToken))
                .Throws<ArgumentOutOfRangeException>().WithParameterName("id");
            await Assert.That(client.Handler.Requests).IsEmpty();
        }

        /// <summary>
        /// <see cref="KaonaviClient.Webhook.DeleteAsync"/>は、"/webhook/{webhookId}"にDELETEリクエストを行う。
        /// </summary>
        /// <param name="cancellationToken"><inheritdoc cref="KaonaviClient.IWebhook.DeleteAsync" path="/param[@name='cancellationToken']"/></param>
        [Test($"{nameof(KaonaviClient.Webhook)}.{nameof(KaonaviClient.Webhook.DeleteAsync)} > DELETE /webhook/:webhookId をコールする。")]
        [Category(nameof(HttpMethod.Delete))]
        public async Task Webhook_DeleteAsync_Calls_DeleteApi(CancellationToken cancellationToken = default)
        {
            // Arrange
            const int webhookId = 1;

            using var client = Mock.HttpClient(BaseUriString);
            client.Handler.OnDelete($"/webhook/{webhookId}").Respond(HttpStatusCode.NoContent);

            // Act
            var sut = CreateSut(client, "token");
            await sut.Webhook.DeleteAsync(webhookId, cancellationToken);

            // Assert
            client.Handler.Verify(r => r.Method(HttpMethod.Delete).Path($"/webhook/{webhookId}"), Times.Once);
        }
    }
}
