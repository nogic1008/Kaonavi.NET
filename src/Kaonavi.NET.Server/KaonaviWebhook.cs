using Kaonavi.Net.Json;

namespace Kaonavi.Net.Server;

/// <summary>
/// kaonavi Webhookが送信するリクエストボディ
/// </summary>
/// <param name="Event">発生したイベント</param>
/// <param name="EventTime">イベントが発生した時刻</param>
/// <param name="MemberData">変更のあったメンバー</param>
public record KaonaviWebhook(
    [property: JsonPropertyName("event")] WebhookEvent Event,
    [property: JsonConverter(typeof(DateTimeConverter))][property: JsonPropertyName("event_time")] DateTime EventTime,
    [property: JsonPropertyName("member_data")] IReadOnlyList<Member> MemberData
);

/// <summary>
/// 変更のあったメンバーの情報
/// </summary>
/// <param name="Code">社員番号</param>
public record Member([property: JsonPropertyName("code")] string Code);

/// <summary>
/// Webhookの発生元イベント
/// </summary>
[JsonConverter(typeof(WebhookEventJsonConverter))]
public enum WebhookEvent
{
    /// <summary>メンバーが新しく登録された</summary>
    MemberCreated,
    /// <summary>メンバーの基本情報が更新された</summary>
    MemberUpdated,
    /// <summary>メンバーが削除された</summary>
    MemberDeleted,
}

/// <summary>
/// <see cref="JsonConverter"/> for <see cref="WebhookEvent"/>
/// </summary>
public sealed class WebhookEventJsonConverter : JsonConverter<WebhookEvent>
{
    private static ReadOnlySpan<byte> MemberCreated => "member_created"u8;
    private static ReadOnlySpan<byte> MemberUpdated => "member_updated"u8;
    private static ReadOnlySpan<byte> MemberDeleted => "member_deleted"u8;

    /// <inheritdoc/>
    public override WebhookEvent Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        => reader.ValueSpan.SequenceEqual(MemberCreated) ? WebhookEvent.MemberCreated
        : reader.ValueSpan.SequenceEqual(MemberUpdated) ? WebhookEvent.MemberUpdated
        : reader.ValueSpan.SequenceEqual(MemberDeleted) ? WebhookEvent.MemberDeleted
        : throw new JsonException();

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, WebhookEvent value, JsonSerializerOptions options)
        => writer.WriteStringValue(value switch
        {
            WebhookEvent.MemberCreated => MemberCreated,
            WebhookEvent.MemberUpdated => MemberUpdated,
            WebhookEvent.MemberDeleted => MemberDeleted,
            _ => throw new JsonException(),
        });
}
