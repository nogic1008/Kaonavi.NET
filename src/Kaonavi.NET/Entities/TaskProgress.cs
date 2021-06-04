using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Kaonavi.Net.Entities
{
    /// <summary>タスク進捗状況</summary>
    public record TaskProgress(
        /// <summary>タスクID</summary>
        [property: JsonPropertyName("id")] int Id,
        /// <summary>タスクの進捗状況 ("OK", "NG", "ERROR", "WAITING", "RUNNING")</summary>
        [property: JsonPropertyName("status")] string Status,
        /// <summary><see cref="Status"/>がNG/ERROR時の詳細なメッセージ</summary>
        [property: JsonPropertyName("messages")] IEnumerable<string>? Messages
    );
}
