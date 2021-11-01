using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Kaonavi.Net.Entities;
using Kaonavi.Net.Entities.Api;

namespace Kaonavi.Net.Services
{
    /// <summary>カオナビ API v2の抽象化</summary>
    public interface IKaonaviService
    {
        #region Layout
        /// <summary>
        /// 使用可能なメンバーのレイアウト定義情報を全て取得します。
        /// <see href="https://developer.kaonavi.jp/api/v2.0/index.html#tag/%E3%83%AC%E3%82%A4%E3%82%A2%E3%82%A6%E3%83%88%E5%AE%9A%E7%BE%A9/paths/~1member_layouts/get"/>
        /// </summary>
        /// <param name="cancellationToken">キャンセル通知を受け取るために他のオブジェクトまたはスレッドで使用できるキャンセル トークン。</param>
        ValueTask<MemberLayout> FetchMemberLayoutAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// 使用可能なシートのレイアウト定義情報を全て取得します。
        /// <see href="https://developer.kaonavi.jp/api/v2.0/index.html#tag/%E3%83%AC%E3%82%A4%E3%82%A2%E3%82%A6%E3%83%88%E5%AE%9A%E7%BE%A9/paths/~1sheet_layouts/get"/>
        /// </summary>
        /// <inheritdoc cref="FetchMemberLayoutAsync" path="/param" />
        ValueTask<IReadOnlyList<SheetLayout>> FetchSheetLayoutsAsync(CancellationToken cancellationToken = default);
        #endregion

        #region Member
        /// <summary>
        /// 全てのメンバーの基本情報・所属（主務）・兼務情報を取得します。
        /// <see href="https://developer.kaonavi.jp/api/v2.0/index.html#tag/%E3%83%A1%E3%83%B3%E3%83%90%E3%83%BC%E6%83%85%E5%A0%B1/paths/~1members/get"/>
        /// </summary>
        /// <inheritdoc cref="FetchMemberLayoutAsync" path="/param" />
        ValueTask<IReadOnlyList<MemberData>> FetchMembersDataAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// メンバー登録と、合わせて基本情報・所属（主務）・兼務情報を登録します。
        /// <see href="https://developer.kaonavi.jp/api/v2.0/index.html#tag/%E3%83%A1%E3%83%B3%E3%83%90%E3%83%BC%E6%83%85%E5%A0%B1/paths/~1members/post"/>
        /// </summary>
        /// <remarks>更新リクエスト制限の対象APIです。</remarks>
        /// <param name="payload">追加するデータ</param>
        /// <inheritdoc cref="FetchMemberLayoutAsync" path="/param" />
        /// <returns><inheritdoc cref="TaskProgress.Id" path="/summary/text()" /></returns>
        ValueTask<int> AddMemberDataAsync(IReadOnlyList<MemberData> payload, CancellationToken cancellationToken = default);

        /// <summary>
        /// 全てのメンバーの基本情報・所属（主務）・兼務情報を一括更新します。
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
        /// <inheritdoc cref="FetchMemberLayoutAsync" path="/param" />
        /// <returns><inheritdoc cref="TaskProgress.Id" path="/summary/text()" /></returns>
        ValueTask<int> ReplaceMemberDataAsync(IReadOnlyList<MemberData> payload, CancellationToken cancellationToken = default);

        /// <summary>
        /// 送信されたメンバーの基本情報・所属（主務）・兼務情報のみを更新します。
        /// <paramref name="payload"/>に含まれていない情報は更新されません。
        /// 特定の値を削除する場合は、<c>""</c>を送信してください。
        /// <see href="https://developer.kaonavi.jp/api/v2.0/index.html#tag/%E3%83%A1%E3%83%B3%E3%83%90%E3%83%BC%E6%83%85%E5%A0%B1/paths/~1members/patch"/>
        /// </summary>
        /// <remarks>更新リクエスト制限の対象APIです。</remarks>
        /// <param name="payload">更新するデータ</param>
        /// <inheritdoc cref="FetchMemberLayoutAsync" path="/param" />
        /// <returns><inheritdoc cref="TaskProgress.Id" path="/summary/text()" /></returns>
        ValueTask<int> UpdateMemberDataAsync(IReadOnlyList<MemberData> payload, CancellationToken cancellationToken = default);

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
        /// <param name="codes">削除する<inheritdoc cref="MemberData.Code" path="/summary/text()"/>のリスト</param>
        /// <inheritdoc cref="FetchMemberLayoutAsync" path="/param" />
        /// <returns><inheritdoc cref="TaskProgress.Id" path="/summary/text()" /></returns>
        ValueTask<int> DeleteMemberDataAsync(IReadOnlyList<string> codes, CancellationToken cancellationToken = default);
        #endregion

