using Kaonavi.Net.Entities;

namespace Kaonavi.Net.Services;

/// <summary>カオナビ API v2の抽象化</summary>
public interface IKaonaviService
{
    #region タスク進捗状況
    /// <summary>
    /// <paramref name="taskId"/>と一致する<inheritdoc cref="TaskProgress" path="/summary/text()"/>を取得します。
    /// <see href="https://developer.kaonavi.jp/api/v2.0/index.html#tag/%E3%82%BF%E3%82%B9%E3%82%AF%E9%80%B2%E6%8D%97%E7%8A%B6%E6%B3%81/paths/~1tasks~1{task_id}/get"/>
    /// </summary>
    /// <param name="taskId"><inheritdoc cref="TaskProgress" path="/param[@name='Id']/text()"/></param>
    /// <param name="cancellationToken">キャンセル通知を受け取るために他のオブジェクトまたはスレッドで使用できるキャンセル トークン。</param>
    ValueTask<TaskProgress> FetchTaskProgressAsync(int taskId, CancellationToken cancellationToken = default);
    #endregion タスク進捗状況

    #region レイアウト設定
    /// <summary>
    /// 使用可能なメンバーのレイアウト設定情報を全て取得します。
    /// <see href="https://developer.kaonavi.jp/api/v2.0/index.html#tag/%E3%83%AC%E3%82%A4%E3%82%A2%E3%82%A6%E3%83%88%E8%A8%AD%E5%AE%9A/paths/~1member_layouts/get"/>
    /// </summary>
    /// <param name="cancellationToken"><inheritdoc cref="FetchTaskProgressAsync" path="/param[@name='cancellationToken']/text()"/></param>
    ValueTask<MemberLayout> FetchMemberLayoutAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// 使用可能なシートのレイアウト設定情報を全て取得します。
    /// <see href="https://developer.kaonavi.jp/api/v2.0/index.html#tag/%E3%83%AC%E3%82%A4%E3%82%A2%E3%82%A6%E3%83%88%E8%A8%AD%E5%AE%9A/paths/~1sheet_layouts/get"/>
    /// </summary>
    /// <param name="cancellationToken"><inheritdoc cref="FetchTaskProgressAsync" path="/param[@name='cancellationToken']/text()"/></param>
    ValueTask<IReadOnlyList<SheetLayout>> FetchSheetLayoutsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// <paramref name="sheetId"/>と一致するシートの使用可能なレイアウト設定を全て取得します。
    /// <see href="https://developer.kaonavi.jp/api/v2.0/index.html#tag/%E3%83%AC%E3%82%A4%E3%82%A2%E3%82%A6%E3%83%88%E8%A8%AD%E5%AE%9A/paths/~1sheet_layouts~1{sheet_id}/get"/>
    /// </summary>
    /// <param name="sheetId"><inheritdoc cref="SheetLayout" path="/param[@name='Id']"/></param>
    /// <param name="cancellationToken"><inheritdoc cref="FetchTaskProgressAsync" path="/param[@name='cancellationToken']/text()"/></param>
    ValueTask<SheetLayout> FetchSheetLayoutAsync(int sheetId, CancellationToken cancellationToken = default);
    #endregion レイアウト設定

