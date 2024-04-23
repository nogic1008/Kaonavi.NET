using Kaonavi.Net.Entities;

namespace Kaonavi.Net.Api;

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
