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
using Kaonavi.Net.Entities.Api;

namespace Kaonavi.Net.Services
{
    public class KaonaviV2Service
    {
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

        private readonly HttpClient _client;
        private readonly string _consumerKey;
        private readonly string _consumerSecret;

        private const string TokenHeader = "Kaonavi-Token";
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
        /// https://developer.kaonavi.jp/api/v2.0/index.html#section/dryrun
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

        public KaonaviV2Service(HttpClient client, string consumerKey, string consumerSecret)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _consumerKey = consumerKey ?? throw new ArgumentNullException(nameof(consumerKey));
            _consumerSecret = consumerSecret ?? throw new ArgumentNullException(nameof(consumerSecret));

            _client.BaseAddress ??= new(BaseApiAddress);
        }

        public async ValueTask<Token> AuthenticateAsync(CancellationToken cancellationToken = default)
        {
            byte[] byteArray = Encoding.UTF8.GetBytes($"{_consumerKey}:{_consumerSecret}");
            var content = new FormUrlEncodedContent(new Dictionary<string, string>()
            {
                { "grant_type", "client_credentials" }
            });
            _client.DefaultRequestHeaders.Authorization = new("Basic", Convert.ToBase64String(byteArray));

            var response = await _client.PostAsync("/token", content, cancellationToken).ConfigureAwait(false);
            await ValidateApiResponseAsync(response).ConfigureAwait(false);

            var token = await response.Content
                .ReadFromJsonAsync<Token>(cancellationToken: cancellationToken)
                .ConfigureAwait(false);
            _client.DefaultRequestHeaders.Authorization = null;
            return token!;
        }

        public async ValueTask<MemberLayout> FetchMemberLayoutAsync(CancellationToken cancellationToken = default)
        {
            await FetchTokenAsync(cancellationToken).ConfigureAwait(false);

            var response = await _client.GetAsync("/member_layouts").ConfigureAwait(false);
            await ValidateApiResponseAsync(response).ConfigureAwait(false);

            return (await response.Content
                .ReadFromJsonAsync<MemberLayout>(cancellationToken: cancellationToken)
                .ConfigureAwait(false))!;
        }

        public async ValueTask<IEnumerable<SheetLayout>> FetchSheetLayoutsAsync(CancellationToken cancellationToken = default)
        {
            await FetchTokenAsync(cancellationToken).ConfigureAwait(false);

            var response = await _client.GetAsync("/sheet_layouts").ConfigureAwait(false);
            await ValidateApiResponseAsync(response).ConfigureAwait(false);

            return (await response.Content
                .ReadFromJsonAsync<SheetLayoutsResult>(cancellationToken: cancellationToken)
                .ConfigureAwait(false))!.Sheets;
        }
        private record SheetLayoutsResult(
            [property: JsonPropertyName("sheets")] IEnumerable<SheetLayout> Sheets
        );

        #region Member
        /// <summary>
        /// 全てのメンバーの基本情報・所属（主務）・兼務情報を取得します。
        /// https://developer.kaonavi.jp/api/v2.0/index.html#tag/%E3%83%A1%E3%83%B3%E3%83%90%E3%83%BC%E6%83%85%E5%A0%B1/paths/~1members/get
        /// </summary>
        /// <param name="cancellationToken">キャンセル通知を受け取るために他のオブジェクトまたはスレッドで使用できるキャンセル トークン。</param>
        public async ValueTask<IEnumerable<MemberData>> FetchMembersDataAsync(CancellationToken cancellationToken = default)
        {
            await FetchTokenAsync(cancellationToken).ConfigureAwait(false);

            var response = await _client.GetAsync("/members").ConfigureAwait(false);
            await ValidateApiResponseAsync(response).ConfigureAwait(false);

            return (await response.Content
                .ReadFromJsonAsync<ApiResult<MemberData>>(_options, cancellationToken)
                .ConfigureAwait(false))!.MemberData;
        }

