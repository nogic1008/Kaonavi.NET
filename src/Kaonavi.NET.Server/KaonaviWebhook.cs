using Kaonavi.Net.Entities;
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
    [property: JsonConverter(typeof(DateTimeConverter)), JsonPropertyName("event_time")] DateTime EventTime,
    [property: JsonPropertyName("member_data")] IReadOnlyList<Member> MemberData
);

/// <summary>
/// 変更のあったメンバーの情報
/// </summary>
/// <param name="Code">社員番号</param>
public record Member([property: JsonPropertyName("code")] string Code);
