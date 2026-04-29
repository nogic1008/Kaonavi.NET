using Kaonavi.Net.Entities;

namespace Kaonavi.Net;

/// <summary>カオナビ API v2の抽象化</summary>
public interface IKaonaviClient
{
    /// <summary>
    /// アクセストークン文字列を取得または設定します。
    /// 各種API呼び出し時、この項目が<see langword="null"/>の場合は自動的に<see cref="AuthenticateAsync"/>を呼び出します。
    /// </summary>
    public string? AccessToken { get; set; }

    /// <summary>
    /// アクセストークンを発行します。
    /// <see href="https://developer.kaonavi.jp/api/v2.0/index.html#tag/%E3%82%A2%E3%82%AF%E3%82%BB%E3%82%B9%E3%83%88%E3%83%BC%E3%82%AF%E3%83%B3/paths/~1token/post"/>
    /// </summary>
    /// <param name="cancellationToken"><inheritdoc cref="HttpClient.SendAsync(HttpRequestMessage, CancellationToken)" path="/param[@name='cancellationToken']"/></param>
    public ValueTask<Token> AuthenticateAsync(CancellationToken cancellationToken = default);

    /// <inheritdoc cref="KaonaviClient.ITask"/>
    public KaonaviClient.ITask Task { get; }

    /// <inheritdoc cref="KaonaviClient.ILayout"/>
    public KaonaviClient.ILayout Layout { get; }

    /// <inheritdoc cref="KaonaviClient.IMember"/>
    public KaonaviClient.IMember Member { get; }

    /// <inheritdoc cref="KaonaviClient.ISheet"/>
    public KaonaviClient.ISheet Sheet { get; }

    /// <inheritdoc cref="KaonaviClient.IDepartment"/>
    public KaonaviClient.IDepartment Department { get; }

    /// <inheritdoc cref="KaonaviClient.IUser"/>
    public KaonaviClient.IUser User { get; }

    /// <inheritdoc cref="KaonaviClient.IRole"/>
    public KaonaviClient.IRole Role { get; }

    /// <inheritdoc cref="KaonaviClient.IAdvancedPermission"/>
    public KaonaviClient.IAdvancedPermission AdvancedPermission { get; }

    /// <inheritdoc cref="KaonaviClient.IEnumOption"/>
    public KaonaviClient.IEnumOption EnumOption { get; }

    /// <inheritdoc cref="KaonaviClient.IWebhook"/>
    public KaonaviClient.IWebhook Webhook { get; }
}