        /// <summary>
        /// メンバー登録と、合わせて基本情報・所属（主務）・兼務情報を登録します。
        /// https://developer.kaonavi.jp/api/v2.0/index.html#tag/%E3%83%A1%E3%83%B3%E3%83%90%E3%83%BC%E6%83%85%E5%A0%B1/paths/~1members/post
        /// </summary>
        /// <remarks>更新リクエスト制限の対象APIです。</remarks>
        /// <param name="payload">追加するデータ</param>
        /// <param name="cancellationToken">キャンセル通知を受け取るために他のオブジェクトまたはスレッドで使用できるキャンセル トークン。</param>
        /// <returns><see cref="TaskProgress.Id"/></returns>
        public async ValueTask<int> AddMemberDataAsync(IEnumerable<MemberData> payload, CancellationToken cancellationToken = default)
        {
            await FetchTokenAsync(cancellationToken).ConfigureAwait(false);

            var postPayload = new ApiResult<MemberData>(payload);
            var response = await _client.PostAsJsonAsync("/members", postPayload, _options).ConfigureAwait(false);
            await ValidateApiResponseAsync(response).ConfigureAwait(false);

            return (await response.Content
                .ReadFromJsonAsync<TaskResult>(cancellationToken: cancellationToken)
                .ConfigureAwait(false))!.Id;
        }

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
        public async ValueTask<int> ReplaceMemberDataAsync(IEnumerable<MemberData> payload, CancellationToken cancellationToken = default)
        {
            await FetchTokenAsync(cancellationToken).ConfigureAwait(false);

            var postPayload = new ApiResult<MemberData>(payload);
            var response = await _client.PutAsJsonAsync("/members", postPayload, _options).ConfigureAwait(false);
            await ValidateApiResponseAsync(response).ConfigureAwait(false);

            return (await response.Content
                .ReadFromJsonAsync<TaskResult>(cancellationToken: cancellationToken)
                .ConfigureAwait(false))!.Id;
        }

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
        public async ValueTask<int> UpdateMemberDataAsync(IEnumerable<MemberData> payload, CancellationToken cancellationToken = default)
        {
            await FetchTokenAsync(cancellationToken).ConfigureAwait(false);

            var patchPayload = new ApiResult<MemberData>(payload);
            var content = new ByteArrayContent(JsonSerializer.SerializeToUtf8Bytes(patchPayload, _options));
            content.Headers.ContentType = new("application/json");
            var req = new HttpRequestMessage(new("PATCH"), "/members") { Content = content };

            var response = await _client.SendAsync(req).ConfigureAwait(false);
            await ValidateApiResponseAsync(response).ConfigureAwait(false);

            return (await response.Content
                .ReadFromJsonAsync<TaskResult>(cancellationToken: cancellationToken)
                .ConfigureAwait(false))!.Id;
        }
        #endregion

        #region Sheet
        /// <summary>
        /// 指定したシートの全情報を取得します。
        /// </summary>
        /// <param name="sheetId">シートID</param>
        /// <param name="cancellationToken">キャンセル通知を受け取るために他のオブジェクトまたはスレッドで使用できるキャンセル トークン。</param>
        public async ValueTask<IEnumerable<SheetData>> FetchSheetDataListAsync(int sheetId, CancellationToken cancellationToken = default)
        {
            await FetchTokenAsync(cancellationToken).ConfigureAwait(false);

            var response = await _client.GetAsync($"/sheets/{sheetId:D}", cancellationToken).ConfigureAwait(false);
            await ValidateApiResponseAsync(response).ConfigureAwait(false);

            return (await response.Content
                .ReadFromJsonAsync<ApiResult<SheetData>>(cancellationToken: cancellationToken)
                .ConfigureAwait(false))!.MemberData;
        }

