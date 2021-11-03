using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Kaonavi.Net.Entities;

namespace Kaonavi.Net.Services
{
    /// <summary>
    /// カオナビ API v2 を呼び出すサービスの実装
    /// </summary>
    public class KaonaviV2Service : IKaonaviService
    {
        /// <summary>カオナビ API v2 のルートアドレス</summary>
        private const string BaseApiAddress = "https://api.kaonavi.jp/api/v2.0";

        /// <summary>
        /// APIに送信するJSONペイロードのエンコード設定
        /// </summary>
        private static readonly JsonSerializerOptions _options = new(JsonSerializerDefaults.Web)
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        };
        static KaonaviV2Service()
            => _options.Converters.Add(new NullableDateTimeConverter());

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
        /// 各種API呼び出し時、この項目が<c>null</c>の場合は自動的に<see cref="AuthenticateAsync"/>を呼び出します。
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
        /// <paramref name="client"/>, <paramref name="consumerKey"/>または<paramref name="consumerSecret"/>が<c>null</c>の場合にスローされます。
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
        public async ValueTask<MemberLayout> FetchMemberLayoutAsync(CancellationToken cancellationToken = default)
        {
            await FetchTokenAsync(cancellationToken).ConfigureAwait(false);

            var response = await _client.GetAsync("/member_layouts", cancellationToken).ConfigureAwait(false);
            await ValidateApiResponseAsync(response, cancellationToken).ConfigureAwait(false);

            return (await response.Content
                .ReadFromJsonAsync<MemberLayout>(cancellationToken: cancellationToken)
                .ConfigureAwait(false))!;
        }

        /// <inheritdoc/>
        public async ValueTask<IReadOnlyList<SheetLayout>> FetchSheetLayoutsAsync(CancellationToken cancellationToken = default)
        {
            await FetchTokenAsync(cancellationToken).ConfigureAwait(false);

            var response = await _client.GetAsync("/sheet_layouts", cancellationToken).ConfigureAwait(false);
            await ValidateApiResponseAsync(response, cancellationToken).ConfigureAwait(false);

            return (await response.Content
                .ReadFromJsonAsync<SheetLayoutsResult>(cancellationToken: cancellationToken)
                .ConfigureAwait(false))!.Sheets;
        }
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
        {
            await FetchTokenAsync(cancellationToken).ConfigureAwait(false);

            var response = await _client.GetAsync("/members", cancellationToken).ConfigureAwait(false);
            await ValidateApiResponseAsync(response, cancellationToken).ConfigureAwait(false);

            return (await response.Content
                .ReadFromJsonAsync<ApiResult<MemberData>>(_options, cancellationToken)
                .ConfigureAwait(false))!.MemberData;
        }

        /// <inheritdoc/>
        public async ValueTask<int> AddMemberDataAsync(IReadOnlyList<MemberData> payload, CancellationToken cancellationToken = default)
        {
            await FetchTokenAsync(cancellationToken).ConfigureAwait(false);

            var postPayload = new ApiResult<MemberData>(payload);
            var response = await _client.PostAsJsonAsync("/members", postPayload, _options, cancellationToken).ConfigureAwait(false);
            await ValidateApiResponseAsync(response, cancellationToken).ConfigureAwait(false);

            return (await response.Content
                .ReadFromJsonAsync<TaskResult>(cancellationToken: cancellationToken)
                .ConfigureAwait(false))!.Id;
        }

        /// <inheritdoc/>
        public async ValueTask<int> ReplaceMemberDataAsync(IReadOnlyList<MemberData> payload, CancellationToken cancellationToken = default)
        {
            await FetchTokenAsync(cancellationToken).ConfigureAwait(false);

            var postPayload = new ApiResult<MemberData>(payload);
            var response = await _client.PutAsJsonAsync("/members", postPayload, _options, cancellationToken).ConfigureAwait(false);
            await ValidateApiResponseAsync(response, cancellationToken).ConfigureAwait(false);

            return (await response.Content
                .ReadFromJsonAsync<TaskResult>(cancellationToken: cancellationToken)
                .ConfigureAwait(false))!.Id;
        }

