namespace Kaonavi.Net.Entities;

/// <summary>タスクの進捗状況</summary>
[JsonConverter(typeof(JsonStringEnumConverter<TaskState>))]
public enum TaskState
{
    /// <summary>正常終了</summary>
    [JsonStringEnumMemberName("OK")] OK,
    /// <summary>バリデーションエラー</summary>
    /// <remarks>
    /// <see cref="TaskProgress.Id"/>が返った時点ではリクエストに問題がないが、非同期のタスク実行中に問題があった場合のエラー
    /// <para>例) 「シート情報 一括更新」のタスク実行時に対象のシートが存在しない</para>
    /// </remarks>
    [JsonStringEnumMemberName("NG")] NG,
    /// <summary>サーバーで発生した予期せぬエラー</summary>
    [JsonStringEnumMemberName("ERROR")] Error,
    /// <summary>実行待ち</summary>
    [JsonStringEnumMemberName("WAITING")] Waiting,
    /// <summary>実行中</summary>
    [JsonStringEnumMemberName("RUNNING")] Running,
}