        /// <summary>
        /// 指定したシートのシート情報を一括更新します。
        /// <paramref name="payload"/> に含まれていない情報は削除されます。
        /// 複数レコードの情報は送信された配列順に登録されます。
        /// </summary>
        /// <param name="sheetId">シートID</param>
        /// <param name="payload">一括更新するデータ</param>
        /// <param name="cancellationToken">キャンセル通知を受け取るために他のオブジェクトまたはスレッドで使用できるキャンセル トークン。</param>
        /// <returns><see cref="TaskProgress.Id"/></returns>
        public async ValueTask<int> ReplaceSheetDataAsync(int sheetId, IEnumerable<SheetData> payload, CancellationToken cancellationToken = default)
        {
            await FetchTokenAsync(cancellationToken).ConfigureAwait(false);

            var putPayload = new ApiResult<SheetData>(payload);
            var response = await _client.PutAsJsonAsync($"/sheets/{sheetId:D}", putPayload, _options, cancellationToken).ConfigureAwait(false);
            await ValidateApiResponseAsync(response).ConfigureAwait(false);

            return (await response.Content
                .ReadFromJsonAsync<TaskResult>(cancellationToken: cancellationToken)
                .ConfigureAwait(false))!.Id;
        }

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
        public async ValueTask<int> UpdateSheetDataAsync(int sheetId, IEnumerable<SheetData> payload, CancellationToken cancellationToken = default)
        {
            await FetchTokenAsync(cancellationToken).ConfigureAwait(false);

            var patchPayload = new ApiResult<SheetData>(payload);
            var content = new ByteArrayContent(JsonSerializer.SerializeToUtf8Bytes(patchPayload, _options));
            content.Headers.ContentType = new("application/json");
            var req = new HttpRequestMessage(new("PATCH"), $"/sheets/{sheetId:D}") { Content = content };

            var response = await _client.SendAsync(req).ConfigureAwait(false);
            await ValidateApiResponseAsync(response).ConfigureAwait(false);

            return (await response.Content
                .ReadFromJsonAsync<TaskResult>(cancellationToken: cancellationToken)
                .ConfigureAwait(false))!.Id;
        }
        #endregion

        /// <summary>
        /// 所属情報の一覧を取得します。
        /// https://developer.kaonavi.jp/api/v2.0/index.html#tag/%E6%89%80%E5%B1%9E/paths/~1departments/get
        /// </summary>
        /// <param name="cancellationToken">キャンセル通知を受け取るために他のオブジェクトまたはスレッドで使用できるキャンセル トークン。</param>
        public async ValueTask<IEnumerable<DepartmentInfo>> FetchDepartmentsAsync(CancellationToken cancellationToken = default)
        {
            await FetchTokenAsync(cancellationToken).ConfigureAwait(false);

            var response = await _client.GetAsync("/departments").ConfigureAwait(false);
            await ValidateApiResponseAsync(response).ConfigureAwait(false);

            return (await response.Content
                .ReadFromJsonAsync<DepartmentsResult>(cancellationToken: cancellationToken)
                .ConfigureAwait(false))!.DepartmentData;
        }
        private record DepartmentsResult(
            [property: JsonPropertyName("department_data")] IEnumerable<DepartmentInfo> DepartmentData
        );

        /// <summary>
        /// <paramref name="taskId"/>と一致するタスクの進捗状況を取得します。
        /// </summary>
        /// <param name="taskId">タスクID</param>
        /// <param name="cancellationToken">キャンセル通知を受け取るために他のオブジェクトまたはスレッドで使用できるキャンセル トークン。</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="taskId"/>が0より小さい場合にスローされます。</exception>
        public async ValueTask<TaskProgress> FetchTaskProgressAsync(int taskId, CancellationToken cancellationToken = default)
        {
            if (taskId < 0)
                throw new ArgumentOutOfRangeException(nameof(taskId));

            await FetchTokenAsync(cancellationToken).ConfigureAwait(false);

            var response = await _client.GetAsync($"/tasks/{taskId:D}").ConfigureAwait(false);
            await ValidateApiResponseAsync(response).ConfigureAwait(false);

            return (await response.Content
                .ReadFromJsonAsync<TaskProgress>(cancellationToken: cancellationToken)
                .ConfigureAwait(false))!;
        }

