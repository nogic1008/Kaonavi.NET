using System.Buffers;
using Kaonavi.Net.Entities;
using Kaonavi.Net.Json;

namespace Kaonavi.Net;

public partial class KaonaviClient : KaonaviClient.IMember
{
    /// <summary>
    /// メンバー情報 API
    /// <see href="https://developer.kaonavi.jp/api/v2.0/index.html#tag/%E3%83%A1%E3%83%B3%E3%83%90%E3%83%BC%E6%83%85%E5%A0%B1"/>
    /// </summary>
    public interface IMember
    {
        /// <summary>
        /// 全てのメンバーの基本情報・所属(主務)・兼務情報を取得します。
        /// <see href="https://developer.kaonavi.jp/api/v2.0/index.html#tag/%E3%83%A1%E3%83%B3%E3%83%90%E3%83%BC%E6%83%85%E5%A0%B1/paths/~1members/get"/>
        /// </summary>
        /// <param name="cancellationToken"><inheritdoc cref="HttpClient.SendAsync(HttpRequestMessage, CancellationToken)" path="/param[@name='cancellationToken']"/></param>
        public ValueTask<IReadOnlyList<MemberData>> ListAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// メンバー登録と、合わせて基本情報・所属(主務)・兼務情報を登録します。
        /// <see href="https://developer.kaonavi.jp/api/v2.0/index.html#tag/%E3%83%A1%E3%83%B3%E3%83%90%E3%83%BC%E6%83%85%E5%A0%B1/paths/~1members/post"/>
        /// </summary>
        /// <remarks>更新リクエスト制限の対象APIです。</remarks>
        /// <param name="payload">追加するデータ</param>
        /// <param name="cancellationToken"><inheritdoc cref="HttpClient.SendAsync(HttpRequestMessage, CancellationToken)" path="/param[@name='cancellationToken']"/></param>
        public ValueTask<int> CreateAsync(IReadOnlyList<MemberData> payload, CancellationToken cancellationToken = default);

        /// <summary>
        /// 全てのメンバーの基本情報・所属(主務)・兼務情報を一括更新します。
        /// <paramref name="payload"/>に含まれていない情報は削除されます。
        /// <see href="https://developer.kaonavi.jp/api/v2.0/index.html#tag/%E3%83%A1%E3%83%B3%E3%83%90%E3%83%BC%E6%83%85%E5%A0%B1/paths/~1members/put"/>
        /// </summary>
        /// <remarks>
        /// <list type="bullet">
        /// <item>メンバーの登録・削除は行われません。</item>
        /// <item>更新リクエスト制限の対象APIです。</item>
        /// </list>
        /// </remarks>
        /// <param name="payload">一括更新するデータ</param>
        /// <param name="cancellationToken"><inheritdoc cref="HttpClient.SendAsync(HttpRequestMessage, CancellationToken)" path="/param[@name='cancellationToken']"/></param>
        /// <returns><inheritdoc cref="TaskProgress" path="/param[@name='Id']"/></returns>
        public ValueTask<int> ReplaceAsync(IReadOnlyList<MemberData> payload, CancellationToken cancellationToken = default);

        /// <summary>
        /// 送信されたメンバーの基本情報・所属(主務)・兼務情報のみを更新します。
        /// <paramref name="payload"/>に含まれていない情報は更新されません。
        /// 特定の値を削除する場合は、<c>""</c>を送信してください。
        /// <see href="https://developer.kaonavi.jp/api/v2.0/index.html#tag/%E3%83%A1%E3%83%B3%E3%83%90%E3%83%BC%E6%83%85%E5%A0%B1/paths/~1members/patch"/>
        /// </summary>
        /// <remarks>更新リクエスト制限の対象APIです。</remarks>
        /// <param name="payload">更新するデータ</param>
        /// <param name="cancellationToken"><inheritdoc cref="HttpClient.SendAsync(HttpRequestMessage, CancellationToken)" path="/param[@name='cancellationToken']"/></param>
        /// <returns><inheritdoc cref="TaskProgress" path="/param[@name='Id']"/></returns>
        public ValueTask<int> UpdateAsync(IReadOnlyList<MemberData> payload, CancellationToken cancellationToken = default);

