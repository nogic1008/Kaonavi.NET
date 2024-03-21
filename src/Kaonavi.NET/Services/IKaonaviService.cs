using Kaonavi.Net.Api;
using Kaonavi.Net.Entities;

namespace Kaonavi.Net.Services;

/// <summary>カオナビ API v2の抽象化</summary>
public interface IKaonaviService
{
    #region タスク進捗状況
    /// <summary>
    /// <paramref name="taskId"/>と一致する<inheritdoc cref="TaskProgress" path="/summary/text()"/>を取得します。
    /// <see href="https://developer.kaonavi.jp/api/v2.0/index.html#tag/%E3%82%BF%E3%82%B9%E3%82%AF%E9%80%B2%E6%8D%97%E7%8A%B6%E6%B3%81/paths/~1tasks~1{task_id}/get"/>
    /// </summary>
    /// <param name="taskId"><inheritdoc cref="TaskProgress" path="/param[@name='Id']/text()"/></param>
    /// <param name="cancellationToken">キャンセル通知を受け取るために他のオブジェクトまたはスレッドで使用できるキャンセル トークン。</param>
    ValueTask<TaskProgress> FetchTaskProgressAsync(int taskId, CancellationToken cancellationToken = default);
    #endregion タスク進捗状況

    #region レイアウト設定
    /// <summary>
    /// 使用可能なメンバーのレイアウト設定情報を全て取得します。
    /// <see href="https://developer.kaonavi.jp/api/v2.0/index.html#tag/%E3%83%AC%E3%82%A4%E3%82%A2%E3%82%A6%E3%83%88%E8%A8%AD%E5%AE%9A/paths/~1member_layouts/get"/>
    /// </summary>
    /// <param name="cancellationToken"><inheritdoc cref="FetchTaskProgressAsync" path="/param[@name='cancellationToken']/text()"/></param>
    ValueTask<MemberLayout> FetchMemberLayoutAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 使用可能なシートのレイアウト設定情報を全て取得します。
    /// <see href="https://developer.kaonavi.jp/api/v2.0/index.html#tag/%E3%83%AC%E3%82%A4%E3%82%A2%E3%82%A6%E3%83%88%E8%A8%AD%E5%AE%9A/paths/~1sheet_layouts/get"/>
    /// </summary>
    /// <param name="cancellationToken"><inheritdoc cref="FetchTaskProgressAsync" path="/param[@name='cancellationToken']/text()"/></param>
    ValueTask<IReadOnlyCollection<SheetLayout>> FetchSheetLayoutsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// <paramref name="sheetId"/>と一致するシートの使用可能なレイアウト設定を全て取得します。
    /// <see href="https://developer.kaonavi.jp/api/v2.0/index.html#tag/%E3%83%AC%E3%82%A4%E3%82%A2%E3%82%A6%E3%83%88%E8%A8%AD%E5%AE%9A/paths/~1sheet_layouts~1{sheet_id}/get"/>
    /// </summary>
    /// <param name="sheetId"><inheritdoc cref="SheetLayout" path="/param[@name='Id']"/></param>
    /// <param name="cancellationToken"><inheritdoc cref="FetchTaskProgressAsync" path="/param[@name='cancellationToken']/text()"/></param>
    ValueTask<SheetLayout> FetchSheetLayoutAsync(int sheetId, CancellationToken cancellationToken = default);
    #endregion レイアウト設定

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
