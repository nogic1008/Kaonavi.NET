using Kaonavi.Net.Api;

namespace Kaonavi.Net;

/// <summary>カオナビ API v2の抽象化</summary>
public interface IKaonaviClient
{
    /// <inheritdoc cref="ITask"/>
    ITask Task { get; }

    /// <inheritdoc cref="ILayout"/>
    ILayout Layout { get; }

    /// <inheritdoc cref="IMember"/>
    IMember Member { get; }

    /// <inheritdoc cref="ISheet"/>
    ISheet Sheet { get; }

    /// <inheritdoc cref="IDepartment"/>
    IDepartment Department { get; }

    /// <inheritdoc cref="IUser"/>
    IUser User { get; }

    /// <inheritdoc cref="IRole"/>
    IRole Role { get; }

    /// <inheritdoc cref="IAdvancedPermission"/>
    IAdvancedPermission AdvancedPermission { get; }

    /// <inheritdoc cref="IEnumOption"/>
    IEnumOption EnumOption { get; }

    /// <inheritdoc cref="IWebhook"/>
    IWebhook Webhook { get; }
}