        /// <inheritdoc/>
        public async ValueTask<int> UpdateMemberDataAsync(IReadOnlyList<MemberData> payload, CancellationToken cancellationToken = default)
        {
            await FetchTokenAsync(cancellationToken).ConfigureAwait(false);

            var patchPayload = new ApiResult<MemberData>(payload);
            var content = new ByteArrayContent(JsonSerializer.SerializeToUtf8Bytes(patchPayload, _options));
            content.Headers.ContentType = new("application/json");
            var req = new HttpRequestMessage(new("PATCH"), "/members") { Content = content };

            var response = await _client.SendAsync(req, cancellationToken).ConfigureAwait(false);
            await ValidateApiResponseAsync(response, cancellationToken).ConfigureAwait(false);

            return (await response.Content
                .ReadFromJsonAsync<TaskResult>(cancellationToken: cancellationToken)
                .ConfigureAwait(false))!.Id;
        }

        /// <inheritdoc/>
        public async ValueTask<int> DeleteMemberDataAsync(IReadOnlyList<string> codes, CancellationToken cancellationToken = default)
        {
            await FetchTokenAsync(cancellationToken).ConfigureAwait(false);

            var payload = new DeleteMemberDataPayload(codes);
            var response = await _client.PostAsJsonAsync("/members/delete", payload, _options, cancellationToken).ConfigureAwait(false);
            await ValidateApiResponseAsync(response, cancellationToken).ConfigureAwait(false);

            return (await response.Content
                .ReadFromJsonAsync<TaskResult>(cancellationToken: cancellationToken)
                .ConfigureAwait(false))!.Id;
        }
        private record DeleteMemberDataPayload(IReadOnlyList<string> Codes);
        #endregion

        #region シート情報
        /// <inheritdoc/>
        public async ValueTask<IReadOnlyList<SheetData>> FetchSheetDataListAsync(int sheetId, CancellationToken cancellationToken = default)
        {
            await FetchTokenAsync(cancellationToken).ConfigureAwait(false);

            var response = await _client.GetAsync($"/sheets/{sheetId:D}", cancellationToken).ConfigureAwait(false);
            await ValidateApiResponseAsync(response, cancellationToken).ConfigureAwait(false);

            return (await response.Content
                .ReadFromJsonAsync<ApiResult<SheetData>>(cancellationToken: cancellationToken)
                .ConfigureAwait(false))!.MemberData;
        }

        /// <inheritdoc/>
        public async ValueTask<int> ReplaceSheetDataAsync(int sheetId, IReadOnlyList<SheetData> payload, CancellationToken cancellationToken = default)
        {
            await FetchTokenAsync(cancellationToken).ConfigureAwait(false);

            var putPayload = new ApiResult<SheetData>(payload);
            var response = await _client.PutAsJsonAsync($"/sheets/{sheetId:D}", putPayload, _options, cancellationToken).ConfigureAwait(false);
            await ValidateApiResponseAsync(response, cancellationToken).ConfigureAwait(false);

            return (await response.Content
                .ReadFromJsonAsync<TaskResult>(cancellationToken: cancellationToken)
                .ConfigureAwait(false))!.Id;
        }

        /// <inheritdoc/>
        public async ValueTask<int> UpdateSheetDataAsync(int sheetId, IReadOnlyList<SheetData> payload, CancellationToken cancellationToken = default)
        {
            await FetchTokenAsync(cancellationToken).ConfigureAwait(false);

            var patchPayload = new ApiResult<SheetData>(payload);
            var content = new ByteArrayContent(JsonSerializer.SerializeToUtf8Bytes(patchPayload, _options));
            content.Headers.ContentType = new("application/json");
            var req = new HttpRequestMessage(new("PATCH"), $"/sheets/{sheetId:D}") { Content = content };

            var response = await _client.SendAsync(req, cancellationToken).ConfigureAwait(false);
            await ValidateApiResponseAsync(response, cancellationToken).ConfigureAwait(false);

            return (await response.Content
                .ReadFromJsonAsync<TaskResult>(cancellationToken: cancellationToken)
                .ConfigureAwait(false))!.Id;
        }
        #endregion

