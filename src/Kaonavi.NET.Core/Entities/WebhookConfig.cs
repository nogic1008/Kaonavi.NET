namespace Kaonavi.Net.Entities;

/// <summary>
/// <inheritdoc cref="WebhookConfig" path="/summary"/> 登録用Payload
/// </summary>
/// <param name="Url">
/// Webhookの送信先URL
/// <para>https:// から始まるURLのみ設定できます。</para>
/// </param>
/// <param name="Events">
/// Webhookを送信するイベント
/// <para>一つ以上のイベント名を重複なく指定してください。</para>
/// </param>
/// <param name="SecretToken">
/// 検証用トークン
/// <para>WebhookのHTTPリクエスト送信時にリクエストヘッダーのKaonavi-Tokenに入る値を指定できます。</para>
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
