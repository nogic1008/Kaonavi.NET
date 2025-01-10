namespace Kaonavi.Net.Entities;

/// <summary>タスク進捗状況</summary>
/// <param name="Id">タスクID</param>
/// <param name="Status">タスクの進捗状況</param>
/// <param name="Messages"><see cref="Status"/>が<see cref="TaskState.NG"/>, <see cref="TaskState.Error"/>時の詳細なメッセージ</param>
public record TaskProgress(int Id, TaskState Status, IReadOnlyList<string>? Messages = null);
