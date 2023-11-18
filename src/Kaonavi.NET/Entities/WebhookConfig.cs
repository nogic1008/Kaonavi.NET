using Kaonavi.Net.Json;

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
public record WebhookConfigPayload(Uri Url, IReadOnlyCollection<WebhookEvent> Events, string SecretToken);

/// <summary>
/// Webhook設定
/// </summary>
/// <param name="Id">Webhook ID</param>
/// <inheritdoc cref="WebhookConfigPayload" path="/param" />
public record WebhookConfig(int Id, Uri Url, IReadOnlyCollection<WebhookEvent> Events, string SecretToken)
    : WebhookConfigPayload(Url, Events, SecretToken);

/// <summary>
/// Webhookで利用できるイベント
/// </summary>
[JsonConverter(typeof(WebhookEventJsonConverter))]
public enum WebhookEvent
{
    /// <summary>メンバーが新しく登録された</summary>
    MemberCreated = 1,
    /// <summary>メンバーの基本情報が更新された</summary>
    MemberUpdated = 2,
    /// <summary>メンバーが削除された</summary>
    MemberDeleted = 3,
}
