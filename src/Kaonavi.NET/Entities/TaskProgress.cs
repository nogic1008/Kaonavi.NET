namespace Kaonavi.Net.Entities;

using System.Collections.Generic;
using System.Text.Json.Serialization;

/// <summary>タスク進捗状況</summary>
/// <param name="Id">タスクID</param>
/// <param name="Status">タスクの進捗状況 ("OK", "NG", "ERROR", "WAITING", "RUNNING")</param>
/// <param name="Messages"><see cref="Status"/>がNG/ERROR時の詳細なメッセージ</param>
public record TaskProgress(
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("messages")] IReadOnlyList<string>? Messages = null
);
