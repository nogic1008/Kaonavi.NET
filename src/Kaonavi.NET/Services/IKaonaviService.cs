using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Kaonavi.Net.Entities;
using Kaonavi.Net.Entities.Api;

namespace Kaonavi.Net.Services
{
    /// <summary>
    /// カオナビ API v2の抽象化
    /// </summary>
    public interface IKaonaviService
    {
        /// <summary>
        /// 使用可能なメンバーのレイアウト定義情報を全て取得します。
        /// https://developer.kaonavi.jp/api/v2.0/index.html#tag/%E3%83%AC%E3%82%A4%E3%82%A2%E3%82%A6%E3%83%88%E5%AE%9A%E7%BE%A9/paths/~1member_layouts/get
        /// </summary>
        /// <param name="cancellationToken">キャンセル通知を受け取るために他のオブジェクトまたはスレッドで使用できるキャンセル トークン。</param>
        ValueTask<MemberLayout> FetchMemberLayoutAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// 使用可能なシートのレイアウト定義情報を全て取得します。
        /// https://developer.kaonavi.jp/api/v2.0/index.html#tag/%E3%83%AC%E3%82%A4%E3%82%A2%E3%82%A6%E3%83%88%E5%AE%9A%E7%BE%A9/paths/~1sheet_layouts/get
        /// </summary>
        /// <param name="cancellationToken">キャンセル通知を受け取るために他のオブジェクトまたはスレッドで使用できるキャンセル トークン。</param>
        ValueTask<IReadOnlyList<SheetLayout>> FetchSheetLayoutsAsync(CancellationToken cancellationToken = default);

        #region Member
        /// <summary>
        /// 全てのメンバーの基本情報・所属（主務）・兼務情報を取得します。
        /// https://developer.kaonavi.jp/api/v2.0/index.html#tag/%E3%83%A1%E3%83%B3%E3%83%90%E3%83%BC%E6%83%85%E5%A0%B1/paths/~1members/get
        /// </summary>
        /// <param name="cancellationToken">キャンセル通知を受け取るために他のオブジェクトまたはスレッドで使用できるキャンセル トークン。</param>
        ValueTask<IReadOnlyList<MemberData>> FetchMembersDataAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// メンバー登録と、合わせて基本情報・所属（主務）・兼務情報を登録します。
        /// https://developer.kaonavi.jp/api/v2.0/index.html#tag/%E3%83%A1%E3%83%B3%E3%83%90%E3%83%BC%E6%83%85%E5%A0%B1/paths/~1members/post
        /// </summary>
        /// <remarks>更新リクエスト制限の対象APIです。</remarks>
        /// <param name="payload">追加するデータ</param>
        /// <param name="cancellationToken">キャンセル通知を受け取るために他のオブジェクトまたはスレッドで使用できるキャンセル トークン。</param>
        /// <returns><see cref="TaskProgress.Id"/></returns>
        ValueTask<int> AddMemberDataAsync(IReadOnlyList<MemberData> payload, CancellationToken cancellationToken = default);

        /// <summary>
        /// 全てのメンバーの基本情報・所属（主務）・兼務情報を一括更新します。
        /// Request Body に含まれていない情報は削除されます。
        /// https://developer.kaonavi.jp/api/v2.0/index.html#tag/%E3%83%A1%E3%83%B3%E3%83%90%E3%83%BC%E6%83%85%E5%A0%B1/paths/~1members/put
        /// </summary>
        /// <remarks>
        /// メンバーの登録・削除は行われません。
        /// 更新リクエスト制限の対象APIです。
        /// </remarks>
        /// <param name="payload">一括更新するデータ</param>
        /// <param name="cancellationToken">キャンセル通知を受け取るために他のオブジェクトまたはスレッドで使用できるキャンセル トークン。</param>
        /// <returns><see cref="TaskProgress.Id"/></returns>
        ValueTask<int> ReplaceMemberDataAsync(IReadOnlyList<MemberData> payload, CancellationToken cancellationToken = default);

