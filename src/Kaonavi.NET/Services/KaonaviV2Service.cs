using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Serialization.Metadata;
using Kaonavi.Net.Entities;
using Kaonavi.Net.Json;

namespace Kaonavi.Net.Services;

/// <summary>カオナビ API v2 を呼び出すサービスの実装</summary>
public class KaonaviV2Service : IKaonaviService
{
    /// <summary>カオナビ API v2 のルートアドレス</summary>
    private const string BaseApiAddress = "https://api.kaonavi.jp/api/v2.0/";

    /// <summary>更新リクエストの制限数</summary>
    /// <seealso href="https://developer.kaonavi.jp/api/v2.0/index.html#section/%E3%83%AA%E3%82%AF%E3%82%A8%E3%82%B9%E3%83%88%E5%88%B6%E9%99%90"/>
    private const int UpdateRequestLimit = 5;

    /// <summary>更新リクエストがリセットされる時間(秒)</summary>
    /// <seealso href="https://developer.kaonavi.jp/api/v2.0/index.html#section/%E3%83%AA%E3%82%AF%E3%82%A8%E3%82%B9%E3%83%88%E5%88%B6%E9%99%90"/>
    private const int WaitSecondsForUpdateLimit = 60;

    /// <summary><inheritdoc cref="KaonaviV2Service.KaonaviV2Service" path="/param[1]"/></summary>
    private readonly HttpClient _client;

    /// <summary><inheritdoc cref="KaonaviV2Service.KaonaviV2Service" path="/param[2]"/></summary>
    private readonly string _consumerKey;

    /// <summary><inheritdoc cref="KaonaviV2Service.KaonaviV2Service" path="/param[3]"/></summary>
    private readonly string _consumerSecret;

    #region Properties
    private const string TokenHeader = "Kaonavi-Token";
    /// <summary>
    /// アクセストークン文字列を取得または設定します。
    /// 各種API呼び出し時、この項目が<see langword="null"/>の場合は自動的に<see cref="AuthenticateAsync"/>を呼び出します。
    /// </summary>
    public string? AccessToken
    {
        get => _client.DefaultRequestHeaders.TryGetValues(TokenHeader, out var values) ? values.First() : null;
        set
        {
            _ = _client.DefaultRequestHeaders.Remove(TokenHeader);
            if (!string.IsNullOrWhiteSpace(value))
                _client.DefaultRequestHeaders.Add(TokenHeader, value);
        }
    }

    private const string DryRunHeader = "Dry-Run";
    /// <summary>
    /// dryrunモードの動作有無を取得または設定します。
    /// <c>true</c>に設定することで、データベースの操作を行わず、リクエストの内容が適切であるかを検証することが出来ます。
    /// <see href="https://developer.kaonavi.jp/api/v2.0/index.html#section/dryrun"/>
    /// </summary>
    public bool UseDryRun
    {
        get => _client.DefaultRequestHeaders.TryGetValues(DryRunHeader, out var values) && values.First() == "1";
        set
        {
            _ = _client.DefaultRequestHeaders.Remove(DryRunHeader);
            if (value)
                _client.DefaultRequestHeaders.Add(DryRunHeader, "1");
        }
    }

    /// <summary>更新リクエストを最後に呼び出した日時</summary>
    private DateTime _lastUpdateApiCalled;
    /// <summary>
    /// 更新リクエストの呼び出し回数
    /// </summary>
    /// <seealso href="https://developer.kaonavi.jp/api/v2.0/index.html#section/%E3%83%AA%E3%82%AF%E3%82%A8%E3%82%B9%E3%83%88%E5%88%B6%E9%99%90"/>
    public int UpdateRequestCount { get; private set; }
    #endregion Properties

    /// <summary>
    /// KaonaviV2Serviceの新しいインスタンスを生成します。
    /// </summary>
    /// <param name="client">APIコール時に利用する<see cref="HttpClient"/>のインスタンス</param>
    /// <param name="consumerKey">Consumer Key</param>
    /// <param name="consumerSecret">Consumer Secret</param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="client"/>, <paramref name="consumerKey"/>または<paramref name="consumerSecret"/>が<see langword="null"/>の場合にスローされます。
    /// </exception>
    public KaonaviV2Service(HttpClient client, string consumerKey, string consumerSecret)
    {
        ArgumentNullException.ThrowIfNull(client);
        ArgumentNullException.ThrowIfNull(consumerKey);
        ArgumentNullException.ThrowIfNull(consumerSecret);

        (_client, _consumerKey, _consumerSecret) = (client, consumerKey, consumerSecret);
        _client.BaseAddress ??= new(BaseApiAddress);
    }