    #region メンバー情報
    /// <summary>
    /// 全てのメンバーの基本情報・所属(主務)・兼務情報を取得します。
    /// <see href="https://developer.kaonavi.jp/api/v2.0/index.html#tag/%E3%83%A1%E3%83%B3%E3%83%90%E3%83%BC%E6%83%85%E5%A0%B1/paths/~1members/get"/>
    /// </summary>
    /// <param name="cancellationToken"><inheritdoc cref="FetchTaskProgressAsync" path="/param[@name='cancellationToken']/text()"/></param>
    ValueTask<IReadOnlyList<MemberData>> FetchMembersDataAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// メンバー登録と、合わせて基本情報・所属(主務)・兼務情報を登録します。
    /// <see href="https://developer.kaonavi.jp/api/v2.0/index.html#tag/%E3%83%A1%E3%83%B3%E3%83%90%E3%83%BC%E6%83%85%E5%A0%B1/paths/~1members/post"/>
    /// </summary>
    /// <remarks>更新リクエスト制限の対象APIです。</remarks>
    /// <param name="payload">追加するデータ</param>
    /// <param name="cancellationToken"><inheritdoc cref="FetchTaskProgressAsync" path="/param[@name='cancellationToken']/text()"/></param>
    /// <returns><inheritdoc cref="TaskProgress" path="/param[@name='Id']/text()"/></returns>
    ValueTask<int> AddMemberDataAsync(IReadOnlyList<MemberData> payload, CancellationToken cancellationToken = default);

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
    /// <param name="cancellationToken"><inheritdoc cref="FetchTaskProgressAsync" path="/param[@name='cancellationToken']/text()"/></param>
    /// <returns><inheritdoc cref="TaskProgress" path="/param[@name='Id']/text()"/></returns>
    ValueTask<int> ReplaceMemberDataAsync(IReadOnlyList<MemberData> payload, CancellationToken cancellationToken = default);

    /// <summary>
    /// 送信されたメンバーの基本情報・所属(主務)・兼務情報のみを更新します。
    /// <paramref name="payload"/>に含まれていない情報は更新されません。
    /// 特定の値を削除する場合は、<c>""</c>を送信してください。
    /// <see href="https://developer.kaonavi.jp/api/v2.0/index.html#tag/%E3%83%A1%E3%83%B3%E3%83%90%E3%83%BC%E6%83%85%E5%A0%B1/paths/~1members/patch"/>
    /// </summary>
    /// <remarks>更新リクエスト制限の対象APIです。</remarks>
    /// <param name="payload">更新するデータ</param>
    /// <param name="cancellationToken"><inheritdoc cref="FetchTaskProgressAsync" path="/param[@name='cancellationToken']/text()"/></param>
    /// <returns><inheritdoc cref="TaskProgress" path="/param[@name='Id']/text()"/></returns>
    ValueTask<int> UpdateMemberDataAsync(IReadOnlyList<MemberData> payload, CancellationToken cancellationToken = default);

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
    /// <param name="cancellationToken"><inheritdoc cref="FetchTaskProgressAsync" path="/param[@name='cancellationToken']/text()"/></param>
    /// <returns><inheritdoc cref="TaskProgress" path="/param[@name='Id']/text()"/></returns>
    ValueTask<int> OverWriteMemberDataAsync(IReadOnlyList<MemberData> payload, CancellationToken cancellationToken = default);

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
    /// <param name="cancellationToken"><inheritdoc cref="FetchTaskProgressAsync" path="/param[@name='cancellationToken']/text()"/></param>
    /// <returns><inheritdoc cref="TaskProgress" path="/param[@name='Id']/text()"/></returns>
    ValueTask<int> DeleteMemberDataAsync(IReadOnlyList<string> codes, CancellationToken cancellationToken = default);
    #endregion メンバー情報

    #region シート情報
    /// <summary>
    /// <paramref name="sheetId"/>と一致するシートの全情報を取得します。
    /// <see href="https://developer.kaonavi.jp/api/v2.0/index.html#tag/%E3%82%B7%E3%83%BC%E3%83%88%E6%83%85%E5%A0%B1/paths/~1sheets~1{sheet_id}/get"/>
    /// </summary>
    /// <param name="sheetId"><inheritdoc cref="SheetLayout" path="/param[@name='Id']/text()"/></param>
    /// <param name="cancellationToken"><inheritdoc cref="FetchTaskProgressAsync" path="/param[@name='cancellationToken']/text()"/></param>
    ValueTask<IReadOnlyList<SheetData>> FetchSheetDataListAsync(int sheetId, CancellationToken cancellationToken = default);

