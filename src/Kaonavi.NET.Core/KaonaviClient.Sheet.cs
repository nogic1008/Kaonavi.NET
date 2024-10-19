using Kaonavi.Net.Entities;
using Kaonavi.Net.Json;


namespace Kaonavi.Net;

public partial class KaonaviClient : KaonaviClient.ISheet
{
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
        /// <param name="id"><inheritdoc cref="SheetLayout" path="/param[@name='Id']"/></param>
        /// <param name="cancellationToken"><inheritdoc cref="HttpClient.SendAsync(HttpRequestMessage, CancellationToken)" path="/param[@name='cancellationToken']"/></param>
        ValueTask<IReadOnlyList<SheetData>> ListAsync(int id, CancellationToken cancellationToken = default);

        /// <summary>
        /// <paramref name="id"/>と一致する<inheritdoc cref="SheetData" path="/summary"/>を一括更新します。
        /// <paramref name="payload"/>に含まれていない情報は削除されます。
        /// 複数レコードの情報は送信された配列順に登録されます。
        /// <see href="https://developer.kaonavi.jp/api/v2.0/index.html#tag/%E3%82%B7%E3%83%BC%E3%83%88%E6%83%85%E5%A0%B1/paths/~1sheets~1{sheet_id}/put"/>
        /// </summary>
        /// <remarks>更新リクエスト制限の対象APIです。</remarks>
        /// <param name="id"><inheritdoc cref="SheetLayout" path="/param[@name='Id']"/></param>
        /// <param name="payload">一括更新するデータ</param>
        /// <param name="cancellationToken"><inheritdoc cref="HttpClient.SendAsync(HttpRequestMessage, CancellationToken)" path="/param[@name='cancellationToken']"/></param>
        /// <returns><inheritdoc cref="TaskProgress" path="/param[@name='Id']"/></returns>
        ValueTask<int> ReplaceAsync(int id, IReadOnlyList<SheetData> payload, CancellationToken cancellationToken = default);

        /// <summary>
        /// <paramref name="id"/>と一致する<inheritdoc cref="SheetData" path="/summary"/>の一部を更新します。
        /// <list type="bullet">
        /// <item>
        ///   <term><inheritdoc cref="RecordType.Single" path="/summary"/></term>
        ///   <description>
        ///     送信された情報のみが更新されます。
        ///     <paramref name="payload"/> に含まれていない情報は更新されません。
        ///     特定の値を削除する場合は、<c>""</c>を送信してください。
        ///   </description>
        /// </item>
        /// <item>
        ///   <term><inheritdoc cref="RecordType.Multiple" path="/summary"/></term>
        ///   <description>
        ///     メンバーごとのデータが一括更新されます。
        ///     特定の値を削除する場合は、<c>""</c>を送信してください。
        ///     特定のレコードだけを更新することは出来ません。
        ///     <inheritdoc cref="SheetData.Code" path="/summary"/>が指定されていないメンバーの情報は更新されません。
        ///     送信された配列順に登録されます。
        ///   </description>
        /// </item>
        /// </list>
        /// <see href="https://developer.kaonavi.jp/api/v2.0/index.html#tag/%E3%82%B7%E3%83%BC%E3%83%88%E6%83%85%E5%A0%B1/paths/~1sheets~1{sheet_id}/patch"/>
        /// </summary>
        /// <remarks>更新リクエスト制限の対象APIです。</remarks>
        /// <param name="id"><inheritdoc cref="SheetLayout" path="/param[@name='Id']"/></param>
        /// <param name="payload">更新するデータ</param>
        /// <param name="cancellationToken"><inheritdoc cref="HttpClient.SendAsync(HttpRequestMessage, CancellationToken)" path="/param[@name='cancellationToken']"/></param>
        /// <returns><inheritdoc cref="TaskProgress" path="/param[@name='Id']"/></returns>
        ValueTask<int> UpdateAsync(int id, IReadOnlyList<SheetData> payload, CancellationToken cancellationToken = default);

