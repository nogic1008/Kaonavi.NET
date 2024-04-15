using Kaonavi.Net.Entities;

namespace Kaonavi.Net.Api;

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
    /// <param name="cancellationToken">キャンセル通知を受け取るために他のオブジェクトまたはスレッドで使用できるキャンセル トークン。</param>
    ValueTask<IReadOnlyList<MemberData>> ListAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// メンバー登録と、合わせて基本情報・所属(主務)・兼務情報を登録します。
    /// <see href="https://developer.kaonavi.jp/api/v2.0/index.html#tag/%E3%83%A1%E3%83%B3%E3%83%90%E3%83%BC%E6%83%85%E5%A0%B1/paths/~1members/post"/>
    /// </summary>
    /// <remarks>更新リクエスト制限の対象APIです。</remarks>
    /// <param name="payload">追加するデータ</param>
    /// <param name="cancellationToken">キャンセル通知を受け取るために他のオブジェクトまたはスレッドで使用できるキャンセル トークン。</param>
    ValueTask<int> CreateAsync(IReadOnlyList<MemberData> payload, CancellationToken cancellationToken = default);

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
    /// <param name="cancellationToken">キャンセル通知を受け取るために他のオブジェクトまたはスレッドで使用できるキャンセル トークン。</param>
    /// <returns><inheritdoc cref="TaskProgress" path="/param[@name='Id']/text()"/></returns>
    ValueTask<int> ReplaceAsync(IReadOnlyList<MemberData> payload, CancellationToken cancellationToken = default);

    /// <summary>
    /// 送信されたメンバーの基本情報・所属(主務)・兼務情報のみを更新します。
    /// <paramref name="payload"/>に含まれていない情報は更新されません。
    /// 特定の値を削除する場合は、<c>""</c>を送信してください。
    /// <see href="https://developer.kaonavi.jp/api/v2.0/index.html#tag/%E3%83%A1%E3%83%B3%E3%83%90%E3%83%BC%E6%83%85%E5%A0%B1/paths/~1members/patch"/>
    /// </summary>
    /// <remarks>更新リクエスト制限の対象APIです。</remarks>
    /// <param name="payload">更新するデータ</param>
    /// <param name="cancellationToken">キャンセル通知を受け取るために他のオブジェクトまたはスレッドで使用できるキャンセル トークン。</param>
    /// <returns><inheritdoc cref="TaskProgress" path="/param[@name='Id']/text()"/></returns>
    ValueTask<int> UpdateAsync(IReadOnlyList<MemberData> payload, CancellationToken cancellationToken = default);

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
    /// <param name="cancellationToken">キャンセル通知を受け取るために他のオブジェクトまたはスレッドで使用できるキャンセル トークン。</param>
    /// <returns><inheritdoc cref="TaskProgress" path="/param[@name='Id']/text()"/></returns>
    ValueTask<int> OverWriteAsync(IReadOnlyList<MemberData> payload, CancellationToken cancellationToken = default);

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
    /// <param name="codes">削除する<inheritdoc cref="MemberData" path="/param[@name='Code']/text()"/>のリスト</param>
    /// <param name="cancellationToken">キャンセル通知を受け取るために他のオブジェクトまたはスレッドで使用できるキャンセル トークン。</param>
    /// <returns><inheritdoc cref="TaskProgress" path="/param[@name='Id']/text()"/></returns>
    ValueTask<int> DeleteAsync(IReadOnlyList<string> codes, CancellationToken cancellationToken = default);
}