        #region Sheet
        /// <summary>
        /// <paramref name="sheetId"/>と一致するシートの全情報を取得します。
        /// <see href="https://developer.kaonavi.jp/api/v2.0/index.html#tag/%E3%82%B7%E3%83%BC%E3%83%88%E6%83%85%E5%A0%B1/paths/~1sheets~1{sheet_id}/get"/>
        /// </summary>
        /// <param name="sheetId"><inheritdoc cref="SheetLayout.Id" path="/summary/text()" /></param>
        /// <inheritdoc cref="FetchMemberLayoutAsync" path="/param" />
        ValueTask<IReadOnlyList<SheetData>> FetchSheetDataListAsync(int sheetId, CancellationToken cancellationToken = default);

        /// <summary>
        /// <paramref name="sheetId"/>と一致する<inheritdoc cref="SheetData" path="/summary/text()"/>を一括更新します。
        /// <paramref name="payload"/>に含まれていない情報は削除されます。
        /// 複数レコードの情報は送信された配列順に登録されます。
        /// <see href="https://developer.kaonavi.jp/api/v2.0/index.html#tag/%E3%82%B7%E3%83%BC%E3%83%88%E6%83%85%E5%A0%B1/paths/~1sheets~1{sheet_id}/put"/>
        /// </summary>
        /// <remarks>更新リクエスト制限の対象APIです。</remarks>
        /// <param name="sheetId"><inheritdoc cref="SheetLayout.Id" path="/summary/text()" /></param>
        /// <param name="payload">一括更新するデータ</param>
        /// <inheritdoc cref="FetchMemberLayoutAsync" path="/param" />
        /// <returns><inheritdoc cref="TaskProgress.Id" path="/summary" /></returns>
        ValueTask<int> ReplaceSheetDataAsync(int sheetId, IReadOnlyList<SheetData> payload, CancellationToken cancellationToken = default);

        /// <summary>
        /// <paramref name="sheetId"/>と一致する<inheritdoc cref="SheetData" path="/summary/text()"/>の一部を更新します。
        /// <list type="bullet">
        /// <item>
        ///   <term>単一レコード</term>
        ///   <description>
        ///     送信された情報のみが更新されます。
        ///     <paramref name="payload"/> に含まれていない情報は更新されません。
        ///     特定の値を削除する場合は、<c>""</c>を送信してください。
        ///   </description>
        /// </item>
        /// <item>
        ///   <term>複数レコード</term>
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
        /// <remarks>
        /// <list type="bullet">
        /// <item>追加・挿入のみのAPIはありません。値を追加する場合は、元のデータも含めて更新してください。</item>
        /// <item>更新リクエスト制限の対象APIです。</item>
        /// </list>
        /// </remarks>
        /// <param name="sheetId"><inheritdoc cref="SheetLayout.Id" path="/summary/text()" /></param>
        /// <param name="payload">更新するデータ</param>
        /// <inheritdoc cref="FetchMemberLayoutAsync" path="/param" />
        /// <returns><inheritdoc cref="TaskProgress.Id" path="/summary/text()" /></returns>
        ValueTask<int> UpdateSheetDataAsync(int sheetId, IReadOnlyList<SheetData> payload, CancellationToken cancellationToken = default);
        #endregion

        #region Department
        /// <summary>
        /// <inheritdoc cref="DepartmentInfo" path="/summary/text()"/>の情報を取得します。
        /// <see href="https://developer.kaonavi.jp/api/v2.0/index.html#tag/%E6%89%80%E5%B1%9E%E3%83%84%E3%83%AA%E3%83%BC/paths/~1departments/get"/>
        /// </summary>
        /// <inheritdoc cref="FetchMemberLayoutAsync" path="/param" />
        ValueTask<IReadOnlyList<DepartmentInfo>> FetchDepartmentsAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// <inheritdoc cref="DepartmentInfo" path="/summary/text()"/>を一括更新します。
        /// <paramref name="payload"/>に含まれていない情報は削除されます。
        /// <see href="https://developer.kaonavi.jp/api/v2.0/index.html#tag/%E6%89%80%E5%B1%9E%E3%83%84%E3%83%AA%E3%83%BC/paths/~1departments/put"/>
        /// </summary>
        /// <remarks>更新リクエスト制限の対象APIです。</remarks>
        /// <param name="payload">一括更新するデータ</param>
        /// <inheritdoc cref="FetchMemberLayoutAsync" path="/param" />
        /// <returns><inheritdoc cref="TaskProgress.Id" path="/summary/text()" /></returns>
        ValueTask<int> ReplaceDepartmentsAsync(IReadOnlyList<DepartmentInfo> payload, CancellationToken cancellationToken = default);
        #endregion

        /// <summary>
        /// <paramref name="taskId"/>と一致する<inheritdoc cref="TaskProgress" path="/summary/text()"/>を取得します。
        /// <see href="https://developer.kaonavi.jp/api/v2.0/index.html#tag/%E3%82%BF%E3%82%B9%E3%82%AF%E9%80%B2%E6%8D%97%E7%8A%B6%E6%B3%81"/>
        /// </summary>
        /// <param name="taskId"><inheritdoc cref="TaskProgress.Id" path="/summary/text()"/></param>
        /// <inheritdoc cref="FetchMemberLayoutAsync" path="/param" />
        ValueTask<TaskProgress> FetchTaskProgressAsync(int taskId, CancellationToken cancellationToken = default);