        /// <summary>
        /// <paramref name="id"/>と一致する<inheritdoc cref="RecordType.Multiple"/>にレコードを追加します。
        /// <inheritdoc cref="RecordType.Single"/>は対象外です。
        /// <see href="https://developer.kaonavi.jp/api/v2.0/index.html#tag/%E3%82%B7%E3%83%BC%E3%83%88%E6%83%85%E5%A0%B1/paths/~1sheets~1{sheet_id}~1add/post"/>
        /// </summary>
        /// <remarks>更新リクエスト制限の対象APIです。</remarks>
        /// <param name="id"><inheritdoc cref="SheetLayout" path="/param[@name='Id']/text()"/></param>
        /// <param name="payload">追加するデータ</param>
        /// <param name="cancellationToken"><inheritdoc cref="HttpClient.SendAsync(HttpRequestMessage, CancellationToken)" path="/param[@name='cancellationToken']"/></param>
        /// <returns><inheritdoc cref="TaskProgress" path="/param[@name='Id']"/></returns>
        ValueTask<int> CreateAsync(int id, IReadOnlyList<SheetData> payload, CancellationToken cancellationToken = default);

        /// <summary>
        /// <paramref name="id"/>と一致するシートのファイルパーツにファイルをアップロードします。
        /// <list type="bullet">
        /// <item>
        ///   <term><inheritdoc cref="RecordType.Single" path="/summary"/></term>
        ///   <description>
        ///     ファイルが未登録の場合、アップロードが可能です。
        ///     ファイルが登録済みの場合、エラーになります。
        ///   </description>
        /// </item>
        /// <item>
        ///   <term><inheritdoc cref="RecordType.Multiple" path="/summary"/></term>
        ///   <description>
        ///     レコードを追加する形でファイルをアップロードします。
        ///     ファイル名はアップロード済みのファイルとは重複出来ません。
        ///     送信された配列順に登録されます。
        ///   </description>
        /// </item>
        /// <item>
        ///   <term>1リクエストあたりの上限</term>
        ///   <description>
        ///     <list type="bullet">
        ///       <item>
        ///         <term>ファイル数</term>
        ///         <description>100ファイル</description>
        ///       </item>
        ///       <item>
        ///         <term>ファイルサイズ</term>
        ///         <description>各5MBまで</description>
        ///       </item>
        ///     </list>
        ///   </description>
        /// </item>
        /// </list>
        /// <see href="https://developer.kaonavi.jp/api/v2.0/index.html#tag/%E3%82%B7%E3%83%BC%E3%83%88%E6%83%85%E5%A0%B1/paths/~1sheets~1{sheet_id}~1custom_fields~1{custom_field_id}~1file/post"/>
        /// </summary>
        /// <remarks>更新リクエスト制限の対象APIです。</remarks>
        /// <param name="id"><inheritdoc cref="SheetLayout" path="/param[@name='Id']"/></param>
        /// <param name="customFieldId"><inheritdoc cref="CustomFieldLayout" path="/param[@name='Id']"/></param>
        /// <param name="payload">追加するデータ</param>
        /// <param name="cancellationToken"><inheritdoc cref="HttpClient.SendAsync(HttpRequestMessage, CancellationToken)" path="/param[@name='cancellationToken']"/></param>
        /// <returns><inheritdoc cref="TaskProgress" path="/param[@name='Id']"/></returns>
        ValueTask<int> AddFileAsync(int id, int customFieldId, IReadOnlyList<Attachment> payload, CancellationToken cancellationToken = default);

