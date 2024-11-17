using Kaonavi.Net.Entities;
using Kaonavi.Net.Json;

namespace Kaonavi.Net;

public partial class KaonaviClient : KaonaviClient.ILayout
{
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
        /// <param name="cancellationToken"><inheritdoc cref="HttpClient.SendAsync(HttpRequestMessage, CancellationToken)" path="/param[@name='cancellationToken']"/></param>
        ValueTask<MemberLayout> ReadMemberLayoutAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// 使用可能なシートのレイアウト設定情報を全て取得します。
        /// <see href="https://developer.kaonavi.jp/api/v2.0/index.html#tag/%E3%83%AC%E3%82%A4%E3%82%A2%E3%82%A6%E3%83%88%E8%A8%AD%E5%AE%9A/paths/~1sheet_layouts/get"/>
        /// </summary>
        /// <param name="getCalcType">計算式パーツの取得</param>
        /// <param name="cancellationToken"><inheritdoc cref="HttpClient.SendAsync(HttpRequestMessage, CancellationToken)" path="/param[@name='cancellationToken']"/></param>
        ValueTask<IReadOnlyList<SheetLayout>> ListAsync(bool getCalcType = false, CancellationToken cancellationToken = default);

        /// <summary>
        /// <paramref name="id"/>と一致するシートの使用可能なレイアウト設定を全て取得します。
        /// <see href="https://developer.kaonavi.jp/api/v2.0/index.html#tag/%E3%83%AC%E3%82%A4%E3%82%A2%E3%82%A6%E3%83%88%E8%A8%AD%E5%AE%9A/paths/~1sheet_layouts~1{sheet_id}/get"/>
        /// </summary>
        /// <param name="id"><inheritdoc cref="SheetLayout" path="/param[@name='Id']"/></param>
        /// <param name="getCalcType">計算式パーツの取得</param>
        /// <param name="cancellationToken"><inheritdoc cref="HttpClient.SendAsync(HttpRequestMessage, CancellationToken)" path="/param[@name='cancellationToken']"/></param>
        ValueTask<SheetLayout> ReadAsync(int id, bool getCalcType = false, CancellationToken cancellationToken = default);
    }

    /// <inheritdoc/>
    public ILayout Layout => this;

    /// <inheritdoc/>
    ValueTask<MemberLayout> ILayout.ReadMemberLayoutAsync(CancellationToken cancellationToken)
        => CallApiAsync(new(HttpMethod.Get, "member_layouts"), Context.Default.MemberLayout, cancellationToken);

    /// <inheritdoc/>
    ValueTask<IReadOnlyList<SheetLayout>> ILayout.ListAsync(bool getCalcType, CancellationToken cancellationToken)
        => CallApiAsync(new(HttpMethod.Get, $"sheet_layouts{(getCalcType ? "?get_calc_type=true" : "")}"), "sheets", Context.Default.IReadOnlyListSheetLayout, cancellationToken);

    /// <inheritdoc/>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="id"/>が0より小さい場合にスローされます。</exception>
    ValueTask<SheetLayout> ILayout.ReadAsync(int id, bool getCalcType, CancellationToken cancellationToken)
        => CallApiAsync(new(HttpMethod.Get, $"sheet_layouts/{ThrowIfNegative(id):D}{(getCalcType ? "?get_calc_type=true" : "")}"), Context.Default.SheetLayout, cancellationToken);
}