        #region 所属ツリー
        /// <inheritdoc/>
        public async ValueTask<IReadOnlyList<DepartmentTree>> FetchDepartmentsAsync(CancellationToken cancellationToken = default)
        {
            await FetchTokenAsync(cancellationToken).ConfigureAwait(false);

            var response = await _client.GetAsync("/departments", cancellationToken).ConfigureAwait(false);
            await ValidateApiResponseAsync(response, cancellationToken).ConfigureAwait(false);

            return (await response.Content
                .ReadFromJsonAsync<DepartmentsResult>(cancellationToken: cancellationToken)
                .ConfigureAwait(false))!.DepartmentData;
        }

        /// <inheritdoc/>
        public async ValueTask<int> ReplaceDepartmentsAsync(IReadOnlyList<DepartmentTree> payload, CancellationToken cancellationToken = default)
        {
            await FetchTokenAsync(cancellationToken).ConfigureAwait(false);

            var postPayload = new DepartmentsResult(payload);
            var response = await _client.PutAsJsonAsync("/departments", postPayload, _options, cancellationToken).ConfigureAwait(false);
            await ValidateApiResponseAsync(response, cancellationToken).ConfigureAwait(false);

            return (await response.Content
                .ReadFromJsonAsync<TaskResult>(cancellationToken: cancellationToken)
                .ConfigureAwait(false))!.Id;
        }
        private record DepartmentsResult(
            [property: JsonPropertyName("department_data")] IReadOnlyList<DepartmentTree> DepartmentData
        );
        #endregion

        #region タスク進捗状況
        /// <inheritdoc/>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="taskId"/>が0より小さい場合にスローされます。</exception>
        public async ValueTask<TaskProgress> FetchTaskProgressAsync(int taskId, CancellationToken cancellationToken = default)
        {
            if (taskId < 0)
                throw new ArgumentOutOfRangeException(nameof(taskId));

            await FetchTokenAsync(cancellationToken).ConfigureAwait(false);

            var response = await _client.GetAsync($"/tasks/{taskId:D}", cancellationToken).ConfigureAwait(false);
            await ValidateApiResponseAsync(response, cancellationToken).ConfigureAwait(false);

            return (await response.Content
                .ReadFromJsonAsync<TaskProgress>(cancellationToken: cancellationToken)
                .ConfigureAwait(false))!;
        }
        #endregion

        #region ユーザー情報
        /// <inheritdoc/>
        public async ValueTask<IReadOnlyList<User>> FetchUsersAsync(CancellationToken cancellationToken = default)
        {
            await FetchTokenAsync(cancellationToken).ConfigureAwait(false);

            var response = await _client.GetAsync("/users", cancellationToken).ConfigureAwait(false);
            await ValidateApiResponseAsync(response, cancellationToken).ConfigureAwait(false);

            return (await response.Content
                .ReadFromJsonAsync<UsersResult>(cancellationToken: cancellationToken)
                .ConfigureAwait(false))!.UserData;
        }
        private record UsersResult([property: JsonPropertyName("user_data")] IReadOnlyList<User> UserData);

        /// <inheritdoc/>
        public async ValueTask<User> AddUserAsync(UserPayload payload, CancellationToken cancellationToken = default)
        {
            await FetchTokenAsync(cancellationToken).ConfigureAwait(false);

            var postPayload = new UserJsonPayload(payload.EMail, payload.MemberCode, payload.Password, new(payload.RoleId, null!, null!));
            var response = await _client.PostAsJsonAsync("/users", postPayload, _options, cancellationToken).ConfigureAwait(false);
            await ValidateApiResponseAsync(response, cancellationToken).ConfigureAwait(false);

            return (await response.Content
                .ReadFromJsonAsync<User>(cancellationToken: cancellationToken)
                .ConfigureAwait(false))!;
        }
        private record UserJsonPayload(
            [property: JsonPropertyName("email")] string EMail,
            [property: JsonPropertyName("member_code")] string? MemberCode,
            [property: JsonPropertyName("password")] string Password,
            [property: JsonPropertyName("role")] Role Role
        );

