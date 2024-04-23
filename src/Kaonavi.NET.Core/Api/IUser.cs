using Kaonavi.Net.Entities;

namespace Kaonavi.Net.Api;

/// <summary>
/// ユーザー情報 API
/// <see href="https://developer.kaonavi.jp/api/v2.0/index.html#tag/%E3%83%A6%E3%83%BC%E3%82%B6%E3%83%BC%E6%83%85%E5%A0%B1"/>
/// </summary>
public interface IUser
{
    /// <summary>
    /// <inheritdoc cref="UserWithLoginAt" path="/summary/text()"/>の一覧を取得します。
    /// <see href="https://developer.kaonavi.jp/api/v2.0/index.html#tag/%E3%83%A6%E3%83%BC%E3%82%B6%E3%83%BC%E6%83%85%E5%A0%B1/paths/~1users/get"/>
    /// </summary>
    /// <param name="cancellationToken"><inheritdoc cref="HttpClient.SendAsync(HttpRequestMessage, CancellationToken)" path="/param[@name='cancellationToken']"/></param>
    ValueTask<IReadOnlyList<UserWithLoginAt>> ListAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// <inheritdoc cref="User" path="/summary/text()"/>を登録します。
    /// <see href="https://developer.kaonavi.jp/api/v2.0/index.html#tag/%E3%83%A6%E3%83%BC%E3%82%B6%E3%83%BC%E6%83%85%E5%A0%B1/paths/~1users/post"/>
    /// </summary>
    /// <param name="payload">リクエスト</param>
    /// <param name="cancellationToken"><inheritdoc cref="HttpClient.SendAsync(HttpRequestMessage, CancellationToken)" path="/param[@name='cancellationToken']"/></param>
    /// <remarks>
    /// 管理者メニュー > ユーザー管理 にてユーザー作成時に設定可能なオプションについては、以下の内容で作成されます。
    /// <list type="bullet">
    /// <item>
    ///   <term>スマホオプション</term>
    ///   <description>停止</description>
    /// </item>
    /// <item>
    ///   <term>セキュアアクセス</term>
    ///   <description>停止</description>
    /// </item>
    /// </list>
    /// </remarks>
    ValueTask<User> CreateAsync(UserPayload payload, CancellationToken cancellationToken = default);

    /// <summary>
    /// <paramref name="id"/>と一致する<inheritdoc cref="UserWithLoginAt" path="/summary/text()"/>を取得します。
    /// <see href="https://developer.kaonavi.jp/api/v2.0/index.html#tag/%E3%83%A6%E3%83%BC%E3%82%B6%E3%83%BC%E6%83%85%E5%A0%B1/paths/~1users~1{user_id}/get"/>
    /// </summary>
    /// <param name="id"><inheritdoc cref="UserWithLoginAt" path="/param[@name='Id']/text()"/></param>
    /// <param name="cancellationToken"><inheritdoc cref="HttpClient.SendAsync(HttpRequestMessage, CancellationToken)" path="/param[@name='cancellationToken']"/></param>
    ValueTask<UserWithLoginAt> ReadAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// <paramref name="id"/>と一致する<inheritdoc cref="User" path="/summary/text()"/>を更新します。
    /// <see href="https://developer.kaonavi.jp/api/v2.0/index.html#tag/%E3%83%A6%E3%83%BC%E3%82%B6%E3%83%BC%E6%83%85%E5%A0%B1/paths/~1users~1{user_id}/patch"/>
    /// </summary>
    /// <param name="id"><inheritdoc cref="User" path="/param[@name='Id']/text()"/></param>
    /// <param name="payload">リクエスト</param>
    /// <param name="cancellationToken"><inheritdoc cref="HttpClient.SendAsync(HttpRequestMessage, CancellationToken)" path="/param[@name='cancellationToken']"/></param>
    /// <remarks>
    /// 管理者メニュー > ユーザー管理 にて更新可能な以下のオプションについては元の値が維持されます。
    /// <list type="bullet">
    /// <item>スマホオプション</item>
    /// <item>セキュアアクセス</item>
    /// <item>アカウント状態</item>
    /// <item>パスワードロック</item>
    /// </list>
    /// </remarks>
    ValueTask<User> UpdateAsync(int id, UserPayload payload, CancellationToken cancellationToken = default);

    /// <summary>
    /// <paramref name="id"/>と一致する<inheritdoc cref="User" path="/summary/text()"/>を削除します。
    /// <see href="https://developer.kaonavi.jp/api/v2.0/index.html#tag/%E3%83%A6%E3%83%BC%E3%82%B6%E3%83%BC%E6%83%85%E5%A0%B1/paths/~1users~1{user_id}/delete"/>
    /// </summary>
    /// <param name="id"><inheritdoc cref="User" path="/param[@name='Id']/text()"/></param>
    /// <param name="cancellationToken"><inheritdoc cref="HttpClient.SendAsync(HttpRequestMessage, CancellationToken)" path="/param[@name='cancellationToken']"/></param>
    ValueTask DeleteAsync(int id, CancellationToken cancellationToken = default);
}