        #region User
        /// <summary>
        /// <inheritdoc cref="User" path="/summary/text()"/>の一覧を取得します。
        /// <see href="https://developer.kaonavi.jp/api/v2.0/index.html#tag/%E3%83%A6%E3%83%BC%E3%82%B6%E3%83%BC%E6%83%85%E5%A0%B1/paths/~1users/get"/>
        /// </summary>
        /// <inheritdoc cref="FetchMemberLayoutAsync" path="/param"/>
        ValueTask<IReadOnlyList<User>> FetchUsersAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// <inheritdoc cref="User" path="/summary/text()"/>を登録します。
        /// <see href="https://developer.kaonavi.jp/api/v2.0/index.html#tag/%E3%83%A6%E3%83%BC%E3%82%B6%E3%83%BC%E6%83%85%E5%A0%B1/paths/~1users/post"/>
        /// </summary>
        /// <param name="payload">リクエスト</param>
        /// <inheritdoc cref="FetchMemberLayoutAsync" path="/param"/>
        /// <remarks>
        /// 管理者メニュー > ユーザー管理 にてユーザー作成時に設定可能なオプションについては、以下の内容で作成されます。
        /// <list type="bullet">
        /// <item>
        ///   <term>スマホオプション</term>
        ///   <description>停止</description>
        /// </item>
        /// <item>
        ///   <term>セキュアアクセス</term>
        ///   <description>停止</description>
        /// </item>
        /// </list>
        /// </remarks>
        ValueTask<User> AddUserAsync(UserPayload payload, CancellationToken cancellationToken = default);

        /// <summary>
        /// <paramref name="userId"/>と一致する<inheritdoc cref="User" path="/summary/text()"/>を取得します。
        /// <see href="https://developer.kaonavi.jp/api/v2.0/index.html#tag/%E3%83%A6%E3%83%BC%E3%82%B6%E3%83%BC%E6%83%85%E5%A0%B1/paths/~1users~1{user_id}/get"/>
        /// </summary>
        /// <param name="userId"><inheritdoc cref="User.Id" path="/summary"/></param>
        /// <inheritdoc cref="FetchMemberLayoutAsync" path="/param"/>
        ValueTask<User> FetchUserAsync(int userId, CancellationToken cancellationToken = default);

        /// <summary>
        /// <paramref name="userId"/>と一致する<inheritdoc cref="User" path="/summary/text()"/>を更新します。
        /// <see href="https://developer.kaonavi.jp/api/v2.0/index.html#tag/%E3%83%A6%E3%83%BC%E3%82%B6%E3%83%BC%E6%83%85%E5%A0%B1/paths/~1users~1{user_id}/get"/>
        /// </summary>
        /// <param name="userId"><inheritdoc cref="User.Id" path="/summary/text()"/></param>
        /// <param name="payload">リクエスト</param>
        /// <param name="cancellationToken">キャンセル通知を受け取るために他のオブジェクトまたはスレッドで使用できるキャンセル トークン。</param>
        /// <remarks>
        /// 管理者メニュー > ユーザー管理 にて更新可能な以下のオプションについては元の値が維持されます。
        /// <list type="bullet">
        /// <item>スマホオプション</item>
        /// <item>セキュアアクセス</item>
        /// <item>アカウント状態</item>
        /// <item>パスワードロック</item>
        /// </list>
        /// </remarks>
        ValueTask<User> UpdateUserAsync(int userId, UserPayload payload, CancellationToken cancellationToken = default);

        /// <summary>
        /// <paramref name="userId"/>と一致する<inheritdoc cref="User" path="/summary/text()"/>を削除します。
        /// <see href="https://developer.kaonavi.jp/api/v2.0/index.html#tag/%E3%83%A6%E3%83%BC%E3%82%B6%E3%83%BC%E6%83%85%E5%A0%B1/paths/~1users~1{user_id}/delete"/>
        /// </summary>
        /// <param name="userId"><inheritdoc cref="User.Id" path="/summary/text()"/></param>
        /// <inheritdoc cref="FetchMemberLayoutAsync" path="/param"/>
        ValueTask DeleteUserAsync(int userId, CancellationToken cancellationToken = default);
        #endregion

        /// <summary>
        /// <inheritdoc cref="Role" path="/summary/text()"/>の一覧を取得します。
        /// <see href="https://developer.kaonavi.jp/api/v2.0/index.html#tag/%E3%83%AD%E3%83%BC%E3%83%AB/paths/~1roles/get"/>
        /// </summary>
        /// <inheritdoc cref="FetchMemberLayoutAsync" path="/param"/>
        ValueTask<IReadOnlyList<Role>> FetchRolesAsync(CancellationToken cancellationToken = default);
    }
}