    /// <summary>
    /// アクセストークンを発行します。
    /// <see href="https://developer.kaonavi.jp/api/v2.0/index.html#tag/%E3%82%A2%E3%82%AF%E3%82%BB%E3%82%B9%E3%83%88%E3%83%BC%E3%82%AF%E3%83%B3/paths/~1token/post"/>
    /// </summary>
    /// <param name="cancellationToken">キャンセル通知を受け取るために他のオブジェクトまたはスレッドで使用できるキャンセル トークン。</param>
    public async ValueTask<Token> AuthenticateAsync(CancellationToken cancellationToken = default)
    {
        byte[] byteArray = Encoding.UTF8.GetBytes($"{_consumerKey}:{_consumerSecret}");
        var content = new FormUrlEncodedContent(new Dictionary<string, string>()
        {
            { "grant_type", "client_credentials" }
        }!);
        _client.DefaultRequestHeaders.Authorization = new("Basic", Convert.ToBase64String(byteArray));

        var response = await _client.PostAsync("token", content, cancellationToken).ConfigureAwait(false);
        await ValidateApiResponseAsync(response, cancellationToken).ConfigureAwait(false);

        var token = await response.Content
            .ReadFromJsonAsync(Context.Default.Token, cancellationToken)
            .ConfigureAwait(false);
        _client.DefaultRequestHeaders.Authorization = null;
        return token!;
    }

    #region タスク進捗状況
    /// <inheritdoc/>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="taskId"/>が0より小さい場合にスローされます。</exception>
    public ValueTask<TaskProgress> FetchTaskProgressAsync(int taskId, CancellationToken cancellationToken = default)
        => CallApiAsync(new(HttpMethod.Get, $"tasks/{ThrowIfNegative(taskId):D}"), Context.Default.TaskProgress, cancellationToken);
    #endregion タスク進捗状況

    #region レイアウト設定
    /// <inheritdoc/>
    public ValueTask<MemberLayout> FetchMemberLayoutAsync(CancellationToken cancellationToken = default)
        => CallApiAsync(new(HttpMethod.Get, "member_layouts"), Context.Default.MemberLayout, cancellationToken);

    /// <inheritdoc/>
    public async ValueTask<IReadOnlyCollection<SheetLayout>> FetchSheetLayoutsAsync(CancellationToken cancellationToken = default)
        => (await CallApiAsync(new(HttpMethod.Get, "sheet_layouts"), Context.Default.ApiListResultSheetLayout, cancellationToken)).Values;

    /// <inheritdoc/>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="sheetId"/>が0より小さい場合にスローされます。</exception>
    public ValueTask<SheetLayout> FetchSheetLayoutAsync(int sheetId, CancellationToken cancellationToken = default)
        => CallApiAsync(new(HttpMethod.Get, $"sheet_layouts/{ThrowIfNegative(sheetId):D}"), Context.Default.SheetLayout, cancellationToken);
    #endregion レイアウト設定

    #region メンバー情報
    /// <inheritdoc/>
    public async ValueTask<IReadOnlyCollection<MemberData>> FetchMembersDataAsync(CancellationToken cancellationToken = default)
        => (await CallApiAsync(new(HttpMethod.Get, "members"), Context.Default.ApiListResultMemberData, cancellationToken)).Values;

    /// <inheritdoc/>
    public ValueTask<int> AddMemberDataAsync(IReadOnlyCollection<MemberData> payload, CancellationToken cancellationToken = default)
        => CallTaskApiAsync(HttpMethod.Post, "members", new("member_data", payload), Context.Default.ApiListResultMemberData, cancellationToken);

    /// <inheritdoc/>
    public ValueTask<int> ReplaceMemberDataAsync(IReadOnlyCollection<MemberData> payload, CancellationToken cancellationToken = default)
        => CallTaskApiAsync(HttpMethod.Put, "members", new("member_data", payload), Context.Default.ApiListResultMemberData, cancellationToken);

