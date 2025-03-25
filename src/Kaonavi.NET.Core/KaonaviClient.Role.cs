using Kaonavi.Net.Entities;
using Kaonavi.Net.Json;

namespace Kaonavi.Net;

public partial class KaonaviClient : KaonaviClient.IRole
{
    /// <summary>
    /// ロール
    /// <see href="https://developer.kaonavi.jp/api/v2.0/index.html#tag/%E3%83%AD%E3%83%BC%E3%83%AB"/>
    /// </summary>
    public interface IRole
    {
        /// <summary>
        /// <inheritdoc cref="Role" path="/summary"/>の一覧を取得します。
        /// <see href="https://developer.kaonavi.jp/api/v2.0/index.html#tag/%E3%83%AD%E3%83%BC%E3%83%AB/paths/~1roles/get"/>
        /// </summary>
        /// <param name="cancellationToken"><inheritdoc cref="HttpClient.SendAsync(HttpRequestMessage, CancellationToken)" path="/param[@name='cancellationToken']"/></param>
        public ValueTask<IReadOnlyList<Role>> ListAsync(CancellationToken cancellationToken = default);
    }

    /// <inheritdoc/>
    public IRole Role => this;

    /// <inheritdoc/>
    public ValueTask<IReadOnlyList<Role>> IRole.ListAsync(CancellationToken cancellationToken)
        => CallApiAsync(new(HttpMethod.Get, "roles"), "role_data", Context.Default.IReadOnlyListRole, cancellationToken);
}