        /// <inheritdoc/>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="userId"/>が0より小さい場合にスローされます。</exception>
        public async ValueTask<User> FetchUserAsync(int userId, CancellationToken cancellationToken = default)
        {
            if (userId < 0)
                throw new ArgumentOutOfRangeException(nameof(userId));

            await FetchTokenAsync(cancellationToken).ConfigureAwait(false);

            var response = await _client.GetAsync($"/users/{userId:D}", cancellationToken).ConfigureAwait(false);
            await ValidateApiResponseAsync(response, cancellationToken).ConfigureAwait(false);

            return (await response.Content
                .ReadFromJsonAsync<User>(cancellationToken: cancellationToken)
                .ConfigureAwait(false))!;
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="userId"/>が0より小さい場合にスローされます。</exception>
        public async ValueTask<User> UpdateUserAsync(int userId, UserPayload payload, CancellationToken cancellationToken = default)
        {
            if (userId < 0)
                throw new ArgumentOutOfRangeException(nameof(userId));

            await FetchTokenAsync(cancellationToken).ConfigureAwait(false);

            var patchPayload = new UserJsonPayload(payload.EMail, payload.MemberCode, payload.Password, new(payload.RoleId, null!, null!));
            var content = new ByteArrayContent(JsonSerializer.SerializeToUtf8Bytes(patchPayload, _options));
            content.Headers.ContentType = new("application/json");
            var req = new HttpRequestMessage(new("PATCH"), $"/users/{userId:D}") { Content = content };

            var response = await _client.SendAsync(req, cancellationToken).ConfigureAwait(false);
            await ValidateApiResponseAsync(response, cancellationToken).ConfigureAwait(false);

            return (await response.Content
                .ReadFromJsonAsync<User>(cancellationToken: cancellationToken)
                .ConfigureAwait(false))!;
        }

        /// <inheritdoc/>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="userId"/>が0より小さい場合にスローされます。</exception>
        public async ValueTask DeleteUserAsync(int userId, CancellationToken cancellationToken = default)
        {
            if (userId < 0)
                throw new ArgumentOutOfRangeException(nameof(userId));

            await FetchTokenAsync(cancellationToken).ConfigureAwait(false);

            var response = await _client.DeleteAsync($"/users/{userId:D}", cancellationToken).ConfigureAwait(false);
            await ValidateApiResponseAsync(response, cancellationToken).ConfigureAwait(false);
        }
        #endregion

        #region ロール
        /// <inheritdoc/>
        public async ValueTask<IReadOnlyList<Role>> FetchRolesAsync(CancellationToken cancellationToken = default)
        {
            await FetchTokenAsync(cancellationToken).ConfigureAwait(false);

            var response = await _client.GetAsync("/roles", cancellationToken).ConfigureAwait(false);
            await ValidateApiResponseAsync(response, cancellationToken).ConfigureAwait(false);

            return (await response.Content
                .ReadFromJsonAsync<RolesResult>(cancellationToken: cancellationToken)
                .ConfigureAwait(false))!.RoleData;
        }
        private record RolesResult([property: JsonPropertyName("role_data")] IReadOnlyList<Role> RoleData);
        #endregion

        #region マスター管理
        /// <inheritdoc/>
        public async ValueTask<IReadOnlyList<EnumOption>> FetchEnumOptionsAsync(CancellationToken cancellationToken = default)
        {
            await FetchTokenAsync(cancellationToken).ConfigureAwait(false);

            var response = await _client.GetAsync("/enum_options", cancellationToken).ConfigureAwait(false);
            await ValidateApiResponseAsync(response, cancellationToken).ConfigureAwait(false);

            return (await response.Content
                .ReadFromJsonAsync<EnumOptionsResult>(cancellationToken: cancellationToken)
                .ConfigureAwait(false))!.CustomFieldData;
        }
        private record EnumOptionsResult([property: JsonPropertyName("custom_field_data")] IReadOnlyList<EnumOption> CustomFieldData);

        /// <inheritdoc/>
        public async ValueTask<EnumOption> FetchEnumOptionAsync(int customFieldId, CancellationToken cancellationToken = default)
        {
            await FetchTokenAsync(cancellationToken).ConfigureAwait(false);

            var response = await _client.GetAsync($"/enum_options/{customFieldId}", cancellationToken).ConfigureAwait(false);
            await ValidateApiResponseAsync(response, cancellationToken).ConfigureAwait(false);

            return (await response.Content
                .ReadFromJsonAsync<EnumOption>(cancellationToken: cancellationToken)
                .ConfigureAwait(false))!;
        }

        /// <inheritdoc/>
        public async ValueTask<int> UpdateEnumOptionAsync(int customFieldId, IReadOnlyList<(int?, string)> payload, CancellationToken cancellationToken = default)
        {
            await FetchTokenAsync(cancellationToken).ConfigureAwait(false);

            var putPayload = new EnumOptionPayload(payload.Select(d => new EnumOptionPayload.Data(d.Item1, d.Item2)).ToArray());
            var response = await _client.PutAsJsonAsync($"/enum_options/{customFieldId}", putPayload, _options, cancellationToken).ConfigureAwait(false);
            await ValidateApiResponseAsync(response, cancellationToken).ConfigureAwait(false);

            return (await response.Content
                .ReadFromJsonAsync<TaskResult>(cancellationToken: cancellationToken)
                .ConfigureAwait(false))!.Id;
        }
        private record EnumOptionPayload([property: JsonPropertyName("enum_option_data")] IReadOnlyList<EnumOptionPayload.Data> EnumOptionData)
        {
            internal record Data(int? Id, string Name);
        }
        #endregion

        #region Common Method
        /// <summary>APIコール前に必要な認証を行います。</summary>
        private async ValueTask FetchTokenAsync(CancellationToken cancellationToken)
            => AccessToken ??= (await AuthenticateAsync(cancellationToken).ConfigureAwait(false)).AccessToken;

        private record ApiResult<T>([property: JsonPropertyName("member_data")] IReadOnlyList<T> MemberData);

        private record TaskResult([property: JsonPropertyName("task_id")] int Id);

        private record ErrorResponse([property: JsonPropertyName("errors")] IReadOnlyList<string> Errors);

        /// <summary>
        /// APIを呼び出し、受け取ったJSONを<typeparamref name="T"/>に変換して返します。
        /// </summary>
        /// <param name="request">APIに対するリクエスト</param>
        /// <param name="cancellationToken">キャンセル通知を受け取るために他のオブジェクトまたはスレッドで使用できるキャンセル トークン。</param>
        /// <typeparam name="T">JSONの型</typeparam>
        /// <exception cref="ApplicationException">
        /// APIからのHTTPステータスコードが200-299番でない場合にスローされます。
        /// </exception>
        private async ValueTask<T> CallApiAsync<T>(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            await FetchTokenAsync(cancellationToken).ConfigureAwait(false);
            var response = await _client.SendAsync(request, cancellationToken: cancellationToken).ConfigureAwait(false);
            await ValidateApiResponseAsync(response, cancellationToken).ConfigureAwait(false);
            return (await response.Content.ReadFromJsonAsync<T>(cancellationToken: cancellationToken).ConfigureAwait(false))!;
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
                    ? string.Join("\n", (await response.Content.ReadFromJsonAsync<ErrorResponse>(cancellationToken: cancellationToken).ConfigureAwait(false))!.Errors)
#if NET5_0
                    : await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
#else
                    : await response.Content.ReadAsStringAsync().ConfigureAwait(false);
#endif
                throw new ApplicationException(errorMessage, ex);
            }
        }
        #endregion
    }
}