    /// <inheritdoc/>
    public ValueTask<int> UpdateMemberDataAsync(IReadOnlyCollection<MemberData> payload, CancellationToken cancellationToken = default)
        => CallTaskApiAsync(HttpMethod.Patch, "members", new("member_data", payload), Context.Default.ApiListResultMemberData, cancellationToken);

    /// <inheritdoc/>
    public ValueTask<int> OverWriteMemberDataAsync(IReadOnlyCollection<MemberData> payload, CancellationToken cancellationToken = default)
        => CallTaskApiAsync(HttpMethod.Put, "members/overwrite", new("member_data", payload), Context.Default.ApiListResultMemberData, cancellationToken);

    /// <inheritdoc/>
    public ValueTask<int> DeleteMemberDataAsync(IReadOnlyCollection<string> codes, CancellationToken cancellationToken = default)
        => CallTaskApiAsync(HttpMethod.Post, "members/delete", new("codes", codes), Context.Default.ApiListResultString, cancellationToken);
    #endregion メンバー情報

    #region シート情報
    /// <inheritdoc/>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="sheetId"/>が0より小さい場合にスローされます。</exception>
    async public ValueTask<IReadOnlyCollection<SheetData>> FetchSheetDataListAsync(int sheetId, CancellationToken cancellationToken = default)
         => (await CallApiAsync(new(HttpMethod.Get, $"sheets/{ThrowIfNegative(sheetId):D}"), Context.Default.ApiListResultSheetData, cancellationToken)).Values;

    /// <inheritdoc/>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="sheetId"/>が0より小さい場合にスローされます。</exception>
    public ValueTask<int> ReplaceSheetDataAsync(int sheetId, IReadOnlyCollection<SheetData> payload, CancellationToken cancellationToken = default)
        => CallTaskApiAsync(HttpMethod.Put, $"sheets/{ThrowIfNegative(sheetId):D}", new("member_data", payload), Context.Default.ApiListResultSheetData, cancellationToken);

    /// <inheritdoc/>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="sheetId"/>が0より小さい場合にスローされます。</exception>
    public ValueTask<int> UpdateSheetDataAsync(int sheetId, IReadOnlyCollection<SheetData> payload, CancellationToken cancellationToken = default)
        => CallTaskApiAsync(HttpMethod.Patch, $"sheets/{ThrowIfNegative(sheetId):D}", new("member_data", payload), Context.Default.ApiListResultSheetData, cancellationToken);

    /// <inheritdoc/>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="sheetId"/>が0より小さい場合にスローされます。</exception>
    public ValueTask<int> AddSheetDataAsync(int sheetId, IReadOnlyCollection<SheetData> payload, CancellationToken cancellationToken = default)
        => CallTaskApiAsync(HttpMethod.Post, $"sheets/{ThrowIfNegative(sheetId):D}/add", new("member_data", payload), Context.Default.ApiListResultSheetData, cancellationToken);
    #endregion シート情報

    #region 所属ツリー
    /// <inheritdoc/>
    public async ValueTask<IReadOnlyCollection<DepartmentTree>> FetchDepartmentsAsync(CancellationToken cancellationToken = default)
          => (await CallApiAsync(new(HttpMethod.Get, "departments"), Context.Default.ApiListResultDepartmentTree, cancellationToken)).Values;

    /// <inheritdoc/>
    public ValueTask<int> ReplaceDepartmentsAsync(IReadOnlyCollection<DepartmentTree> payload, CancellationToken cancellationToken = default)
        => CallTaskApiAsync(HttpMethod.Put, "departments", new("department_data", payload), Context.Default.ApiListResultDepartmentTree, cancellationToken);
    internal record DepartmentsResult(IReadOnlyCollection<DepartmentTree> DepartmentData);
    #endregion 所属ツリー

    #region ユーザー情報
    /// <inheritdoc/>
    public async ValueTask<IReadOnlyCollection<UserWithLoginAt>> FetchUsersAsync(CancellationToken cancellationToken = default)
        => (await CallApiAsync(new(HttpMethod.Get, "users"), Context.Default.ApiListResultUserWithLoginAt, cancellationToken)).Values;

    /// <inheritdoc/>
    public ValueTask<User> AddUserAsync(UserPayload payload, CancellationToken cancellationToken = default)
        => CallApiAsync(new(HttpMethod.Post, "users")
        {
            Content = JsonContent.Create(new(payload.Email, payload.MemberCode, payload.Password, new(payload.RoleId, null!, null!)), Context.Default.UserJsonPayload)
        }, Context.Default.User, cancellationToken);
    internal record UserJsonPayload(string Email, string? MemberCode, string Password, Role Role);

