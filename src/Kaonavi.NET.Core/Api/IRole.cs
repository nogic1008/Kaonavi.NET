using Kaonavi.Net.Entities;

namespace Kaonavi.Net.Api;

/// <summary>
/// ロール
/// <see href="https://developer.kaonavi.jp/api/v2.0/index.html#tag/%E3%83%AD%E3%83%BC%E3%83%AB"/>
/// </summary>
public interface IRole
{
    /// <summary>
    /// <inheritdoc cref="Role" path="/summary/text()"/>の一覧を取得します。
    /// <see href="https://developer.kaonavi.jp/api/v2.0/index.html#tag/%E3%83%AD%E3%83%BC%E3%83%AB/paths/~1roles/get"/>
    /// </summary>
    /// <param name="cancellationToken"><inheritdoc cref="HttpClient.SendAsync(HttpRequestMessage, CancellationToken)" path="/param[@name='cancellationToken']"/></param>
    ValueTask<IReadOnlyList<Role>> ListAsync(CancellationToken cancellationToken = default);
}
