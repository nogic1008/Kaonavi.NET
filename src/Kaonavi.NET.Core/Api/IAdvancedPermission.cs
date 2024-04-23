using Kaonavi.Net.Entities;

namespace Kaonavi.Net.Api;

/// <summary>
/// 拡張アクセス設定 API
/// <see href="https://developer.kaonavi.jp/api/v2.0/index.html#tag/%E6%8B%A1%E5%BC%B5%E3%82%A2%E3%82%AF%E3%82%BB%E3%82%B9%E8%A8%AD%E5%AE%9A"/>
/// </summary>
public interface IAdvancedPermission
{
    /// <summary>
    /// <inheritdoc cref="AdvancedPermission" path="/summary"/>の一覧を取得します。
    /// <see href="https://developer.kaonavi.jp/api/v2.0/index.html#tag/%E6%8B%A1%E5%BC%B5%E3%82%A2%E3%82%AF%E3%82%BB%E3%82%B9%E8%A8%AD%E5%AE%9A/paths/~1advanced_permissions~1{advanced_type}/get"/>
    /// </summary>
    /// <param name="type"><inheritdoc cref="AdvancedType" path="/summary"/></param>
    /// <param name="cancellationToken"><inheritdoc cref="HttpClient.SendAsync(HttpRequestMessage, CancellationToken)" path="/param[@name='cancellationToken']"/></param>
    ValueTask<IReadOnlyList<AdvancedPermission>> ListAsync(AdvancedType type, CancellationToken cancellationToken = default);

    /// <summary>
    /// 現在登録されている<inheritdoc cref="AdvancedPermission" path="/summary"/>を全て、リクエストしたデータで入れ替えます。
    /// <see href="https://developer.kaonavi.jp/api/v2.0/index.html#tag/%E6%8B%A1%E5%BC%B5%E3%82%A2%E3%82%AF%E3%82%BB%E3%82%B9%E8%A8%AD%E5%AE%9A/paths/~1advanced_permissions~1{advanced_type}/put"/>
    /// </summary>
    /// <remarks>更新リクエスト制限の対象APIです。</remarks>
    /// <param name="type"><inheritdoc cref="AdvancedType" path="/summary"/></param>
    /// <param name="payload">入れ替え対象となるデータ</param>
    /// <param name="cancellationToken"><inheritdoc cref="HttpClient.SendAsync(HttpRequestMessage, CancellationToken)" path="/param[@name='cancellationToken']"/></param>
    ValueTask<int> ReplaceAsync(AdvancedType type, IReadOnlyList<AdvancedPermission> payload, CancellationToken cancellationToken = default);
}
