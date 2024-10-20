using Kaonavi.Net.Json;

namespace Kaonavi.Net.Entities;

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