    /// <inheritdoc/>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="userId"/>が0より小さい場合にスローされます。</exception>
    public ValueTask<UserWithLoginAt> FetchUserAsync(int userId, CancellationToken cancellationToken = default)
        => CallApiAsync(new(HttpMethod.Get, $"users/{ThrowIfNegative(userId):D}"), Context.Default.UserWithLoginAt, cancellationToken);

    /// <inheritdoc/>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="userId"/>が0より小さい場合にスローされます。</exception>
    public ValueTask<User> UpdateUserAsync(int userId, UserPayload payload, CancellationToken cancellationToken = default)
        => CallApiAsync(new(HttpMethod.Patch, $"users/{ThrowIfNegative(userId):D}")
        {
            Content = JsonContent.Create(new(payload.Email, payload.MemberCode, payload.Password, new(payload.RoleId, null!, null!)), Context.Default.UserJsonPayload)
        }, Context.Default.User, cancellationToken);

    /// <inheritdoc/>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="userId"/>が0より小さい場合にスローされます。</exception>
    public async ValueTask DeleteUserAsync(int userId, CancellationToken cancellationToken = default)
        => await CallApiAsync(new(HttpMethod.Delete, $"users/{ThrowIfNegative(userId):D}"), cancellationToken).ConfigureAwait(false);
    #endregion ユーザー情報

    #region ロール
    /// <inheritdoc/>
    public async ValueTask<IReadOnlyCollection<Role>> FetchRolesAsync(CancellationToken cancellationToken = default)
        => (await CallApiAsync(new(HttpMethod.Get, "roles"), Context.Default.ApiListResultRole, cancellationToken)).Values;
    #endregion ロール

    #region 拡張アクセス設定
    private static string AdvancedTypeToString(AdvancedType type, [CallerArgumentExpression(nameof(type))] string? argument = null) => type switch
    {
        AdvancedType.Member => "member",
        AdvancedType.Department => "department",
        _ => throw new ArgumentOutOfRangeException(argument),
    };
    /// <summary>
    /// <inheritdoc cref="AdvancedPermission" path="/summary/text()"/>の一覧を取得します。
    /// <see href="https://developer.kaonavi.jp/api/v2.0/index.html#tag/%E6%8B%A1%E5%BC%B5%E3%82%A2%E3%82%AF%E3%82%BB%E3%82%B9%E8%A8%AD%E5%AE%9A/paths/~1advanced_permissions~1{advanced_type}/get"/>
    /// </summary>
    /// <param name="type"><inheritdoc cref="AdvancedType" path="/summary/text()"/></param>
    /// <param name="cancellationToken"><inheritdoc cref="FetchTaskProgressAsync" path="/param[@name='cancellationToken']/text()"/></param>
    public async ValueTask<IReadOnlyCollection<AdvancedPermission>> FetchAdvancedPermissionListAsync(AdvancedType type, CancellationToken cancellationToken = default)
        => (await CallApiAsync(new(HttpMethod.Get, $"advanced_permissions/{AdvancedTypeToString(type)}"), Context.Default.ApiListResultAdvancedPermission, cancellationToken)).Values;

    /// <summary>
    /// 現在登録されている<inheritdoc cref="AdvancedPermission" path="/summary/text()"/>を全て、リクエストしたデータで入れ替えます。
    /// <see href="https://developer.kaonavi.jp/api/v2.0/index.html#tag/%E6%8B%A1%E5%BC%B5%E3%82%A2%E3%82%AF%E3%82%BB%E3%82%B9%E8%A8%AD%E5%AE%9A/paths/~1advanced_permissions~1{advanced_type}/put"/>
    /// </summary>
    /// <remarks>更新リクエスト制限の対象APIです。</remarks>
    /// <param name="type"><inheritdoc cref="AdvancedType" path="/summary/text()"/></param>
    /// <param name="payload">入れ替え対象となるデータ</param>
    /// <param name="cancellationToken"><inheritdoc cref="FetchTaskProgressAsync" path="/param[@name='cancellationToken']/text()"/></param>
    public ValueTask<int> ReplaceAdvancedPermissionAsync(AdvancedType type, IReadOnlyCollection<AdvancedPermission> payload, CancellationToken cancellationToken = default)
        => CallTaskApiAsync(HttpMethod.Put, $"advanced_permissions/{AdvancedTypeToString(type)}", new("advanced_permission_data", payload), Context.Default.ApiListResultAdvancedPermission, cancellationToken);
    #endregion 拡張アクセス設定