        /// <summary>
        /// 現在登録されているメンバーとそれに紐づく基本情報・所属(主務)・兼務情報を全て、<paramref name="payload"/>で入れ替えます。
        /// <list type="bullet">
        /// <item>存在しない社員番号を指定した場合、新しくメンバーを登録します。</item>
        /// <item>存在する社員番号を指定した場合、メンバー情報を更新します。</item>
        /// <item>存在する社員番号を指定していない場合、メンバーを削除します。</item>
        /// </list>
        /// <see href="https://developer.kaonavi.jp/api/v2.0/index.html#tag/%E3%83%A1%E3%83%B3%E3%83%90%E3%83%BC%E6%83%85%E5%A0%B1/paths/~1members~1overwrite/put"/>
        /// </summary>
        /// <remarks>
        /// <list type="bullet">
        /// <item>更新リクエスト制限の対象APIです。</item>
        /// <item>メンバーの削除はリクエスト時に登録処理が完了しているメンバーに対してのみ実行されます。</item>
        /// </list>
        /// </remarks>
        /// <param name="payload">入れ替え対象となるデータ</param>
        /// <param name="cancellationToken"><inheritdoc cref="HttpClient.SendAsync(HttpRequestMessage, CancellationToken)" path="/param[@name='cancellationToken']"/></param>
        /// <returns><inheritdoc cref="TaskProgress" path="/param[@name='Id']"/></returns>
        public ValueTask<int> OverWriteAsync(IReadOnlyList<MemberData> payload, CancellationToken cancellationToken = default);

        /// <summary>
        /// 送信されたメンバーを削除します。
        /// 紐付けユーザーがいる場合、そのアカウント種別によってはユーザーも同時に削除されます。
        /// <list type="bullet">
        /// <item>一般の場合、ユーザーも同時に削除されます。</item>
        /// <item>Admの場合、ユーザーの紐付けが解除。引き続きそのユーザーでログインすることは可能です。</item>
        /// </list>
        /// <see href="https://developer.kaonavi.jp/api/v2.0/index.html#tag/%E3%83%A1%E3%83%B3%E3%83%90%E3%83%BC%E6%83%85%E5%A0%B1/paths/~1members~1delete/post"/>
        /// </summary>
        /// <remarks>更新リクエスト制限の対象APIです。</remarks>
        /// <param name="codes">削除する<inheritdoc cref="MemberData" path="/param[@name='Code']"/>のリスト</param>
        /// <param name="cancellationToken"><inheritdoc cref="HttpClient.SendAsync(HttpRequestMessage, CancellationToken)" path="/param[@name='cancellationToken']"/></param>
        /// <returns><inheritdoc cref="TaskProgress" path="/param[@name='Id']"/></returns>
        public ValueTask<int> DeleteAsync(IReadOnlyList<string> codes, CancellationToken cancellationToken = default);

        /// <summary>
        /// メンバーの顔写真をダウンロードするためのURL一覧を取得します。
        /// <list type="bullet">
        /// <item>URLの有効期限は15分です。ファイル数や回線状況によっては期限内に全てのダウンロードが完了しない場合があります。その際は、並行処理や一括取得の再リクエストなどの方法をご検討ください。</item>
        /// <item>URLは発行時点のメンバーの顔写真に紐づいています。</item>
        /// <item>取得される顔写真はjpg/jpeg形式です。</item>
        /// </list>
        /// </summary>
        /// <param name="updatedSince">指定した日以降に顔写真が更新されたメンバーに絞り込みます。</param>
        /// <param name="cancellationToken"><inheritdoc cref="HttpClient.SendAsync(HttpRequestMessage, CancellationToken)" path="/param[@name='cancellationToken']"/></param>
        public ValueTask<IReadOnlyList<FaceImageInfo>> GetFaceImageListAsync(DateOnly updatedSince, CancellationToken cancellationToken = default);

