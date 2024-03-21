using Kaonavi.Net.Entities;

namespace Kaonavi.Net.Api;

/// <summary>
/// シート情報 API
/// <see href="https://developer.kaonavi.jp/api/v2.0/index.html#tag/%E3%82%B7%E3%83%BC%E3%83%88%E6%83%85%E5%A0%B1"/>
/// </summary>
public interface ISheet
{
    /// <summary>
    /// <paramref name="id"/>と一致するシートの全情報を取得します。
    /// <see href="https://developer.kaonavi.jp/api/v2.0/index.html#tag/%E3%82%B7%E3%83%BC%E3%83%88%E6%83%85%E5%A0%B1/paths/~1sheets~1{sheet_id}/get"/>
    /// </summary>
    /// <param name="id"><inheritdoc cref="SheetLayout" path="/param[@name='Id']/text()"/></param>
    /// <param name="cancellationToken"><inheritdoc cref="FetchTaskProgressAsync" path="/param[@name='cancellationToken']/text()"/></param>
    ValueTask<IReadOnlyCollection<SheetData>> ListAsync(int id, CancellationToken cancellationToken = default);

    /// <summary>
    /// <paramref name="id"/>と一致する<inheritdoc cref="SheetData" path="/summary/text()"/>を一括更新します。
    /// <paramref name="payload"/>に含まれていない情報は削除されます。
    /// 複数レコードの情報は送信された配列順に登録されます。
    /// <see href="https://developer.kaonavi.jp/api/v2.0/index.html#tag/%E3%82%B7%E3%83%BC%E3%83%88%E6%83%85%E5%A0%B1/paths/~1sheets~1{sheet_id}/put"/>
    /// </summary>
    /// <remarks>更新リクエスト制限の対象APIです。</remarks>
    /// <param name="id"><inheritdoc cref="SheetLayout" path="/param[@name='Id']/text()"/></param>
    /// <param name="payload">一括更新するデータ</param>
    /// <param name="cancellationToken"><inheritdoc cref="FetchTaskProgressAsync" path="/param[@name='cancellationToken']/text()"/></param>
    /// <returns><inheritdoc cref="TaskProgress" path="/param[@name='Id']/text()"/></returns>
    ValueTask<int> ReplaceAsync(int id, IReadOnlyCollection<SheetData> payload, CancellationToken cancellationToken = default);

    /// <summary>
    /// <paramref name="id"/>と一致する<inheritdoc cref="SheetData" path="/summary/text()"/>の一部を更新します。
    /// <list type="bullet">
    /// <item>
    ///   <term><inheritdoc cref="RecordType.Single" path="/summary/text()"/></term>
    ///   <description>
    ///     送信された情報のみが更新されます。
    ///     <paramref name="payload"/> に含まれていない情報は更新されません。
    ///     特定の値を削除する場合は、<c>""</c>を送信してください。
    ///   </description>
    /// </item>
    /// <item>
    ///   <term><inheritdoc cref="RecordType.Multiple" path="/summary/text()"/></term>
    ///   <description>
    ///     メンバーごとのデータが一括更新されます。
    ///     特定の値を削除する場合は、<c>""</c>を送信してください。
    ///     特定のレコードだけを更新することは出来ません。
    ///     <inheritdoc cref="SheetData.Code" path="/summary/text()"/>が指定されていないメンバーの情報は更新されません。
    ///     送信された配列順に登録されます。
    ///   </description>
    /// </item>
    /// </list>
    /// <see href="https://developer.kaonavi.jp/api/v2.0/index.html#tag/%E3%82%B7%E3%83%BC%E3%83%88%E6%83%85%E5%A0%B1/paths/~1sheets~1{sheet_id}/patch"/>
    /// </summary>
    /// <remarks>更新リクエスト制限の対象APIです。</remarks>
    /// <param name="id"><inheritdoc cref="SheetLayout" path="/param[@name='Id']/text()"/></param>
    /// <param name="payload">更新するデータ</param>
    /// <param name="cancellationToken"><inheritdoc cref="FetchTaskProgressAsync" path="/param[@name='cancellationToken']/text()"/></param>
    /// <returns><inheritdoc cref="TaskProgress" path="/param[@name='Id']/text()"/></returns>
    ValueTask<int> UpdateAsync(int id, IReadOnlyCollection<SheetData> payload, CancellationToken cancellationToken = default);

    /// <summary>
    /// <paramref name="id"/>と一致する<inheritdoc cref="RecordType.Multiple"/>にレコードを追加します。
    /// <inheritdoc cref="RecordType.Single"/>は対象外です。
    /// <see href="https://developer.kaonavi.jp/api/v2.0/index.html#tag/%E3%82%B7%E3%83%BC%E3%83%88%E6%83%85%E5%A0%B1/paths/~1sheets~1{sheet_id}~1add/post"/>
    /// </summary>
    /// <remarks>更新リクエスト制限の対象APIです。</remarks>
    /// <param name="id"><inheritdoc cref="SheetLayout" path="/param[@name='Id']/text()"/></param>
    /// <param name="payload">追加するデータ</param>
    /// <param name="cancellationToken"><inheritdoc cref="FetchTaskProgressAsync" path="/param[@name='cancellationToken']/text()"/></param>
    /// <returns><inheritdoc cref="TaskProgress" path="/param[@name='Id']/text()"/></returns>
    ValueTask<int> CreateAsync(int id, IReadOnlyCollection<SheetData> payload, CancellationToken cancellationToken = default);
}
