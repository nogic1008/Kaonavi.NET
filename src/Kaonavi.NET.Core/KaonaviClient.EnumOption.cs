using Kaonavi.Net.Entities;
using Kaonavi.Net.Json;


namespace Kaonavi.Net;

public partial class KaonaviClient : KaonaviClient.IEnumOption
{
    /// <summary>
    /// マスター管理 API
    /// <see href="https://developer.kaonavi.jp/api/v2.0/index.html#tag/%E3%83%9E%E3%82%B9%E3%82%BF%E3%83%BC%E7%AE%A1%E7%90%86"/>
    /// </summary>
    public interface IEnumOption
    {
        /// <summary>
        /// マスター管理で編集可能な項目のうち、APIv2 で編集可能な<inheritdoc cref="EnumOption" path="/summary"/>の一覧を取得します。
        /// <see href="https://developer.kaonavi.jp/api/v2.0/index.html#tag/%E3%83%9E%E3%82%B9%E3%82%BF%E3%83%BC%E7%AE%A1%E7%90%86/paths/~1enum_options/get"/>
        /// </summary>
        /// <remarks>
        /// APIv2 で編集できるのは、プルダウンリスト、ラジオボタン、チェックボックスで作成された項目です。
        /// ただし、データ連携中の項目はマスター管理で編集不可能なため、上記のパーツ種別であっても取得は出来ません。
        /// </remarks>
        /// <param name="cancellationToken"><inheritdoc cref="HttpClient.SendAsync(HttpRequestMessage, CancellationToken)" path="/param[@name='cancellationToken']"/></param>
        ValueTask<IReadOnlyList<EnumOption>> ListAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// <paramref name="id"/>と一致する<inheritdoc cref="EnumOption" path="/summary"/>を取得します。
        /// <see href="https://developer.kaonavi.jp/api/v2.0/index.html#tag/%E3%83%9E%E3%82%B9%E3%82%BF%E3%83%BC%E7%AE%A1%E7%90%86/paths/~1enum_options~1{custom_field_id}/get"/>
        /// </summary>
        /// <inheritdoc cref="ListAsync" path="/remarks"/>
        /// <param name="id"><inheritdoc cref="EnumOption.Id" path="/summary"/></param>
        /// <param name="cancellationToken"><inheritdoc cref="HttpClient.SendAsync(HttpRequestMessage, CancellationToken)" path="/param[@name='cancellationToken']"/></param>
        ValueTask<EnumOption> ReadAsync(int id, CancellationToken cancellationToken = default);

        /// <summary>
        /// <paramref name="id"/>と一致する<inheritdoc cref="EnumOption" path="/summary"/>を一括更新します。
        /// <see href="https://developer.kaonavi.jp/api/v2.0/index.html#tag/%E3%83%9E%E3%82%B9%E3%82%BF%E3%83%BC%E7%AE%A1%E7%90%86/paths/~1enum_options~1{custom_field_id}/put"/>
        /// </summary>
        /// <remarks>
        /// 更新リクエスト制限の対象APIです。
        /// <list type="bullet">
        /// <item>マスターを追加したい場合は、id を指定せず name のみ指定してください。</item>
        /// <item><paramref name="payload"/>に含まれていないマスターは削除されます。ただし、削除できるマスターは、メンバー数が「0」のマスターのみです。</item>
        /// <item>並び順は送信された配列順に登録されます。</item>
        /// <item>変更履歴が設定されたシートのマスター情報を更新した際には履歴が作成されます。</item>
        /// <item><paramref name="payload"/>は0件での指定は出来ません。1件以上指定してください。</item>
        /// </list>
        /// </remarks>
        /// <param name="id"><inheritdoc cref="EnumOption.Id" path="/summary"/></param>
        /// <param name="payload">リクエスト</param>
        /// <param name="cancellationToken"><inheritdoc cref="HttpClient.SendAsync(HttpRequestMessage, CancellationToken)" path="/param[@name='cancellationToken']"/></param>
        /// <returns><inheritdoc cref="TaskProgress.Id" path="/summary" /></returns>
        ValueTask<int> UpdateAsync(int id, IReadOnlyList<(int? id, string name)> payload, CancellationToken cancellationToken = default);
    }

    /// <inheritdoc/>
    public IEnumOption EnumOption => this;

    /// <inheritdoc/>
    async ValueTask<IReadOnlyList<EnumOption>> IEnumOption.ListAsync(CancellationToken cancellationToken)
        => (await CallApiAsync(new(HttpMethod.Get, "enum_options"), Context.Default.ApiListResultEnumOption, cancellationToken)).Values;

    /// <inheritdoc/>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="id"/>が0より小さい場合にスローされます。</exception>
    ValueTask<EnumOption> IEnumOption.ReadAsync(int id, CancellationToken cancellationToken)
        => CallApiAsync(new(HttpMethod.Get, $"enum_options/{ThrowIfNegative(id):D}"), Context.Default.EnumOption, cancellationToken);

    /// <inheritdoc/>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="id"/>が0より小さい場合にスローされます。</exception>
    ValueTask<int> IEnumOption.UpdateAsync(int id, IReadOnlyList<(int? id, string name)> payload, CancellationToken cancellationToken)
        => CallTaskApiAsync(
            HttpMethod.Put,
            $"enum_options/{ThrowIfNegative(id):D}",
            new("enum_option_data", payload.Select(d => new EnumOptionPayloadData(d.id, d.name)).ToArray()),
            Context.Default.ApiListResultEnumOptionPayloadData,
            cancellationToken);

    internal record EnumOptionPayloadData(int? Id, string Name);
}