        /// <summary>
        /// <paramref name="id"/>と一致するシートのファイルパーツのファイルを置き換えます。
        /// <list type="bullet">
        /// <item>
        ///   <term><inheritdoc cref="RecordType.Single" path="/summary"/></term>
        ///   <description>登録済みファイルと同一名のファイルを置き換えます。</description>
        /// </item>
        /// <item>
        ///   <term><inheritdoc cref="RecordType.Multiple" path="/summary"/></term>
        ///   <description>
        ///     登録済みファイルと同一名のファイルを置き換えます。
        ///     メンバーに同一ファイル名のレコードが複数ある場合、更新出来ません。
        ///     <list type="bullet">
        ///       <item>
        ///         <term>更新可能</term>
        ///         <description>社員番号:A0001と社員番号:A0002がそれぞれ「sample.txt」という名前のファイルを持つ</description>
        ///       </item>
        ///       <item>
        ///         <term>更新不可</term>
        ///         <description>社員番号:A0001が「sample.txt」という名前のファイルを2つ以上持つ</description>
        ///       </item>
        ///     </list>
        ///   </description>
        /// </item>
        /// <item>
        ///   <term>1リクエストあたりの上限</term>
        ///   <description>
        ///     <list type="bullet">
        ///       <item>
        ///         <term>ファイル数</term>
        ///         <description>100ファイル</description>
        ///       </item>
        ///       <item>
        ///         <term>ファイルサイズ</term>
        ///         <description>各5MBまで</description>
        ///       </item>
        ///     </list>
        ///   </description>
        /// </item>
        /// </list>
        /// <see href="https://developer.kaonavi.jp/api/v2.0/index.html#tag/%E3%82%B7%E3%83%BC%E3%83%88%E6%83%85%E5%A0%B1/paths/~1sheets~1{sheet_id}~1custom_fields~1{custom_field_id}~1file/post"/>
        /// </summary>
        /// <remarks>更新リクエスト制限の対象APIです。</remarks>
        /// <param name="id"><inheritdoc cref="SheetLayout" path="/param[@name='Id']"/></param>
        /// <param name="customFieldId"><inheritdoc cref="CustomFieldLayout" path="/param[@name='Id']"/></param>
        /// <param name="payload">追加するデータ</param>
        /// <param name="cancellationToken"><inheritdoc cref="HttpClient.SendAsync(HttpRequestMessage, CancellationToken)" path="/param[@name='cancellationToken']"/></param>
        /// <returns><inheritdoc cref="TaskProgress" path="/param[@name='Id']"/></returns>
        ValueTask<int> UpdateFileAsync(int id, int customFieldId, IReadOnlyList<Attachment> payload, CancellationToken cancellationToken = default);
    }

    /// <inheritdoc/>
    public ISheet Sheet => this;

    /// <inheritdoc/>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="id"/>が0より小さい場合にスローされます。</exception>
    async ValueTask<IReadOnlyList<SheetData>> ISheet.ListAsync(int id, CancellationToken cancellationToken)
        => (await CallApiAsync(new(HttpMethod.Get, $"sheets/{ThrowIfNegative(id):D}"), Context.Default.ApiListResultSheetData, cancellationToken)).Values;

    /// <inheritdoc/>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="id"/>が0より小さい場合にスローされます。</exception>
    ValueTask<int> ISheet.ReplaceAsync(int id, IReadOnlyList<SheetData> payload, CancellationToken cancellationToken)
        => CallTaskApiAsync(HttpMethod.Put, $"sheets/{ThrowIfNegative(id):D}", new("member_data", payload), Context.Default.ApiListResultSheetData, cancellationToken);

    /// <inheritdoc/>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="id"/>が0より小さい場合にスローされます。</exception>
    ValueTask<int> ISheet.UpdateAsync(int id, IReadOnlyList<SheetData> payload, CancellationToken cancellationToken)
        => CallTaskApiAsync(HttpMethod.Patch, $"sheets/{ThrowIfNegative(id):D}", new("member_data", payload), Context.Default.ApiListResultSheetData, cancellationToken);

    /// <inheritdoc/>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="id"/>が0より小さい場合にスローされます。</exception>
    ValueTask<int> ISheet.CreateAsync(int id, IReadOnlyList<SheetData> payload, CancellationToken cancellationToken)
        => CallTaskApiAsync(HttpMethod.Post, $"sheets/{ThrowIfNegative(id):D}/add", new("member_data", payload), Context.Default.ApiListResultSheetData, cancellationToken);

    /// <inheritdoc/>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="id"/>または<paramref name="customFieldId"/>が0より小さい場合にスローされます。</exception>
    ValueTask<int> ISheet.AddFileAsync(int id, int customFieldId, IReadOnlyList<Attachment> payload, CancellationToken cancellationToken)
        => CallTaskApiAsync(HttpMethod.Post, $"sheets/{ThrowIfNegative(id):D}/custom_fields/{ThrowIfNegative(customFieldId):D}/file", new("member_data", payload), Context.Default.ApiListResultAttachment, cancellationToken);

    /// <inheritdoc/>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="id"/>または<paramref name="customFieldId"/>が0より小さい場合にスローされます。</exception>
    ValueTask<int> ISheet.UpdateFileAsync(int id, int customFieldId, IReadOnlyList<Attachment> payload, CancellationToken cancellationToken)
        => CallTaskApiAsync(HttpMethod.Patch, $"sheets/{ThrowIfNegative(id):D}/custom_fields/{ThrowIfNegative(customFieldId):D}/file", new("member_data", payload), Context.Default.ApiListResultAttachment, cancellationToken);
}