        /// <summary>
        /// 指定したメンバーの顔写真をアップロードします。
        /// <list type="bullet">
        /// <item>顔写真が未登録の場合、アップロードが可能です。</item>
        /// <item>顔写真が登録済みの場合、エラーになります。</item>
        /// </list>
        /// <para>
        /// 1リクエストあたりの上限
        /// <list type="bullet">
        /// <item><term>メンバー数</term><description>100メンバー</description></item>
        /// <item><term>ファイルサイズ</term><description>各5MBまで</description></item>
        /// </list>
        /// </para>
        /// <see href="https://developer.kaonavi.jp/api/v2.0/index.html#tag/%E3%83%A1%E3%83%B3%E3%83%90%E3%83%BC%E6%83%85%E5%A0%B1/paths/~1members~1face_image/post"/>
        /// </summary>
        /// <remarks>更新リクエスト制限の対象APIです。</remarks>
        /// <param name="payload">追加対象となるデータ</param>
        /// <param name="enableTrimming">
        /// 顔写真の中の顔の位置を自動認識して、トリミングする機能を利用する
        /// <list type="bullet">
        /// <item><term><see langword="true"/></term><description>有効</description></item>
        /// <item><term><see langword="false"/></term><description>無効</description></item>
        /// </list>
        /// </param>
        /// <param name="cancellationToken"><inheritdoc cref="HttpClient.SendAsync(HttpRequestMessage, CancellationToken)" path="/param[@name='cancellationToken']"/></param>
        /// <returns><inheritdoc cref="TaskProgress" path="/param[@name='Id']"/></returns>
        public ValueTask<int> AddFaceImageAsync(IReadOnlyList<FaceImage> payload, bool enableTrimming = true, CancellationToken cancellationToken = default);

        /// <summary>
        /// 指定したメンバーの顔写真を置き換えます。
        /// <list type="bullet">
        /// <item>顔写真が登録済みの場合、顔写真を置き換えます。</item>
        /// <item>顔写真が未登録の場合、エラーになります。</item>
        /// </list>
        /// <para>
        /// 1リクエストあたりの上限
        /// <list type="bullet">
        /// <item><term>メンバー数</term><description>100メンバー</description></item>
        /// <item><term>ファイルサイズ</term><description>各5MBまで</description></item>
        /// </list>
        /// </para>
        /// <see href="https://developer.kaonavi.jp/api/v2.0/index.html#tag/%E3%83%A1%E3%83%B3%E3%83%90%E3%83%BC%E6%83%85%E5%A0%B1/paths/~1members~1face_image/post"/>
        /// </summary>
        /// <remarks>更新リクエスト制限の対象APIです。</remarks>
        /// <param name="payload">更新対象となるデータ</param>
        /// <param name="enableTrimming"><inheritdoc cref="IMember.AddFaceImageAsync" path="/param[@name='enableTrimming']"/></param>
        /// <param name="cancellationToken"><inheritdoc cref="HttpClient.SendAsync(HttpRequestMessage, CancellationToken)" path="/param[@name='cancellationToken']"/></param>
        /// <returns><inheritdoc cref="TaskProgress" path="/param[@name='Id']"/></returns>
        public ValueTask<int> UpdateFaceImageAsync(IReadOnlyList<FaceImage> payload, bool enableTrimming = true, CancellationToken cancellationToken = default);
    }

    /// <inheritdoc/>
    public IMember Member => this;

    /// <inheritdoc/>
    ValueTask<IReadOnlyList<MemberData>> IMember.ListAsync(CancellationToken cancellationToken)
        => CallApiAsync(new(HttpMethod.Get, "members"), "member_data", Context.Default.IReadOnlyListMemberData, cancellationToken);

    /// <inheritdoc/>
    ValueTask<int> IMember.CreateAsync(IReadOnlyList<MemberData> payload, CancellationToken cancellationToken)
        => CallTaskApiAsync(HttpMethod.Post, "members", payload, "member_data"u8, Context.Default.IReadOnlyListMemberData, cancellationToken);

