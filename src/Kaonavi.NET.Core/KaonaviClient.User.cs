using System.Net.Http.Json;
using Kaonavi.Net.Entities;
using Kaonavi.Net.Json;


namespace Kaonavi.Net;

public partial class KaonaviClient : KaonaviClient.IUser
{
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

    /// <inheritdoc/>
    public IUser User => this;

    /// <inheritdoc/>
    async ValueTask<IReadOnlyList<UserWithLoginAt>> IUser.ListAsync(CancellationToken cancellationToken)
        => (await CallApiAsync(new(HttpMethod.Get, "users"), Context.Default.ApiListResultUserWithLoginAt, cancellationToken)).Values;

    /// <inheritdoc/>
    ValueTask<User> IUser.CreateAsync(UserPayload payload, CancellationToken cancellationToken)
        => CallApiAsync(new(HttpMethod.Post, "users")
        {
            Content = JsonContent.Create(new(payload), Context.Default.UserJsonPayload)
        }, Context.Default.User, cancellationToken);

    /// <inheritdoc/>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="id"/>が0より小さい場合にスローされます。</exception>
    ValueTask<UserWithLoginAt> IUser.ReadAsync(int id, CancellationToken cancellationToken)
        => CallApiAsync(new(HttpMethod.Get, $"users/{ThrowIfNegative(id):D}"), Context.Default.UserWithLoginAt, cancellationToken);

    /// <inheritdoc/>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="id"/>が0より小さい場合にスローされます。</exception>
    ValueTask<User> IUser.UpdateAsync(int id, UserPayload payload, CancellationToken cancellationToken)
        => CallApiAsync(new(HttpMethod.Patch, $"users/{ThrowIfNegative(id):D}")
        {
            Content = JsonContent.Create(new(payload), Context.Default.UserJsonPayload)
        }, Context.Default.User, cancellationToken);

    /// <inheritdoc/>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="id"/>が0より小さい場合にスローされます。</exception>
    async ValueTask IUser.DeleteAsync(int id, CancellationToken cancellationToken)
        => await CallApiAsync(new(HttpMethod.Delete, $"users/{ThrowIfNegative(id):D}"), cancellationToken).ConfigureAwait(false);

    internal record UserJsonPayload(string Email, string? MemberCode, string Password, Role Role, bool IsActive, bool PasswordLocked, bool UseSmartphone)
    {
        public UserJsonPayload(UserPayload payload) : this(payload.Email, payload.MemberCode, payload.Password, new(payload.RoleId, null!, null!), payload.IsActive, payload.PasswordLocked, payload.UseSmartphone)
        { }
    }
}