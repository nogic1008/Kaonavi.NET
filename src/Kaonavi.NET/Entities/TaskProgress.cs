using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Kaonavi.Net.Entities
{
    /// <summary>タスク進捗状況</summary>
    public record TaskProgress
    {
        /// <summary>
        /// TaskProgressの新しいインスタンスを生成します。
        /// </summary>
        /// <param name="id">タスクID</param>
        /// <param name="status">タスクの進捗状況 ("OK", "NG", "ERROR", "WAITING", "RUNNING")</param>
        /// <param name="messages"><paramref name="status"/>がNG/ERROR時の詳細なメッセージ</param>
        public TaskProgress(int id, string status, IReadOnlyList<string>? messages)
            => (Id, Status, Messages) = (id, status, messages);

        /// <summary>タスクID</summary>
        [JsonPropertyName("id")]
        public int Id { get; init; }

        /// <summary>タスクの進捗状況 ("OK", "NG", "ERROR", "WAITING", "RUNNING")</summary>
        [JsonPropertyName("status")]
        public string Status { get; init; }

        /// <summary><see cref="Status"/>がNG/ERROR時の詳細なメッセージ</summary>
        [JsonPropertyName("messages")]
        public IReadOnlyList<string>? Messages { get; init; }
    }
}
