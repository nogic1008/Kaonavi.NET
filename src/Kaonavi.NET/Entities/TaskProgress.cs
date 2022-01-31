namespace Kaonavi.Net.Entities;

/// <summary>タスク進捗状況</summary>
/// <param name="Id">タスクID</param>
/// <param name="Status">タスクの進捗状況 ("OK", "NG", "ERROR", "WAITING", "RUNNING")</param>
/// <param name="Messages"><see cref="Status"/>がNG/ERROR時の詳細なメッセージ</param>
public record TaskProgress(int Id, string Status, IReadOnlyList<string>? Messages = null);
