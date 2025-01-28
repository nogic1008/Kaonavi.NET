using System.Net.Http.Json;
using Kaonavi.Net.Entities;
using Kaonavi.Net.Json;

namespace Kaonavi.Net;

public partial class KaonaviClient : KaonaviClient.IWebhook
{
    /// <summary>
    /// Webhook設定 API
    /// <see href="https://developer.kaonavi.jp/api/v2.0/index.html#tag/Webhook"/>
    /// </summary>
    public interface IWebhook
    {
        /// <summary>
        /// 登録した<inheritdoc cref="WebhookConfig" path="/summary"/>の一覧を取得します。
        /// <see href="https://developer.kaonavi.jp/api/v2.0/index.html#tag/Webhook/paths/~1webhook/get"/>
        /// </summary>
        /// <param name="cancellationToken"><inheritdoc cref="HttpClient.SendAsync(HttpRequestMessage, CancellationToken)" path="/param[@name='cancellationToken']"/></param>
        ValueTask<IReadOnlyList<WebhookConfig>> ListAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// <inheritdoc cref="WebhookConfig" path="/summary"/>を登録します。
        /// <see href="https://developer.kaonavi.jp/api/v2.0/index.html#tag/Webhook/paths/~1webhook/post"/>
        /// </summary>
        /// <param name="payload">リクエスト</param>
        /// <param name="cancellationToken"><inheritdoc cref="HttpClient.SendAsync(HttpRequestMessage, CancellationToken)" path="/param[@name='cancellationToken']"/></param>
        ValueTask<WebhookConfig> CreateAsync(WebhookConfigPayload payload, CancellationToken cancellationToken = default);

        /// <summary>
        /// <see cref="WebhookConfig.Id"/>と一致する<inheritdoc cref="WebhookConfig" path="/summary"/>情報を更新します。
        /// <see href="https://developer.kaonavi.jp/api/v2.0/index.html#tag/Webhook/paths/~1webhook~1{webhook_id}/patch"/>
        /// </summary>
        /// <param name="payload">リクエスト</param>
        /// <param name="cancellationToken"><inheritdoc cref="HttpClient.SendAsync(HttpRequestMessage, CancellationToken)" path="/param[@name='cancellationToken']"/></param>
        ValueTask<WebhookConfig> UpdateAsync(WebhookConfig payload, CancellationToken cancellationToken = default);

        /// <summary>
        /// <paramref name="id"/>と一致する<inheritdoc cref="WebhookConfig" path="/summary"/>を削除します。
        /// <see href="https://developer.kaonavi.jp/api/v2.0/index.html#tag/Webhook/paths/~1webhook~1{webhook_id}/delete"/>
        /// </summary>
        /// <param name="id"><inheritdoc cref="WebhookConfig" path="/param[@name='Id']"/></param>
        /// <param name="cancellationToken"><inheritdoc cref="HttpClient.SendAsync(HttpRequestMessage, CancellationToken)" path="/param[@name='cancellationToken']"/></param>
        ValueTask DeleteAsync(int id, CancellationToken cancellationToken = default);
    }

    /// <inheritdoc/>
    public IWebhook Webhook => this;

    /// <inheritdoc/>
    ValueTask<IReadOnlyList<WebhookConfig>> IWebhook.ListAsync(CancellationToken cancellationToken)
        => CallApiAsync(new(HttpMethod.Get, "webhook"), "webhook_data", Context.Default.IReadOnlyListWebhookConfig, cancellationToken);

    /// <inheritdoc/>
    ValueTask<WebhookConfig> IWebhook.CreateAsync(WebhookConfigPayload payload, CancellationToken cancellationToken)
        => CallApiAsync(new(HttpMethod.Post, "webhook")
        {
            Content = JsonContent.Create(payload, Context.Default.WebhookConfigPayload)
        }, Context.Default.WebhookConfig, cancellationToken);

    /// <inheritdoc/>
    ValueTask<WebhookConfig> IWebhook.UpdateAsync(WebhookConfig payload, CancellationToken cancellationToken)
        => CallApiAsync(new(HttpMethod.Patch, $"webhook/{payload.Id}")
        {
            Content = JsonContent.Create(payload, Context.Default.WebhookConfig)
        }, Context.Default.WebhookConfig, cancellationToken);

    /// <inheritdoc/>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="id"/>が0より小さい場合にスローされます。</exception>
    async ValueTask IWebhook.DeleteAsync(int id, CancellationToken cancellationToken)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(id);
        await CallApiAsync(new(HttpMethod.Delete, $"webhook/{id:D}"), cancellationToken).ConfigureAwait(false);
    }
}