    /// <summary>
    /// <paramref name="sheetId"/>と一致する<inheritdoc cref="SheetData" path="/summary/text()"/>を一括更新します。
    /// <paramref name="payload"/>に含まれていない情報は削除されます。
    /// 複数レコードの情報は送信された配列順に登録されます。
    /// <see href="https://developer.kaonavi.jp/api/v2.0/index.html#tag/%E3%82%B7%E3%83%BC%E3%83%88%E6%83%85%E5%A0%B1/paths/~1sheets~1{sheet_id}/put"/>
    /// </summary>
    /// <remarks>更新リクエスト制限の対象APIです。</remarks>
    /// <param name="sheetId"><inheritdoc cref="SheetLayout" path="/param[@name='Id']/text()"/></param>
    /// <param name="payload">一括更新するデータ</param>
    /// <param name="cancellationToken"><inheritdoc cref="FetchTaskProgressAsync" path="/param[@name='cancellationToken']/text()"/></param>
    /// <returns><inheritdoc cref="TaskProgress" path="/param[@name='Id']/text()"/></returns>
    ValueTask<int> ReplaceSheetDataAsync(int sheetId, IReadOnlyList<SheetData> payload, CancellationToken cancellationToken = default);

    /// <summary>
    /// <paramref name="sheetId"/>と一致する<inheritdoc cref="SheetData" path="/summary/text()"/>の一部を更新します。
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
    /// <remarks>
    /// <list type="bullet">
    /// <item>更新リクエスト制限の対象APIです。</item>
    /// </list>
    /// </remarks>
    /// <param name="sheetId"><inheritdoc cref="SheetLayout" path="/param[@name='Id']/text()"/></param>
    /// <param name="payload">更新するデータ</param>
    /// <param name="cancellationToken"><inheritdoc cref="FetchTaskProgressAsync" path="/param[@name='cancellationToken']/text()"/></param>
    /// <returns><inheritdoc cref="TaskProgress" path="/param[@name='Id']/text()"/></returns>
    ValueTask<int> UpdateSheetDataAsync(int sheetId, IReadOnlyList<SheetData> payload, CancellationToken cancellationToken = default);

    /// <summary>
    /// <paramref name="sheetId"/>と一致する<inheritdoc cref="RecordType.Multiple"/>にレコードを追加します。
    /// <inheritdoc cref="RecordType.Single"/>は対象外です。
    /// <see href="https://developer.kaonavi.jp/api/v2.0/index.html#tag/%E3%82%B7%E3%83%BC%E3%83%88%E6%83%85%E5%A0%B1/paths/~1sheets~1{sheet_id}~1add/post"/>
    /// </summary>
    /// <param name="sheetId"><inheritdoc cref="SheetLayout" path="/param[@name='Id']/text()"/></param>
    /// <param name="payload">追加するデータ</param>
    /// <param name="cancellationToken"><inheritdoc cref="FetchTaskProgressAsync" path="/param[@name='cancellationToken']/text()"/></param>
    /// <returns><inheritdoc cref="TaskProgress" path="/param[@name='Id']/text()"/></returns>
    ValueTask<int> AddSheetDataAsync(int sheetId, IReadOnlyList<SheetData> payload, CancellationToken cancellationToken = default);
    #endregion シート情報

    #region 所属ツリー
    /// <summary>
    /// <inheritdoc cref="DepartmentTree" path="/summary/text()"/>の情報を取得します。
    /// <see href="https://developer.kaonavi.jp/api/v2.0/index.html#tag/%E6%89%80%E5%B1%9E%E3%83%84%E3%83%AA%E3%83%BC/paths/~1departments/get"/>
    /// </summary>
    /// <param name="cancellationToken"><inheritdoc cref="FetchTaskProgressAsync" path="/param[@name='cancellationToken']/text()"/></param>
    ValueTask<IReadOnlyList<DepartmentTree>> FetchDepartmentsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// <inheritdoc cref="DepartmentTree" path="/summary/text()"/>を一括更新します。
    /// <paramref name="payload"/>に含まれていない情報は削除されます。
    /// <see href="https://developer.kaonavi.jp/api/v2.0/index.html#tag/%E6%89%80%E5%B1%9E%E3%83%84%E3%83%AA%E3%83%BC/paths/~1departments/put"/>
    /// </summary>
    /// <remarks>更新リクエスト制限の対象APIです。</remarks>
    /// <param name="payload">一括更新するデータ</param>
    /// <param name="cancellationToken"><inheritdoc cref="FetchTaskProgressAsync" path="/param[@name='cancellationToken']/text()"/></param>
    /// <returns><inheritdoc cref="TaskProgress" path="/param[@name='Id']/text()"/></returns>
    ValueTask<int> ReplaceDepartmentsAsync(IReadOnlyList<DepartmentTree> payload, CancellationToken cancellationToken = default);
    #endregion 所属ツリー

