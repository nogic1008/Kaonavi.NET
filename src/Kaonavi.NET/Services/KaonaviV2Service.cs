namespace Kaonavi.Net.Services;

using System.Net.Http.Json;
using System.Text;
using Kaonavi.Net.Entities;

/// <summary>カオナビ API v2 を呼び出すサービスの実装</summary>
public class KaonaviV2Service : IKaonaviService
{
    /// <summary>カオナビ API v2 のルートアドレス</summary>
    private const string BaseApiAddress = "https://api.kaonavi.jp/api/v2.0";

    /// <summary>
    /// APIに送信するJSONペイロードのエンコード設定
    /// </summary>
    private static readonly JsonSerializerOptions _options = new(JsonSerializerDefaults.Web)
    {
        Converters = { new JsonConverterFactoryForApiResult() },
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    };

    #region DI Objects
    /// <summary><inheritdoc cref="KaonaviV2Service.KaonaviV2Service" path="/param[1]"/></summary>
    private readonly HttpClient _client;

    /// <summary><inheritdoc cref="KaonaviV2Service.KaonaviV2Service" path="/param[2]"/></summary>
    private readonly string _consumerKey;

    /// <summary><inheritdoc cref="KaonaviV2Service.KaonaviV2Service" path="/param[3]"/></summary>
    private readonly string _consumerSecret;
    #endregion

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
            _client.DefaultRequestHeaders.Remove(TokenHeader);
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
            _client.DefaultRequestHeaders.Remove(DryRunHeader);
            if (value)
                _client.DefaultRequestHeaders.Add(DryRunHeader, "1");
        }
    }
    #endregion

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
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _consumerKey = consumerKey ?? throw new ArgumentNullException(nameof(consumerKey));
        _consumerSecret = consumerSecret ?? throw new ArgumentNullException(nameof(consumerSecret));

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

        var response = await _client.PostAsync("/token", content, cancellationToken).ConfigureAwait(false);
        await ValidateApiResponseAsync(response, cancellationToken).ConfigureAwait(false);

        var token = await response.Content
            .ReadFromJsonAsync<Token>(cancellationToken: cancellationToken)
            .ConfigureAwait(false);
        _client.DefaultRequestHeaders.Authorization = null;
        return token!;
    }

    #region レイアウト定義
    /// <inheritdoc/>
    public ValueTask<MemberLayout> FetchMemberLayoutAsync(CancellationToken cancellationToken = default)
        => CallApiAsync<MemberLayout>(new(HttpMethod.Get, "/member_layouts"), cancellationToken);

    /// <inheritdoc/>
    public async ValueTask<IReadOnlyList<SheetLayout>> FetchSheetLayoutsAsync(CancellationToken cancellationToken = default)
        => (await CallApiAsync<SheetLayoutsResult>(new(HttpMethod.Get, "/sheet_layouts"), cancellationToken)
            .ConfigureAwait(false)).Sheets;
    private record SheetLayoutsResult(
        [property: JsonPropertyName("sheets")] IReadOnlyList<SheetLayout> Sheets
    );

    /// <inheritdoc/>
    public ValueTask<SheetLayout> FetchSheetLayoutAsync(int sheetId, CancellationToken cancellationToken = default)
        => CallApiAsync<SheetLayout>(new(HttpMethod.Get, $"/sheet_layouts/{sheetId}"), cancellationToken);
    #endregion

    #region メンバー情報
    /// <inheritdoc/>
    public async ValueTask<IReadOnlyList<MemberData>> FetchMembersDataAsync(CancellationToken cancellationToken = default)
        => (await CallApiAsync<ApiResult<MemberData>>(new(HttpMethod.Get, "/members"), cancellationToken)
            .ConfigureAwait(false)).Data;

    /// <inheritdoc/>
    public ValueTask<int> AddMemberDataAsync(IReadOnlyList<MemberData> payload, CancellationToken cancellationToken = default)
        => CallTaskApiAsync(HttpMethod.Post, "/members", new ApiResult<MemberData>("member_data", payload), cancellationToken);

    /// <inheritdoc/>
    public ValueTask<int> ReplaceMemberDataAsync(IReadOnlyList<MemberData> payload, CancellationToken cancellationToken = default)
        => CallTaskApiAsync(HttpMethod.Put, "/members", new ApiResult<MemberData>("member_data", payload), cancellationToken);

    /// <inheritdoc/>
    public ValueTask<int> UpdateMemberDataAsync(IReadOnlyList<MemberData> payload, CancellationToken cancellationToken = default)
        => CallTaskApiAsync(new("PATCH"), "/members", new ApiResult<MemberData>("member_data", payload), cancellationToken);

    /// <inheritdoc/>
    public ValueTask<int> DeleteMemberDataAsync(IReadOnlyList<string> codes, CancellationToken cancellationToken = default)
        => CallTaskApiAsync(HttpMethod.Post, "/members/delete", new DeleteMemberDataPayload(codes), cancellationToken);
    private record DeleteMemberDataPayload(IReadOnlyList<string> Codes);
    #endregion

    #region シート情報
    /// <inheritdoc/>
    public async ValueTask<IReadOnlyList<SheetData>> FetchSheetDataListAsync(int sheetId, CancellationToken cancellationToken = default)
        => (await CallApiAsync<ApiResult<SheetData>>(new(HttpMethod.Get, $"/sheets/{sheetId:D}"), cancellationToken)
            .ConfigureAwait(false)).Data;

    /// <inheritdoc/>
    public ValueTask<int> ReplaceSheetDataAsync(int sheetId, IReadOnlyList<SheetData> payload, CancellationToken cancellationToken = default)
        => CallTaskApiAsync(HttpMethod.Put, $"/sheets/{sheetId:D}", new ApiResult<SheetData>("member_data", payload), cancellationToken);

    /// <inheritdoc/>
    public ValueTask<int> UpdateSheetDataAsync(int sheetId, IReadOnlyList<SheetData> payload, CancellationToken cancellationToken = default)
        => CallTaskApiAsync(new("PATCH"), $"/sheets/{sheetId:D}", new ApiResult<SheetData>("member_data", payload), cancellationToken);
    #endregion

    #region 所属ツリー
    /// <inheritdoc/>
    public async ValueTask<IReadOnlyList<DepartmentTree>> FetchDepartmentsAsync(CancellationToken cancellationToken = default)
        => (await CallApiAsync<ApiResult<DepartmentTree>>(new(HttpMethod.Get, "/departments"), cancellationToken)
            .ConfigureAwait(false)).Data;

    /// <inheritdoc/>
    public ValueTask<int> ReplaceDepartmentsAsync(IReadOnlyList<DepartmentTree> payload, CancellationToken cancellationToken = default)
        => CallTaskApiAsync(HttpMethod.Put, "/departments", new ApiResult<DepartmentTree>("department_data", payload), cancellationToken);
    #endregion

    #region タスク進捗状況
    /// <inheritdoc/>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="taskId"/>が0より小さい場合にスローされます。</exception>
    public ValueTask<TaskProgress> FetchTaskProgressAsync(int taskId, CancellationToken cancellationToken = default)
        => CallApiAsync<TaskProgress>(new(HttpMethod.Get, $"/tasks/{ThrowIfNegative(taskId, nameof(taskId)):D}"), cancellationToken);
    #endregion

    #region ユーザー情報
    /// <inheritdoc/>
    public async ValueTask<IReadOnlyList<User>> FetchUsersAsync(CancellationToken cancellationToken = default)
        => (await CallApiAsync<ApiResult<User>>(new(HttpMethod.Get, "/users"), cancellationToken)
            .ConfigureAwait(false)).Data;

    /// <inheritdoc/>
    public ValueTask<User> AddUserAsync(UserPayload payload, CancellationToken cancellationToken = default)
        => CallApiAsync<User>(new(HttpMethod.Post, "/users")
        {
            Content = JsonContent.Create(new UserJsonPayload(payload.EMail, payload.MemberCode, payload.Password, new(payload.RoleId, null!, null!)), options: _options)
        }, cancellationToken);
    private record UserJsonPayload(
        [property: JsonPropertyName("email")] string EMail,
        [property: JsonPropertyName("member_code")] string? MemberCode,
        [property: JsonPropertyName("password")] string Password,
        [property: JsonPropertyName("role")] Role Role
    );

    /// <inheritdoc/>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="userId"/>が0より小さい場合にスローされます。</exception>
    public ValueTask<User> FetchUserAsync(int userId, CancellationToken cancellationToken = default)
        => CallApiAsync<User>(new(HttpMethod.Get, $"/users/{ThrowIfNegative(userId, nameof(userId)):D}"), cancellationToken);

    /// <inheritdoc/>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="userId"/>が0より小さい場合にスローされます。</exception>
    public ValueTask<User> UpdateUserAsync(int userId, UserPayload payload, CancellationToken cancellationToken = default)
        => CallApiAsync<User>(new(new("PATCH"), $"/users/{ThrowIfNegative(userId, nameof(userId)):D}")
        {
            Content = JsonContent.Create(new UserJsonPayload(payload.EMail, payload.MemberCode, payload.Password, new(payload.RoleId, null!, null!)), options: _options)
        }, cancellationToken);

    /// <inheritdoc/>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="userId"/>が0より小さい場合にスローされます。</exception>
    public async ValueTask DeleteUserAsync(int userId, CancellationToken cancellationToken = default)
        => await CallApiAsync(new(HttpMethod.Delete, $"/users/{ThrowIfNegative(userId, nameof(userId)):D}"), cancellationToken).ConfigureAwait(false);
    #endregion

    #region ロール
    /// <inheritdoc/>
    public async ValueTask<IReadOnlyList<Role>> FetchRolesAsync(CancellationToken cancellationToken = default)
        => (await CallApiAsync<ApiResult<Role>>(new(HttpMethod.Get, "/roles"), cancellationToken)
            .ConfigureAwait(false))!.Data;
    #endregion

    #region マスター管理
    /// <inheritdoc/>
    public async ValueTask<IReadOnlyList<EnumOption>> FetchEnumOptionsAsync(CancellationToken cancellationToken = default)
        => (await CallApiAsync<ApiResult<EnumOption>>(new(HttpMethod.Get, "/enum_options"), cancellationToken)
            .ConfigureAwait(false)).Data;

    /// <inheritdoc/>
    public ValueTask<EnumOption> FetchEnumOptionAsync(int customFieldId, CancellationToken cancellationToken = default)
        => CallApiAsync<EnumOption>(new(HttpMethod.Get, $"/enum_options/{customFieldId}"), cancellationToken);

    /// <inheritdoc/>
    public ValueTask<int> UpdateEnumOptionAsync(int customFieldId, IReadOnlyList<(int?, string)> payload, CancellationToken cancellationToken = default)
        => CallTaskApiAsync(
            HttpMethod.Put,
            $"/enum_options/{customFieldId}",
            new EnumOptionPayload(payload.Select(d => new EnumOptionPayload.Data(d.Item1, d.Item2)).ToArray()),
            cancellationToken);
    private record EnumOptionPayload([property: JsonPropertyName("enum_option_data")] IReadOnlyList<EnumOptionPayload.Data> EnumOptionData)
    {
        internal record Data(int? Id, string Name);
    }
    #endregion

    #region Common Method
    /// <summary>APIコール前に必要な認証を行います。</summary>
    private async ValueTask FetchTokenAsync(CancellationToken cancellationToken)
        => AccessToken ??= (await AuthenticateAsync(cancellationToken).ConfigureAwait(false)).AccessToken;

    private record TaskResult([property: JsonPropertyName("task_id")] int Id);

    private record ErrorResponse([property: JsonPropertyName("errors")] IReadOnlyList<string> Errors);

    /// <summary>
    /// APIを呼び出します。
    /// </summary>
    /// <param name="request">APIに対するリクエスト</param>
    /// <param name="cancellationToken"><inheritdoc cref="FetchMemberLayoutAsync" path="/param[@name='cancellationToken']/text()"/></param>
    /// <exception cref="ApplicationException">
    /// APIからのHTTPステータスコードが200-299番でない場合にスローされます。
    /// </exception>
    private async ValueTask<HttpResponseMessage> CallApiAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        await FetchTokenAsync(cancellationToken).ConfigureAwait(false);
        var response = await _client.SendAsync(request, cancellationToken: cancellationToken).ConfigureAwait(false);
        await ValidateApiResponseAsync(response, cancellationToken).ConfigureAwait(false);
        return response;
    }

    /// <summary>
    /// APIを呼び出し、受け取ったJSONを<typeparamref name="T"/>に変換して返します。
    /// </summary>
    /// <param name="request"><inheritdoc cref="CallApiAsync" path="/param[@name='request']"/></param>
    /// <param name="cancellationToken"><inheritdoc cref="CallApiAsync" path="/param[@name='cancellationToken']"/></param>
    /// <typeparam name="T">JSONの型</typeparam>
    /// <inheritdoc cref="CallApiAsync" path="/exception"/>
    private async ValueTask<T> CallApiAsync<T>(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var response = await CallApiAsync(request, cancellationToken).ConfigureAwait(false);
        return (await response.Content.ReadFromJsonAsync<T>(_options, cancellationToken).ConfigureAwait(false))!;
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
    private async ValueTask<int> CallTaskApiAsync<T>(HttpMethod method, string uri, T payload, CancellationToken cancellationToken)
        => (await CallApiAsync<TaskResult>(new(method, uri)
        {
            Content = JsonContent.Create(payload, options: _options)
        }, cancellationToken).ConfigureAwait(false)).Id;

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
                ? string.Join("\n", (await response.Content.ReadFromJsonAsync<ErrorResponse>(cancellationToken: cancellationToken).ConfigureAwait(false))!.Errors)
#if NET5_0_OR_GREATER
                    : await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
#else
                    : await response.Content.ReadAsStringAsync().ConfigureAwait(false);
#endif
            throw new ApplicationException(errorMessage, ex);
        }
    }

    private int ThrowIfNegative(int id, string paramName)
        => id >= 0 ? id : throw new ArgumentOutOfRangeException(paramName);
    #endregion
}