        /// <summary>
        /// 送信されたメンバーの基本情報・所属（主務）・兼務情報のみを更新します。
        /// Request Body に含まれていない情報は更新されません。
        /// 特定の値を削除する場合は、空文字 "" を送信してください。
        /// https://developer.kaonavi.jp/api/v2.0/index.html#tag/%E3%83%A1%E3%83%B3%E3%83%90%E3%83%BC%E6%83%85%E5%A0%B1/paths/~1members/patch
        /// </summary>
        /// <remarks>更新リクエスト制限の対象APIです。</remarks>
        /// <param name="payload">更新するデータ</param>
        /// <param name="cancellationToken">キャンセル通知を受け取るために他のオブジェクトまたはスレッドで使用できるキャンセル トークン。</param>
        /// <returns><see cref="TaskProgress.Id"/></returns>
        ValueTask<int> UpdateMemberDataAsync(IReadOnlyList<MemberData> payload, CancellationToken cancellationToken = default);
        #endregion

        #region Sheet
        /// <summary>
        /// 指定したシートの全情報を取得します。
        /// </summary>
        /// <param name="sheetId">シートID</param>
        /// <param name="cancellationToken">キャンセル通知を受け取るために他のオブジェクトまたはスレッドで使用できるキャンセル トークン。</param>
        ValueTask<IReadOnlyList<SheetData>> FetchSheetDataListAsync(int sheetId, CancellationToken cancellationToken = default);

        /// <summary>
        /// 指定したシートのシート情報を一括更新します。
        /// <paramref name="payload"/> に含まれていない情報は削除されます。
        /// 複数レコードの情報は送信された配列順に登録されます。
        /// </summary>
        /// <param name="sheetId">シートID</param>
        /// <param name="payload">一括更新するデータ</param>
        /// <param name="cancellationToken">キャンセル通知を受け取るために他のオブジェクトまたはスレッドで使用できるキャンセル トークン。</param>
        /// <returns><see cref="TaskProgress.Id"/></returns>
        ValueTask<int> ReplaceSheetDataAsync(int sheetId, IReadOnlyList<SheetData> payload, CancellationToken cancellationToken = default);

        /// <summary>
        /// 指定したシートのシート情報の一部を更新します。
        /// <para>
        /// 単一レコード
        /// 送信された情報のみが更新されます。
        /// <paramref name="payload"/> に含まれていない情報は更新されません。
        /// 特定の値を削除する場合は、<c>""</c> を送信してください。
        /// </para>
        /// <para>
        /// 複数レコード
        /// メンバーごとのデータが一括更新されます。
        /// 特定の値を削除する場合は、<c>""</c> を送信してください。
        /// 特定のレコードだけを更新することは出来ません。
        /// <see cref="SheetData.Code"/> が指定されていないメンバーの情報は更新されません。
        /// 送信された配列順に登録されます。
        /// </para>
        /// </summary>
        /// <param name="sheetId">シートID</param>
        /// <param name="payload">更新するデータ</param>
        /// <param name="cancellationToken">キャンセル通知を受け取るために他のオブジェクトまたはスレッドで使用できるキャンセル トークン。</param>
        /// <returns><see cref="TaskProgress.Id"/></returns>
        ValueTask<int> UpdateSheetDataAsync(int sheetId, IReadOnlyList<SheetData> payload, CancellationToken cancellationToken = default);
        #endregion

        /// <summary>
        /// 所属情報の一覧を取得します。
        /// https://developer.kaonavi.jp/api/v2.0/index.html#tag/%E6%89%80%E5%B1%9E%E3%83%84%E3%83%AA%E3%83%BC/paths/~1departments/get
        /// </summary>
        /// <param name="cancellationToken">キャンセル通知を受け取るために他のオブジェクトまたはスレッドで使用できるキャンセル トークン。</param>
        ValueTask<IReadOnlyList<DepartmentInfo>> FetchDepartmentsAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// <paramref name="taskId"/>と一致するタスクの進捗状況を取得します。
        /// </summary>
        /// <param name="taskId">タスクID</param>
        /// <param name="cancellationToken">キャンセル通知を受け取るために他のオブジェクトまたはスレッドで使用できるキャンセル トークン。</param>
        ValueTask<TaskProgress> FetchTaskProgressAsync(int taskId, CancellationToken cancellationToken = default);

