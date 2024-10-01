namespace Kaonavi.Net;

/// <summary>カオナビ API v2の抽象化</summary>
public interface IKaonaviClient
{
    /// <inheritdoc cref="KaonaviClient.ITask"/>
    KaonaviClient.ITask Task { get; }

    /// <inheritdoc cref="KaonaviClient.ILayout"/>
    KaonaviClient.ILayout Layout { get; }

    /// <inheritdoc cref="KaonaviClient.IMember"/>
    KaonaviClient.IMember Member { get; }

    /// <inheritdoc cref="KaonaviClient.ISheet"/>
    KaonaviClient.ISheet Sheet { get; }

    /// <inheritdoc cref="KaonaviClient.IDepartment"/>
    KaonaviClient.IDepartment Department { get; }

    /// <inheritdoc cref="KaonaviClient.IUser"/>
    KaonaviClient.IUser User { get; }

    /// <inheritdoc cref="KaonaviClient.IRole"/>
    KaonaviClient.IRole Role { get; }

    /// <inheritdoc cref="KaonaviClient.IAdvancedPermission"/>
    KaonaviClient.IAdvancedPermission AdvancedPermission { get; }

    /// <inheritdoc cref="KaonaviClient.IEnumOption"/>
    KaonaviClient.IEnumOption EnumOption { get; }

    /// <inheritdoc cref="KaonaviClient.IWebhook"/>
    KaonaviClient.IWebhook Webhook { get; }
}