    /// <inheritdoc/>
    ValueTask<int> IMember.ReplaceAsync(IReadOnlyList<MemberData> payload, CancellationToken cancellationToken)
        => CallTaskApiAsync(HttpMethod.Put, "members", payload, "member_data"u8, Context.Default.IReadOnlyListMemberData, cancellationToken);

    /// <inheritdoc/>
    ValueTask<int> IMember.UpdateAsync(IReadOnlyList<MemberData> payload, CancellationToken cancellationToken)
        => CallTaskApiAsync(HttpMethod.Patch, "members", payload, "member_data"u8, Context.Default.IReadOnlyListMemberData, cancellationToken);

    /// <inheritdoc/>
    ValueTask<int> IMember.OverWriteAsync(IReadOnlyList<MemberData> payload, CancellationToken cancellationToken)
        => CallTaskApiAsync(HttpMethod.Put, "members/overwrite", payload, "member_data"u8, Context.Default.IReadOnlyListMemberData, cancellationToken);

    /// <inheritdoc/>
    ValueTask<int> IMember.DeleteAsync(IReadOnlyList<string> codes, CancellationToken cancellationToken)
        => CallTaskApiAsync(HttpMethod.Post, "members/delete", codes, "codes"u8, Context.Default.IReadOnlyListString, cancellationToken);

    /// <inheritdoc/>
    ValueTask<IReadOnlyList<FaceImageInfo>> IMember.GetFaceImageListAsync(DateOnly updatedSince, CancellationToken cancellationToken)
        => CallApiAsync(new(HttpMethod.Get, $"members/face_image?updated_since={updatedSince:yyyy-MM-dd}"), "member_data", Context.Default.IReadOnlyListFaceImageInfo, cancellationToken);

    /// <inheritdoc/>
    ValueTask<int> IMember.AddFaceImageAsync(IReadOnlyList<FaceImage> payload, bool enableTrimming, CancellationToken cancellationToken)
        => CallFaceImageApiAsync(HttpMethod.Post, payload, enableTrimming, cancellationToken);

    /// <inheritdoc/>
    ValueTask<int> IMember.UpdateFaceImageAsync(IReadOnlyList<FaceImage> payload, bool enableTrimming, CancellationToken cancellationToken)
        => CallFaceImageApiAsync(HttpMethod.Patch, payload, enableTrimming, cancellationToken);

    /// <summary>
    /// リクエストのJSON Bodyを生成し、メンバー情報 顔写真APIを呼び出します。
    /// </summary>
    /// <param name="method">HTTPメソッド</param>
    /// <param name="enableTrimming"><inheritdoc cref="IMember.AddFaceImageAsync" path="/param[@name='enableTrimming']"/></param>
    /// <param name="payload">追加/更新対象となるデータ</param>
    /// <param name="cancellationToken"><inheritdoc cref="HttpClient.SendAsync(HttpRequestMessage, CancellationToken)" path="/param[@name='cancellationToken']"/></param>
    /// <returns><inheritdoc cref="TaskProgress" path="/param[@name='Id']"/></returns>
    /// <inheritdoc cref="ObjectDisposedException.ThrowIf(bool, Type)" path="/exception"/>
    private ValueTask<int> CallFaceImageApiAsync(HttpMethod method, IReadOnlyList<FaceImage> payload, bool enableTrimming, CancellationToken cancellationToken)
    {
        ObjectDisposedException.ThrowIf(_disposedValue, GetType());

        var buffer = new ArrayBufferWriter<byte>();
        using var writer = new Utf8JsonWriter(buffer);
        writer.WriteStartObject();
        writer.WriteBoolean("enable_trimming"u8, enableTrimming);
        writer.WritePropertyName("member_data"u8);
        JsonSerializer.Serialize(writer, payload, Context.Default.IReadOnlyListFaceImage);
        writer.WriteEndObject();
        writer.Flush();

        return CallRequestLimitApiAsync(new(method, "members/face_image")
        {
            Content = new ReadOnlyMemoryContent(buffer.WrittenMemory)
            {
                Headers = { ContentType = new("application/json") }
            }
        }, cancellationToken);
    }
}
