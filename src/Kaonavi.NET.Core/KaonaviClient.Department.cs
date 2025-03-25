using Kaonavi.Net.Entities;
using Kaonavi.Net.Json;

namespace Kaonavi.Net;

public partial class KaonaviClient : KaonaviClient.IDepartment
{
    /// <summary>
    /// 所属ツリー API
    /// <see href="https://developer.kaonavi.jp/api/v2.0/index.html#tag/%E6%89%80%E5%B1%9E%E3%83%84%E3%83%AA%E3%83%BC"/>
    /// </summary>
    public interface IDepartment
    {
        /// <summary>
        /// <inheritdoc cref="DepartmentTree" path="/summary"/>の情報を取得します。
        /// <see href="https://developer.kaonavi.jp/api/v2.0/index.html#tag/%E6%89%80%E5%B1%9E%E3%83%84%E3%83%AA%E3%83%BC/paths/~1departments/get"/>
        /// </summary>
        /// <param name="cancellationToken"><inheritdoc cref="HttpClient.SendAsync(HttpRequestMessage, CancellationToken)" path="/param[@name='cancellationToken']"/></param>
        public ValueTask<IReadOnlyList<DepartmentTree>> ListAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// <inheritdoc cref="DepartmentTree" path="/summary"/>を一括更新します。
        /// <paramref name="payload"/>に含まれていない情報は削除されます。
        /// <see href="https://developer.kaonavi.jp/api/v2.0/index.html#tag/%E6%89%80%E5%B1%9E%E3%83%84%E3%83%AA%E3%83%BC/paths/~1departments/put"/>
        /// </summary>
        /// <remarks>更新リクエスト制限の対象APIです。</remarks>
        /// <param name="payload">一括更新するデータ</param>
        /// <param name="cancellationToken"><inheritdoc cref="HttpClient.SendAsync(HttpRequestMessage, CancellationToken)" path="/param[@name='cancellationToken']"/></param>
        /// <returns><inheritdoc cref="TaskProgress" path="/param[@name='Id']"/></returns>
        public ValueTask<int> ReplaceAsync(IReadOnlyList<DepartmentTree> payload, CancellationToken cancellationToken = default);
    }

    /// <inheritdoc/>
    public IDepartment Department => this;

    /// <inheritdoc/>
    public ValueTask<IReadOnlyList<DepartmentTree>> IDepartment.ListAsync(CancellationToken cancellationToken)
        => CallApiAsync(new(HttpMethod.Get, "departments"), "department_data", Context.Default.IReadOnlyListDepartmentTree, cancellationToken);

    /// <inheritdoc/>
    public ValueTask<int> IDepartment.ReplaceAsync(IReadOnlyList<DepartmentTree> payload, CancellationToken cancellationToken)
        => CallTaskApiAsync(HttpMethod.Put, "departments", payload, "department_data"u8, Context.Default.IReadOnlyListDepartmentTree, cancellationToken);
}
