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
        /// マスター管理およびAPIv2で編集可能な<inheritdoc cref="Entities.EnumOption" path="/summary"/>の一覧を取得します。
        /// <para>※データ連携中の項目は、マスター管理画面で編集できないため取得できません。</para>
        /// <see href="https://developer.kaonavi.jp/api/v2.0/index.html#tag/%E3%83%9E%E3%82%B9%E3%82%BF%E3%83%BC%E7%AE%A1%E7%90%86/paths/~1enum_options/get"/>
        /// </summary>
        /// <remarks>
        /// APIv2で編集可能な項目
        /// <list type="bullet">
        /// <item><inheritdoc cref="FieldInput.PullDown" path="/summary"/></item>
        /// <item><inheritdoc cref="FieldInput.RadioButton" path="/summary"/></item>
        /// <item><inheritdoc cref="FieldInput.CheckBox" path="/summary"/></item>
        /// </list>
        /// </remarks>
        /// <param name="cancellationToken"><inheritdoc cref="HttpClient.SendAsync(HttpRequestMessage, CancellationToken)" path="/param[@name='cancellationToken']"/></param>
        public ValueTask<IReadOnlyList<EnumOption>> ListAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// <paramref name="id"/>と一致する<inheritdoc cref="Entities.EnumOption" path="/summary"/>を取得します。
        /// <para>※データ連携中の項目は、マスター管理画面で編集できないため取得できません。</para>
        /// <see href="https://developer.kaonavi.jp/api/v2.0/index.html#tag/%E3%83%9E%E3%82%B9%E3%82%BF%E3%83%BC%E7%AE%A1%E7%90%86/paths/~1enum_options~1{custom_field_id}/get"/>
        /// </summary>
        /// <inheritdoc cref="ListAsync" path="/remarks"/>
        /// <param name="id"><inheritdoc cref="EnumOption.Id" path="/summary"/></param>
        /// <param name="cancellationToken"><inheritdoc cref="HttpClient.SendAsync(HttpRequestMessage, CancellationToken)" path="/param[@name='cancellationToken']"/></param>
        public ValueTask<EnumOption> ReadAsync(int id, CancellationToken cancellationToken = default);

        /// <summary>
        /// <paramref name="id"/>と一致する<inheritdoc cref="Entities.EnumOption" path="/summary"/>を一括更新します。
        /// <para>※データ連携中の項目は、マスター管理画面で編集できないため取得できません。</para>
        /// <para>
        /// <paramref name="payload"/>に含まれていないマスターは削除されます。マスターを追加したい場合は、id を指定せず name のみ指定してください。
        /// </para>
        /// <see href="https://developer.kaonavi.jp/api/v2.0/index.html#tag/%E3%83%9E%E3%82%B9%E3%82%BF%E3%83%BC%E7%AE%A1%E7%90%86/paths/~1enum_options~1{custom_field_id}/put"/>
        /// </summary>
        /// <remarks>
        /// <list type="bullet">
        /// <item>並び順は送信された配列順に登録されます。</item>
        /// <item>変更履歴が設定されたシートのマスター情報を更新した際には履歴が作成されます。</item>
        /// <item><paramref name="payload"/>は0件での指定は出来ません。1件以上指定してください。</item>
        /// <item>削除できるマスターは、メンバー数が「0」のマスターのみです。</item>
        /// <item>更新リクエスト制限の対象APIです。</item>
        /// </list>
        /// </remarks>
        /// <param name="id"><inheritdoc cref="EnumOption.Id" path="/summary"/></param>
        /// <param name="payload">リクエスト</param>
        /// <param name="cancellationToken"><inheritdoc cref="HttpClient.SendAsync(HttpRequestMessage, CancellationToken)" path="/param[@name='cancellationToken']"/></param>
        /// <returns><inheritdoc cref="TaskProgress.Id" path="/summary" /></returns>
        public ValueTask<int> UpdateAsync(int id, IReadOnlyList<(int? id, string name)> payload, CancellationToken cancellationToken = default);
    }

    /// <inheritdoc/>
    public IEnumOption EnumOption => this;

    /// <inheritdoc/>
    ValueTask<IReadOnlyList<EnumOption>> IEnumOption.ListAsync(CancellationToken cancellationToken)
        => CallApiAsync(new(HttpMethod.Get, "enum_options"), "custom_field_data", Context.Default.IReadOnlyListEnumOption, cancellationToken);

    /// <inheritdoc/>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="id"/>が0より小さい場合にスローされます。</exception>
    ValueTask<EnumOption> IEnumOption.ReadAsync(int id, CancellationToken cancellationToken)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(id);
        return CallApiAsync(new(HttpMethod.Get, $"enum_options/{id:D}"), Context.Default.EnumOption, cancellationToken);
    }

    /// <inheritdoc/>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="id"/>が0より小さい場合にスローされます。</exception>
    ValueTask<int> IEnumOption.UpdateAsync(int id, IReadOnlyList<(int? id, string name)> payload, CancellationToken cancellationToken)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(id);
        return CallTaskApiAsync(HttpMethod.Put, $"enum_options/{id:D}", payload, "enum_option_data"u8, Context.Default.IReadOnlyListValueTupleNullableInt32String, cancellationToken);
    }
}