        #region User
        /// <summary>
        /// ユーザー情報の一覧を取得します。
        /// https://developer.kaonavi.jp/api/v2.0/index.html#tag/%E3%83%A6%E3%83%BC%E3%82%B6%E3%83%BC%E6%83%85%E5%A0%B1/paths/~1users/get
        /// </summary>
        /// <param name="cancellationToken">キャンセル通知を受け取るために他のオブジェクトまたはスレッドで使用できるキャンセル トークン。</param>
        public async ValueTask<IEnumerable<User>> FetchUsersAsync(CancellationToken cancellationToken = default)
        {
            await FetchTokenAsync(cancellationToken).ConfigureAwait(false);

            var response = await _client.GetAsync("/users").ConfigureAwait(false);
            await ValidateApiResponseAsync(response).ConfigureAwait(false);

            return (await response.Content
                .ReadFromJsonAsync<UsersResult>(cancellationToken: cancellationToken)
                .ConfigureAwait(false))!.UserData;
        }
        private record UsersResult(
            [property: JsonPropertyName("user_data")] IEnumerable<User> UserData
        );

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
        public async ValueTask<User> AddUserAsync(UserPayload payload, CancellationToken cancellationToken = default)
        {
            await FetchTokenAsync(cancellationToken).ConfigureAwait(false);

            var postPayload = new UserJsonPayload(payload.EMail, payload.MemberCode, payload.Password, new(payload.RoleId, null!, null!));
            var response = await _client.PostAsJsonAsync("/users", postPayload, _options).ConfigureAwait(false);
            await ValidateApiResponseAsync(response).ConfigureAwait(false);

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

        /// <summary>
        /// <paramref name="userId"/>と一致するログインユーザー情報を取得します。
        /// https://developer.kaonavi.jp/api/v2.0/index.html#tag/%E3%83%A6%E3%83%BC%E3%82%B6%E3%83%BC%E6%83%85%E5%A0%B1/paths/~1users~1{user_id}/get
        /// </summary>
        /// <param name="userId">ユーザーID</param>
        /// <param name="cancellationToken">キャンセル通知を受け取るために他のオブジェクトまたはスレッドで使用できるキャンセル トークン。</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="userId"/>が0より小さい場合にスローされます。</exception>
        public async ValueTask<User> FetchUserAsync(int userId, CancellationToken cancellationToken = default)
        {
            if (userId < 0)
                throw new ArgumentOutOfRangeException(nameof(userId));

            await FetchTokenAsync(cancellationToken).ConfigureAwait(false);

            var response = await _client.GetAsync($"/users/{userId:D}").ConfigureAwait(false);
            await ValidateApiResponseAsync(response).ConfigureAwait(false);

            return (await response.Content
                .ReadFromJsonAsync<User>(cancellationToken: cancellationToken)
                .ConfigureAwait(false))!;
        }

        /// <summary>
        /// <paramref name="userId"/>と一致するログインユーザー情報を更新します。
        /// https://developer.kaonavi.jp/api/v2.0/index.html#tag/%E3%83%A6%E3%83%BC%E3%82%B6%E3%83%BC%E6%83%85%E5%A0%B1/paths/~1users~1{user_id}/patch
        /// </summary>
        /// <param name="userId">ユーザーID</param>
        /// <param name="payload">リクエスト</param>
        /// <param name="cancellationToken">キャンセル通知を受け取るために他のオブジェクトまたはスレッドで使用できるキャンセル トークン。</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="userId"/>が0より小さい場合にスローされます。</exception>
        /// <remarks>
        /// 管理者メニュー > ユーザー管理 にて更新可能な以下のオプションについては元の値が維持されます。
        /// - スマホオプション
        /// - セキュアアクセス
        /// - アカウント状態
        /// - パスワードロック
        /// </remarks>
        public async ValueTask<User> UpdateUserAsync(int userId, UserPayload payload, CancellationToken cancellationToken = default)
        {
            if (userId < 0)
                throw new ArgumentOutOfRangeException(nameof(userId));

            await FetchTokenAsync(cancellationToken).ConfigureAwait(false);

            var patchPayload = new UserJsonPayload(payload.EMail, payload.MemberCode, payload.Password, new(payload.RoleId, null!, null!));
            var content = new ByteArrayContent(JsonSerializer.SerializeToUtf8Bytes(patchPayload, _options));
            content.Headers.ContentType = new("application/json");
            var req = new HttpRequestMessage(new("PATCH"), $"/users/{userId:D}") { Content = content };

            var response = await _client.SendAsync(req).ConfigureAwait(false);
            await ValidateApiResponseAsync(response).ConfigureAwait(false);

            return (await response.Content
                .ReadFromJsonAsync<User>(cancellationToken: cancellationToken)
                .ConfigureAwait(false))!;
        }

        /// <summary>
        /// <paramref name="userId"/>と一致するログインユーザー情報を削除します。
        /// https://developer.kaonavi.jp/api/v2.0/index.html#tag/%E3%83%A6%E3%83%BC%E3%82%B6%E3%83%BC%E6%83%85%E5%A0%B1/paths/~1users~1{user_id}/delete
        /// </summary>
        /// <param name="userId">ユーザーID</param>
        /// <param name="cancellationToken">キャンセル通知を受け取るために他のオブジェクトまたはスレッドで使用できるキャンセル トークン。</param>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="userId"/>が0より小さい場合にスローされます。</exception>
        public async ValueTask DeleteUserAsync(int userId, CancellationToken cancellationToken = default)
        {
            if (userId < 0)
                throw new ArgumentOutOfRangeException(nameof(userId));

            await FetchTokenAsync(cancellationToken).ConfigureAwait(false);

            var response = await _client.DeleteAsync($"/users/{userId:D}").ConfigureAwait(false);
            await ValidateApiResponseAsync(response).ConfigureAwait(false);
        }
        #endregion

        /// <summary>
        /// ロール情報の一覧を取得します。
        /// https://developer.kaonavi.jp/api/v2.0/index.html#tag/%E3%83%AD%E3%83%BC%E3%83%AB/paths/~1roles/get
        /// </summary>
        /// <param name="cancellationToken">キャンセル通知を受け取るために他のオブジェクトまたはスレッドで使用できるキャンセル トークン。</param>
        public async ValueTask<IEnumerable<Role>> FetchRolesAsync(CancellationToken cancellationToken = default)
        {
            await FetchTokenAsync(cancellationToken).ConfigureAwait(false);

            var response = await _client.GetAsync("/roles").ConfigureAwait(false);
            await ValidateApiResponseAsync(response).ConfigureAwait(false);

            return (await response.Content
                .ReadFromJsonAsync<RolesResult>(cancellationToken: cancellationToken)
                .ConfigureAwait(false))!.RoleData;
        }
        private record RolesResult(
            [property: JsonPropertyName("role_data")] IEnumerable<Role> RoleData
        );

        #region Common Method
        private async ValueTask FetchTokenAsync(CancellationToken cancellationToken = default)
            => AccessToken ??= (await AuthenticateAsync(cancellationToken).ConfigureAwait(false)).AccessToken;

        private record ApiResult<T>(
            [property: JsonPropertyName("member_data")] IEnumerable<T> MemberData
        );

        private record TaskResult([property: JsonPropertyName("task_id")] int Id);

        private record ErrorResponse([property: JsonPropertyName("errors")] IEnumerable<string> Errors);
        private async ValueTask ValidateApiResponseAsync(HttpResponseMessage response)
        {
            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            {
                string errorMessage = response.Content.Headers.ContentType.MediaType == "application/json"
                    ? string.Join("\n", (await response.Content.ReadFromJsonAsync<ErrorResponse>().ConfigureAwait(false))!.Errors)
                    : await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                throw new ApplicationException(errorMessage, ex);
            }
        }
        #endregion
    }
}
