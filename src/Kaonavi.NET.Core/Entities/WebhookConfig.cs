namespace Kaonavi.Net.Entities;

/// <summary>
/// <inheritdoc cref="WebhookConfig" path="/summary"/> 登録用Payload
/// </summary>
/// <param name="Url">
/// Webhookの送信先URL
/// https:// から始まるURLのみ設定できます。
/// </param>
/// <param name="Events">Webhookを送信するイベント</param>
/// <param name="SecretToken">
/// 検証用トークン
/// WebhookのHTTPリクエスト送信時にリクエストヘッダーのKaonavi-Tokenに入る値を指定できます。
/// </param>
public record WebhookConfigPayload(Uri Url, IReadOnlyList<WebhookEvent> Events, string SecretToken);

/// <summary>
/// Webhook設定
/// </summary>
/// <param name="Id">Webhook ID</param>
/// <param name="Url"><inheritdoc cref="WebhookConfigPayload" path="/param[@name='Url']"/></param>
/// <param name="Events"><inheritdoc cref="WebhookConfigPayload" path="/param[@name='Events']"/></param>
/// <param name="SecretToken"><inheritdoc cref="WebhookConfigPayload" path="/param[@name='SecretToken']"/></param>
public record WebhookConfig(int Id, Uri Url, IReadOnlyList<WebhookEvent> Events, string SecretToken)
    : WebhookConfigPayload(Url, Events, SecretToken);
