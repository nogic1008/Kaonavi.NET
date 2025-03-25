namespace Kaonavi.Net;

/// <summary>カオナビ API v2の抽象化</summary>
public interface IKaonaviClient
{
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
