namespace Kaonavi.Net.Entities;

/// <summary>
/// Webhookで利用できるイベント
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter<WebhookEvent>))]
public enum WebhookEvent
{
    /// <summary>メンバーが新しく登録された</summary>
    [JsonStringEnumMemberName("member_created")] MemberCreated = 1,
    /// <summary>メンバーの基本情報が更新された</summary>
    [JsonStringEnumMemberName("member_updated")] MemberUpdated = 2,
    /// <summary>メンバーが削除された</summary>
    [JsonStringEnumMemberName("member_deleted")] MemberDeleted = 3,
}
