using System.Diagnostics.CodeAnalysis;

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

/// <summary>
/// <see cref="JsonConverter"/> for <see cref="WebhookEvent"/>
/// </summary>
[ExcludeFromCodeCoverage]
internal class WebhookEventJsonConverter : JsonConverter<WebhookEvent>
{
    private const string MemberCreatedValue = "member_created";
    private const string MemberUpdatedValue = "member_updated";
    private const string MemberDeletedValue = "member_deleted";

    public override WebhookEvent Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => reader.GetString() switch
        {
            MemberCreatedValue => WebhookEvent.MemberCreated,
            MemberUpdatedValue => WebhookEvent.MemberUpdated,
            MemberDeletedValue => WebhookEvent.MemberDeleted,
            _ => throw new JsonException(),
        };

    public override void Write(Utf8JsonWriter writer, WebhookEvent value, JsonSerializerOptions options)
    {
        switch (value)
        {
            case WebhookEvent.MemberCreated:
                writer.WriteStringValue(MemberCreatedValue);
                break;
            case WebhookEvent.MemberUpdated:
                writer.WriteStringValue(MemberUpdatedValue);
                break;
            case WebhookEvent.MemberDeleted:
                writer.WriteStringValue(MemberDeletedValue);
                break;
            default:
                throw new JsonException();
        }
    }
}
