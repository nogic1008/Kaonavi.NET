using Kaonavi.Net.Entities;

namespace Kaonavi.Net.Api;

/// <summary>
/// レイアウト設定 API
/// <see href="https://developer.kaonavi.jp/api/v2.0/index.html#tag/%E3%83%AC%E3%82%A4%E3%82%A2%E3%82%A6%E3%83%88%E8%A8%AD%E5%AE%9A"/>
/// </summary>
public interface ILayout
{
    /// <summary>
    /// 使用可能なメンバーのレイアウト設定情報を全て取得します。
    /// <see href="https://developer.kaonavi.jp/api/v2.0/index.html#tag/%E3%83%AC%E3%82%A4%E3%82%A2%E3%82%A6%E3%83%88%E8%A8%AD%E5%AE%9A/paths/~1member_layouts/get"/>
    /// </summary>
    /// <param name="cancellationToken">キャンセル通知を受け取るために他のオブジェクトまたはスレッドで使用できるキャンセル トークン。</param>
    ValueTask<MemberLayout> ReadMemberLayoutAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 使用可能なシートのレイアウト設定情報を全て取得します。
    /// <see href="https://developer.kaonavi.jp/api/v2.0/index.html#tag/%E3%83%AC%E3%82%A4%E3%82%A2%E3%82%A6%E3%83%88%E8%A8%AD%E5%AE%9A/paths/~1sheet_layouts/get"/>
    /// </summary>
    /// <param name="cancellationToken">キャンセル通知を受け取るために他のオブジェクトまたはスレッドで使用できるキャンセル トークン。</param>
    ValueTask<IReadOnlyList<SheetLayout>> ListAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// <paramref name="id"/>と一致するシートの使用可能なレイアウト設定を全て取得します。
    /// <see href="https://developer.kaonavi.jp/api/v2.0/index.html#tag/%E3%83%AC%E3%82%A4%E3%82%A2%E3%82%A6%E3%83%88%E8%A8%AD%E5%AE%9A/paths/~1sheet_layouts~1{sheet_id}/get"/>
    /// </summary>
    /// <param name="id"><inheritdoc cref="SheetLayout" path="/param[@name='Id']"/></param>
    /// <param name="cancellationToken">キャンセル通知を受け取るために他のオブジェクトまたはスレッドで使用できるキャンセル トークン。</param>
    ValueTask<SheetLayout> ReadAsync(int id, CancellationToken cancellationToken = default);
}