    #region マスター管理
    /// <inheritdoc/>
    public async ValueTask<IReadOnlyCollection<EnumOption>> FetchEnumOptionsAsync(CancellationToken cancellationToken = default)
        => (await CallApiAsync(new(HttpMethod.Get, "enum_options"), Context.Default.ApiListResultEnumOption, cancellationToken)).Values;

    /// <inheritdoc/>
    public ValueTask<EnumOption> FetchEnumOptionAsync(int customFieldId, CancellationToken cancellationToken = default)
        => CallApiAsync(new(HttpMethod.Get, $"enum_options/{ThrowIfNegative(customFieldId):D}"), Context.Default.EnumOption, cancellationToken);

    /// <inheritdoc/>
    public ValueTask<int> UpdateEnumOptionAsync(int customFieldId, IReadOnlyCollection<(int?, string)> payload, CancellationToken cancellationToken = default)
        => CallTaskApiAsync(
            HttpMethod.Put,
            $"enum_options/{ThrowIfNegative(customFieldId):D}",
            new("enum_option_data", payload.Select(d => new EnumOptionPayloadData(d.Item1, d.Item2)).ToArray()),
            Context.Default.ApiListResultEnumOptionPayloadData,
            cancellationToken);
    internal record EnumOptionPayloadData(int? Id, string Name);
    #endregion マスター管理

    #region Webhook設定
    /// <inheritdoc/>
    public async ValueTask<IReadOnlyCollection<WebhookConfig>> FetchWebhookConfigListAsync(CancellationToken cancellationToken = default)
        => (await CallApiAsync(new(HttpMethod.Get, "webhook"), Context.Default.ApiListResultWebhookConfig, cancellationToken)).Values;

    /// <inheritdoc/>
    public ValueTask<WebhookConfig> AddWebhookConfigAsync(WebhookConfigPayload payload, CancellationToken cancellationToken = default)
        => CallApiAsync(new(HttpMethod.Post, "webhook")
        {
            Content = JsonContent.Create(payload, Context.Default.WebhookConfigPayload)
        }, Context.Default.WebhookConfig, cancellationToken);

    /// <inheritdoc/>
    public ValueTask<WebhookConfig> UpdateWebhookConfigAsync(WebhookConfig payload, CancellationToken cancellationToken = default)
        => CallApiAsync(new(HttpMethod.Patch, $"webhook/{payload.Id}")
        {
            Content = JsonContent.Create(payload, Context.Default.WebhookConfig)
        }, Context.Default.WebhookConfig, cancellationToken);

    /// <inheritdoc/>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="webhookId"/>が0より小さい場合にスローされます。</exception>
    public async ValueTask DeleteWebhookConfigAsync(int webhookId, CancellationToken cancellationToken = default)
        => await CallApiAsync(new(HttpMethod.Delete, $"webhook/{ThrowIfNegative(webhookId):D}"), cancellationToken).ConfigureAwait(false);
    #endregion Webhook設定

    #region Common Method
    /// <summary>APIコール前に必要な認証を行います。</summary>
    /// <param name="cancellationToken"><inheritdoc cref="FetchMemberLayoutAsync" path="/param[@name='cancellationToken']/text()"/></param>
    private async ValueTask FetchTokenAsync(CancellationToken cancellationToken)
        => AccessToken ??= (await AuthenticateAsync(cancellationToken).ConfigureAwait(false)).AccessToken;

    /// <summary>
    /// APIを呼び出します。
    /// </summary>
    /// <param name="request">APIに対するリクエスト</param>
    /// <param name="cancellationToken"><inheritdoc cref="FetchMemberLayoutAsync" path="/param[@name='cancellationToken']/text()"/></param>
    /// <param name="isUpdateLimitApi">更新リクエスト制限の対象APIかどうか</param>
    /// <exception cref="ApplicationException">
    /// APIからのHTTPステータスコードが200-299番でない場合にスローされます。
    /// </exception>
    private async ValueTask<HttpResponseMessage> CallApiAsync(HttpRequestMessage request, CancellationToken cancellationToken, bool isUpdateLimitApi = false)
    {
        await FetchTokenAsync(cancellationToken).ConfigureAwait(false);
        var response = await _client.SendAsync(request, cancellationToken: cancellationToken).ConfigureAwait(false);
        await ValidateApiResponseAsync(response, cancellationToken).ConfigureAwait(false);
        if (isUpdateLimitApi)
            UpdateRequestCount++;
        return response;
    }