    #region ユーザー情報
    /// <summary>
    /// <inheritdoc cref="User" path="/summary/text()"/>の一覧を取得します。
    /// 一度もログインしたことがないユーザーの場合、last_login_at:nullと返却されます。
    /// <see href="https://developer.kaonavi.jp/api/v2.0/index.html#tag/%E3%83%A6%E3%83%BC%E3%82%B6%E3%83%BC%E6%83%85%E5%A0%B1/paths/~1users/get"/>
    /// </summary>
    /// <param name="cancellationToken"><inheritdoc cref="FetchTaskProgressAsync" path="/param[@name='cancellationToken']/text()"/></param>
    ValueTask<IReadOnlyList<User>> FetchUsersAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// <inheritdoc cref="User" path="/summary/text()"/>を登録します。
    /// <see href="https://developer.kaonavi.jp/api/v2.0/index.html#tag/%E3%83%A6%E3%83%BC%E3%82%B6%E3%83%BC%E6%83%85%E5%A0%B1/paths/~1users/post"/>
    /// </summary>
    /// <param name="payload">リクエスト</param>
    /// <param name="cancellationToken"><inheritdoc cref="FetchTaskProgressAsync" path="/param[@name='cancellationToken']/text()"/></param>
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
    /// 一度もログインしたことがないユーザーの場合、last_login_at:nullと返却されます。
    /// <see href="https://developer.kaonavi.jp/api/v2.0/index.html#tag/%E3%83%A6%E3%83%BC%E3%82%B6%E3%83%BC%E6%83%85%E5%A0%B1/paths/~1users~1{user_id}/get"/>
    /// </summary>
    /// <param name="userId"><inheritdoc cref="User" path="/param[@name='Id']/text()"/></param>
    /// <param name="cancellationToken"><inheritdoc cref="FetchTaskProgressAsync" path="/param[@name='cancellationToken']/text()"/></param>
    ValueTask<User> FetchUserAsync(int userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// <paramref name="userId"/>と一致する<inheritdoc cref="User" path="/summary/text()"/>を更新します。
    /// <see href="https://developer.kaonavi.jp/api/v2.0/index.html#tag/%E3%83%A6%E3%83%BC%E3%82%B6%E3%83%BC%E6%83%85%E5%A0%B1/paths/~1users~1{user_id}/get"/>
    /// </summary>
    /// <param name="userId"><inheritdoc cref="User" path="/param[@name='Id']/text()"/></param>
    /// <param name="payload">リクエスト</param>
    /// <param name="cancellationToken"><inheritdoc cref="FetchTaskProgressAsync" path="/param[@name='cancellationToken']/text()"/></param>
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
    /// <param name="userId"><inheritdoc cref="User" path="/param[@name='Id']/text()"/></param>
    /// <param name="cancellationToken"><inheritdoc cref="FetchTaskProgressAsync" path="/param[@name='cancellationToken']/text()"/></param>
    ValueTask DeleteUserAsync(int userId, CancellationToken cancellationToken = default);
    #endregion ユーザー情報

    #region ロール
    /// <summary>
    /// <inheritdoc cref="Role" path="/summary/text()"/>の一覧を取得します。
    /// <see href="https://developer.kaonavi.jp/api/v2.0/index.html#tag/%E3%83%AD%E3%83%BC%E3%83%AB/paths/~1roles/get"/>
    /// </summary>
    /// <param name="cancellationToken"><inheritdoc cref="FetchTaskProgressAsync" path="/param[@name='cancellationToken']/text()"/></param>
    ValueTask<IReadOnlyList<Role>> FetchRolesAsync(CancellationToken cancellationToken = default);
    #endregion ロール

    #region マスター管理
    /// <summary>
    /// マスター管理で編集可能な項目のうち、APIv2 で編集可能な<inheritdoc cref="EnumOption" path="/summary/text()"/>の一覧を取得します。
    /// <see href="https://developer.kaonavi.jp/api/v2.0/index.html#tag/%E3%83%9E%E3%82%B9%E3%82%BF%E3%83%BC%E7%AE%A1%E7%90%86/paths/~1enum_options/get"/>
    /// </summary>
    /// <remarks>
    /// APIv2 で編集できるのは、プルダウンリスト、ラジオボタン、チェックボックスで作成された項目です。
    /// ただし、データ連携中の項目はマスター管理で編集不可能なため、上記のパーツ種別であっても取得は出来ません。
    /// </remarks>
    /// <param name="cancellationToken"><inheritdoc cref="FetchTaskProgressAsync" path="/param[@name='cancellationToken']/text()"/></param>
    ValueTask<IReadOnlyList<EnumOption>> FetchEnumOptionsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// <paramref name="customFieldId"/>と一致する<inheritdoc cref="EnumOption" path="/summary/text()"/>を取得します。
    /// <see href="https://developer.kaonavi.jp/api/v2.0/index.html#tag/%E3%83%9E%E3%82%B9%E3%82%BF%E3%83%BC%E7%AE%A1%E7%90%86/paths/~1enum_options~1{custom_field_id}/get"/>
    /// </summary>
    /// <inheritdoc cref="FetchEnumOptionsAsync" path="/remarks"/>
    /// <param name="customFieldId"><inheritdoc cref="EnumOption.Id" path="/summary/text()"/></param>
    /// <param name="cancellationToken"><inheritdoc cref="FetchTaskProgressAsync" path="/param[@name='cancellationToken']/text()"/></param>
    ValueTask<EnumOption> FetchEnumOptionAsync(int customFieldId, CancellationToken cancellationToken = default);

    /// <summary>
    /// <paramref name="customFieldId"/>と一致する<inheritdoc cref="EnumOption" path="/summary/text()"/>を一括更新します。
    /// <see href="https://developer.kaonavi.jp/api/v2.0/index.html#tag/%E3%83%9E%E3%82%B9%E3%82%BF%E3%83%BC%E7%AE%A1%E7%90%86/paths/~1enum_options~1{custom_field_id}/put"/>
    /// </summary>
    /// <remarks>
    /// <list type="bullet">
    /// <item>マスターを追加したい場合は、id を指定せず name のみ指定してください。</item>
    /// <item><paramref name="payload"/>に含まれていないマスターは削除されます。ただし、削除できるマスターは、メンバー数が「0」のマスターのみです。</item>
    /// <item>並び順は送信された配列順に登録されます。</item>
    /// <item>変更履歴が設定されたシートのマスター情報を更新した際には履歴が作成されます。</item>
    /// <item><paramref name="payload"/>は0件での指定は出来ません。1件以上指定してください。</item>
    /// </list>
    /// </remarks>
    /// <param name="customFieldId"><inheritdoc cref="EnumOption.Id" path="/summary/text()"/></param>
    /// <param name="payload">リクエスト</param>
    /// <param name="cancellationToken"><inheritdoc cref="FetchTaskProgressAsync" path="/param[@name='cancellationToken']/text()"/></param>
    /// <returns><inheritdoc cref="TaskProgress.Id" path="/summary/text()" /></returns>
    ValueTask<int> UpdateEnumOptionAsync(int customFieldId, IReadOnlyList<(int? id, string name)> payload, CancellationToken cancellationToken = default);
    #endregion マスター管理
}
