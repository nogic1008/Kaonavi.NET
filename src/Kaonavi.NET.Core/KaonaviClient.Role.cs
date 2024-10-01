using System.Net.Http.Json;
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
        ValueTask<IReadOnlyList<Role>> ListAsync(CancellationToken cancellationToken = default);
    }

    /// <inheritdoc/>
    public IRole Role => this;

    /// <inheritdoc/>
    async ValueTask<IReadOnlyList<Role>> IRole.ListAsync(CancellationToken cancellationToken)
        => (await CallApiAsync(new(HttpMethod.Get, "roles"), Context.Default.ApiListResultRole, cancellationToken)).Values;
}