        #region User
        /// <summary>
        /// ユーザー情報の一覧を取得します。
        /// https://developer.kaonavi.jp/api/v2.0/index.html#tag/%E3%83%A6%E3%83%BC%E3%82%B6%E3%83%BC%E6%83%85%E5%A0%B1/paths/~1users/get
        /// </summary>
        /// <param name="cancellationToken">キャンセル通知を受け取るために他のオブジェクトまたはスレッドで使用できるキャンセル トークン。</param>
        ValueTask<IReadOnlyList<User>> FetchUsersAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// ユーザー情報を登録します。
        /// https://developer.kaonavi.jp/api/v2.0/index.html#tag/%E3%83%A6%E3%83%BC%E3%82%B6%E3%83%BC%E6%83%85%E5%A0%B1/paths/~1users/post
        /// </summary>
        /// <param name="payload">リクエスト</param>
        /// <param name="cancellationToken">キャンセル通知を受け取るために他のオブジェクトまたはスレッドで使用できるキャンセル トークン。</param>
        /// <remarks>
        /// 管理者メニュー > ユーザー管理 にてユーザー作成時に設定可能なオプションについては、以下の内容で作成されます。
        /// - スマホオプション: 停止
        /// - セキュアアクセス: 停止
        /// </remarks>
        ValueTask<User> AddUserAsync(UserPayload payload, CancellationToken cancellationToken = default);

        /// <summary>
        /// <paramref name="userId"/>と一致するログインユーザー情報を取得します。
        /// https://developer.kaonavi.jp/api/v2.0/index.html#tag/%E3%83%A6%E3%83%BC%E3%82%B6%E3%83%BC%E6%83%85%E5%A0%B1/paths/~1users~1{user_id}/get
        /// </summary>
        /// <param name="userId">ユーザーID</param>
        /// <param name="cancellationToken">キャンセル通知を受け取るために他のオブジェクトまたはスレッドで使用できるキャンセル トークン。</param>
        ValueTask<User> FetchUserAsync(int userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// <paramref name="userId"/>と一致するログインユーザー情報を更新します。
        /// https://developer.kaonavi.jp/api/v2.0/index.html#tag/%E3%83%A6%E3%83%BC%E3%82%B6%E3%83%BC%E6%83%85%E5%A0%B1/paths/~1users~1{user_id}/patch
        /// </summary>
        /// <param name="userId">ユーザーID</param>
        /// <param name="payload">リクエスト</param>
        /// <param name="cancellationToken">キャンセル通知を受け取るために他のオブジェクトまたはスレッドで使用できるキャンセル トークン。</param>
        /// <remarks>
        /// 管理者メニュー > ユーザー管理 にて更新可能な以下のオプションについては元の値が維持されます。
        /// - スマホオプション
        /// - セキュアアクセス
        /// - アカウント状態
        /// - パスワードロック
        /// </remarks>
        ValueTask<User> UpdateUserAsync(int userId, UserPayload payload, CancellationToken cancellationToken = default);

        /// <summary>
        /// <paramref name="userId"/>と一致するログインユーザー情報を削除します。
        /// https://developer.kaonavi.jp/api/v2.0/index.html#tag/%E3%83%A6%E3%83%BC%E3%82%B6%E3%83%BC%E6%83%85%E5%A0%B1/paths/~1users~1{user_id}/delete
        /// </summary>
        /// <param name="userId">ユーザーID</param>
        /// <param name="cancellationToken">キャンセル通知を受け取るために他のオブジェクトまたはスレッドで使用できるキャンセル トークン。</param>
        ValueTask DeleteUserAsync(int userId, CancellationToken cancellationToken = default);
        #endregion

        /// <summary>
        /// ロール情報の一覧を取得します。
        /// https://developer.kaonavi.jp/api/v2.0/index.html#tag/%E3%83%AD%E3%83%BC%E3%83%AB/paths/~1roles/get
        /// </summary>
        /// <param name="cancellationToken">キャンセル通知を受け取るために他のオブジェクトまたはスレッドで使用できるキャンセル トークン。</param>
        ValueTask<IReadOnlyList<Role>> FetchRolesAsync(CancellationToken cancellationToken = default);
    }
}
