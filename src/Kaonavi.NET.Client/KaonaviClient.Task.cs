using Kaonavi.Net.Entities;
using Kaonavi.Net.Json;

namespace Kaonavi.Net;

public partial class KaonaviClient : KaonaviClient.ITask
{
    /// <summary>
    /// タスク進捗状況 API
    /// <see href="https://developer.kaonavi.jp/api/v2.0/index.html#tag/%E3%82%BF%E3%82%B9%E3%82%AF%E9%80%B2%E6%8D%97%E7%8A%B6%E6%B3%81"/>
    /// </summary>
    public interface ITask
    {
        /// <summary>
        /// <paramref name="id"/>と一致する<inheritdoc cref="TaskProgress" path="/summary"/>を取得します。
        /// <see href="https://developer.kaonavi.jp/api/v2.0/index.html#tag/%E3%82%BF%E3%82%B9%E3%82%AF%E9%80%B2%E6%8D%97%E7%8A%B6%E6%B3%81/paths/~1tasks~1{task_id}/get"/>
        /// </summary>
        /// <param name="id"><inheritdoc cref="TaskProgress" path="/param[@name='Id']"/></param>
        /// <param name="cancellationToken"><inheritdoc cref="HttpClient.SendAsync(HttpRequestMessage, CancellationToken)" path="/param[@name='cancellationToken']"/></param>
        ValueTask<TaskProgress> ReadAsync(int id, CancellationToken cancellationToken = default);
    }

    /// <inheritdoc/>
    public ITask Task => this;

    /// <inheritdoc/>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="id"/>が0より小さい場合にスローされます。</exception>
    ValueTask<TaskProgress> ITask.ReadAsync(int id, CancellationToken cancellationToken)
        => CallApiAsync(new(HttpMethod.Get, $"tasks/{ThrowIfNegative(id):D}"), Context.Default.TaskProgress, cancellationToken);
}