    /// <summary>
    /// APIを呼び出し、受け取ったJSONを<typeparamref name="T"/>に変換して返します。
    /// </summary>
    /// <inheritdoc cref="CallApiAsync" path="/param"/>
    /// <typeparam name="T">JSONの型</typeparam>
    /// <inheritdoc cref="CallApiAsync" path="/exception"/>
    private async ValueTask<T> CallApiAsync<T>(HttpRequestMessage request, JsonTypeInfo<T> typeInfo, CancellationToken cancellationToken, bool isUpdateLimitApi = false)
    {
        var response = await CallApiAsync(request, cancellationToken, isUpdateLimitApi).ConfigureAwait(false);
        return (await response.Content.ReadFromJsonAsync(typeInfo, cancellationToken).ConfigureAwait(false))!;
    }

    /// <summary>
    /// APIを呼び出し、受け取った<inheritdoc cref="TaskProgress" path="/param[@name='Id']/text()"/>を返します。
    /// </summary>
    /// <param name="method">HTTP Method</param>
    /// <param name="uri">リクエストURI</param>
    /// <param name="payload">APIに対するリクエスト</param>
    /// <param name="cancellationToken"><inheritdoc cref="CallApiAsync" path="/param[@name='cancellationToken']"/></param>
    /// <returns><inheritdoc cref="TaskProgress" path="/param[@name='Id']/text()"/></returns>
    /// <inheritdoc cref="CallApiAsync" path="/exception"/>
    private async ValueTask<int> CallTaskApiAsync<T>(HttpMethod method, string uri, T payload, JsonTypeInfo<T> typeInfo, CancellationToken cancellationToken)
    {
        if (UpdateRequestCount >= UpdateRequestLimit)
        {
            var timeSpan = _lastUpdateApiCalled.AddSeconds(WaitSecondsForUpdateLimit) - _lastUpdateApiCalled;
            if (timeSpan > TimeSpan.Zero)
                await Task.Delay(timeSpan, cancellationToken);
            UpdateRequestCount -= UpdateRequestLimit;
        }
        _lastUpdateApiCalled = DateTime.Now;
        return (await CallApiAsync(new(method, uri)
        {
            Content = JsonContent.Create(payload, typeInfo)
        }, Context.Default.JsonElement, cancellationToken, true).ConfigureAwait(false)).GetProperty("task_id").GetInt32();
    }

    /// <summary>
    /// APIが正しく終了したかどうかを検証します。
    /// エラーが返ってきた場合は、エラーメッセージを取得し例外をスローします。
    /// </summary>
    /// <param name="response">APIレスポンス</param>
    /// <param name="cancellationToken">キャンセル通知を受け取るために他のオブジェクトまたはスレッドで使用できるキャンセル トークン。</param>
    /// <exception cref="ApplicationException">
    /// APIからのHTTPステータスコードが200-299番でない場合にスローされます。
    /// </exception>
    private static async ValueTask ValidateApiResponseAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        try
        {
            response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException ex)
        {
            string errorMessage = response.Content.Headers.ContentType!.MediaType == "application/json"
                ? string.Join("\n", (await response.Content.ReadFromJsonAsync(Context.Default.ApiListResultString, cancellationToken))!.Values)
                : await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
            throw new ApplicationException(errorMessage, ex);
        }
    }

    /// <summary>
    /// <paramref name="id"/>が負の値の場合に<see cref="ArgumentOutOfRangeException"/>をスローします。
    /// </summary>
    /// <param name="id">チェックするID</param>
    /// <param name="paramName"><paramref name="id"/>として渡された変数名</param>
    private static int ThrowIfNegative(int id, [CallerArgumentExpression(nameof(id))] string? paramName = null)
        => id >= 0 ? id : throw new ArgumentOutOfRangeException(paramName);
    #endregion Common Method
}
